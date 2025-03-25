using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using TheraRis_UsersIFLinkage.Data.Export.Entity;
using TheraRis_UsersIFLinkage.Data.Import.Common;
using TheraRis_UsersIFLinkage.Data.Import.Entity;
using TheraRis_UsersIFLinkage.Util;

namespace TheraRis_UsersIFLinkage.Data.Import
{
    class THERARIS_RTRIS_UserAppManage
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
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_USERAPPMANAGE_LICENCETOUSE);

        #endregion

        #region function

        /// <summary>
        /// マッピング処理
        /// </summary>
        /// <param name="tousersRow"></param>
        /// <param name="appmanageList"></param>
        /// <param name="db"></param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Mapping(DataRow tousersRow, ref List<THERARIS_RTRIS_UserAppManageEntity> appmanageList, OracleDataBase db)
        {
            try
            {
                foreach (string appcode in tousersRow[ToUsersInfoEntity.F_APPCODE].ToString().Split(','))
                {
                    THERARIS_RTRIS_UserAppManageEntity appmanage = new THERARIS_RTRIS_UserAppManageEntity();

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
        public static bool InitUpdate(List<THERARIS_RTRIS_UserAppManageEntity> appmanageList, DataRow tousersRow, OracleDataBase db)
        {
            try
            {
                foreach (THERARIS_RTRIS_UserAppManageEntity appmanage in appmanageList)
                {
                    // 新規「US01」以外の場合
                    if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() !=
                            ToUsersInfoEntity.REQUESTTYPE_US01)
                    {
                        // 更新
                        db.ExecuteQuery(
                            THERARIS_QUERY.RTRIS_USERAPPMANAGE_UPDATE,
                            appmanage.Userid,
                            appmanage.Hospitalid,
                            THERARIS_RTRIS_UserAppManageEntity.LICENCETOUSE_FALSE
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
        public static bool Merge(List<THERARIS_RTRIS_UserAppManageEntity> appmanageList, DataRow tousersRow, OracleDataBase db)
        {
            try
            {
                foreach (THERARIS_RTRIS_UserAppManageEntity appmanage in appmanageList)
                {
                    // 新規の場合
                    if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() ==
                            ToUsersInfoEntity.REQUESTTYPE_US01)
                    {
                        // 登録
                        db.ExecuteQuery(
                            string.Format(THERARIS_QUERY.RTRIS_USERAPPMANAGE_MERGE,
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
        /// 使用許可フラグ取得
        /// </summary>
        /// <param name="requesttype"></param>
        /// <returns></returns>
        private static string GetLicencetouse(string requesttype)
        {
            string flg = string.Empty;

            if (requesttype == ToUsersInfoEntity.REQUESTTYPE_US99)
            {
                flg = THERARIS_RTRIS_UserAppManageEntity.LICENCETOUSE_FALSE;
            }
            else
            {
                flg = THERARIS_RTRIS_UserAppManageEntity.LICENCETOUSE_TRUE;
            }

            return flg;
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
            return string.Format(THERARIS_RTRIS_UserAppManageEntity.MYATTRID, appcode, userid, hospitalid);
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
