
namespace TheraRis_UsersIFLinkage.Util
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
    /// RTRIS接続文字列
    /// </summary>
    public static string RTRIS_Conn = "RTRIS_ConnectionString";

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
    public static string AUTHUSER_TheraRIS = "AUTHUSER_TheraRIS";
    // Y_Higuchi -- add --

    #endregion

    #region RTRIS設定

    /// <summary>
    /// RTRISパスワード変換
    /// </summary>
    public static string RTRIS_CONVERT_MD5 = "RTRIS_CONVERT_MD5";

    /// <summary>
    /// RTRIS外字変換
    /// </summary>
    public static string RTRIS_CONVERT_GAIJI = "RTRIS_CONVERT_GAIJI";

    /// <summary>
    /// RTRIS外字変換後置換文字列
    /// </summary>
    public static string RTRIS_GAIJI_REPLACE = "RTRIS_GAIJI_REPLACE";

    ///// <summary>
    ///// RTRISユーザ管理更新対象カラム
    ///// </summary>
    //Y_Higuchi -- del -- public static string RTRIS_USERMANAGE_UPD_COLS = "RTRIS_USERMANAGE_UPD_COLS";

    /// <summary>
    /// RTRISユーザアプリケーション管理アプリケーション使用許可フラグ設定値
    /// </summary>
    public static string RTRIS_USERAPPMANAGE_LICENCETOUSE = "RTRIS_USERAPPMANAGE_LICENCETOUSE";

    /// <summary>
    /// RTRISパスワード変換
    /// </summary>
    public static string RTRIS_USERMANAGECOMP_CONVERT_MD5 = "RTRIS_USERMANAGECOMP_CONVERT_MD5";

    /// <summary>
    /// ATTRMANAGE.TEXTVALUEデフォルト値
    /// </summary>
    public static string RTRIS_ATTRMANAGE_TEXTVALUE_DEFAULT = "RTRIS_ATTRMANAGE_TEXTVALUE_DEFAULT";

    ///// <summary>
    ///// RTRISクライアントユーザ管理更新対象カラム
    ///// </summary>
    //Y_Higuchi -- del -- public static string RTRIS_USERMANAGECOMP_UPD_COLS = "RTRIS_USERMANAGECOMP_UPD_COLS";

    ///// <summary>
    ///// 登録対象制御
    ///// </summary>
    //Y_Higuchi -- del -- public static string RTRIS_SECTIONDOCTORMASTER_IMPORT = "RTRIS_SECTIONDOCTORMASTER_IMPORT";

    /// <summary>
    /// 表示順 デフォルト値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT = "RTRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT";

    /// <summary>
    /// 表示順
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_SHOWORDER = "RTRIS_SECTIONDOCTORMASTER_SHOWORDER_";

    /// <summary>
    /// デフォルト選択メニューグループ デフォルト値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_DEFAULT = "RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_DEFAULT";

    /// <summary>
    /// デフォルト選択メニューグループ
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP = "RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_";

    ///// <summary>
    ///// SECTIONDOCTORMASTER更新対象カラム
    ///// </summary>
    //Y_Higuchi -- del -- public static string RTRIS_SECTIONDOCTORMASTER_UPD_COLS = "RTRIS_SECTIONDOCTORMASTER_UPD_COLS";

    /// <summary>
    /// SECTIONDOCTORMASTER使用可否ﾌﾗｸﾞ設定値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_USEFLAG = "RTRIS_SECTIONDOCTORMASTER_USEFLAG";

//Y_Higuchi -- add -- 
    /// <summary>
    /// 物理士業務フラグ 設定値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_PHYSICIST_FLG = "RTRIS_SECTIONDOCTORMASTER_PHYSICIST_FLG";
    /// <summary>
    /// プラン確定権限フラグ 設定値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_PLAN_DECISION_AUTHORITY_FLG = "RTRIS_SECTIONDOCTORMASTER_PLAN_DECISION_AUTHORITY_FLG";
    /// <summary>
    /// 治療医登録対象フラグ 設定値
    /// </summary>
    public static string RTRIS_SECTIONDOCTORMASTER_TREAT_DOCTORS_FLG = "RTRIS_SECTIONDOCTORMASTER_TREAT_DOCTORS_FLG";
//Y_Higuchi -- add -- 

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
