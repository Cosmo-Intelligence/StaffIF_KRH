#define pwdcnv_dll
//#undef pwdcnv_dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
#if pwdcnv_dll
using ExBSecurity;
#endif
using StaffLinkage.Exe.Entity;
using StaffLinkage.Util;
using System.Collections;

namespace StaffLinkage.Exe
{
  class ToUsersInfo
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

#if pwdcnv_dll
    /// <summary>
    /// 複合化DLL
    /// </summary>
    private static OutProc outproc = new OutProc();
#endif

    /// <summary>
    /// DBの種類を配列で格納
    /// </summary>
    private static string[] DB =
                (AppConfigController.GetInstance().GetValueString(AppConfigParameter.DB)).Split(',');

    /// <summary>
    /// 職員コード変換 初期値取得
    /// </summary>
    private static string Default =
            AppConfigController.GetInstance().GetValueString(AppConfigParameter.DEFAULT);

    /// <summary>
    /// 出力ファイル文字コードを取得
    /// </summary>
    private static Encoding OutputEnocode = CommonParameter.CommonEnocode;

    /// <summary>
    /// CSVファイル名
    /// </summary>
    private static string csv = ToUsersInfoEntity.EntityName + ".csv";

    #endregion

    #region function

    ///// <summary>
    ///// マッピング処理
    ///// </summary>
    ///// <param name="today"></param>
    ///// <param name="riyoushaList"></param>
    ///// <param name="toUsersList"></param>
    ///// <returns>正常ならtrue、異常ならfalse</returns>
    //public static bool Mapping(DateTime today, List<RiyoushaInfoEntity> riyoushaList, ref List<ToUsersInfoEntity> toUsersList)
    //{
    //  try
    //  {
    //    // ユーザ情報分格納
    //    for(int i = 0; i < riyoushaList.Count; i++)
    //    {
    //      // DB種類分格納
    //      for(int n = 0; n < DB.Length; n++)
    //      {
    //        ToUsersInfoEntity tousersinfo = new ToUsersInfoEntity();

    //        try
    //        {
    //          // 暫定文字を格納
    //          tousersinfo.RequestId = DateTime.Now.Day.ToString();

    //          // 暫定文字を格納
    //          tousersinfo.RequestDate = DateTime.Now.ToString();

    //          // DBを格納
    //          tousersinfo.Db = DB[n];

    //          // DBのAPPCODEを格納
    //          tousersinfo.AppCode = AppConfigController.GetInstance().GetValueString(DB[n]);

    //          // 利用者番号を格納
    //          tousersinfo.UserId = riyoushaList[i].RiyoushaNo;

    //          // 病院IDに空文字を格納
    //          tousersinfo.HospitalId = string.Empty;

    //          // 複合化したパスワードを格納
    //          tousersinfo.Password = Decrypt(riyoushaList[i].Password);

    //          // 利用者漢字氏名を格納
    //          tousersinfo.UserNameKanji = riyoushaList[i].RiyoushaKanjiName;

    //          // 利用者英字氏名を格納
    //          tousersinfo.UserNameEng = riyoushaList[i].RiyoushaEijiName;

    //          // 所属部科コード １件目を格納
    //          tousersinfo.Section_Id = riyoushaList[i].SyozokubukaCode1;

    //          // 所属部科名称 １件目を格納
    //          tousersinfo.Section_Name = riyoushaList[i].SyozokubukaName1;

    //          // 所属部科コード １～５件目を格納
    //          tousersinfo.Tanto_Section_Id = GetTantoSectionId(tousersinfo.Tanto_Section_Id, riyoushaList[i].SyozokubukaCode1);
    //          tousersinfo.Tanto_Section_Id += GetTantoSectionId(tousersinfo.Tanto_Section_Id, riyoushaList[i].SyozokubukaCode2);
    //          tousersinfo.Tanto_Section_Id += GetTantoSectionId(tousersinfo.Tanto_Section_Id, riyoushaList[i].SyozokubukaCode3);
    //          tousersinfo.Tanto_Section_Id += GetTantoSectionId(tousersinfo.Tanto_Section_Id, riyoushaList[i].SyozokubukaCode4);
    //          tousersinfo.Tanto_Section_Id += GetTantoSectionId(tousersinfo.Tanto_Section_Id, riyoushaList[i].SyozokubukaCode5);

    //          // 利用者番号を格納
    //          tousersinfo.StaffId = riyoushaList[i].RiyoushaNo;

    //          // 職種コードを格納 (初期値)
    //          tousersinfo.Syokuin_Kbn = Default;

    //          // 設定ファイルより職種コードが取得できた場合
    //          string Syokuin_Kbn = ConfigurationManager.AppSettings[(riyoushaList[i].SyokusyuCode)];

    //          if(!string.IsNullOrEmpty(Syokuin_Kbn))
    //          {
    //            tousersinfo.Syokuin_Kbn = Syokuin_Kbn;
    //          }

    //          // ポケットベル番号を格納
    //          tousersinfo.Tel = riyoushaList[i].PokettoBel;

    //          DateTime enddt = DateTime.MinValue;

    //          // パスワード有効期限が日付形式の場合
    //          if(CommonUtil.ConvertDateTime(ConvertMaxDate(riyoushaList[i].YuukoukikanEndDay), CommonParameter.YYYYMMDD, ref enddt))
    //          {
    //            // パスワード有効期限を格納
    //            tousersinfo.PasswordExpiryDate = enddt.ToString(CommonParameter.YYYYMMDD);
    //            // 1ヵ月前の日時を格納
    //            tousersinfo.PasswordWarningDate = enddt.AddMonths(-1).ToString(CommonParameter.YYYYMMDD);
    //          }
    //          else
    //          {
    //            // 空文字を格納
    //            tousersinfo.PasswordExpiryDate = string.Empty;
    //            // 空文字を格納
    //            tousersinfo.PasswordWarningDate = string.Empty;
    //          }

    //          // 有効フラグ「有効：1」を格納
    //          tousersinfo.UserIdValidityFlag = ToUsersInfoEntity.USERID_VALIDITY_FLAG_TRUE;

    //          // 有効期限終了日がシステム日時より過去の場合
    //          if(enddt != DateTime.MinValue && enddt < today)
    //          {
    //            // 有効フラグ「無効：0」を格納
    //            tousersinfo.UserIdValidityFlag = ToUsersInfoEntity.USERID_VALIDITY_FLAG_FALSE;
    //          }

    //          // 処理種別ID「登録：US01」を格納
    //          tousersinfo.RequestType = ToUsersInfoEntity.REQUESTTYPE_US01;

    //          // 使用停止フラグが「使用停止：1」の場合
    //          if(riyoushaList[i].StopKbn == RiyoushaInfoEntity.SIYOUTEISIFLG_TRUE)
    //          {
    //            // 処理種別ID「削除：US99」を格納
    //            tousersinfo.RequestType = ToUsersInfoEntity.REQUESTTYPE_US99;
    //          }

    //          // 空文字を格納
    //          tousersinfo.MessageId1 = string.Empty;

    //          // 空文字を格納
    //          tousersinfo.MessageId2 = string.Empty;

    //          // 空文字を格納
    //          tousersinfo.MessageId3 = string.Empty;

    //          // 未送信(固定)を格納
    //          tousersinfo.TransferStatus = ToUsersInfoEntity.TRANSFERSTATUS_00;

    //          // 空文字を格納
    //          tousersinfo.TransferDate = string.Empty;

    //          // 空文字を格納
    //          tousersinfo.TransferResult = string.Empty;

    //          // 空文字を格納
    //          tousersinfo.TransferText = string.Empty;

    //          // 登録対象確認
    //          if(IsRegist(tousersinfo.Db, tousersinfo.Syokuin_Kbn, tousersinfo.Section_Id))
    //          {
    //            toUsersList.Add(tousersinfo);

    //            // ユーザ連携情報を出力する
    //            _log.Debug(tousersinfo.ToString());
    //          }
    //          else
    //          {
    //            // ユーザ連携情報を出力する
    //            _log.DebugFormat("職員区分と診療科IDが不一致のため、取込対象外です。{0}", tousersinfo.ToString());
    //          }
    //        }
    //        catch(Exception ex)
    //        {
    //          _log.WarnFormat("【ユーザID】{0}【内容】{1}", tousersinfo.UserId, ex.Message);
    //        }
    //      }
    //    }
    //  }
    //  catch(Exception ex)
    //  {
    //    _log.Error(ex.Message);
    //    return false;
    //  }

    //  return true;
    //}

    /// <summary>
    /// CSVファイル出力処理
    /// </summary>
    /// <param name="toUsersInfoList"></param>
    /// <param name="work"></param>
    /// <returns></returns>
    public static bool Output(List<ToUsersInfoEntity> toUsersInfoList, string work)
    {
      StreamWriter write = null;
      StringBuilder sb = new StringBuilder();

      try
      {
        // 文字コードを指定して書き込む
        write = new StreamWriter(Path.Combine(work, csv), false, OutputEnocode);

        // ヘッダー作成
        sb.Append(CustomizeColumn("RequestId") + ",");
        sb.Append(CustomizeColumn("RequestDate") + ",");
        sb.Append(CustomizeColumn("Db") + ",");
        sb.Append(CustomizeColumn("AppCode") + ",");
        sb.Append(CustomizeColumn("UserId") + ",");
        sb.Append(CustomizeColumn("HospitalId") + ",");
        sb.Append(CustomizeColumn("Password") + ",");
        sb.Append(CustomizeColumn("UserNameKanji") + ",");
        sb.Append(CustomizeColumn("UserNameEng") + ",");
        sb.Append(CustomizeColumn("Section_Id") + ",");
        // Y_Higuchi -- del --
        //sb.Append(CustomizeColumn("Section_Name") + ",");
        // Y_Higuchi -- del --
        sb.Append(CustomizeColumn("Tanto_Section_Id") + ",");
        sb.Append(CustomizeColumn("StaffId") + ",");
        sb.Append(CustomizeColumn("Syokuin_Kbn") + ",");
        sb.Append(CustomizeColumn("Tel") + ",");
        sb.Append(CustomizeColumn("PasswordExpiryDate") + ",");
        sb.Append(CustomizeColumn("PasswordWarningDate") + ",");
        sb.Append(CustomizeColumn("UserIdValidityFlag") + ",");
        sb.Append(CustomizeColumn("RequestType") + ",");
        sb.Append(CustomizeColumn("MessageId1") + ",");
        sb.Append(CustomizeColumn("MessageId2") + ",");
        sb.Append(CustomizeColumn("MessageId3") + ",");
        sb.Append(CustomizeColumn("TransferStatus") + ",");
        sb.Append(CustomizeColumn("TransferDate") + ",");
        sb.Append(CustomizeColumn("TransferResult") + ",");
        sb.Append(CustomizeColumn("TransferText"));

        // CSVファイルに書き出し
        write.WriteLine(sb.ToString());

        // 解放処理
        sb.Remove(0, sb.Length);

        foreach(ToUsersInfoEntity toUsersInfo in toUsersInfoList)
        {
          try
          {
            sb.Append(CustomizeColumn(toUsersInfo.RequestId) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.RequestDate) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.Db) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.AppCode) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.UserId) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.HospitalId) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.Password) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.UserNameKanji) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.UserNameEng) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.Section_Id) + ",");
            // Y_Higuchi -- del --
            //sb.Append(CustomizeColumn(toUsersInfo.Section_Name) + ",");
            // Y_Higuchi -- del --
            sb.Append(CustomizeColumn(toUsersInfo.Tanto_Section_Id) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.StaffId) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.Syokuin_Kbn) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.Tel) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.PasswordExpiryDate) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.PasswordWarningDate) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.UserIdValidityFlag) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.RequestType) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.MessageId1) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.MessageId2) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.MessageId3) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.TransferStatus) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.TransferDate) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.TransferResult) + ",");
            sb.Append(CustomizeColumn(toUsersInfo.TransferText));

            // CSVファイルに書き出し
            write.WriteLine(sb.ToString());

            // 解放処理
            sb.Remove(0, sb.Length);
          }
          catch(Exception ex)
          {
            _log.Warn(ex.Message);
          }
        }
      }
      catch(Exception ex)
      {
        _log.Error(ex.Message);
        return false;
      }
      finally
      {
        if(write != null)
        {
          // 解放処理
          write.Close();
        }
      }
      return true;
    }

    /// <summary>
    /// 登録処理
    /// </summary>
    /// <param name="work"></param>
    /// <returns></returns>
    public static bool Insert(string work)
    {
      // Y_Higuchi -- add --
      bool blnret = false;
      foreach(string strDB in DB)
      {
        blnret = SqlLoader.ProcSqlLoader(work, ToUsersInfoEntity.EntityName, strDB);
        if(!blnret)
        {
          return blnret;
        }
      }
      return blnret;
      // Y_Higuchi -- add --
      // Y_Higuchi -- del --
      //return SqlLoader.ProcSqlLoader(work, ToUsersInfoEntity.EntityName);
      // Y_Higuchi -- del --
    }

    /// <summary>
    /// ダブルコーテーションを付ける
    /// </summary>
    /// <param name="column"></param>
    /// <returns>カスタマイズしたカラム名</returns>
    private static string CustomizeColumn(string column)
    {
        column = @"""" + EscapeString(column) + @"""";
        return column;
    }

    /// <summary>
    /// 入力値をエスケープする
    /// <param name="param"></param>
    /// <returns></returns>
    /// </summary>
    private static string EscapeString(string param)
    {
        if (param == null)
        {
            return param;
        }

        param = param.Replace(@"""", @"""""");

        return param;
    }

    /// <summary>
    /// パスワード複合化
    /// </summary>
    /// <param name="strpassword"></param>
    /// <returns>複合化したパスワード</returns>
    private static string Decrypt(string strpassword)
    {
#if pwdcnv_dll
      int ret = outproc.DecodeSecretCode(ref strpassword);
#else
			int ret = 0;
#endif
      if(ret == -1)
      {
        throw new Exception("複合化に失敗しました。");
      }

      return strpassword;
    }

    /// <summary>
    /// 登録対象確認
    /// </summary>
    /// <param name="Db"></param>
    /// <param name="syokuinkbn"></param>
    /// <param name="sectionid"></param>
    /// <returns>true:登録対象 false:登録対象外</returns>
    private static bool IsRegist(string Db, string syokuinkbn, string sectionid)
    {
      // 登録対象「横河職員区分|診療科ID」リスト取得
      string registList =
              ConfigurationManager.AppSettings[AppConfigParameter.IMPORT_DB + Db];

      // 値チェック
      if(string.IsNullOrEmpty(registList))
      {
        return true;
      }

      foreach(string regist in registList.Split(','))
      {
        // 要素[0] = 職員区分, 要素[1] = 診療科ID
        string[] registSet = regist.Split('=');

        // 職員区分が一致するか確認
        if(registSet[0] == syokuinkbn)
        {
          // 診療科IDが未設定か確認
          if(registSet.Length == 1)
          {
            return true;
          }

          //// 職員連携．診療科IDが未設定か確認
          //if (string.IsNullOrEmpty(sectionid))
          //{
          //    return true;
          //}

          // 診療科IDと一致するか確認
          if(Array.IndexOf(registSet[1].Split('|'), sectionid) > -1)
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// 最大日付変換
    /// </summary>
    /// <param name="enddate"></param>
    /// <returns></returns>
    public static string ConvertMaxDate(string enddate)
    {
      if(enddate == RiyoushaInfoEntity.MAX_DATE)
      {
        return ToUsersInfoEntity.MAX_DATE_CONV;
      }

      return enddate;
    }

    /// <summary>
    /// 担当科ID取得
    /// </summary>
    /// <param name="tanto_Section_Id"></param>
    /// <param name="syozokubukacode"></param>
    /// <returns></returns>
    private static string GetTantoSectionId(string tanto_Section_Id, string syozokubukacode)
    {
      string ret = string.Empty;

      if(!string.IsNullOrEmpty(syozokubukacode))
      {
        if(!string.IsNullOrEmpty(tanto_Section_Id))
        {
          ret += ",";
        }

        ret += syozokubukacode;
      }

      return ret;
    }

    #endregion
  }
}
