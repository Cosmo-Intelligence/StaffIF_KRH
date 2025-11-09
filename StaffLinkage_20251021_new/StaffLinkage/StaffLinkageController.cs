using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using StaffLinkage.Exe;
using StaffLinkage.Exe.Entity;
using StaffLinkage.Util;

namespace StaffLinkage
{
  class StaffLinkageController
  {
    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// 差分取得用制御ファイル
    /// </summary>
    private static string modymdFile =
            Path.Combine(
                    Application.StartupPath,
                    AppConfigController.GetInstance().GetValueString(AppConfigParameter.UserModymdFile));

    /// <summary>
    /// SQLLoader処理 + システム日付フォルダ
    /// </summary>
    private static string todayWork = string.Empty;

    /// <summary>
    /// 差分取得日書式
    /// </summary>
    private static string modymdFormat = CommonParameter.YYYYMMDD;

    /// <summary>
    /// 最終実行日付
    /// </summary>
    private static DateTime lastday_from = DateTime.MinValue;
    // Y_Higuchi -- add --
    private static DateTime lastday_to = DateTime.MinValue;
    // Y_Higuchi -- add --

    /// <summary>
    /// システム日付
    /// </summary>
    private static DateTime today = DateTime.Today;

    /// <summary>
    /// 有効終了日 
    /// </summary>
    private static string yuko_ymd = AppConfigController.GetInstance().GetValueString(AppConfigParameter.YUKO_YMD);

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <returns>正常ならtrue、異常ならfalse</returns>
        public bool Execute()
    {
      _log.Info("初期処理を実行します。");
      // 初期処理
      if(!Init())
      {
        return false;
      }

      _log.Info("差分取得用制御ファイル取得処理を実行します。");
      // 差分取得用制御ファイル取得処理
      if(!GetModymdFile())
      {
        return false;
      }

      _log.Info("FTPダウンロード処理を実行します。");
      // FTPダウンロード処理
      if(!RiyoushaInfo.DownLoad(todayWork))
      {
        _log.Info("FTPダウンロード処理が失敗しました。");
        return false;
      }

      // 利用者情報リスト作成
//      List<RiyoushaInfoEntity> riyoushaList = new List<RiyoushaInfoEntity>();

      _log.Info("利用者情報データ取得処理を実行します。");
      // 利用者情報データ取得処理
      // Y_Higuchi -- del --
      //if (!RiyoushaInfo.GetRiyoushaList(lastday_from, todayWork, ref riyoushaList))
      //{
      //    _log.Info("利用者情報データ取得処理が失敗しました。");
      //    return false;
      //}
      // Y_Higuchi -- del --
      // Y_Higuchi -- add --
      UserInfo_Dataset rysList;
      if(!RiyoushaInfo.GetRiyoushaListII(lastday_from, lastday_to, todayWork, out rysList))
      {
        _log.Info("利用者情報データ取得処理が失敗しました。");
        return false;
      }
      // Y_Higuchi -- add --

      // ユーザ情報連携I/Fリスト
      List<ToUsersInfoEntity> toUsersInfoList = new List<ToUsersInfoEntity>();

      // Y_Higuchi -- add --
      _log.Info("電文：利用者情報->ToUsersInfoユーザ情報変換処理を実行します。");
      ToUsersInfo_act tuia = new ToUsersInfo_act();
      if(!tuia.Convert_ToUsersInfo(today, rysList, ref toUsersInfoList))
      {
        _log.Info("電文：利用者情報->ToUsersInfoユーザ情報変換処理が失敗しました。");
        return false;
      }
      // Y_Higuchi -- add --

      // Y_Higuchi -- del --
      //_log.Info("ユーザ情報連携I/Fマッピング処理を実行します。");
      //         // ユーザ情報連携I/Fマッピング処理
      //if(!ToUsersInfo.Mapping(today, riyoushaList, ref toUsersInfoList))
      //{
      //  _log.Info("ユーザ情報連携I/Fマッピング処理が失敗しました。");
      //  return false;
      //}
      // Y_Higuchi -- del --

      // 破棄
      //riyoushaList.Clear();
      //riyoushaList = null;
      GC.Collect();

      _log.Info("ユーザ情報連携I/F登録用CSVファイル出力処理を実行します。");
      // ユーザ情報連携I/FCSVファイル出力処理
      if(!ToUsersInfo.Output(toUsersInfoList, todayWork))
      {
        _log.Info("ユーザ情報連携I/F登録用CSVファイル出力処理が失敗しました。");
        return false;
      }

      // 破棄
      toUsersInfoList = null;
      GC.Collect();

      _log.Info("ユーザ情報連携I/F登録処理を実行します。");
      // ユーザ情報連携I/F登録処理
      if(!ToUsersInfo.Insert(todayWork))
      {
        _log.Info("ユーザ情報連携I/F登録処理が失敗しました。");
        return false;
      }

      _log.Info("差分取得用制御ファイル更新処理を実行します。");
      // 差分取得用制御ファイル更新処理
      if(!UpdateModymdFile())
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    private static bool Init()
    {
      // ログフォルダ削除
      Logger.Delete();

      try
      {
        // SQLLoader処理フォルダ保持期間を取得
        string day = AppConfigController.GetInstance().GetValueString(AppConfigParameter.SqlldrFolderKeepDays);
        _log.DebugFormat("SQLLoader処理フォルダ保持期間：{0}", day);

        int keepdays = 0;

        // 数値変換
        if(!int.TryParse(day, out keepdays) || keepdays < 1)
        {
          keepdays = 7;
        }

        // SQLLoader処理フォルダ取得
        string work = AppConfigController.GetInstance().GetValueString(AppConfigParameter.SqlldrFolder);
        _log.DebugFormat("SQLLoader処理フォルダ：{0}", work);
        // ディレクトリ存在チェック
        if(!Directory.Exists(work))
        {
          // 存在しない場合は作成する
          Directory.CreateDirectory(work);
          _log.DebugFormat("フォルダ作成しました。：{0}", work);
        }

        // SQLLoader処理 + システム日付フォルダ取得
        todayWork = Path.Combine(work, today.ToString(CommonParameter.YYYYMMDD));
        _log.DebugFormat("SQLLoader処理 + システム日付フォルダ：{0}", todayWork);

        // 日付フォルダの存在チェック
        if(!Directory.Exists(todayWork))
        {
          // 存在しない場合は作成する
          Directory.CreateDirectory(todayWork);
          _log.DebugFormat("フォルダ作成しました。：{0}", todayWork);
        }

        // サブフォルダの取得
        foreach(string sub in Directory.GetDirectories(work))
        {
          try
          {
            DateTime ChkDate = DateTime.ParseExact(Path.GetFileName(sub), CommonParameter.YYYYMMDD, null);

            // 保持期間判定
            if(DateTime.Today.AddDays(-keepdays) >= ChkDate)
            {
              // フォルダを削除する
              Directory.Delete(sub, true);
            }
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

      return true;
    }
    /// <summary>
    /// 差分取得日時ファイルデータ取得処理
    /// </summary>
    /// <returns></returns>
    private static bool GetModymdFile()
    {
      // ファイル存在確認
      if(!File.Exists(modymdFile))
      {
        // Y_higuchi -- add --
        _log.ErrorFormat("差分制御ファイルが存在しません。({0})", modymdFile);
        return false;
        // Y_higuchi -- add --
        // Y_higuchi -- del --
        // return false;
        // Y_higuchi -- del --
      }

      // ファイル読込
      StreamReader sr = null;

      try
      {
        sr = new StreamReader(modymdFile);
        string modymd = sr.ReadToEnd();
        // 最終実行日を日付形式に変換
        // Y_Higuchi -- add --
        string[] ymdfromto = modymd.Split('-');
        if(!CommonUtil.ConvertDateTime(ymdfromto[0], modymdFormat, ref lastday_from))
        {
          _log.Error("差分制御ファイルのfrom日付が不正です。([from日付=yyyyMMdd:数字8桁]-[to日付=yyyyMMdd:数字8桁] 全17桁で指定してください。)");
          return false;
        }
        if(!CommonUtil.ConvertDateTime(ymdfromto[1], modymdFormat, ref lastday_to))
        {
          _log.Error("差分制御ファイルのto日付が不正です。([from日付=yyyyMMdd:数字8桁]-[to日付=yyyyMMdd:数字8桁] 全17桁で指定してください。)");
          return false;
        }
        if(lastday_from > lastday_to)
        {
          _log.Error("差分制御ファイルの定義が不正です。([from日付=yyyyMMdd:数字8桁]-[to日付=yyyyMMdd:数字8桁] 全17桁で指定してください。)");
          return false;
        }
        // Y_Higuchi -- add --
        // Y_Higuchi -- del --
        // CommonUtil.ConvertDateTime(modymd, modymdFormat, ref lastday_from);
        // Y_Higuchi -- del --
      }
      catch(Exception ex)
      {
        _log.Error(ex.ToString());
        return false;
      }
      finally
      {
        if(sr != null)
        {
          sr.Close();
        }
      }

      return true;
    }

    /// <summary>
    /// 差分取得日時ファイル作成・更新処理
    /// </summary>
    /// <returns></returns>
    private static bool UpdateModymdFile()
    {
      // ファイル書込み
      StreamWriter sw = null;
      try
      {
        sw = new StreamWriter(modymdFile, false);
        // Y_Higuchi -- add --
        string strbuf = "";
        // 次回取込期間 = 前回実行日から１年間
        //strbuf = DateTime.Now.ToString(modymdFormat) + "-" + DateTime.Now.AddYears(1).ToString(modymdFormat);
        // 次回取込期間 = 設定ファイルから取得
        strbuf = DateTime.Now.ToString(modymdFormat) + "-" + yuko_ymd;
                sw.Write(strbuf);
        // Y_Higuchi -- add --
        // Y_Higuchi -- del --
        // sw.Write(DateTime.Now.ToString(modymdFormat));
        // Y_Higuchi -- del --
      }
      catch(Exception ex)
      {
        _log.Error(ex.ToString());
      }
      finally
      {
        if(sw != null)
        {
          sw.Close();
        }
      }

      return true;
    }
  }
}
