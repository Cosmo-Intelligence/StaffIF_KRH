using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Report_UsersIFLinkage.Data.Export.Entity;
using Report_UsersIFLinkage.Data.Import.Common;
using Report_UsersIFLinkage.Data.Import.Entity;
using Report_UsersIFLinkage.Util;

namespace Report_UsersIFLinkage.Data.Import
{
  class REPORT_MRMS_RequestDoctorMaster
    {
        #region private

        // ログ出力
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region function

        /// <summary>
        /// マッピング処理
        /// </summary>
        /// <param name="tousersRow">ユーザ情報1件</param>
        /// <param name="requestDrList">ユーザ情報(診療科IDの件数分)</param>
        /// <param name="db">DB接続情報</param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Mapping(DataRow tousersRow, ref List<REPORT_MRMS_RequestDoctorMasterEntity> requestDrList, OracleDataBase db)
        {
            try
            {
                // ユーザの診療科IDの項目数分マッピング処理を行う
                foreach (string section_id in tousersRow[ToUsersInfoEntity.F_TANTO_SECTION_ID].ToString().Split(','))
                {
                    REPORT_MRMS_RequestDoctorMasterEntity requestDr = new REPORT_MRMS_RequestDoctorMasterEntity();

                    // IDとShoworderはテーブルの最大値＋1の値を入れる
                    requestDr.Id = REPORT_QUERY.MRMS_REQUESTDOCTORMASTER_SELECT_MAX_ID;
                    requestDr.Name = tousersRow[ToUsersInfoEntity.F_USERNAMEKANJI].ToString();
                    requestDr.Attribute = section_id;
                    requestDr.Showorder = REPORT_QUERY.MRMS_REQUESTDOCTORMASTER_SELECT_MAX_SHOWORDER;

                    // ユーザの診療科IDの項目数分Listに格納する
                    requestDrList.Add(requestDr);
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
        /// <param name="tousersRow">ユーザ情報1件</param>
        /// <param name="requestDrList">医師情報List</param>
        /// <param name="db">DB接続情報</param>
        /// <returns>正常ならtrue、異常ならfalse</returns>
        public static bool Merge(DataRow tousersRow, List<REPORT_MRMS_RequestDoctorMasterEntity> requestDrList, OracleDataBase db)
        {
            string query = string.Empty;

            try
            {
                // 新規「US01」の場合
                if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() == ToUsersInfoEntity.REQUESTTYPE_US01)
                {
                    // ユーザの診療科IDの件数分、登録処理を行う
                    foreach (REPORT_MRMS_RequestDoctorMasterEntity requestDr in requestDrList)
                    {
                        // 同じユーザ情報が登録されていない場合
                        if (requestDrCheck(requestDr.Name, requestDr.Attribute, db))
                        {
                            // 新規登録用SQL文作成
                            query = string.Format(REPORT_QUERY.MRMS_REQUESTDOCTORMASTER_MERGE,
                                                    requestDr.Id,
                                                    OracleDataBase.SingleQuotes(requestDr.Name),
                                                    requestDr.Attribute,
                                                    requestDr.Showorder
                                                    );

                            // 新規登録処理
                            db.ExecuteQuery(query);
                        }
                    }
                }
                else
                {
                    _log.InfoFormat(" 処理種別IDが「US01」でないため登録できません。【登録対象】Name={0} Tanto_Section_ID={1} RequestType={2} ",
                        tousersRow[ToUsersInfoEntity.F_USERNAMEKANJI].ToString(),
                        tousersRow[ToUsersInfoEntity.F_TANTO_SECTION_ID].ToString(),
                        tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString());
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
        /// ユーザ情報存在チェック
        /// </summary>
        /// <param name="Name">医師名</param>
        /// <param name="Atteribute">診療科ID</param>
        /// <param name="db">DB接続情報</param>
        /// <returns>trueなら登録、falseなら登録しない</returns>
        private static bool requestDrCheck(string Name, string Attribute, OracleDataBase db)
        {
            DataTable dt = new DataTable();
            string query = string.Empty;

            // マッピングした医師名または診療科IDが空文字またはNullでないかチェックする
            if (string.IsNullOrEmpty(Name) ||　string.IsNullOrEmpty(Attribute))
            {
                _log.InfoFormat(" 医師名または診療科IDに値がないためREQUESTDOCTORMASTERに登録できません。【登録対象】Name={0}, Attribute={1} ",
                    Name,　Attribute);
                return false;
            }

            // ユーザ情報チェック用Select文作成
            query = string.Format(REPORT_QUERY.MRMS_REQUESTDOCTORMASTER_SELECT_DATA,
                        OracleDataBase.SingleQuotes(Name),
                        OracleDataBase.SingleQuotes(Attribute));

            // ユーザ情報取得
            db.GetDataTable(query, ref dt);

            // すでに登録されているかチェックする
            if (dt.Rows.Count > 0)
            {
                _log.InfoFormat(" 登録対象のレコードは既にREQUESTDOCTORMASTERに登録されています。【登録対象】Name={0}, Attribute={1} ",
                    Name, Attribute);
                return false;
            }

            return true;
        }

        #endregion
    }
}
