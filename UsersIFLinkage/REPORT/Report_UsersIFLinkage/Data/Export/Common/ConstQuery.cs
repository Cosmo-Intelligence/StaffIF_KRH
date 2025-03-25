
namespace Report_UsersIFLinkage.Data.Export.Common
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
                //+ "     SECTION_NAME,"
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
                + "     TOUSERINFO_REP"
                + "   where"
      // Y_Higuchi -- change --
                + "     (TRANSFERSTATUS = '00') AND (MESSAGEID1 IS NOT NULL) AND (MESSAGEID1 <> 'ORIGINAL_USER_INFO_RECORD')"
      // Y_Higuchi -- change --
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
                + "   TOUSERINFO_REP"
                + " where"
                + "   TRANSFERSTATUS in ({0})"
                + " and"
                + "   REQUESTDATE <= (SYSDATE - {1})";

        /// <summary>
        /// ユーザ情報連携I/F処理結果更新
        /// </summary>
        public const string TOUSERSINFO_UPDATE =
                  " update"
                + "   TOUSERINFO_REP"
                + " set"
                // Y_Higuchi -- change -- MESSAGEID1 も更新する
                + "   MESSAGEID1 = {4},"
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
