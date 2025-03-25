using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Serv_UsersIFLinkage.Util
{
    /// <summary>
    /// 共通処理クラス
    /// </summary>
    class CommonUtil
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 設定ファイルより設定値を取得
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="table">設定ファイルテーブル</param>
        /// <returns>成否</returns>
        public static bool getNotEmptyAppConfigValue(string key, Hashtable table)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
            {
                string appConfigName = Path.GetFileName(Application.ExecutablePath) + ".config";
                _log.ErrorFormat("{0}の設定値が未設定です。key：{1}", appConfigName, key);
                return false;
            }
            _log.InfoFormat("key：{0}, value：{1}", key, value);
            table.Add(key, value);
            return true;
        }

        /// <summary>
        /// 設定ファイルより設定値を取得
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="table">設定ファイルテーブル</param>
        /// <returns>成否</returns>
        public static bool getAppConfigValue(string key, Hashtable table)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                string appConfigName = Path.GetFileName(Application.ExecutablePath) + ".config";
                _log.ErrorFormat("{0}の設定値が未設定です。key：{1}", appConfigName, key);
                return false;
            }
            _log.InfoFormat("key：{0}, value：{1}", key, value);
            table.Add(key, value);
            return true;
        }

        /// <summary>
        /// 処理モード取得
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetMode(string[] args)
        {
            if (args == null)
            {
                return string.Empty;
            }

            if (args.Length != 2)
            {
                return string.Empty;
            }

            return args[1].ToLower();
        }

        /// <summary>
        /// DateTime変換
        /// </summary>
        /// <param name="strdate"></param>
        /// <param name="format"></param>
        /// <param name="date"></param>
        public static bool ConvertDateTime(string strdate, string format, ref DateTime date)
        {
            if (DateTime.TryParseExact(strdate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// int?変換
        /// </summary>
        /// <param name="value"></param>
        public static int? ConvertStrToInt(string value)
        {
            int ret = 0;
            if (int.TryParse(value, out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }
    }
}
