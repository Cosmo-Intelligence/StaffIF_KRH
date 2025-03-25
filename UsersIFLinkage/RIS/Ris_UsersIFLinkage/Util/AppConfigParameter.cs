
namespace Ris_UsersIFLinkage.Util
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
    /// RRIS接続文字列
    /// </summary>
    public static string RRIS_Conn = "RRIS_ConnectionString";

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
    public static string AUTHUSER_RIS = "AUTHUSER_RIS";
    // Y_Higuchi -- add --

    #endregion

    #region RIS設定

    /// <summary>
    /// RRISパスワード変換
    /// </summary>
    public static string RRIS_CONVERT_MD5 = "RRIS_CONVERT_MD5";

    /// <summary>
    /// RRIS外字変換
    /// </summary>
    public static string RRIS_CONVERT_GAIJI = "RRIS_CONVERT_GAIJI";

    /// <summary>
    /// RRIS外字変換後置換文字列
    /// </summary>
    public static string RRIS_GAIJI_REPLACE = "RRIS_GAIJI_REPLACE";

    ///// <summary>
    ///// RRISユーザ管理更新対象カラム
    ///// </summary>
    //Y_Higuchi -- del -- public static string RRIS_USERMANAGE_UPD_COLS = "RRIS_USERMANAGE_UPD_COLS";

    /// <summary>
    /// RRISユーザアプリケーション管理アプリケーション使用許可フラグ設定値
    /// </summary>
    public static string RRIS_USERAPPMANAGE_LICENCETOUSE = "RRIS_USERAPPMANAGE_LICENCETOUSE";

    /// <summary>
    /// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）デフォルト値
    /// </summary>
    public static string RRIS_USERINFO_CA_ATTRIBUTE_DEFAULT = "RRIS_USERINFO_CA_ATTRIBUTE_DEFAULT";

    /// <summary>
    /// USERINFO_CA.ATTRIBUTE:ｸﾞﾙｰﾌﾟID（=GROUPMASTER.ID）職員区分値
    /// </summary>
    public static string RRIS_USERINFO_CA_ATTRIBUTE = "RRIS_USERINFO_CA_ATTRIBUTE_";

    /// <summary>
    /// ATTRMANAGE.TEXTVALUEデフォルト値
    /// </summary>
    public static string RRIS_ATTRMANAGE_TEXTVALUE_DEFAULT = "RRIS_ATTRMANAGE_TEXTVALUE_DEFAULT";

    /// <summary>
    /// 登録対象制御
    /// </summary>
    // Y_Higuchi --del -- public static string RRIS_SECTIONDOCTORMASTER_IMPORT = "RRIS_SECTIONDOCTORMASTER_IMPORT";

    /// <summary>
    /// 登録/更新ユーザID
    /// </summary>
    public static string RRIS_SECTIONDOCTORMASTER_USR_ID = "RRIS_SECTIONDOCTORMASTER_USR_ID";

    /// <summary>
    /// 登録/更新ユーザ名称
    /// </summary>
    public static string RRIS_SECTIONDOCTORMASTER_USR_NAME = "RRIS_SECTIONDOCTORMASTER_USR_NAME";

    /// <summary>
    /// 表示順 デフォルト値
    /// </summary>
    public static string RRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT = "RRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT";

    /// <summary>
    /// 表示順
    /// </summary>
    public static string RRIS_SECTIONDOCTORMASTER_SHOWORDER = "RRIS_SECTIONDOCTORMASTER_SHOWORDER_";

    ///// <summary>
    ///// SECTIONDOCTORMASTER更新対象カラム
    ///// </summary>
    // Y_Higuchi -- del -- public static string RRIS_SECTIONDOCTORMASTER_UPD_COLS = "RRIS_SECTIONDOCTORMASTER_UPD_COLS";

    /// <summary>
    /// SECTIONDOCTORMASTER使用可否ﾌﾗｸﾞ設定値
    /// </summary>
    public static string RRIS_SECTIONDOCTORMASTER_USEFLAG = "RRIS_SECTIONDOCTORMASTER_USEFLAG";

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
