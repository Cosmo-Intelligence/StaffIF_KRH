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
  class THERARIS_RTRIS_UserManage
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
    //Y_Higuchi -- del --         AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_USERMANAGE_UPD_COLS).ToUpper().Replace(" ", "").Split(',');
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
        string strupd = htBuf["USERMANAGE"].ToString();
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
    /// <param name="usermanage"></param>
    /// <param name="db"></param>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    public static bool Mapping(DataRow tousersRow, ref THERARIS_RTRIS_UserManageEntity usermanage, OracleDataBase db)
    {
      try
      {
        // Y_Higuchi -- add --
        Read_UPD_COLS();
        // Y_Higuchi -- add --
        usermanage.Userid = tousersRow[ToUsersInfoEntity.F_USERID].ToString();
        usermanage.Hospitalid = tousersRow[ToUsersInfoEntity.F_HOSPITALID].ToString();
        usermanage.Password = tousersRow[ToUsersInfoEntity.F_PASSWORD].ToString();
        usermanage.Username = ImportUtil.ConvertGaiji(
                                    tousersRow[ToUsersInfoEntity.F_USERNAMEKANJI].ToString(),
                                    AppConfigParameter.RTRIS_CONVERT_GAIJI,
                                    AppConfigParameter.RTRIS_GAIJI_REPLACE);
        usermanage.Usernameeng = tousersRow[ToUsersInfoEntity.F_USERNAMEENG].ToString();
        usermanage.Passwordexpirydate = ImportUtil.ConvertDateTime(tousersRow[ToUsersInfoEntity.F_PASSWORDEXPIRYDATE].ToString());
        usermanage.Passwordwarningdate = ImportUtil.ConvertDateTime(tousersRow[ToUsersInfoEntity.F_PASSWORDWARNINGDATE].ToString());
        usermanage.Useridvalidityflag = tousersRow[ToUsersInfoEntity.F_USERIDVALIDITYFLAG].ToString();
        usermanage.Belongingdepartment = null;
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
            THERARIS_RTRIS_UserManageEntity usermanage, DataRow tousersRow, OracleDataBase db)
    {
      string query = string.Empty;

      try
      {
        // 新規「US01」の場合
        if (tousersRow[ToUsersInfoEntity.F_REQUESTTYPE].ToString() ==
                ToUsersInfoEntity.REQUESTTYPE_US01)
        {
          query = string.Format(
                      THERARIS_QUERY.RTRIS_USERMANAGE_MERGE,
                      OracleDataBase.SingleQuotes(usermanage.Userid),
                      OracleDataBase.SingleQuotes(usermanage.Hospitalid),
                      ImportUtil.ConvertMD5(usermanage.Password, usermanage.Userid, AppConfigParameter.RTRIS_CONVERT_MD5),
                      OracleDataBase.SingleQuotes(usermanage.Username),
                      OracleDataBase.SingleQuotes(usermanage.Usernameeng),
                      OracleDataBase.ConvertNull(usermanage.Passwordexpirydate),
                      OracleDataBase.ConvertNull(usermanage.Passwordwarningdate),
                      OracleDataBase.SingleQuotes(usermanage.Useridvalidityflag),
                      OracleDataBase.SingleQuotes(usermanage.Belongingdepartment),
                      OracleDataBase.SingleQuotes(usermanage.Maingroupid),
                      OracleDataBase.SingleQuotes(usermanage.Subgroupidlist),
                      usermanage.Updatedatetime,
                      OracleDataBase.SingleQuotes(usermanage.Office_id),
                      GetUpdateSql(usermanage)
                      );
        }
        else
        {
          query = string.Format(
                      THERARIS_QUERY.RTRIS_USERMANAGE_DELETE,
                      OracleDataBase.SingleQuotes(usermanage.Userid),
                      OracleDataBase.SingleQuotes(usermanage.Hospitalid),
                      OracleDataBase.SingleQuotes(usermanage.Useridvalidityflag),
                      usermanage.Updatedatetime
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
    /// UPDATE文取得
    /// </summary>
    /// <param name="usermanage"></param>
    /// <returns></returns>
    private static string GetUpdateSql(THERARIS_RTRIS_UserManageEntity usermanage)
    {
      string updateSql = string.Empty;
      string col = string.Empty;

      // パスワード
      col = "PASSWORD";
      if (Array.IndexOf(updCols, col) > -1)
      {
        updateSql += col + " = " + ImportUtil.ConvertMD5(usermanage.Password, usermanage.Userid, AppConfigParameter.RTRIS_CONVERT_MD5);
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

      // UPDATEする項目が存在した場合
      if (!string.IsNullOrEmpty(updateSql))
      {
        updateSql = " when matched then update set " + updateSql;
        updateSql += ",UPDATEDATETIME = " + usermanage.Updatedatetime;
      }

      return updateSql;
    }

    #endregion
  }
}
