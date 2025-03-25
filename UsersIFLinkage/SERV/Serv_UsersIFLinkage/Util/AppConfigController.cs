using System;
using System.Collections;

namespace Serv_UsersIFLinkage.Util
{
	/// <summary>
	/// 設定ファイル管理クラス
	/// </summary>
    public sealed class AppConfigController
	{
        /// <summary>
        /// インスタンス
        /// </summary>
        public static AppConfigController instance = new AppConfigController();

        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private static object syncRoot = new Object();

        /// <summary>
        /// 設定ファイルテーブル
        /// </summary>
        private Hashtable appConfigTable = null;

        /// <summary>
        /// AppConfigControllerクラスのインスタンスを取得する
        /// </summary>
        /// <returns>AppConfigControllerクラスのインスタンス</returns>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        static public AppConfigController GetInstance()
        {
            lock (syncRoot)
            {
                if (instance == null)
                {
                    instance = new AppConfigController();
                }
            }
            return instance;
        }

		/// <summary>
		/// 設定ファイルの項目を設定する
		/// </summary>
        /// <param name="val">設定ファイルテーブル</param>
        public void SetEAppConfigTableImpl(Hashtable val)
		{
            this.appConfigTable = val;
		}

		/// <summary>
		/// 設定値を取得する
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>設定値</returns>
		public string GetValueString(string key)
		{
			string ret = "";

            if ((this.appConfigTable != null) && (this.appConfigTable.Contains(key)))
			{
                ret = this.appConfigTable[key].ToString();
			}

			return ret;
		}

    // Y_Higuchi -- add --
    /// <summary>
    /// string型 設定値を取得する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="dfv">変換できなかった時のデフォルト値</param>
    /// <returns></returns>
    public string GetValueString2(string key, string dfv)
    {
      string strbuf;
      try
      {
        strbuf = GetValueString(key);
      }
      catch
      {
        strbuf = dfv;
      }
      return strbuf;
    } // Y_Higuchi -- add --

    // Y_Higuchi -- add --
    /// <summary>
    /// int型 設定値を取得する
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="dfv">変換できなかった時のデフォルト値</param>
    /// <param name="minv">最小値</param>
    /// <param name="maxv">最大値</param>
    /// <returns></returns>
    public int GetValueInt(string key, int dfv, int minv = int.MinValue, int maxv = int.MaxValue)
    {
      int intbuf;
      try
      {
        string strbuf = GetValueString(key);
        if (int.TryParse(strbuf, out intbuf)) 
        {
          if ((intbuf < minv) || (intbuf > maxv))
          {
            // 規定の範囲を超えている場合
            intbuf = dfv;
          }
        }
        else 
        {
          // int変換できなかった場合
          intbuf = dfv;
        }
      }
      catch
      {
        intbuf = dfv;
      }
      return intbuf;
    } // Y_Higuchi -- add --

    /// <summary>
    /// 設定値を取得する
    /// </summary>
    /// <param name="key">キー</param>
    /// <returns>設定値</returns>
    public object GetValue(string key)
        {
            object ret = "";

            if ((this.appConfigTable != null) && (this.appConfigTable.Contains(key)))
            {
                ret = this.appConfigTable[key];
            }

            return ret;
        }
    }
}
