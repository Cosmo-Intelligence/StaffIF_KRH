using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using TheraRis_UsersIFLinkage.Data.Export.Entity;
using TheraRis_UsersIFLinkage.Data.Import;
using TheraRis_UsersIFLinkage.Data.Import.Entity;
using TheraRis_UsersIFLinkage.Util;

namespace TheraRis_UsersIFLinkage.Ctrl
{
  class THERARIS_RTRIS_LinkageController
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

    public THERARIS_RTRIS_LinkageController(OracleDataBase odbc)
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

      // Y_Higuchi --del --
      //if (tousersRow[ToUsersInfoEntity.F_DB].ToString().ToUpper() == ToUsersInfoEntity.DB_THERARIS)
      //{
      // Y_Higuchi --del --
      // Y_Higuchi --add --
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC01)
      {
        // Y_Higuchi --add --

        // ① ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = THERARIS_RTRIS_UserManageEntity.EntityName;

        THERARIS_RTRIS_UserManageEntity manage = new THERARIS_RTRIS_UserManageEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザ管理マッピング処理
        if (!THERARIS_RTRIS_UserManage.Mapping(tousersRow, ref manage, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザ管理更新処理
        if (!THERARIS_RTRIS_UserManage.Merge(manage, tousersRow, db))
        {
          _log.InfoFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // Y_Higuchi --add --
      }
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC02)
      {
        // Y_Higuchi --add --

        // ② ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = THERARIS_RTRIS_UserAppManageEntity.EntityName;

        List<THERARIS_RTRIS_UserAppManageEntity> appmanageList = new List<THERARIS_RTRIS_UserAppManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザアプリケーション管理マッピング処理
        if (!THERARIS_RTRIS_UserAppManage.Mapping(tousersRow, ref appmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザアプリケーション管理更新処理
        if (!THERARIS_RTRIS_UserAppManage.Merge(appmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // ③ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = THERARIS_RTRIS_AttrManageEntity.EntityName;

        List<THERARIS_RTRIS_AttrManageEntity> attrmanageList = new List<THERARIS_RTRIS_AttrManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 属性管理マッピング処理
        if (!THERARIS_RTRIS_AttrManage.Mapping(tousersRow, ref attrmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // 属性管理更新処理
        if (!THERARIS_RTRIS_AttrManage.Merge(attrmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // Y_Higuchi --add --
      }
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC01)
      {
        // Y_Higuchi --add --
        //// ④ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = THERARIS_RTRIS_UserManageCompEntity.EntityName;

        THERARIS_RTRIS_UserManageCompEntity managecomp = new THERARIS_RTRIS_UserManageCompEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // クライアントユーザ管理処理
        if (!THERARIS_RTRIS_UserManageComp.Mapping(tousersRow, ref managecomp, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // クライアントユーザ管理更新処理
        if (!THERARIS_RTRIS_UserManageComp.Merge(managecomp, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }
        // Y_Higuchi --add --
      }
      // Y_Higuchi --add --

      // 登録対象確認
      // Y_Higuchi --del --
      //if (ImportUtil.IsRegist(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_IMPORT,
      //        tousersRow[ToUsersInfoEntity.F_SYOKUIN_KBN].ToString(),
      //        tousersRow[ToUsersInfoEntity.F_SECTION_ID].ToString()))
      //{
      // Y_Higuchi --del --
      // Y_Higuchi --add --
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC03)
      {
        // Y_Higuchi --add --

        // ⑤ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = THERARIS_RRIS_SectionDoctorMasterEntity.EntityName;

        THERARIS_RRIS_SectionDoctorMasterEntity doc = new THERARIS_RRIS_SectionDoctorMasterEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 診療科医師マスタマッピング処理
        if (!THERARIS_RRIS_SectionDoctorMaster.Mapping(tousersRow, ref doc, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // 診療科医師マスタ更新処理
        if (!THERARIS_RRIS_SectionDoctorMaster.Merge(doc, tousersRow, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }
      }

      return true;
    }

      #endregion
    }
  }
