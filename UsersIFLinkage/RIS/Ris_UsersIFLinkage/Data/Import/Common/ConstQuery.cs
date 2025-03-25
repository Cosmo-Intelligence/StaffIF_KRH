namespace Ris_UsersIFLinkage.Data.Import.Common
{
    /// <summary>
    /// RISクエリクラス
    /// </summary>
    class RIS_QUERY
    {
        #region RRIS ユーザ管理

        /// <summary>
        /// ユーザ管理 Merge
        /// </summary>
        public const string RRIS_USERMANAGE_MERGE =
                  " merge into USERMANAGE"
                + " using("
                + "   select {0} as USERID, {1} as HOSPITALID from dual"
                + " ) dummy"
                + " on ("
                + "     USERMANAGE.USERID = dummy.USERID"
                + "   and"
                + "     USERMANAGE.HOSPITALID = dummy.HOSPITALID"
                + " )"
                + " {12}"
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
                //+ "     UPDATEDATETIME = {11}"
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
                + "      UPDATEDATETIME)"
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
                + "      {11})";

        /// <summary>
        /// ユーザ管理 削除
        /// </summary>
        public const string RRIS_USERMANAGE_DELETE =
                  " update USERMANAGE"
                + " set"
                + "   USERIDVALIDITYFLAG = {2},"
                + "   UPDATEDATETIME = {3}"
                + " where"
                + "   USERID = {0}"
                + " and"
                + "   HOSPITALID = {1}";

		// Y_Higuchi --add --2017.09.28-->>
			public const string RRIS_USERMANAGE_UPDATE =
				" UPDATE USERMANAGE {2}"
				+ " WHERE (USERMANAGE.USERID = {0}) AND (USERMANAGE.HOSPITALID = {1})";
		// Y_Higuchi --add --2017.09.28-->>

        #endregion

        #region RRIS ユーザ詳細情報管理

        /// <summary>
        /// ユーザ詳細情報管理 Merge
        /// </summary>
        public const string RRIS_USERINFO_CA_MERGE =
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
                //+ "     STAFFID = {2},"
                //+ "     HOSPITALID = {3},"
                //+ "     SYOKUIN_KBN = {4},"
                //+ "     ATTRIBUTE = {5},"
                //+ "     SHOWORDER = {6}"
                + " when not matched then"
                + "   insert"
                + "     (ID,"
                + "      LOGINID,"
                + "      STAFFID,"
                + "      HOSPITALID,"
                + "      SYOKUIN_KBN,"
                + "      ATTRIBUTE,"
                + "      SHOWORDER)"
                + "   values"
                + "     ((select nvl(max(to_number(ID)), 0) + 1 as ID from USERINFO_CA),"
                + "      {1},"
                + "      {2},"
                + "      {3},"
                + "      {4},"
                + "      {5},"
                + "      (select nvl(max(to_number(SHOWORDER)), 0) + 1 as SHOWORDER from USERINFO_CA))";

        #endregion

        #region RRIS ユーザアプリケーション管理

        /// <summary>
        /// ユーザアプリケーション管理 Update
        /// </summary>
        public const string RRIS_USERAPPMANAGE_UPDATE =
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
        public const string RRIS_USERAPPMANAGE_MERGE =
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

        #region RRIS 属性管理

        /// <summary>
        /// 属性管理 Merge
        /// </summary>
        public const string RRIS_ATTRMANAGE_MERGE =
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
        public const string RRIS_ATTRMANAGE_SELECT_TEXTVALUE =
                  " (select"
                + "   nvl(max(TEXTVALUE), {2})"
                + " from"
                + "   ATTRMANAGE"
                + " where"
                + "   ATTROWNERID = {0}"
                + " and"
                + "   ATTRNAME = {1})";

        #endregion

        #region RRIS 診療科医師マスタ

        /// <summary>
        /// 診療科医師マスタ 担当科取得
        /// </summary>
        public const string RRIS_SECTIONDOCTORMASTER_SELECT_TANTO_SECTION_ID =
                  " select"
                + "   TANTO_SECTION_ID"
                + " from"
                + "   SECTIONDOCTORMASTER"
                + " where"
                + "   DOCTOR_ID = {0}";

        /// <summary>
        /// 診療科医師マスタ Merge
        /// </summary>
        public const string RRIS_SECTIONDOCTORMASTER_MERGE =
                  " merge into SECTIONDOCTORMASTER"
                + " using("
                + "   select {0} as DOCTOR_ID from dual"
                + " ) dummy"
                + " on (SECTIONDOCTORMASTER.DOCTOR_ID = dummy.DOCTOR_ID)"
                + " {14}"
                //+ " when matched then"
                //+ "   update set"
                //+ "     DOCTOR_ID = {0},"
                //+ "     DOCTOR_NAME = {1},"
                //+ "     DOCTOR_ENGLISH_NAME = {2},"
                //+ "     SECTION_ID = {3},"
                //+ "     DOCTOR_TEL = {4},"
                //+ "     TANTO_SECTION_ID = {5},"
                //+ "     USEFLAG = {6},"
                //+ "     SHOWORDER = {7},"
                //+ "     ENTRY_DATE = {8},"
                //+ "     ENTRY_USR_ID = {9},"
                //+ "     ENTRY_USR_NAME = {10},"
                //+ "     UPD_DATE = {11},"
                //+ "     UPD_USR_ID = {12},"
                //+ "     UPD_USR_NAME = {13}"
                + " when not matched then"
                + "   insert"
                + "     (DOCTOR_ID,"
                + "      DOCTOR_NAME,"
                + "      DOCTOR_ENGLISH_NAME,"
                + "      SECTION_ID,"
                + "      DOCTOR_TEL,"
                + "      TANTO_SECTION_ID,"
                + "      USEFLAG,"
                + "      SHOWORDER,"
                + "      ENTRY_DATE,"
                + "      ENTRY_USR_ID,"
                + "      ENTRY_USR_NAME,"
                + "      UPD_DATE,"
                + "      UPD_USR_ID,"
                + "      UPD_USR_NAME)"
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
                + "      null,"
                + "      null,"
                + "      null)";

        /// <summary>
        /// 診療科医師マスタ 削除
        /// </summary>
        public const string RRIS_SECTIONDOCTORMASTER_DELETE =
                  " update SECTIONDOCTORMASTER"
                + " set"
                + "   USEFLAG = {1},"
                + "   UPD_DATE = {2},"
                + "   UPD_USR_ID = {3},"
                + "   UPD_USR_NAME = {4}"
                + " where"
                + "   DOCTOR_ID = {0}";

        /// <summary>
        /// 診療科医師マスタ 表示順取得
        /// </summary>
        public const string RRIS_SECTIONDOCTORMASTER_SELECT_SHOWORDER =
              "  (select"
            + "   case"
            + "     when a.SHOWORDER is null then {0}"
            + "     when a.SHOWORDER < {1} then a.SHOWORDER + 1"
            + "     when b.SHOWORDER is null then {2}"
            + "     else b.SHOWORDER + 1"
            + "  end as SHOWORDER"
            + "  from"
            + "  (select max(SHOWORDER) as SHOWORDER from SECTIONDOCTORMASTER WHERE SHOWORDER BETWEEN {0} AND {1}) a,"
            + "  (select max(SHOWORDER) as SHOWORDER from SECTIONDOCTORMASTER WHERE SHOWORDER >= {2}) b)";

        /// <summary>
        /// 診療科医師マスタ 表示順取得
        /// </summary>
        public const string RRIS_SECTIONDOCTORMASTER_SELECT_MAX_SHOWORDER =
              " (select nvl(max(SHOWORDER), 0) + 1 as SHOWORDER from SECTIONDOCTORMASTER)";

        #endregion
    }

}
