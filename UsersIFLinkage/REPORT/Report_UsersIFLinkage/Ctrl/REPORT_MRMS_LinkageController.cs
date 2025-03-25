using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Report_UsersIFLinkage.Data.Export.Entity;
using Report_UsersIFLinkage.Data.Import;
using Report_UsersIFLinkage.Data.Import.Entity;
using Report_UsersIFLinkage.Util;

namespace Report_UsersIFLinkage.Ctrl
{
  class REPORT_MRMS_LinkageController
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// DBクラス
    /// </summary>
    private OracleDataBase db = null;

    #endregion

    #region コンストラクタ

    public REPORT_MRMS_LinkageController(OracleDataBase odbc)
    {
      db = odbc;
    }

    #endregion

    #region ファンクション、メソッド

    /// <summary>
    /// 連携実行
    /// </summary>
    /// <param name="tousersRow"></param>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    public bool Execute(DataRow tousersRow)
    {
      string process = string.Empty;

      // Y_Higuchi --add --
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC01)
      {
        // Y_Higuchi --add --

        // ① ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = REPORT_MRMS_UserManageEntity.EntityName;

        REPORT_MRMS_UserManageEntity manage = new REPORT_MRMS_UserManageEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザ管理マッピング処理
        if (!REPORT_MRMS_UserManage.Mapping(tousersRow, ref manage, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザ管理更新処理
        if (!REPORT_MRMS_UserManage.Merge(manage, tousersRow, db))
        {
          _log.InfoFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // ② ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = REPORT_MRMS_UserInfo_CAEntity.EntityName;

        REPORT_MRMS_UserInfo_CAEntity userinfoca = new REPORT_MRMS_UserInfo_CAEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザ詳細情報管理マッピング処理
        if (!REPORT_MRMS_UserInfo_CA.Mapping(tousersRow, ref userinfoca, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザ詳細情報管理更新処理
        if (!REPORT_MRMS_UserInfo_CA.Merge(userinfoca, tousersRow, db))
        {
          _log.InfoFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // Y_Higuchi --add --
      }
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC02)
      {
        // Y_Higuchi --add --

        // ③ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = REPORT_MRMS_UserAppManageEntity.EntityName;

        List<REPORT_MRMS_UserAppManageEntity> appmanageList = new List<REPORT_MRMS_UserAppManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザアプリケーション管理マッピング処理
        if (!REPORT_MRMS_UserAppManage.Mapping(tousersRow, ref appmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザアプリケーション管理更新処理
        if (!REPORT_MRMS_UserAppManage.Merge(appmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // ④ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = REPORT_MRMS_AttrManageEntity.EntityName;

        List<REPORT_MRMS_AttrManageEntity> attrmanageList = new List<REPORT_MRMS_AttrManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 属性管理マッピング処理
        if (!REPORT_MRMS_AttrManage.Mapping(tousersRow, ref attrmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // 属性管理更新処理
        if (!REPORT_MRMS_AttrManage.Merge(attrmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // Y_Higuchi --add --
      }
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC03)
      {

        process = REPORT_MRMS_RequestDoctorMasterEntity.EntityName;

        List<REPORT_MRMS_RequestDoctorMasterEntity> requestDrList = new List<REPORT_MRMS_RequestDoctorMasterEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 依頼医マスタマッピング処理
        if (!REPORT_MRMS_RequestDoctorMaster.Mapping(tousersRow, ref requestDrList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}登録処理を実行します。", process);
        // 依頼医マスタ登録処理
        if (!REPORT_MRMS_RequestDoctorMaster.Merge(tousersRow, requestDrList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

      }
      // Y_Higuchi --add --
      return true;
    }

    #endregion
  }
}
