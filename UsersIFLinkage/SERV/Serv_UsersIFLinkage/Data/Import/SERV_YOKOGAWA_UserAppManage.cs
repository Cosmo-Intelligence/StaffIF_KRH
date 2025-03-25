using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Serv_UsersIFLinkage.Data.Export.Entity;
using Serv_UsersIFLinkage.Data.Import.Common;
using Serv_UsersIFLinkage.Data.Import.Entity;
using Serv_UsersIFLinkage.Util;

namespace Serv_UsersIFLinkage.Data.Import
{
    class SERV_YOKOGAWA_UserAppManage
    {
        #region private

        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 設定ファイル：有効フラグ設定値
        /// </summary>
        private static string licencetouse =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.YOKOGAWA_USERAPPMANAGE_LICENCETOUSE);

        #endregion

        #region function

        /// <summary>
        /// マッピング処理
        /// </summary>
        /// <param name="tousersRow"></param>
        /// <param name="appmanageList"></param>
        /// <param name="db"></param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Mapping(DataRow tousersRow, ref List<SERV_YOKOGAWA_UserAppManageEntity> appmanageList, OracleDataBase db)
        {
            try
            {
                foreach (string appcode in tousersRow[ToUsersInfoEntity.F_APPCODE].ToString().Split(','))
                {
                    SERV_YOKOGAWA_UserAppManageEntity appmanage = new SERV_YOKOGAWA_UserAppManageEntity();

                    appmanage.Userid = tousersRow[ToUsersInfoEntity.F_USERID].ToString();
                    appmanage.Hospitalid = tousersRow[ToUsersInfoEntity.F_HOSPITALID].ToString();
                    appmanage.Appcode = appcode;
                    appmanage.Licencetouse = GetUseFlag(tousersRow[ToUsersInfoEntity.F_USERIDVALIDITYFLAG].ToString());
                    appmanage.Myattrid = GetMyattrid(
                                                    appcode,
                                                    tousersRow[ToUsersInfoEntity.F_USERID].ToString(),
                                                    tousersRow[ToUsersInfoEntity.F_HOSPITALID].ToString()
                                                    );
                    appmanage.Updatedatetime = ImportUtil.SYSDATE;

                    appmanageList.Add(appmanage);
                    // データをログに出力
                    //_log.Debug(appmanage.ToString());
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
        /// 初期化更新処理
        /// </summary>
        /// <param name="appmanageList"></param>
        /// <param name="tousersRow"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool InitUpdate(List<SERV_YOKOGAWA_UserAppManageEntity> appmanageList, DataRow tousersRow, OracleDataBase db)
        {
            try
            {
                foreach (SERV_YOKOGAWA_UserAppManageEntity appmanage in appmanageList)
                {
                    // 新規「US01」以外の場合
                    if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() !=
                            ToUsersInfoEntity.REQUESTTYPE_US01)
                    {
                        // 更新
                        db.ExecuteQuery(
                            SERV_QUERY.YOKOGAWA_USERAPPMANAGE_UPDATE,
                            appmanage.Userid,
                            appmanage.Hospitalid,
                            SERV_YOKOGAWA_UserAppManageEntity.LICENCETOUSE_FALSE
                            );
                    }
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
        /// 登録処理
        /// </summary>
        /// <param name="appmanageList"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static bool Merge(List<SERV_YOKOGAWA_UserAppManageEntity> appmanageList, DataRow tousersRow, OracleDataBase db)
        {
            try
            {
                foreach (SERV_YOKOGAWA_UserAppManageEntity appmanage in appmanageList)
                {
                    // 新規の場合
                    if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() ==
                            ToUsersInfoEntity.REQUESTTYPE_US01)
                    {
                        // 登録
                        db.ExecuteQuery(
                            string.Format(SERV_QUERY.YOKOGAWA_USERAPPMANAGE_MERGE,
                                                OracleDataBase.SingleQuotes(appmanage.Userid),
                                                OracleDataBase.SingleQuotes(appmanage.Hospitalid),
                                                OracleDataBase.SingleQuotes(appmanage.Appcode),
                                                OracleDataBase.SingleQuotes(appmanage.Licencetouse),
                                                OracleDataBase.SingleQuotes(appmanage.Myattrid),
                                                appmanage.Updatedatetime)
                                                );
                    }
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
        /// 属性管理識別子取得
        /// </summary>
        /// <param name="appcode"></param>
        /// <param name="userid"></param>
        /// <param name="hospitalid"></param>
        /// <returns></returns>
        private static string GetMyattrid(string appcode, string userid, string hospitalid)
        {
            return string.Format(SERV_YOKOGAWA_UserAppManageEntity.MYATTRID, appcode, userid, hospitalid);
        }

        /// <summary>
        /// 有効フラグ取得
        /// </summary>
        /// <param name="useridvalidityflag"></param>
        /// <returns></returns>
        private static string GetUseFlag(string useridvalidityflag)
        {
            if (!string.IsNullOrEmpty(licencetouse))
            {
                return licencetouse;
            }

            return useridvalidityflag;
        }

        #endregion
    }
}
