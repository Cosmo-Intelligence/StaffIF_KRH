
namespace Serv_UsersIFLinkage.Data.Export.Common
{
    class ConstQuery
    {
        #region QUERY

        #region QUEUEテーブル

        /// <summary>
        /// ユーザ情報連携I/F取得
        /// </summary>
        public const string TOUSERSINFO_SELECT =
                  " select"
                + "   *"
                + " from"
                + "   (select"
                + "     REQUESTID,"
                + "     REQUESTDATE,"
                + "     DB,"
                + "     APPCODE,"
                + "     USERID,"
                + "     HOSPITALID,"
                + "     PASSWORD,"
                + "     USERNAMEKANJI,"
                + "     USERNAMEENG,"
                + "     SECTION_ID,"
				// Y_Higuchi -- del --
                //+ "     SECTION_NAME,"
				// Y_Higuchi -- del --
                + "     TANTO_SECTION_ID,"
                + "     STAFFID,"
                + "     SYOKUIN_KBN,"
                + "     TEL,"
                + "     PASSWORDEXPIRYDATE,"
                + "     PASSWORDWARNINGDATE,"
                + "     USERIDVALIDITYFLAG,"
                + "     REQUESTTYPE,"
                + "     MESSAGEID1,"
                + "     MESSAGEID2,"
                + "     MESSAGEID3,"
                + "     TRANSFERSTATUS,"
                + "     TRANSFERDATE,"
                + "     TRANSFERRESULT,"
                + "     TRANSFERTEXT"
                + "   from"
      // Y_Higuchi -- add --
	      + "     TOUSERINFO_SRV"
      // Y_Higuchi -- add --
      // Y_Higuchi -- del --
      //	+ "     TOUSERSINFO"
      // Y_Higuchi -- del --
	      + "   where"
      // Y_Higuchi -- add --
                + "     (TRANSFERSTATUS = '00') AND (MESSAGEID1 IS NOT NULL) AND (MESSAGEID1 <> 'ORIGINAL_USER_INFO_RECORD')"
      // Y_Higuchi -- add --
      // Y_Higuchi -- del --
      //          + "     TRANSFERSTATUS = '00'"
      // Y_Higuchi -- del --
                + "   order by"
        + "     REQUESTDATE)"
                + " where"
                + "   ROWNUM <= {0}";

        /// <summary>
        /// ユーザ情報連携I/F削除
        /// </summary>
        public const string TOUSERSINFO_DELETE =
                  " delete"
                + " from"
			// Y_Higuchi -- add --
				+ "     TOUSERINFO_SRV"
			// Y_Higuchi -- add --
			// Y_Higuchi -- del --
            //  + "   TOUSERSINFO"
			// Y_Higuchi -- del --
                + " where"
                + "   TRANSFERSTATUS in ({0})"
                + " and"
                + "   REQUESTDATE <= (SYSDATE - {1})";

        /// <summary>
        /// ユーザ情報連携I/F処理結果更新
        /// </summary>
        public const string TOUSERSINFO_UPDATE =
                  " update"
			// Y_Higuchi -- add --
				+ "     TOUSERINFO_SRV"
			// Y_Higuchi -- add --
			// Y_Higuchi -- del --
            //  + "   TOUSERSINFO"
			// Y_Higuchi -- del --
                + " set"
                + "   TRANSFERSTATUS = {1},"
                + "   TRANSFERRESULT = {2},"
                + "   TRANSFERTEXT   = {3},"
                + "   TRANSFERDATE   = sysdate"
                + " where"
                + "   REQUESTID = {0}";

        #endregion

        #endregion
    }
}
