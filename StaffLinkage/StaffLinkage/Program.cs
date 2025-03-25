using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using StaffLinkage.Frm;
using StaffLinkage.Util;

namespace StaffLinkage
{
  class Program
  {
    // ログ出力
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    static void Main()
    {
      // 二重起動にならないか確認する
      if(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
      {
        _log.Error("アプリケーションを多重起動しようとした為、アプリケーションを強制終了します。");
        //処理を終了する
        return;
      }

      Program proc = new Program();
      proc.StartApplication();
    }

    /// <summary>
    /// 起動処理
    /// </summary>
    private void StartApplication()
    {
      _log.Info("アプリケーションを起動します。");

      try
      {
        // StaffLinkage.exe.config読込み
        Hashtable appConfigTable = new Hashtable();
        if(!CreateAppConfigParameter(appConfigTable))
        {
          return;
        }
        AppConfigController.GetInstance().SetEAppConfigTableImpl(appConfigTable);

        // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
        // 処理モード判定および各モードでの稼働を追加
        // 処理モード取得
        string[] args = Environment.GetCommandLineArgs();
        string mode = CommonUtil.GetMode(args);

        // 処理モード判定
        if(!string.IsNullOrEmpty(mode))
        {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);

          if(mode == CommonParameter.MODE_GUI)
          {
            // GUIモード
            Application.Run(new frmNotifyIcon());
          }
          else if(mode == CommonParameter.MODE_TASK)
          {
            // Y_Higuchi -- change -- 参考=sa_相模原協同病院 --
            //// タスクスケジューラモード
            //StaffLinkageController.Linkage();
            // Y_Higuchi -- change -- 参考=sa_相模原協同病院 --

            // Controllerクラスを新規生成
            StaffLinkageController staff = new StaffLinkageController();
            // 開始
            staff.Execute();
          }
          else
          {
            new Exception("処理モードが不正です。MODE:" + mode);
          }
          return;
        }
      }
      catch(Exception ex)
      {
        _log.Fatal(ex.Message);
        return;
      }
      finally
      {
        _log.Info("アプリケーションを終了します。");
      }
        // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --

      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
      // プロジェクトへファイル追加：ProjectInstaller.cs
      // プロジェクトへファイル追加：ServiceMain.cs 
      // プロジェクトへフォルダ&ファイル追加：Frm
      // プロジェクトへフォルダ&ファイル追加：Icon
      // プロジェクトへフォルダ&ファイル追加：Install
      // サービス起動
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[] { new ServiceMain() };
      ServiceBase.Run(ServicesToRun);
      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --

    }

    /// <summary>
    /// 設定値をHashtableに保存
    /// </summary>
    /// <param name="param">設定ファイルテーブル</param>
    /// <returns>false : 不正</returns>
    private bool CreateAppConfigParameter(Hashtable table)
    {
      // NotEmpty項目
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpIPAdress, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpUser, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpPassword, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpRetryCount, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpEncode, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpFolder, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.FtpFile, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.SqlldrConnectionString, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.SqlldrFolder, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.SqlldrFolderKeepDays, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.ConvKanjiFile, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.UserModymdFile, table))
      {
        return false;
      }
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.DB, table))
      {
        return false;
      }

      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
      // スレッドの間隔
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.ThreadInterval, table))
      {
        return false;
      }
      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --

      // Y_Higuchi -- add --
      // 電文マッピングファイル(空NG)
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.TelegramMap, table))
      {
        return false;
      }
      // パスワード変換あり/なし(空NG)
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.PasswordChange, table))
      {
        return false;
      }
      // 旧字変換あり/なし(空NG)
      if(!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.ConvKanji, table))
      {
        return false;
      }
      // Y_Higuchi -- add --

      // APPCODE は使用しない
      // Y_Higuchi -- del --
      //// Empty許容項目
      //string[] Db = table[AppConfigParameter.DB].ToString().Split(',');
      //         for (int i = 0; i < Db.Length; i++)
      //         {
      //             if (!CommonUtil.getAppConfigValue(Db[i], table)) { return false; }
      //         }
      // Y_Higuchi -- del --

      // 職員区分デフォルト値
      if(!CommonUtil.getAppConfigValue(AppConfigParameter.DEFAULT, table))
      {
        return false;
      }
      // Logファイル保存期間
      if(!CommonUtil.getAppConfigValue(AppConfigParameter.LogKeepDays, table))
      {
        return false;
      }

      return true;
    }
  }
}
