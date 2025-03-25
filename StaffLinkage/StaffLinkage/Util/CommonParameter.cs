using System.Text;

namespace StaffLinkage.Util
{
  class CommonParameter
  {
    /// <summary>
    /// 文字コード
    /// </summary>
    public static Encoding CommonEnocode = Encoding.GetEncoding("Shift_JIS");

    /// <summary>
    /// yyyyMMdd書式
    /// </summary>
    public const string YYYYMMDD = "yyyyMMdd";

    // Y_Higuchi -- add -- from --
    /// <summary>
    /// 処理モード：GUI
    /// </summary>
    public const string MODE_GUI = "gui";

    /// <summary>
    /// 処理モード：タスクスケジューラ
    /// </summary>
    public const string MODE_TASK = "task";

    /// <summary>
    /// 処理モード：サービス
    /// </summary>
    public const string MODE_SERVICE = "serv";

    /// <summary>
    /// TOUSERSINFO.DB 項目 : SERV の区分
    /// 設定ファイルの DB 項目と一致していること
    /// </summary>
    public const string apgwSERV = "SERV";

    /// <summary>
    /// TOUSERSINFO.DB 項目 : RIS の区分
    /// 設定ファイルの DB 項目と一致していること
    /// </summary>
    public const string apgwRIS = "RIS";

    /// <summary>
    /// TOUSERSINFO.DB 項目 : THERARIS の区分
    /// 設定ファイルの DB 項目と一致していること
    /// </summary>
    public const string apgwTHERARIS = "THERARIS";

    /// <summary>
    /// TOUSERSINFO.DB 項目 : REPOT の区分
    /// 設定ファイルの DB 項目と一致していること
    /// </summary>
    public const string apgwREPORT = "REPORT";

    /// <summary>
    /// UserInfo_Datasetクラスでデータが無かった場合に返す値
    /// </summary>
    public const string tgmNO_DATA = "TELEGRAM_KEY_VALUE_NO_DATA";

    //*************************************************************************
    /// <summary>
    /// 電文ノード 位置|データ長 [byte]指定
    /// </summary>
    public const string xmlNODE_DENBUN = "EYBNWGCP";
    /// <summary>
    /// 特殊仕様が必要な項目(1)
    /// </summary>
    public const string xmlID_PASSWORD = "PASSWORDW";
    /// <summary>
    /// 特殊仕様が必要な項目(2)
    /// </summary>
    public const string xmlID_NAMEN = "NAMEN";
    /// <summary>
    /// 特殊仕様が必要な項目(3)
    /// </summary>
    public const string xmlID_SYOKUKBN = "SYOKUKBN";
    /// <summary>
    /// 特殊仕様が必要な項目(4)
    /// </summary>
    public const string xmlID_TNT_1_KA = "TNT_1_KA";
    /// <summary>
    /// 特殊仕様が必要な項目(5)
    /// </summary>
    public const string xmlID_TNT_2_KA = "TNT_2_KA";
    /// <summary>
    /// 特殊仕様が必要な項目(6)
    /// </summary>
    public const string xmlID_TNT_3_KA = "TNT_3_KA";
    /// <summary>
    /// 特殊仕様が必要な項目(7)
    /// </summary>
    public const string xmlID_TNT_4_KA = "TNT_4_KA";
    /// <summary>
    /// 特殊仕様が必要な項目(8)
    /// </summary>
    public const string xmlID_TNT_5_KA = "TNT_5_KA";
    /// <summary>
    /// 特殊仕様が必要な項目(9)
    /// </summary>
    public const string xmlID_YUKO_ED = "YUKO_ED";
    /// <summary>
    /// 特殊仕様が必要な項目(10)
    /// </summary>
    public const string xmlID_UPD_YMD = "UPD_YMD";
    /// <summary>
    /// 特殊仕様が必要な項目(11)
    /// </summary>
    public const string xmlID_STP_FLG = "STP_FLG";

    //*************************************************************************
    /// <summary>
    /// NODE ToUsersInfo ノード (id=電文項目名 value=ToUsersInfo項目名)
    /// </summary>
    public const string xmlNODE_TABLE = "TOUSERSINFO";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (1)
    /// </summary>
    public const string xmlFLD_REQUESTID = "REQUESTID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (2)
    /// </summary>
    public const string xmlFLD_REQUESTDATE = "REQUESTDATE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (3)
    /// </summary>
    public const string xmlFLD_DB = "DB";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (4)
    /// </summary>
    public const string xmlFLD_APPCODE = "APPCODE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (5)
    /// </summary>
    public const string xmlFLD_USERID = "USERID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (6)
    /// </summary>
    public const string xmlFLD_HOSPITALID = "HOSPITALID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (7)
    /// </summary>
    public const string xmlFLD_PASSWORD = "PASSWORD";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (8)
    /// </summary>
    public const string xmlFLD_USERNAMEKANJI = "USERNAMEKANJI";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (9)
    /// </summary>
    public const string xmlFLD_USERNAMEENG = "USERNAMEENG";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (10)
    /// </summary>
    public const string xmlFLD_SECTION_ID = "SECTION_ID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (11)
    /// </summary>
    public const string xmlFLD_TANTO_SECTION_ID = "TANTO_SECTION_ID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (12)
    /// </summary>
    public const string xmlFLD_STAFFID = "STAFFID";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (13)
    /// </summary>
    public const string xmlFLD_SYOKUIN_KBN = "SYOKUIN_KBN";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (14)
    /// </summary>
    public const string xmlFLD_TEL = "TEL";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (15)
    /// </summary>
    public const string xmlFLD_PASSWORDEXPIRYDATE = "PASSWORDEXPIRYDATE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (16)
    /// </summary>
    public const string xmlFLD_PASSWORDWARNINGDATE = "PASSWORDWARNINGDATE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (17)
    /// </summary>
    public const string xmlFLD_USERIDVALIDITYFLAG = "USERIDVALIDITYFLAG";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (18)
    /// </summary>
    public const string xmlFLD_REQUESTTYPE = "REQUESTTYPE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (19)
    /// </summary>
    public const string xmlFLD_MESSAGEID1 = "MESSAGEID1";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (20)
    /// </summary>
    public const string xmlFLD_MESSAGEID2 = "MESSAGEID2";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (21)
    /// </summary>
    public const string xmlFLD_MESSAGEID3 = "MESSAGEID3";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (22)
    /// </summary>
    public const string xmlFLD_TRANSFERSTATUS = "TRANSFERSTATUS";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (23)
    /// </summary>
    public const string xmlFLD_TRANSFERDATE = "TRANSFERDATE";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (24)
    /// </summary>
    public const string xmlFLD_TRANSFERRESULT = "TRANSFERRESULT";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (25)
    /// </summary>
    public const string xmlFLD_TRANSFERTEXT = "TRANSFERTEXT";
    /// <summary>
    /// ToUsersInfo 項目名(Field物理名称) (--)
    /// </summary>
    // public const string xmlFLD_SECTION_NAME = "SECTION_NAME"; //さいたま赤十字特注
    // Y_Higuchi -- add -- to --

  }
}
