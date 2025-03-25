
namespace Serv_UsersIFLinkage.Util
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
        /// YOKOGAWA接続文字列
        /// </summary>
        public static string YOKO_Conn = "YOKOGAWA_ConnectionString";

        /// <summary>
        /// ARQS接続文字列
        /// </summary>
        public static string ARQS_Conn = "ARQS_ConnectionString";

		// Y_Higuchi -- del --
		#region SERV ではいらない部分(1)
		///// <summary>
		///// MRMS接続文字列
		///// </summary>
		//public static string MRMS_Conn = "MRMS_ConnectionString";

		///// <summary>
		///// RRIS接続文字列
		///// </summary>
		//public static string RRIS_Conn = "RRIS_ConnectionString";

		///// <summary>
		///// RTRIS接続文字列
		///// </summary>
		//public static string RTRIS_Conn = "RTRIS_ConnectionString";
		#endregion
		// Y_Higuchi -- del --

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
    /// 登録対象DB
    /// </summary>
    public static string TARGET_DB = "DB";
    /// <summary>
    /// ユーザ登録条件ファイル名
    /// </summary>
    public static string AUTHUSER_Serv = "AUTHUSER_Serv";
    public static string AUTHUSER_RIS = "AUTHUSER_RIS";
		public static string AUTHUSER_TheraRIS = "AUTHUSER_TheraRIS";
		public static string AUTHUSER_Report = "AUTHUSER_Report";
		// Y_Higuchi -- add --

		#endregion

		#region SERV設定

		/// <summary>
		/// YOKOGAWAパスワード変換
		/// </summary>
		public static string YOKOGAWA_CONVERT_MD5 = "YOKOGAWA_CONVERT_MD5";

        /// <summary>
        /// YOKOGAWA外字変換
        /// </summary>
        public static string YOKOGAWA_CONVERT_GAIJI = "YOKOGAWA_CONVERT_GAIJI";

        /// <summary>
        /// YOKOGAWA外字変換後置換文字列
        /// </summary>
        public static string YOKOGAWA_GAIJI_REPLACE = "YOKOGAWA_GAIJI_REPLACE";

		// Y_Higuchi -- del --
		///// <summary>
		///// YOKOGAWAユーザ管理更新対象カラム
		///// </summary>
		//public static string YOKOGAWA_USERMANAGE_UPD_COLS = "YOKOGAWA_USERMANAGE_UPD_COLS";
		// Y_Higuchi -- del --

		/// <summary>
		/// YOKOGAWAユーザアプリケーション管理アプリケーション使用許可フラグ設定値
		/// </summary>
		public static string YOKOGAWA_USERAPPMANAGE_LICENCETOUSE = "YOKOGAWA_USERAPPMANAGE_LICENCETOUSE";

        /// <summary>
        /// ATTRMANAGE.TEXTVALUEデフォルト値
        /// </summary>
        public static string YOKOGAWA_ATTRMANAGE_TEXTVALUE_DEFAULT = "YOKOGAWA_ATTRMANAGE_TEXTVALUE_DEFAULT";

        /// <summary>
        /// ARQSパスワード変換
        /// </summary>
        public static string ARQS_CONVERT_MD5 = "ARQS_CONVERT_MD5";

        /// <summary>
        /// ARQS外字変換
        /// </summary>
        public static string ARQS_CONVERT_GAIJI = "ARQS_CONVERT_GAIJI";

        /// <summary>
        /// ARQS外字変換後置換文字列
        /// </summary>
        public static string ARQS_GAIJI_REPLACE = "ARQS_GAIJI_REPLACE";

        // Y_Higuchi 20170920 -- add --from : 仕変 固定値->設定ファイル値
        /// <summary>
        /// ARQS.VIEWRACCESSCTRLFLAG
        /// </summary>
        public static string ARQS_VIEWRACCESSCTRLFLAG = "ARQS_VIEWRACCESSCTRLFLAG";

        /// <summary>
        /// ARQS.VIEWCACCESSCTRLFLAG
        /// </summary>
        public static string ARQS_VIEWCACCESSCTRLFLAG = "ARQS_VIEWCACCESSCTRLFLAG";
        // Y_Higuchi 20170920 -- add --to

		// Y_Higuchi -- del --
		///// <summary>
		///// ARQSユーザ管理更新対象カラム
		///// </summary>
		//public static string ARQS_USERMANAGE_UPD_COLS = "ARQS_USERMANAGE_UPD_COLS";
		// Y_Higuchi -- del --

		#endregion

		// Y_Higuchi -- del --
		#region SERV ではいらない部分(2)
		//#region MRMS設定

		///// <summary>
		///// MRMSパスワード変換
		///// </summary>
		//public static string MRMS_CONVERT_MD5 = "MRMS_CONVERT_MD5";

		///// <summary>
		///// MRMS外字変換
		///// </summary>
		//public static string MRMS_CONVERT_GAIJI = "MRMS_CONVERT_GAIJI";

		///// <summary>
		///// MRMS外字変換後置換文字列
		///// </summary>
		//public static string MRMS_GAIJI_REPLACE = "MRMS_GAIJI_REPLACE";

		///// <summary>
		///// MRMSユーザ管理更新対象カラム
		///// </summary>
		//public static string MRMS_USERMANAGE_UPD_COLS = "MRMS_USERMANAGE_UPD_COLS";

		///// <summary>
		///// MRMSユーザアプリケーション管理アプリケーション使用許可フラグ設定値
		///// </summary>
		//public static string MRMS_USERAPPMANAGE_LICENCETOUSE = "MRMS_USERAPPMANAGE_LICENCETOUSE";

		///// <summary>
		///// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）デフォルト値
		///// </summary>
		//public static string MRMS_USERINFO_CA_ATTRIBUTE_DEFAULT = "MRMS_USERINFO_CA_ATTRIBUTE_DEFAULT";

		///// <summary>
		///// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）職員区分値
		///// </summary>
		//public static string MRMS_USERINFO_CA_ATTRIBUTE = "MRMS_USERINFO_CA_ATTRIBUTE_";

		///// <summary>
		///// ATTRMANAGE.TEXTVALUEデフォルト値
		///// </summary>
		//public static string MRMS_ATTRMANAGE_TEXTVALUE_DEFAULT = "MRMS_ATTRMANAGE_TEXTVALUE_DEFAULT";

		//#endregion

		//#region RIS設定

		///// <summary>
		///// RRISパスワード変換
		///// </summary>
		//public static string RRIS_CONVERT_MD5 = "RRIS_CONVERT_MD5";

		///// <summary>
		///// RRIS外字変換
		///// </summary>
		//public static string RRIS_CONVERT_GAIJI = "RRIS_CONVERT_GAIJI";

		///// <summary>
		///// RRIS外字変換後置換文字列
		///// </summary>
		//public static string RRIS_GAIJI_REPLACE = "RRIS_GAIJI_REPLACE";

		///// <summary>
		///// RRISユーザ管理更新対象カラム
		///// </summary>
		//public static string RRIS_USERMANAGE_UPD_COLS = "RRIS_USERMANAGE_UPD_COLS";

		///// <summary>
		///// RRISユーザアプリケーション管理アプリケーション使用許可フラグ設定値
		///// </summary>
		//public static string RRIS_USERAPPMANAGE_LICENCETOUSE = "RRIS_USERAPPMANAGE_LICENCETOUSE";

		///// <summary>
		///// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）デフォルト値
		///// </summary>
		//public static string RRIS_USERINFO_CA_ATTRIBUTE_DEFAULT = "RRIS_USERINFO_CA_ATTRIBUTE_DEFAULT";

		///// <summary>
		///// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）職員区分値
		///// </summary>
		//public static string RRIS_USERINFO_CA_ATTRIBUTE = "RRIS_USERINFO_CA_ATTRIBUTE_";

		///// <summary>
		///// ATTRMANAGE.TEXTVALUEデフォルト値
		///// </summary>
		//public static string RRIS_ATTRMANAGE_TEXTVALUE_DEFAULT = "RRIS_ATTRMANAGE_TEXTVALUE_DEFAULT";

		///// <summary>
		///// 登録対象制御
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_IMPORT = "RRIS_SECTIONDOCTORMASTER_IMPORT";

		///// <summary>
		///// 登録/更新ユーザID
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_USR_ID = "RRIS_SECTIONDOCTORMASTER_USR_ID";

		///// <summary>
		///// 登録/更新ユーザ名称
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_USR_NAME = "RRIS_SECTIONDOCTORMASTER_USR_NAME";

		///// <summary>
		///// 表示順 デフォルト値
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT = "RRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT";

		///// <summary>
		///// 表示順
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_SHOWORDER = "RRIS_SECTIONDOCTORMASTER_SHOWORDER_";

		///// <summary>
		///// SECTIONDOCTORMASTER更新対象カラム
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_UPD_COLS = "RRIS_SECTIONDOCTORMASTER_UPD_COLS";

		///// <summary>
		///// SECTIONDOCTORMASTER使用可否ﾌﾗｸﾞ設定値
		///// </summary>
		//public static string RRIS_SECTIONDOCTORMASTER_USEFLAG = "RRIS_SECTIONDOCTORMASTER_USEFLAG";

		//#endregion

		//#region RTRIS設定

		///// <summary>
		///// RTRISパスワード変換
		///// </summary>
		//public static string RTRIS_CONVERT_MD5 = "RTRIS_CONVERT_MD5";

		///// <summary>
		///// RTRIS外字変換
		///// </summary>
		//public static string RTRIS_CONVERT_GAIJI = "RTRIS_CONVERT_GAIJI";

		///// <summary>
		///// RTRIS外字変換後置換文字列
		///// </summary>
		//public static string RTRIS_GAIJI_REPLACE = "RTRIS_GAIJI_REPLACE";

		///// <summary>
		///// RTRISユーザ管理更新対象カラム
		///// </summary>
		//public static string RTRIS_USERMANAGE_UPD_COLS = "RTRIS_USERMANAGE_UPD_COLS";

		///// <summary>
		///// RTRISユーザアプリケーション管理アプリケーション使用許可フラグ設定値
		///// </summary>
		//public static string RTRIS_USERAPPMANAGE_LICENCETOUSE = "RTRIS_USERAPPMANAGE_LICENCETOUSE";

		///// <summary>
		///// RTRISパスワード変換
		///// </summary>
		//public static string RTRIS_USERMANAGECOMP_CONVERT_MD5 = "RTRIS_USERMANAGECOMP_CONVERT_MD5";

		///// <summary>
		///// ATTRMANAGE.TEXTVALUEデフォルト値
		///// </summary>
		//public static string RTRIS_ATTRMANAGE_TEXTVALUE_DEFAULT = "RTRIS_ATTRMANAGE_TEXTVALUE_DEFAULT";

		///// <summary>
		///// RTRISクライアントユーザ管理更新対象カラム
		///// </summary>
		//public static string RTRIS_USERMANAGECOMP_UPD_COLS = "RTRIS_USERMANAGECOMP_UPD_COLS";

		///// <summary>
		///// 登録対象制御
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_IMPORT = "RTRIS_SECTIONDOCTORMASTER_IMPORT";

		///// <summary>
		///// 表示順 デフォルト値
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT = "RTRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT";

		///// <summary>
		///// 表示順
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_SHOWORDER = "RTRIS_SECTIONDOCTORMASTER_SHOWORDER_";

		///// <summary>
		///// デフォルト選択メニューグループ デフォルト値
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_DEFAULT = "RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_DEFAULT";

		///// <summary>
		///// デフォルト選択メニューグループ
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP = "RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_";

		///// <summary>
		///// SECTIONDOCTORMASTER更新対象カラム
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_UPD_COLS = "RTRIS_SECTIONDOCTORMASTER_UPD_COLS";

		///// <summary>
		///// SECTIONDOCTORMASTER使用可否ﾌﾗｸﾞ設定値
		///// </summary>
		//public static string RTRIS_SECTIONDOCTORMASTER_USEFLAG = "RTRIS_SECTIONDOCTORMASTER_USEFLAG";

		//#endregion
		#endregion
		// Y_Higuchi -- del --

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
