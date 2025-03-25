using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TheraRis_UsersIFLinkage.Data.Export.Entity;
using TheraRis_UsersIFLinkage.Data.Import.Common;
using TheraRis_UsersIFLinkage.Data.Import.Entity;
using TheraRis_UsersIFLinkage.Util;

namespace TheraRis_UsersIFLinkage.Data.Import
{
  class THERARIS_RTRIS_UserManageComp
    {
        #region private

        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// 設定ファイル：ユーザ管理更新対象カラム
    /// </summary>
    //Y_Higuchi -- del -- private static string[] updCols =
    //Y_Higuchi -- del --         AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_USERMANAGECOMP_UPD_COLS).ToUpper().Replace(" ", "").Split(',');
    private static string[] updCols;

        #endregion

        #region function
		// Y_Higuchi -- add --
		private static void Read_UPD_COLS()
		{
			try
			{
				// ユーザ登録条件ファイル(XML)
				Util.XmlUtil xmlR = new Util.XmlUtil();
        xmlR.strFilename = Path.Combine(Application.StartupPath, AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_TheraRIS));
				// 更新するフィールド名を取得
				Hashtable htBuf = new Hashtable();
				bool blnret = xmlR.xmlRead("UPD", htBuf);
				if (!blnret)
				{
					throw new Exception("ユーザ登録条件ファイル(xml)内に[UPD]定義が見つかりません。");
				}
				string strupd = htBuf["USERMANAGECOMP"].ToString();
				updCols = strupd.Split(',');
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		// Y_Higuchi -- add --

        /// <summary>
        /// マッピング処理
        /// </summary>
        /// <param name="tousersRow"></param>
        /// <param name="usermanagecomp"></param>
        /// <param name="db"></param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Mapping(DataRow tousersRow, ref THERARIS_RTRIS_UserManageCompEntity usermanagecomp, OracleDataBase db)
        {
            try
            {
				// Y_Higuchi -- add --
				Read_UPD_COLS();
				// Y_Higuchi -- add --
                usermanagecomp.Userid = tousersRow[ToUsersInfoEntity.F_USERID].ToString();
                usermanagecomp.Hospitalid = tousersRow[ToUsersInfoEntity.F_HOSPITALID].ToString();
                usermanagecomp.Password = tousersRow[ToUsersInfoEntity.F_PASSWORD].ToString();
                usermanagecomp.Commission = THERARIS_RTRIS_UserManageCompEntity.COMMISSION;
                usermanagecomp.Commission2 = null;
                usermanagecomp.Viewraccessctrlflag = THERARIS_RTRIS_UserManageCompEntity.VIEWRACCESSCTRLFLAG;
                usermanagecomp.Viewcaccessctrlflag = THERARIS_RTRIS_UserManageCompEntity.VIEWCACCESSCTRLFLAG;
                
                // データをログに出力
                //_log.Debug(usermanagecomp.ToString());
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 登録処理
        /// </summary>
        /// <param name="usermanagecomp"></param>
        /// <param name="tousersRow"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool Merge(
                THERARIS_RTRIS_UserManageCompEntity usermanagecomp, DataRow tousersRow, OracleDataBase db)
        {
            try
            {
                // 新規「US01」の場合
                if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() == 
                        ToUsersInfoEntity.REQUESTTYPE_US01)
                {
                    // 登録
                    db.ExecuteQuery(
                        string.Format(
                            THERARIS_QUERY.RTRIS_USERMANAGECOMP_MERGE,
                            OracleDataBase.SingleQuotes(usermanagecomp.Userid),
                            OracleDataBase.SingleQuotes(usermanagecomp.Hospitalid),
                            ImportUtil.ConvertMD5(usermanagecomp.Password, usermanagecomp.Userid, AppConfigParameter.RTRIS_USERMANAGECOMP_CONVERT_MD5),
                            OracleDataBase.SingleQuotes(usermanagecomp.Commission),
                            OracleDataBase.SingleQuotes(usermanagecomp.Commission2),
                            OracleDataBase.SingleQuotes(usermanagecomp.Viewraccessctrlflag),
                            OracleDataBase.SingleQuotes(usermanagecomp.Viewcaccessctrlflag),
                            GetUpdateSql(usermanagecomp)
                            )
                        );
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// UPDATE文取得
        /// </summary>
        /// <param name="usermanagecomp"></param>
        /// <returns></returns>
        private static string GetUpdateSql(THERARIS_RTRIS_UserManageCompEntity usermanagecomp)
        {
            string updateSql = string.Empty;
            string col = string.Empty;

            // パスワード
            col = "PASSWORD";
            if (Array.IndexOf(updCols, col) > -1)
            {
                updateSql += col + " = " + ImportUtil.ConvertMD5(usermanagecomp.Password, usermanagecomp.Userid, AppConfigParameter.RTRIS_USERMANAGECOMP_CONVERT_MD5); 
            }

            // UPDATEする項目が存在した場合
            if (!string.IsNullOrEmpty(updateSql))
            {
                updateSql = " when matched then update set " + updateSql; 
            }

            return updateSql;
        }

        #endregion
    }
}
