using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Ris_UsersIFLinkage.Data.Export.Entity;
using Ris_UsersIFLinkage.Data.Import;
using Ris_UsersIFLinkage.Data.Import.Common;
using Ris_UsersIFLinkage.Data.Import.Entity;
using Ris_UsersIFLinkage.Util;

namespace Ris_UsersIFLinkage.Ctrl
{
  class RIS_RRIS_LinkageController
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

    public RIS_RRIS_LinkageController(OracleDataBase odbc)
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
      //if (tousersRow[ToUsersInfoEntity.F_DB].ToString().ToUpper() == ToUsersInfoEntity.DB_RIS)
      //{
      // Y_Higuchi --del --
      // Y_Higuchi --add --
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC01)
      {
        // Y_Higuchi --add --

        // ① ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = RIS_RRIS_UserManageEntity.EntityName;

        RIS_RRIS_UserManageEntity manage = new RIS_RRIS_UserManageEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザ管理マッピング処理
        if (!RIS_RRIS_UserManage.Mapping(tousersRow, ref manage, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザ管理更新処理
        if (!RIS_RRIS_UserManage.Merge(manage, tousersRow, db))
        {
          _log.InfoFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // ② ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = RIS_RRIS_UserInfo_CAEntity.EntityName;

        RIS_RRIS_UserInfo_CAEntity userinfoca = new RIS_RRIS_UserInfo_CAEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザ詳細情報管理マッピング処理
        if (!RIS_RRIS_UserInfo_CA.Mapping(tousersRow, ref userinfoca, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザ詳細情報管理更新処理
        if (!RIS_RRIS_UserInfo_CA.Merge(userinfoca, tousersRow, db))
        {
          _log.InfoFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // Y_Higuchi --add -->>
      }

		// Y_Higuchi --add --2017.09.28-->>
		if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC05)
		{
				// ① ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
				process = RIS_RRIS_UserManageEntity.EntityName;

				RIS_RRIS_UserManageEntity manage = new RIS_RRIS_UserManageEntity();

				_log.InfoFormat("{0}更新用マッピング処理を実行します。", process);
				// ユーザ管理 更新用 マッピング処理
				if (!RIS_RRIS_UserManage.Mapping2(tousersRow, ref manage, db))
				{
					_log.ErrorFormat("{0}更新用マッピング処理でエラーが発生しました。", process);
					throw new Exception(string.Format("{0}更新用マッピング処理でエラーが発生しました。", process));
				}

				_log.InfoFormat("{0}既存ユーザ更新処理を実行します。", process);
				// ユーザ管理 既存ユーザ更新 処理
				if (!RIS_RRIS_UserManage.Merge2(manage, tousersRow, db))
				{
					_log.InfoFormat("{0}既存ユーザ更新処理でエラーが発生しました。", process);
					throw new Exception(string.Format("{0}既存ユーザ更新処理でエラーが発生しました。", process));
				}
			}
			// Y_Higuchi --add --2017.09.28--<<

			if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC02)
      {
        // Y_Higuchi --add --<<

        // ③ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = RIS_RRIS_UserAppManageEntity.EntityName;

        List<RIS_RRIS_UserAppManageEntity> appmanageList = new List<RIS_RRIS_UserAppManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // ユーザアプリケーション管理マッピング処理
        if (!RIS_RRIS_UserAppManage.Mapping(tousersRow, ref appmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // ユーザアプリケーション管理更新処理
        if (!RIS_RRIS_UserAppManage.Merge(appmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }

        // ④ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = RIS_RRIS_AttrManageEntity.EntityName;

        List<RIS_RRIS_AttrManageEntity> attrmanageList = new List<RIS_RRIS_AttrManageEntity>();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 属性管理マッピング処理
        if (!RIS_RRIS_AttrManage.Mapping(tousersRow, ref attrmanageList, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // 属性管理更新処理
        if (!RIS_RRIS_AttrManage.Merge(attrmanageList, tousersRow, db))
        {
          _log.ErrorFormat("{0}更新処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}更新処理でエラーが発生しました。", process));
        }
      }

      // 登録対象確認
      // Y_Higuchi --del --
      //if (ImportUtil.IsRegist(AppConfigParameter.RRIS_SECTIONDOCTORMASTER_IMPORT,
      //		tousersRow[ToUsersInfoEntity.F_SYOKUIN_KBN].ToString(),
      //		tousersRow[ToUsersInfoEntity.F_SECTION_ID].ToString()))
      //{
      // Y_Higuchi --del --
      // Y_Higuchi --add --
      if (tousersRow[ToUsersInfoEntity.F_MESSAGEID1].ToString() == Util.CommonParameter.NODE_NAME_EC03)
      {
        // Y_Higuchi --add --

        // ⑤ ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
        process = RIS_RRIS_SectionDoctorMasterEntity.EntityName;

        RIS_RRIS_SectionDoctorMasterEntity doc = new RIS_RRIS_SectionDoctorMasterEntity();

        _log.InfoFormat("{0}マッピング処理を実行します。", process);
        // 診療科医師マスタマッピング処理
        if (!RIS_RRIS_SectionDoctorMaster.Mapping(tousersRow, ref doc, db))
        {
          _log.ErrorFormat("{0}マッピング処理でエラーが発生しました。", process);
          throw new Exception(string.Format("{0}マッピング処理でエラーが発生しました。", process));
        }

        _log.InfoFormat("{0}更新処理を実行します。", process);
        // 診療科医師マスタ更新処理
        if (!RIS_RRIS_SectionDoctorMaster.Merge(doc, tousersRow, db))
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
