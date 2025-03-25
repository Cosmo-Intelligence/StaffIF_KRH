using System;
using System.Collections;
using System.Configuration;
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
  class THERARIS_RRIS_SectionDoctorMaster
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// 設定ファイル：表示順設定範囲 職員区分設定値内で空き番号がなかった場合の、範囲開始デフォルト値
    /// </summary>
    private static string ShowOrderDefault =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_SHOWORDER_DEFAULT);

    /// <summary>
    /// 設定ファイル：デフォルト選択メニューグループ デフォルト値
    /// </summary>
    private static string MenuGroupDefault =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP_DEFAULT);

    /// <summary>
    /// 設定ファイル：ユーザ管理更新対象カラム
    /// </summary>
    //Y_Higuchi -- del -- private static string[] updCols =
    //Y_Higuchi -- del --         AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_UPD_COLS).ToUpper().Replace(" ", "").Split(',');
    private static string[] updCols;

    /// <summary>
    /// 設定ファイル：有効フラグ設定値
    /// </summary>
    private static string useflag =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_USEFLAG);

//Y_Higuchi -- add -- 
    /// <summary>
    /// 物理士業務フラグ 設定値
    /// </summary>
    private static string physflag = AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_PHYSICIST_FLG);

        /// <summary>
    /// プラン確定権限フラグ 設定値
    /// </summary>
    private static string planflag = AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_PLAN_DECISION_AUTHORITY_FLG);

        /// <summary>
    /// 治療医登録対象フラグ 設定値
    /// </summary>
    private static string tredflag = AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_TREAT_DOCTORS_FLG);
//Y_Higuchi -- add -- 

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
        string strupd = htBuf["RRIS.SECTIONDOCTORMASTER"].ToString();
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
    /// <param name="doc"></param>
    /// <param name="db"></param>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    public static bool Mapping(DataRow tousersRow, ref THERARIS_RRIS_SectionDoctorMasterEntity doc, OracleDataBase db)
    {
      try
      {
        // Y_Higuchi -- add --
        Read_UPD_COLS();
        doc.Physicist_flg = CommonUtil.ConvertStrToInt(physflag);
        doc.Plan_decision_authority_flg = CommonUtil.ConvertStrToInt(planflag);
        doc.Treat_doctors_flg = CommonUtil.ConvertStrToInt(tredflag);
        // Y_Higuchi -- add --
        doc.Doctor_id = tousersRow[ToUsersInfoEntity.F_USERID].ToString();
        doc.Doctor_name = tousersRow[ToUsersInfoEntity.F_USERNAMEKANJI].ToString();
        doc.Doctor_english_name = tousersRow[ToUsersInfoEntity.F_USERNAMEENG].ToString();
        doc.Section_id = tousersRow[ToUsersInfoEntity.F_SECTION_ID].ToString();
        doc.Doctor_tel = tousersRow[ToUsersInfoEntity.F_TEL].ToString();
        doc.Tanto_section_id = GetTantoSectionId(
                                    tousersRow[ToUsersInfoEntity.F_USERID].ToString(),
                                    tousersRow[ToUsersInfoEntity.F_TANTO_SECTION_ID].ToString(), db);
        doc.Showorder = GetShowOrderSQL(tousersRow[ToUsersInfoEntity.F_SYOKUIN_KBN].ToString());
        doc.Syokuin_kbn = tousersRow[ToUsersInfoEntity.F_SYOKUIN_KBN].ToString();
        doc.Useflag = GetUseFlag(tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString(),
                                    tousersRow[ToUsersInfoEntity.F_USERIDVALIDITYFLAG].ToString());
        // Y_Higuchi -- del --
        //doc.Physicist_flg = null;
        //doc.Plan_decision_authority_flg = null;
        // Y_Higuchi -- del --
        doc.Default_menu_group = CommonUtil.ConvertStrToInt(
                                    ConfigurationManager.AppSettings[
                                        AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_DEFAULT_MENU_GROUP
                                        + tousersRow[ToUsersInfoEntity.F_SYOKUIN_KBN].ToString()]);
        if (doc.Default_menu_group == null)
        {
          // 取得できなかった場合はDEFAULT設定
          doc.Default_menu_group = CommonUtil.ConvertStrToInt(MenuGroupDefault);
        }

        // データをログに出力
        //_log.Debug(doc.ToString());
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
    /// <param name="doc"></param>
    /// <param name="tousersRow"></param>
    /// <param name="db"></param>
    /// <returns></returns>
    public static bool Merge(THERARIS_RRIS_SectionDoctorMasterEntity doc, DataRow tousersRow, OracleDataBase db)
    {
      string query = string.Empty;

      try
      {
        // 新規「US01」の場合
        if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() ==
                ToUsersInfoEntity.REQUESTTYPE_US01)
        {
          query = string.Format(
                      THERARIS_QUERY.RRIS_SECTIONDOCTORMASTER_MERGE,
                      OracleDataBase.SingleQuotes(doc.Doctor_id),
                      OracleDataBase.SingleQuotes(doc.Doctor_name),
                      OracleDataBase.SingleQuotes(doc.Doctor_english_name),
                      OracleDataBase.SingleQuotes(doc.Section_id),
                      OracleDataBase.SingleQuotes(doc.Doctor_tel),
                      OracleDataBase.SingleQuotes(doc.Tanto_section_id),
                      doc.Showorder,
                      OracleDataBase.SingleQuotes(doc.Syokuin_kbn),
                      doc.Useflag,
                      OracleDataBase.ConvertNull(doc.Physicist_flg),
                      OracleDataBase.ConvertNull(doc.Plan_decision_authority_flg),
                      OracleDataBase.ConvertNull(doc.Default_menu_group),
                      // Y_Higuchi -- add --
                      OracleDataBase.ConvertNull(doc.Treat_doctors_flg),
                      // Y_Higuchi -- add --
                      GetUpdateSql(doc)
                      );
        }
        else
        {
          query = string.Format(
                      THERARIS_QUERY.RRIS_SECTIONDOCTORMASTER_DELETE,
                      OracleDataBase.SingleQuotes(doc.Doctor_id),
                      doc.Useflag
                      );
        }

        // 登録
        db.ExecuteQuery(query);
      }
      catch (Exception ex)
      {
        _log.Error(ex.Message);
        return false;
      }

      return true;
    }

    /// <summary>
    /// 担当診療科ID取得
    /// </summary>
    /// <param name="docId"></param>
    /// <param name="tantosectionId"></param>
    /// <param name="db"></param>
    /// <returns>カラムサイズが100バイトなので、丁度おさまるようにする。</returns>
    private static string GetTantoSectionId(string docId, string tantosectionId, OracleDataBase db)
    {
      const int size = 100;

      DataTable secDt = new DataTable();

      // 担当診療科取得
      db.GetDataReader(THERARIS_QUERY.RRIS_SECTIONDOCTORMASTER_SELECT_TANTO_SECTION_ID, ref secDt, docId);

      string tsectionId = string.Empty;

      if (secDt.Rows.Count > 0)
      {
        tsectionId = secDt.Rows[0][0].ToString();
      }

      // 担当診療科が取得できなかった場合
      if (string.IsNullOrEmpty(tsectionId))
      {
        return tantosectionId;
      }

      // DBより取得した担当診療科を分割する
      string[] tsecIdList = tsectionId.Split(',');

      foreach (string sectionId in tantosectionId.Split(','))
      {
        // 重複していないか確認
        if (Array.IndexOf(tsecIdList, sectionId) > -1)
        {
          continue;
        }

        // TOUSERSINFO.担当診療科IDを連結すると100バイト超える場合
        if ((tsectionId + "," + sectionId).Length > size)
        {
          break;
        }

        tsectionId += "," + sectionId;
      }

      return tsectionId;
    }

    /// <summary>
    /// ShowOrderSQL文取得
    /// </summary>
    /// <param name="syokuinkbn"></param>
    /// <returns></returns>
    private static string GetShowOrderSQL(string syokuinkbn)
    {
      // 表示順設定範囲 職員区分を取得
      string confshoworder = ConfigurationManager.AppSettings[
                                  AppConfigParameter.RTRIS_SECTIONDOCTORMASTER_SHOWORDER + syokuinkbn];

      // 設定ファイルより取得できない場合
      if (string.IsNullOrEmpty(confshoworder))
      {
        return THERARIS_QUERY.RRIS_SECTIONDOCTORMASTER_SELECT_MAX_SHOWORDER;
      }

      string[] showorder = confshoworder.Split(',');

      return string.Format(THERARIS_QUERY.RRIS_SECTIONDOCTORMASTER_SELECT_SHOWORDER,
                              showorder[0], showorder[1], ShowOrderDefault);
    }

    /// <summary>
    /// 有効フラグ取得
    /// </summary>
    /// <param name="requesttype"></param>
    /// <param name="useridvalidityflag"></param>
    /// <returns></returns>
    private static int GetUseFlag(string requesttype, string useridvalidityflag)
    {
      int flg = THERARIS_RRIS_SectionDoctorMasterEntity.USEFLAG_TRUE;

      // 処理種別「US99：削除」の場合
      if (requesttype == ToUsersInfoEntity.REQUESTTYPE_US99)
      {
        flg = THERARIS_RRIS_SectionDoctorMasterEntity.USEFLAG_FALSE;
      }

      // 有効フラグ「0：無効」の場合
      if (useridvalidityflag == ToUsersInfoEntity.USERID_VALIDITY_FLAG_FALSE)
      {
        flg = THERARIS_RRIS_SectionDoctorMasterEntity.USEFLAG_FALSE;
      }

      int? use = CommonUtil.ConvertStrToInt(useflag);
      // 設定ファイル値が設定されていた場合
      if (use != null)
      {
        flg = (int)use;
      }

      return flg;
    }

    /// <summary>
    /// UPDATE文取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static string GetUpdateSql(THERARIS_RRIS_SectionDoctorMasterEntity doc)
    {
      string updateSql = string.Empty;
      string col = string.Empty;

      // 医師名称
      col = "DOCTOR_NAME";
      if (Array.IndexOf(updCols, col) > -1)
      {
        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Doctor_name);
      }

      // 医師名称（英名）
      col = "DOCTOR_ENGLISH_NAME";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Doctor_english_name);
      }

      // 診療科ID
      col = "SECTION_ID";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Section_id);
      }

      // PHS番号
      col = "DOCTOR_TEL";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Doctor_tel);
      }

      // 担当科
      col = "TANTO_SECTION_ID";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Tanto_section_id);
      }

      // 職員区分
      col = "SYOKUIN_KBN";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + OracleDataBase.SingleQuotes(doc.Syokuin_kbn);
      }

      // 使用可否ﾌﾗｸﾞ
      col = "USEFLAG";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + doc.Useflag;
      }

      // Y_Higuchi -- add --
      // 物理士業務フラグ
      col = "PHYSICIST_FLG";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }
        if (doc.Physicist_flg == null)
        {
          updateSql += col + " = NULL";
        }
        else
        {
          updateSql += col + " = " + doc.Physicist_flg;
        }
      }

      // プラン確定権限フラグ
      col = "PLAN_DECISION_AUTHORITY_FLG";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }
        if (doc.Plan_decision_authority_flg == null)
        {
          updateSql += col + " = NULL";
        }
        else
        {
          updateSql += col + " = " + doc.Plan_decision_authority_flg;
        }
      }
      // Y_Higuchi -- add --

      // デフォルト選択メニューグループ
      col = "DEFAULT_MENU_GROUP";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }

        updateSql += col + " = " + doc.Default_menu_group;
      }

      // Y_Higuchi -- add --
      // 治療医登録対象フラグ
      col = "TREAT_DOCTORS_FLG";
      if (Array.IndexOf(updCols, col) > -1)
      {
        if (!string.IsNullOrEmpty(updateSql))
        {
          updateSql += ",";
        }
        if (doc.Treat_doctors_flg == null)
        {
          updateSql += col + " = NULL";
        }
        else
        {
          updateSql += col + " = " + doc.Treat_doctors_flg;
        }
      }
      // Y_Higuchi -- add --

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
