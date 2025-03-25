
namespace Report_UsersIFLinkage.Data.Import.Common
{
  /// <summary>
  /// REPORTクエリクラス
  /// </summary>
  class REPORT_QUERY
  {
    #region MRMS ユーザ管理

    /// <summary>
    /// ユーザ管理 Merge
    /// </summary>
    public const string MRMS_USERMANAGE_MERGE =
                  " merge into USERMANAGE"
                + " using("
                + "   select {0} as USERID, {1} as HOSPITALID from dual"
                + " ) dummy"
                + " on ("
                + "     USERMANAGE.USERID = dummy.USERID"
                + "   and"
                + "     USERMANAGE.HOSPITALID = dummy.HOSPITALID"
                + " )"
                + " {13}"
                //+ " when matched then"
                //+ "   update set"
                //+ "     USERID = {0},"
                //+ "     HOSPITALID = {1},"
                //+ "     PASSWORD = {2},"
                //+ "     USERNAME = {3},"
                //+ "     USERNAMEENG = {4},"
                //+ "     PASSWORDEXPIRYDATE = {5},"
                //+ "     PASSWORDWARNINGDATE = {6},"
                //+ "     USERIDVALIDITYFLAG = {7}"
                //+ "     BELONGINGDEPARTMENT = {8},"
                //+ "     MAINGROUPID = {9},"
                //+ "     SUBGROUPIDLIST = {10},"
                //+ "     UPDATEDATETIME = {11},"
                //+ "     QUALIFIEDPERSONFLAG = {12}"
                + " when not matched then"
                + "   insert"
                + "     (USERID,"
                + "      HOSPITALID,"
                + "      PASSWORD,"
                + "      USERNAME,"
                + "      USERNAMEENG,"
                + "      PASSWORDEXPIRYDATE,"
                + "      PASSWORDWARNINGDATE,"
                + "      USERIDVALIDITYFLAG,"
                + "      BELONGINGDEPARTMENT,"
                + "      MAINGROUPID,"
                + "      SUBGROUPIDLIST,"
                + "      UPDATEDATETIME,"
                + "      QUALIFIEDPERSONFLAG)"
                + "   values"
                + "     ({0},"
                + "      {1},"
                + "      {2},"
                + "      {3},"
                + "      {4},"
                + "      {5},"
                + "      {6},"
                + "      {7},"
                + "      {8},"
                + "      {9},"
                + "      {10},"
                + "      {11},"
                + "      {12})";

    /// <summary>
    /// ユーザ管理 削除
    /// </summary>
    public const string MRMS_USERMANAGE_DELETE =
                  " update USERMANAGE"
                + " set"
                + "   USERIDVALIDITYFLAG = {2},"
                + "   UPDATEDATETIME = {3}"
                + " where"
                + "   USERID = {0}"
                + " and"
                + "   HOSPITALID = {1}";

    #endregion

    #region MRMS ユーザ詳細情報管理

    /// <summary>
    /// ユーザ詳細情報管理 Merge
    /// </summary>
    public const string MRMS_USERINFO_CA_MERGE =
                  " merge into USERINFO_CA"
                + " using("
                + "   select {1} as LOGINID from dual"
                + " ) dummy"
                + " on ("
                + "     USERINFO_CA.LOGINID = dummy.LOGINID"
                + " )"
                //+ " when matched then"
                //+ "   update set"
                //+ "     ID = {0},"
                //+ "     LOGINID = {1},"
                //+ "     HOSPITALID = {2},"
                //+ "     ATTRIBUTE = {3},"
                //+ "     SHOWORDER = {4},"
                //+ "     LANGUAGE = {5}"
                //+ "     UPDATEDATETIME = {6}"
                + " when not matched then"
                + "   insert"
                + "     (ID,"
                + "      LOGINID,"
                + "      HOSPITALID,"
                + "      ATTRIBUTE,"
                + "      SHOWORDER,"
                + "      LANGUAGE,"
                + "      UPDATEDATETIME)"
               + "   values"
                + "     ((select nvl(max(to_number(ID)), 0) + 1 as ID from USERINFO_CA where ID < 10000000),"
                + "      {1},"
                + "      {2},"
                + "      {3},"
                + "      (select nvl(max(to_number(SHOWORDER)), 0) + 1 as SHOWORDER from USERINFO_CA where ID < 10000000),"
                + "      {5},"
                + "      {6})";

    /// <summary>
    /// ユーザ詳細情報管理ID取得
    /// </summary>
    public const string MRMS_USERINFO_CA_SELECT_ID =
                  " select ID from USERINFO_CA where LOGINID = {0}";

    #endregion

    #region MRMS ユーザアプリケーション管理

    /// <summary>
    /// ユーザアプリケーション管理 Update
    /// </summary>
    public const string MRMS_USERAPPMANAGE_UPDATE =
                  " update USERAPPMANAGE"
                + " set"
                + "   LICENCETOUSE = {2},"
                + "   UPDATEDATETIME = sysdate"
                + " where"
                + "   USERID = {0}"
                + " and"
                + "   HOSPITALID = {1}";

    /// <summary>
    /// ユーザアプリケーション管理 Merge
    /// </summary>
    public const string MRMS_USERAPPMANAGE_MERGE =
                  " merge into USERAPPMANAGE"
                + " using("
                + "   select {0} as USERID, {1} as HOSPITALID, {2} as APPCODE from dual"
                + " ) dummy"
                + " on ("
                + "     USERAPPMANAGE.USERID = dummy.USERID"
                + "   and"
                + "     USERAPPMANAGE.HOSPITALID = dummy.HOSPITALID"
                + "   and"
                + "     USERAPPMANAGE.APPCODE = dummy.APPCODE"
                + " )"
                //+ " when matched then"
                //+ "   update set"
                //+ "     USERID = {0},"
                //+ "     HOSPITALID = {1},"
                //+ "     APPCODE = {2},"
                //+ "     LICENCETOUSE = {3},"
                //+ "     MYATTRID = {4},"
                //+ "     UPDATEDATETIME = {5}"
                + " when not matched then"
                + "   insert"
                + "     (USERID,"
                + "      HOSPITALID,"
                + "      APPCODE,"
                + "      LICENCETOUSE,"
                + "      MYATTRID,"
                + "      UPDATEDATETIME)"
                + "   values"
                + "     ({0},"
                + "      {1},"
                + "      {2},"
                + "      {3},"
                + "      {4},"
                + "      {5})";

    #endregion

    #region MRMS 属性管理

    /// <summary>
    /// 属性管理 Merge
    /// </summary>
    public const string MRMS_ATTRMANAGE_MERGE =
                  " merge into ATTRMANAGE"
                + " using("
                + "   select {1} as ATTROWNERID, {2} as ATTRNAME from dual"
                + " ) dummy"
                + " on ("
                + "     ATTRMANAGE.ATTROWNERID = dummy.ATTROWNERID"
                + "   and"
                + "     ATTRMANAGE.ATTRNAME = dummy.ATTRNAME"
                + " )"
                //+ " when matched then"
                //+ "   update set"
                //+ "     ATTRID = {0},"
                //+ "     ATTROWNERID = {1},"
                //+ "     ATTRNAME = {2},"
                //+ "     VALUETYPE = {3},"
                //+ "     TEXTVALUE = {4},"
                //+ "     BLOBVALUE = {5},"
                //+ "     UPDATEDATETIME = {6}"
                + " when not matched then"
                + "   insert"
                + "     (ATTRID,"
                + "      ATTROWNERID,"
                + "      ATTRNAME,"
                + "      VALUETYPE,"
                + "      TEXTVALUE,"
                + "      BLOBVALUE,"
                + "      UPDATEDATETIME)"
                + "   values"
                + "     ((select nvl(max(to_number(ATTRID)), 0) + 1 as ATTRID from ATTRMANAGE),"
                + "      {1},"
                + "      {2},"
                + "      {3},"
                + "      {4},"
                + "      {5},"
                + "      {6})";

    /// <summary>
    /// 属性管理デフォルトテキスト型属性値取得
    /// </summary>
    public const string MRMS_ATTRMANAGE_SELECT_TEXTVALUE =
                  " (select"
                + "   nvl(max(TEXTVALUE), {2})"
                + " from"
                + "   ATTRMANAGE"
                + " where"
                + "   ATTROWNERID = {0}"
                + " and"
                + "   ATTRNAME = {1})";

    #endregion
    #region REPORT 依頼医マスタ

    /// <summary>
    /// 依頼医マスタ Merge
    /// </summary>
    public const string MRMS_REQUESTDOCTORMASTER_MERGE =
            " merge into REQUESTDOCTORMASTER"
                + " using("
                + "   select {1} as NAME, {2} as ATTRIBUTE from DUAL"
                + " ) dummy"
                + " on (REQUESTDOCTORMASTER.NAME = dummy.NAME"
                + "  AND REQUESTDOCTORMASTER.ATTRIBUTE = dummy.ATTRIBUTE)"
                + " when not matched then"
                + "   insert"
                + "     (ID,"
                + "      NAME,"
                + "      ATTRIBUTE,"
                + "      SHOWORDER)"
                + "   values"
                + "     ({0},"
                + "      {1},"
                + "      {2},"
                + "      {3})";

    /// <summary>
    /// 依頼医マスタ レコード存在チェック
    /// </summary>
    public const string MRMS_REQUESTDOCTORMASTER_SELECT_DATA =
              "  SELECT * FROM REQUESTDOCTORMASTER"
            + " WHERE NAME = {0}"
            + " AND ATTRIBUTE = {1}";

    /// <summary>
    /// 依頼医マスタ ID最大値+1 取得
    /// </summary>
    public const string MRMS_REQUESTDOCTORMASTER_SELECT_MAX_ID =
            " (select nvl(max(ID), 0) + 1 as ID from REQUESTDOCTORMASTER)";

    /// <summary>
    /// 依頼医マスタ SHOWORDER最大値+1 取得
    /// </summary>
    public const string MRMS_REQUESTDOCTORMASTER_SELECT_MAX_SHOWORDER =
              " (select nvl(max(SHOWORDER), 0) + 1 as SHOWORDER from REQUESTDOCTORMASTER)";
    #endregion

  }

}
