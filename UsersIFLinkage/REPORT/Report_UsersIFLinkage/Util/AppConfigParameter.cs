
namespace Report_UsersIFLinkage.Util
{
	/// <summary>
	/// アプリケーションコンフィグ定義
	/// </summary>
	public sealed class AppConfigParameter
    {
        #region 共通

        /// <summary>
        /// USER接続文字列
        /// </summary>
        public static string USER_Conn = "USER_ConnectionString";

        /// <summary>
        /// MRMS接続文字列
        /// </summary>
        public static string MRMS_Conn = "MRMS_ConnectionString";

        /// <summary>
        /// ログフォルダ保持期間(日数)
        /// </summary>
        public static string LogKeepDays = "LogKeepDays";

        /// <summary>
        /// TOUSERSINFOレコード保持期間(日数)
        /// </summary>
        public static string QueueKeepDays = "QueueKeepDays";
        
        /// <summary>
        /// TOUSERSINFOレコード削除対象ステータス
        /// </summary>
        public static string QueueDeleteStatus = "QueueDeleteStatus";
        
        /// <summary>
        /// TOUSERSINFOレコード取得件数
        /// </summary>
        public static string GetQueueCount = "GetQueueCount";

        /// <summary>
        /// スレッド待機時間(ミリ秒)
        /// </summary>
        public static string ThreadInterval = "ThreadInterval";

        /// <summary>
        /// 外字変換対象外文字(Unicode)リストファイル名
        /// </summary>
        public static string SQ_UNICODE_LIST_FILE = "SQ_UNICODE_LIST_FILE";

    // Y_Higuchi -- add --
    /// <summary>
    /// ユーザ登録条件ファイル名
    /// </summary>
    public static string AUTHUSER_Report = "AUTHUSER_Report";
    // Y_Higuchi -- add --

    #endregion

        #region MRMS設定

        /// <summary>
        /// MRMSパスワード変換
        /// </summary>
        public static string MRMS_CONVERT_MD5 = "MRMS_CONVERT_MD5";

        /// <summary>
        /// MRMS外字変換
        /// </summary>
        public static string MRMS_CONVERT_GAIJI = "MRMS_CONVERT_GAIJI";

        /// <summary>
        /// MRMS外字変換後置換文字列
        /// </summary>
        public static string MRMS_GAIJI_REPLACE = "MRMS_GAIJI_REPLACE";

        ///// <summary>
        ///// MRMSユーザ管理更新対象カラム
        ///// </summary>
        // Y_Higuchi -- del -- public static string MRMS_USERMANAGE_UPD_COLS = "MRMS_USERMANAGE_UPD_COLS";

        /// <summary>
        /// MRMSユーザアプリケーション管理アプリケーション使用許可フラグ設定値
        /// </summary>
        public static string MRMS_USERAPPMANAGE_LICENCETOUSE = "MRMS_USERAPPMANAGE_LICENCETOUSE";

        /// <summary>
        /// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）デフォルト値
        /// </summary>
        public static string MRMS_USERINFO_CA_ATTRIBUTE_DEFAULT = "MRMS_USERINFO_CA_ATTRIBUTE_DEFAULT";

        /// <summary>
        /// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）職員区分値
        /// </summary>
        public static string MRMS_USERINFO_CA_ATTRIBUTE = "MRMS_USERINFO_CA_ATTRIBUTE_";

        /// <summary>
        /// ATTRMANAGE.TEXTVALUEデフォルト値
        /// </summary>
        public static string MRMS_ATTRMANAGE_TEXTVALUE_DEFAULT = "MRMS_ATTRMANAGE_TEXTVALUE_DEFAULT";

        #endregion

        #region 設定値

        /// <summary>
        /// パスワード変換 0：変換なし
        /// </summary>
        public static string CONVERT_MD5_0 = "0";

        /// <summary>
        /// パスワード変換 1：MD5変換
        /// </summary>
        public static string CONVERT_MD5_1 = "1";

        /// <summary>
        /// パスワード変換 2：TOUSERSINFO.USERID適用
        /// </summary>
        public static string CONVERT_MD5_2 = "2";

        /// <summary>
        /// 外字変換 0：変換なし
        /// </summary>
        public static string CONVERT_GAIJI_0 = "0";

        /// <summary>
        /// 外字変換 1：変換あり
        /// </summary>
        public static string CONVERT_GAIJI_1 = "1";

        #endregion

    }
}
