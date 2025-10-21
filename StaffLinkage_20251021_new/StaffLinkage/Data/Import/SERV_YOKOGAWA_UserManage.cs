﻿using System;
using System.Data;
using System.Reflection;
using UsersIFLinkage.Data.Export.Entity;
using UsersIFLinkage.Data.Import.Common;
using UsersIFLinkage.Data.Import.Entity;
using StaffLinkage.Util;

namespace UsersIFLinkage.Data.Import
{
    class SERV_YOKOGAWA_UserManage
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
        private static string[] updCols =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.YOKOGAWA_USERMANAGE_UPD_COLS).ToUpper().Replace(" ", "").Split(',');

        #endregion

        #region function

        /// <summary>
        /// マッピング処理
        /// </summary>
        /// <param name="tousersRow"></param>
        /// <param name="usermanage"></param>
        /// <param name="db"></param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Mapping(DataRow tousersRow, ref SERV_YOKOGAWA_UserManageEntity usermanage, OracleDataBase db)
        {
            try
            {
                usermanage.Userid = tousersRow[ToUsersInfoEntity.F_USERID].ToString();
                usermanage.Hospitalid = tousersRow[ToUsersInfoEntity.F_HOSPITALID].ToString();
                usermanage.Password = tousersRow[ToUsersInfoEntity.F_PASSWORD].ToString();
                usermanage.Username = ImportUtil.ConvertGaiji(
                                            tousersRow[ToUsersInfoEntity.F_USERNAMEKANJI].ToString(), 
                                            AppConfigParameter.YOKOGAWA_CONVERT_GAIJI,
                                            AppConfigParameter.YOKOGAWA_GAIJI_REPLACE);
                usermanage.Usernameeng = tousersRow[ToUsersInfoEntity.F_USERNAMEENG].ToString();
                usermanage.Passwordexpirydate = ImportUtil.ConvertDateTime(tousersRow[ToUsersInfoEntity.F_PASSWORDEXPIRYDATE].ToString());
                usermanage.Passwordwarningdate = ImportUtil.ConvertDateTime(tousersRow[ToUsersInfoEntity.F_PASSWORDWARNINGDATE].ToString());
                usermanage.Useridvalidityflag = tousersRow[ToUsersInfoEntity.F_USERIDVALIDITYFLAG].ToString();
                // 2022.04.08 Mod K.Yasuda@COSMO Start
                // 2025.02.xx Mod Cosmo＠Yamamoto Start   自衛隊札幌病院改修対応
                //usermanage.Belongingdepartment = tousersRow[ToUsersInfoEntity.F_SECTION_ID].ToString();
                usermanage.Belongingdepartment = null;
                // 2025.02.xx Mod Cosmo＠Yamamoto End   自衛隊札幌病院改修対応
                //usermanage.Belongingdepartment = SERV_YOKOGAWA_UserManageEntity.BELONGINGDEPARTMENT;
                // 2022.04.08 Mod K.Yasuda@COSMO End
                usermanage.Maingroupid = null;
                usermanage.Subgroupidlist = null;
                usermanage.Updatedatetime = ImportUtil.SYSDATE;
                usermanage.Office_id = null;

                // データをログに出力
                //_log.Debug(usermanage.ToString());
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
        /// <param name="usermanage"></param>
        /// <param name="tousersRow"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool Merge(
                SERV_YOKOGAWA_UserManageEntity usermanage, DataRow tousersRow, OracleDataBase db)
        {
            string query = string.Empty;

            try
            {
                // 新規「US01」の場合
                if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() == 
                        ToUsersInfoEntity.REQUESTTYPE_US01)
                {
                    query = string.Format(
                                SERV_QUERY.YOKOGAWA_USERMANAGE_MERGE,
                                OracleDataBase.SingleQuotes(usermanage.Userid),
                                OracleDataBase.SingleQuotes(usermanage.Hospitalid),
                                ImportUtil.ConvertMD5(usermanage.Password, usermanage.Userid, AppConfigParameter.YOKOGAWA_CONVERT_MD5),
                                OracleDataBase.SingleQuotes(usermanage.Username),
                                OracleDataBase.SingleQuotes(usermanage.Usernameeng),
                                OracleDataBase.ConvertNull(usermanage.Passwordexpirydate),
                                OracleDataBase.ConvertNull(usermanage.Passwordwarningdate),
                                OracleDataBase.SingleQuotes("1"),
                                OracleDataBase.SingleQuotes(usermanage.Belongingdepartment),
                                OracleDataBase.SingleQuotes(usermanage.Maingroupid),
                                OracleDataBase.SingleQuotes(usermanage.Subgroupidlist),
                                usermanage.Updatedatetime,
                                OracleDataBase.SingleQuotes(usermanage.Office_id),
                                GetUpdateSql(usermanage)
                                );
                // 2025.02.xx Mod Cosmo＠Yamamoto Start   自衛隊札幌病院改修対応
                //}
                //else
                //{
                //    query = string.Format(
                //                SERV_QUERY.YOKOGAWA_USERMANAGE_DELETE,
                //                OracleDataBase.SingleQuotes(usermanage.Userid),
                //                OracleDataBase.SingleQuotes(usermanage.Hospitalid),
                //                OracleDataBase.SingleQuotes("0"),
                //                usermanage.Updatedatetime
                //                );
                    // 登録
                    db.ExecuteQuery(query);
                // 2025.02.xx Mod Cosmo＠Yamamoto End   自衛隊札幌病院改修対応
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
        /// <param name="usermanage"></param>
        /// <returns></returns>
        private static string GetUpdateSql(SERV_YOKOGAWA_UserManageEntity usermanage)
        {
            string updateSql = string.Empty;
            string col = string.Empty;

            // パスワード
            col = "PASSWORD";
            if (Array.IndexOf(updCols, col) > -1)
            {
                updateSql += col + " = " + ImportUtil.ConvertMD5(usermanage.Password, usermanage.Userid, AppConfigParameter.YOKOGAWA_CONVERT_MD5); 
            }

            // ユーザ名称
            col = "USERNAME";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }

                updateSql += col + " = " + OracleDataBase.SingleQuotes(usermanage.Username);
            }

            // ユーザ名称英字
            col = "USERNAMEENG";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }

                updateSql += col + " = " + OracleDataBase.SingleQuotes(usermanage.Usernameeng);
            }

            // パスワード有効期限日
            col = "PASSWORDEXPIRYDATE";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }
                
                updateSql += col + " = " + OracleDataBase.ConvertNull(usermanage.Passwordexpirydate);
            }

            // パスワード警告開始日
            col = "PASSWORDWARNINGDATE";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }

                updateSql += col + " = " + OracleDataBase.ConvertNull(usermanage.Passwordwarningdate);
            }

            // ユーザID有効フラグ
            col = "USERIDVALIDITYFLAG";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }
                
                updateSql += col + " = " + OracleDataBase.SingleQuotes(usermanage.Useridvalidityflag);
            }

            // 2022.04.08 Add K.Yasuda@COSMO Start
            // ユーザID有効フラグ
            col = "BELONGINGDEPARTMENT";
            if (Array.IndexOf(updCols, col) > -1)
            {
                if (!string.IsNullOrEmpty(updateSql))
                {
                    updateSql += ",";
                }

                updateSql += col + " = " + OracleDataBase.SingleQuotes(usermanage.Belongingdepartment);
            }
            // 2022.04.08 Add K.Yasuda@COSMO End

            if (!string.IsNullOrEmpty(updateSql))
            {
                updateSql += ",";
            }

            return updateSql;
        }

        #endregion
    }
}
