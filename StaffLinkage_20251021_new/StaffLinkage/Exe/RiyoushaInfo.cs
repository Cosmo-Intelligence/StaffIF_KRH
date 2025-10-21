using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using KanjiConversion;
using StaffLinkage.Exe.Entity;
using StaffLinkage.Util;

namespace StaffLinkage.Exe
{
  class RiyoushaInfo
  {
    /// <summary>
    /// ログ出力
    /// </summary> 
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// FTPサーバIPを取得
    /// </summary>
    private static string ftpIp =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpIPAdress);

    /// <summary>
    /// FTPユーザを取得
    /// </summary>
    private static string ftpUser =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpUser);

    /// <summary>
    /// FTPパスワードを取得
    /// </summary>
    private static string ftpPassword =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpPassword);

    /// <summary>
    /// FTPリトライ数を取得
    /// </summary>
    private static int ftpRetry = 3;

    /// <summary>
    /// FTPファイル文字コードを取得
    /// </summary>
    private static Encoding ftpEncode =
                Encoding.GetEncoding(AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpEncode));

    /// <summary>
    /// 利用者連携フォルダを取得
    /// </summary>
    private static string ftpFolder =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpFolder);

    /// <summary>
    /// 利用者情報ファイルを取得
    /// </summary>
    private static string ftpFile =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpFile);

    /// <summary>
    /// パスワード変換有(=1)無(=0)を取得
    /// </summary>
    private static string convpwd_flg =
        AppConfigController.GetInstance().GetValueString(AppConfigParameter.PasswordChange);

    /// <summary>
    /// 旧字変換有(=1)無(=0)を取得
    /// </summary>
    private static string convfile_flg =
        AppConfigController.GetInstance().GetValueString(AppConfigParameter.ConvKanji);

    /// <summary>
    /// 漢字変換ファイルを取得
    /// </summary>
    private static string convfile =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.ConvKanjiFile);

    //Y_Higuchi
    /// <summary>
    /// 電文マッピングファイルパスを取得
    /// </summary>
    private static string TelegramMap_ConfigValue =
        AppConfigController.GetInstance().GetValueString(AppConfigParameter.TelegramMap);

    /// <summary>
    /// FTPサーバから利用者情報ファイルをダウンロードする
    /// </summary>
    /// <param name="work">作業フォルダ</param>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    public static bool DownLoad(string work)
    {
      // リトライ回数をチェック
      string strRetry = AppConfigController.GetInstance().GetValueString(AppConfigParameter.FtpRetryCount);
      if (int.TryParse(strRetry, out ftpRetry))
      {
        if ((ftpRetry < 1) || (ftpRetry > 30))
        {
          // 1～30 の指定範囲外だった場合、標準値=3 を指定する
          ftpRetry = 3;
        }
      }
      else
      {
        ftpRetry = 3;
      }

      StreamReader sr = null;

      StreamWriter sw = null;

      FtpWebResponse res = null;

      try
      {
        for (int errcnt = 1; errcnt <= ftpRetry;)
        {
          try
          {
            // FTPプロパティ設定
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Path.Combine("ftp://" + ftpIp + "/", ftpFolder + "/" + ftpFile));
            req.Credentials = new NetworkCredential(ftpUser, ftpPassword);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            // 要求完了後に閉じる
            req.KeepAlive = false;
            // バイナリモードで転送
            req.UseBinary = true;
            // パッシブモードを有効にする
            req.UsePassive = true;

            if (res == null)
            {
              res = (FtpWebResponse)req.GetResponse();
            }

            // 文字コードを指定してFTPをダウンロード
            sr = new StreamReader(res.GetResponseStream(), ftpEncode);

            sw = new StreamWriter(Path.Combine(work, ftpFile), false, CommonParameter.CommonEnocode);

            // FTPファイルを読み込む
            string read = sr.ReadToEnd();

            // 指定ファイルに書き込む
            sw.Write(read);

            // 正常に処理できた場合、ループを抜ける
            break;
          }
          catch (Exception ex)
          {
            _log.WarnFormat("利用者情報ファイル取得が失敗しました。：{0}回目", errcnt);
            _log.WarnFormat(ex.Message);
          }

          // インクリメント
          errcnt++;

          // 失敗回数確認
          if (errcnt > ftpRetry)
          {
            throw new Exception();
          }
        }
      }
      catch
      {
        _log.Error("既定回数に達した為、中断します。");
        return false;
      }
      finally
      {
        // 解放処理
        if (sr != null)
        {
          sr.Close();
          sr = null;
        }
        if (sw != null)
        {
          sw.Close();
          sw = null;
        }
      }

      return true;
    }

    /// <summary>
    /// 利用者情報データ取得処理
    /// </summary>
    /// <param name="lastday_from">更新日付 始端日</param>
    /// <param name="lastday_to">更新日付 終端日</param>
    /// <param name="work">電文ファイルが存在するフォルダ</param>
    /// <param name="usrDataset">読込んだデータを格納するオブジェクト</param>
    /// <returns></returns>
    public static bool GetRiyoushaListII(DateTime lastday_from, DateTime lastday_to, string work, out UserInfo_Dataset usrDataset)
    {
      // 利用者情報(電文ファイル)のデータを格納するクラス
      usrDataset = new UserInfo_Dataset();
      bool blnRet = true;
      try
      {
        // 旧字変換する/しない
        if (convfile_flg == "1")
        {
          // 旧字変換クラス準備
          if (!clsKanjiConversion.boolSet_Config(convfile))
          {
            blnRet = false;
            throw new Exception("旧字変換設定ファイルが見つかりませんでした。");
          }
        }

        // 電文マッピング準備
        Util.XmlUtil telmap = new Util.XmlUtil();
        telmap.strFilename = TelegramMap_ConfigValue;
        if (!File.Exists(telmap.strFilename))
        {
          throw new Exception("指定された電文マッピングファイルが見つかりません。");
        }

        // 電文項目の NODE を tblTelmap へ読込む
        Hashtable htDenb = new Hashtable();
        if (!telmap.xmlRead(CommonParameter.xmlNODE_DENBUN, htDenb))
        {
          blnRet = false;
          throw new Exception("電文マッピングファイルの読込(利用者情報)でエラーが発生しました。");
        }

        // ToUsersInfo の NODE を tblTuimap へ読込む
        Hashtable htToUsrInfo = new Hashtable();
        if (!telmap.xmlRead(CommonParameter.xmlNODE_TABLE, htToUsrInfo))
        {
          blnRet = false;
          throw new Exception("電文マッピングファイルの読込(ToUsersInfo)でエラーが発生しました。");
        }

        // 利用者情報の構成を設定する
        if (!usrDataset.Set_Structure(htDenb, lastday_from, lastday_to, htToUsrInfo))
        {
          blnRet = false;
          throw new Exception("電文マッピングファイルの定義が不正なため電文ファイルを読み込めません。");
        }

        // 電文ファイルをストリームでオープンする
        StreamReader sr = new StreamReader(Path.Combine(work, ftpFile), CommonParameter.CommonEnocode);

        // 電文ファイルを読込む
        try
        {
          int intRowCount = 0;
          while (sr.Peek() >= 0)
          {
            // データを読み込む
            string dataread = sr.ReadLine();
            intRowCount++;
            if (!usrDataset.Read_Line(dataread))
            {
              _log.ErrorFormat("電文ファイル取込エラー : {0}行目 : {1}", intRowCount, dataread);
            }
          }
        }
        catch (Exception ex)
        {
          blnRet = false;
          _log.Error(ex.Message);
          return blnRet;
        }
        finally
        {
          // 解放処理
          sr.Close();
          sr = null;
        }

        // 特殊仕様の項目について変換処理を実行する
        int intRetSpec = usrDataset.Special_Specifications(convfile_flg, convpwd_flg);
        if (intRetSpec < 0)
        {
          blnRet = false;
          throw new Exception("特殊仕様項目の変換処理でエラーが発生しました。");
        }

        return blnRet;
      }
      catch (Exception ex)
      {
        _log.Error(ex.Message);
        return false;
      }
    }

    /// <summary>
    /// 利用者情報リスト取得
    /// </summary>
    /// <param name="lastday"></param>
    /// <param name="work">作業フォルダ</param>
    /// <param name="riyoushaList"></param>
    /// <returns></returns>
    public static bool GetRiyoushaList(DateTime lastday, string work, ref List<RiyoushaInfoEntity> riyoushaList)
    {
      // 文字コードを指定してファイルを読み込む
      StreamReader sr = null;

      try
      {
        // 旧字変換クラス準備
        if (!clsKanjiConversion.boolSet_Config(convfile))
        {
          throw new Exception("旧字変換設定ファイルが見つかりませんでした。");
        }

        sr = new StreamReader(Path.Combine(work, ftpFile), CommonParameter.CommonEnocode);

        while (sr.Peek() >= 0)
        {

          // 利用者情報インスタンス生成
          RiyoushaInfoEntity riyousha = new RiyoushaInfoEntity();

          try
          {
            // データを読み込む
            string dataread = sr.ReadLine();

            // 利用者情報番号格納
            riyousha.RiyoushaNo = dataread;
            // パスワードを格納
            riyousha.Password = dataread;
            // 利用者漢字氏名を格納
            riyousha.RiyoushaKanjiName = dataread;
            // 利用者カナ氏名を格納
            riyousha.RiyoushaKanaName = dataread;
            // 利用者英字氏名を格納
            riyousha.RiyoushaEijiName = dataread;
            // 生年月日を格納
            riyousha.SeinenGappi = dataread;
            // 性別を格納
            riyousha.Seibetu = dataread;
            // 電子メールアドレスを格納
            riyousha.DensiMailAdress = dataread;
            // ポケットベル番号を格納
            riyousha.PokettoBel = dataread;
            // 職種コードを格納
            riyousha.SyokusyuCode = dataread;
            // 職種名称を格納
            riyousha.SyokusyuName = dataread;
            // 担当入外区分を格納
            riyousha.TantouNyuugaiKbn = dataread;
            // 依頼医入力必須区分を格納
            riyousha.IraiiNyuuryokuHissuKbn = dataread;
            // 麻薬施用者番号を格納
            riyousha.DrugUserNo = dataread;
            // 人事IDを格納
            riyousha.JinjiId = dataread;
            // 病歴室IDを格納
            riyousha.ByourekisituId = dataread;
            // 所属部科コードを格納  1件目
            riyousha.SyozokubukaCode1 = dataread;
            // 所属部科名称を格納
            riyousha.SyozokubukaName1 = dataread;
            // 所属病棟コードを格納
            riyousha.SyozokubyoutouCode1 = dataread;
            // 所属病棟名称を格納
            riyousha.SyozokubyoutouName1 = dataread;
            // 所属病室コードを格納
            riyousha.SyozokubyousituCode1 = dataread;
            // 職制コードを格納
            riyousha.SyokuseiCode1 = dataread;
            // 有効開始日を格納
            riyousha.YuukouStartDay1 = dataread;
            // 有効終了日を格納
            riyousha.YuukouEndDay1 = dataread;
            // 所属部科コードを格納  2件目
            riyousha.SyozokubukaCode2 = dataread;
            // 所属部科名称を格納
            riyousha.SyozokubukaName2 = dataread;
            // 所属病棟コードを格納
            riyousha.SyozokubyoutouCode2 = dataread;
            // 所属病棟名称を格納
            riyousha.SyozokubyoutouName2 = dataread;
            // 所属病室コードを格納
            riyousha.SyozokubyousituCode2 = dataread;
            // 職制コードを格納
            riyousha.SyokuseiCode2 = dataread;
            // 有効開始日を格納
            riyousha.YuukouStartDay2 = dataread;
            // 有効終了日を格納
            riyousha.YuukouEndDay2 = dataread;
            // 所属部科コードを格納  3件目
            riyousha.SyozokubukaCode3 = dataread;
            // 所属部科名称を格納
            riyousha.SyozokubukaName3 = dataread;
            // 所属病棟コードを格納
            riyousha.SyozokubyoutouCode3 = dataread;
            // 所属病棟名称を格納
            riyousha.SyozokubyoutouName3 = dataread;
            // 所属病室コードを格納
            riyousha.SyozokubyousituCode3 = dataread;
            // 職制コードを格納
            riyousha.SyokuseiCode3 = dataread;
            // 有効開始日を格納
            riyousha.YuukouStartDay3 = dataread;
            // 有効終了日を格納
            riyousha.YuukouEndDay3 = dataread;
            // 所属部科コードを格納  4件目
            riyousha.SyozokubukaCode4 = dataread;
            // 所属部科名称を格納
            riyousha.SyozokubukaName4 = dataread;
            // 所属病棟コードを格納
            riyousha.SyozokubyoutouCode4 = dataread;
            // 所属病棟名称を格納
            riyousha.SyozokubyoutouName4 = dataread;
            // 所属病室コードを格納
            riyousha.SyozokubyousituCode4 = dataread;
            // 職制コードを格納
            riyousha.SyokuseiCode4 = dataread;
            // 有効開始日を格納
            riyousha.YuukouStartDay4 = dataread;
            // 有効終了日を格納
            riyousha.YuukouEndDay4 = dataread;
            // 所属部科コードを格納  5件目
            riyousha.SyozokubukaCode5 = dataread;
            // 所属部科名称を格納
            riyousha.SyozokubukaName5 = dataread;
            // 所属病棟コードを格納
            riyousha.SyozokubyoutouCode5 = dataread;
            // 所属病棟名称を格納
            riyousha.SyozokubyoutouName5 = dataread;
            // 所属病室コードを格納
            riyousha.SyozokubyousituCode5 = dataread;
            // 職制コードを格納
            riyousha.SyokuseiCode5 = dataread;
            // 有効開始日を格納
            riyousha.YuukouStartDay5 = dataread;
            // 有効終了日を格納
            riyousha.YuukouEndDay5 = dataread;
            // 特別職コードを格納
            riyousha.TokubetuSyokuCode = dataread;
            // 利用者有効期間を格納
            riyousha.RiyoushaYuukoukikan = dataread;
            // 有効期間開始日時を格納
            riyousha.YuukoukikanStartDay = dataread;
            // 有効期間終了日時を格納
            riyousha.YuukoukikanEndDay = dataread;
            // 更新者番号を格納
            riyousha.UpdateOwnerNo = dataread;
            // 更新日付を格納
            riyousha.UpdateDate = dataread;
            // 更新時刻を格納
            riyousha.UpdateTime = dataread;
            // 作成者番号を格納
            riyousha.CreateOwnerNo = dataread;
            // 作成日付を格納
            riyousha.CreateDate = dataread;
            // 作成時刻を格納
            riyousha.CreateTime = dataread;
            // 停止区分を格納
            riyousha.StopKbn = dataread;
            // 廃止区分を格納
            riyousha.AbolitionKbn = dataread;

            // 登録対象確認
            if (IsRegist(lastday, riyousha.UpdateDate))
            {
              riyoushaList.Add(riyousha);

              // 利用者情報を出力する
              _log.Debug(riyousha.ToString());

              // 旧字変換処理を行う
              _log.DebugFormat("【旧字変換前】{0}", riyousha.RiyoushaKanjiName);
              riyousha.SetRiyoushaKanjiName(clsKanjiConversion.Convert(riyousha.RiyoushaKanjiName));
              _log.DebugFormat("【旧字変換後】{0}", riyousha.RiyoushaKanjiName);
            }
            else
            {
              // 利用者情報を出力する
              _log.DebugFormat("更新日が古いため、処理対象外です。{0}", riyousha.ToString());
            }
          }
          catch (Exception ex)
          {
            _log.WarnFormat("【利用者番号】{0}【内容】{1}", riyousha.RiyoushaNo, ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        _log.Error(ex.Message);
        return false;
      }
      finally
      {
        if (sr != null)
        {
          // 解放処理
          sr.Close();
          sr = null;
        }
      }

      return true;
    }

    /// <summary>
    /// 更新日による登録対象確認
    /// </summary>
    /// <param name="lastday"></param>
    /// <param name="updatedate"></param>
    /// <returns>true:登録対象 false:登録対象外</returns>
    private static bool IsRegist(DateTime lastday, string updatedate)
    {
      DateTime upddt = DateTime.MinValue;

      // 更新日を日付形式に変換
      CommonUtil.ConvertDateTime(ToUsersInfo.ConvertMaxDate(updatedate), CommonParameter.YYYYMMDD, ref upddt);

      // 更新日が最終実行日以上の場合
      if (upddt != DateTime.MinValue && lastday <= upddt)
      {
        return true;
      }

      return false;
    }
  }
}
