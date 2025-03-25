using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using Report_UsersIFLinkage.Ctrl;
using Report_UsersIFLinkage.Data.Import.Common;
using Report_UsersIFLinkage.Frm;
using Report_UsersIFLinkage.Util;

namespace Report_UsersIFLinkage
{
    class Program
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        static void Main()
        {
            _log.Info("アプリケーションを開始します。");

            // 二重起動にならないか確認する
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                _log.Error("アプリケーションを多重起動しようとした為、アプリケーションを強制終了します。");
                //処理を終了する
                return;
            }

            // UsersIFLinkage.exe.config読込み
            Hashtable appConfigTable = new Hashtable();
            if (!CreateAppConfigParameter(appConfigTable))
            {
                return;
            }

            AppConfigController.GetInstance().SetEAppConfigTableImpl(appConfigTable);

            // UnicodeSQ.txt情報保管
            if (!SetUnicodeSqFile(appConfigTable))
            {
                return;
            }

            string[] args = Environment.GetCommandLineArgs();
            // 処理モード取得
            string mode = CommonUtil.GetMode(args);

            // 処理モード判定
            if (!string.IsNullOrEmpty(mode))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    if (mode == CommonParameter.MODE_GUI)
                    {
                        // GUIモード
                        Application.Run(new frmNotifyIcon());
                    }
                    else if (mode == CommonParameter.MODE_TASK)
                    {
                        // タスクスケジューラモード
                        UsersIFLinkageController.Linkage();
                    }
                    else
                    {
                        throw new Exception("処理モードが不正です。MODE:" + mode);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    _log.Fatal(ex);
                    return;
                }
                finally
                {
                    _log.Info("アプリケーションを終了します。");
                }
            }

            // サービス起動
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ServiceMain() 
			};
            ServiceBase.Run(ServicesToRun);
        }

        /// <summary>
        /// 設定値をHashtableに保存
        /// </summary>
        /// <param name="param">設定ファイルテーブル</param>
        /// <returns>false : 不正</returns>
        private static bool CreateAppConfigParameter(Hashtable table)
        {
            // アプリ必須項目
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.USER_Conn, table))  { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.MRMS_Conn, table))  { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.QueueKeepDays, table))     { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.QueueDeleteStatus, table)) { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.GetQueueCount, table))     { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.ThreadInterval, table))    { return false; }

            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.SQ_UNICODE_LIST_FILE, table)) { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.AUTHUSER_Report, table)) { return false; }

            // アプリ項目
            if (!CommonUtil.getAppConfigValue(AppConfigParameter.LogKeepDays, table)) { return false; }

            // MRMS項目
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.MRMS_CONVERT_MD5, table))           { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.MRMS_CONVERT_GAIJI, table))         { return false; }
            if (!CommonUtil.getNotEmptyAppConfigValue(AppConfigParameter.MRMS_GAIJI_REPLACE, table))         { return false; }
            // Y_Higuchi --del -- if (!CommonUtil.getAppConfigValue(AppConfigParameter.MRMS_USERMANAGE_UPD_COLS, table))           { return false; }
            if (!CommonUtil.getAppConfigValue(AppConfigParameter.MRMS_USERAPPMANAGE_LICENCETOUSE, table))    { return false; }
            if (!CommonUtil.getAppConfigValue(AppConfigParameter.MRMS_USERINFO_CA_ATTRIBUTE_DEFAULT, table)) { return false; }
            if (!CommonUtil.getAppConfigValue(AppConfigParameter.MRMS_ATTRMANAGE_TEXTVALUE_DEFAULT, table))  { return false; }

            return true;
        }

        /// <summary>
        /// SQ登録対象のUnicode文字リストセット
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static bool SetUnicodeSqFile(Hashtable table)
        {
            try
            {
                // ファイル名取得
                string file = table[AppConfigParameter.SQ_UNICODE_LIST_FILE].ToString();

                // パス設定
                file = Path.Combine(Application.StartupPath, file);

                // UnicodeSQ.txt読込み
                List<Int32> unicodeList = new List<Int32>();
                if (!ImportUtil.ReadUnicodeSqFile(file, ref unicodeList))
                {
                    return false;
                }

                // 再格納
                table[AppConfigParameter.SQ_UNICODE_LIST_FILE] = unicodeList;
            }
            catch(Exception ex)
            {
                _log.ErrorFormat(ex.ToString());
            }

            return true;
        }

        /// <summary>
        /// GUIモード判定処理
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool IsGuiMode(string[] args)
        {
            if (args == null)
            {
                return false;
            }

            if (args.Length != 2)
            {
                return false;
            }

            if (args[1] != "gui")
            {
                return false;
            }

            return true;
        }
    }
}
