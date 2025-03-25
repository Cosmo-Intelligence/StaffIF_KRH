using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TheraRis_UsersIFLinkage.Data.Export.Entity
{
  class ToUserInfoEntity_act : ToUsersInfoEntity
  {

    string[] flds = new string[] { F_REQUESTID, F_REQUESTDATE, F_DB, F_APPCODE
      , F_USERID            , F_HOSPITALID         , F_PASSWORD          , F_USERNAMEKANJI, F_USERNAMEENG
      , F_SECTION_ID        , F_TANTO_SECTION_ID   , F_STAFFID           , F_SYOKUIN_KBN  , F_TEL
      , F_PASSWORDEXPIRYDATE, F_PASSWORDWARNINGDATE, F_USERIDVALIDITYFLAG, F_REQUESTTYPE  , F_MESSAGEID1
      , F_MESSAGEID2        , F_MESSAGEID3         , F_TRANSFERSTATUS    , F_TRANSFERDATE , F_TRANSFERRESULT
      , F_TRANSFERTEXT };
    private const string TABLE_NAME="TOUSERINFO_THR";
    private const string SYORI_TAISYO_STATUS = "00";
    private const string SYORI_TAISYO_MSG1 = "ORIGINAL_USER_INFO_RECORD";
    private StringBuilder sbINSERT;
    private StringBuilder sbINSAPC;
    private DataTable dtToUsrInfo;

    /// <summary>
    /// テーブルのカラム情報を作成する
    /// </summary>
    private void Make_Column()
    {
      dtToUsrInfo = new DataTable();
      for (int flc = 0; flc < flds.Length; flc++)
      {
        // Column 情報
        DataColumn col = new DataColumn();

        // Column 名
        col.ColumnName = flds[flc];

        // Column データ型
        switch (flc)
        {
          case 0:
            col.DataType = System.Type.GetType("System.Int32");
            break;
          case 1:
          case 14:
          case 15:
          case 22:
            col.DataType = System.Type.GetType("System.DateTime");
            break;
          case 2:
          case 3:
          case 4:
          case 5:
          case 6:
          case 7:
          case 8:
          case 9:
          case 10:
          case 11:
          case 12:
          case 13:
          case 16:
          case 17:
          case 18:
          case 19:
          case 20:
          case 21:
          case 23:
          case 24:
            col.DataType = System.Type.GetType("System.String");
            break;
          default:
            break;
        }

        // Column 追加
        dtToUsrInfo.Columns.Add(col);
      }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ToUserInfoEntity_act()
    {
      // ([EC01], [EC03] データ挿入用) ---------------------------------------------------
      sbINSERT = new StringBuilder();
      sbINSERT.AppendLine(" INSERT INTO ");
      sbINSERT.AppendLine("   " + TABLE_NAME + "");
      sbINSERT.AppendLine(" SELECT");
      foreach (string val in flds)
      {
        string strbuf = "";
        if (val == F_REQUESTID)
        {
          // REQUESTID は PKのため, 順序より取得する
          strbuf = " to_char(sysdate, 'DD') || lpad(ss_toUsersInfoRequestid.nextval, 6, '0') AS " + F_REQUESTID;
        }
        else if (val == F_MESSAGEID1)
        {
          // TRANSFERRESULT は 投入条件名(EC01/EC02/EC04)を入れる
          strbuf = ", '{0}' AS " + F_MESSAGEID1;
        }
        else if (val == F_TRANSFERTEXT)
        {
          // TRANSFERTEXT は LONG型のため あえての NULL を入れる
          strbuf = ", NULL AS " + F_TRANSFERTEXT;
        }
        else
        {
          strbuf = ", AAA." + val + " AS " + val;
        }
        sbINSERT.AppendLine(strbuf);
      }
      sbINSERT.AppendLine(" FROM");
      sbINSERT.AppendLine("   " + TABLE_NAME + " AAA");
      sbINSERT.AppendLine(" WHERE");
      sbINSERT.AppendLine("   (AAA." + F_TRANSFERSTATUS + " = '" + SYORI_TAISYO_STATUS + "')");
      sbINSERT.AppendLine("   AND (AAA." + F_MESSAGEID1 + " = '" + SYORI_TAISYO_MSG1 + "')");
      // 追加条件をココに入れる
      sbINSERT.AppendLine("{1}");

      // ([EC02] データ挿入用)------------------------------------------------------------
      sbINSAPC = new StringBuilder();
      sbINSAPC.AppendLine(" INSERT INTO ");
      sbINSAPC.AppendLine("   " + TABLE_NAME + "");
      sbINSAPC.AppendLine(" SELECT");
      foreach (string val in flds)
      {
        string strbuf = "";
        if (val == F_REQUESTID)
        {
          // REQUESTID は PKのため, 順序より取得する
          strbuf = " to_char(sysdate, 'DD') || lpad(ss_toUsersInfoRequestid.nextval, 6, '0') AS " + F_REQUESTID;
        }
        else if (val == F_APPCODE)
        {
          // APPCODE を入れる
          strbuf = ", '{0}' AS " + F_APPCODE;
        }
        else if (val == F_MESSAGEID1)
        {
          // TRANSFERRESULT は 投入条件名(EC01/EC02/EC04)を入れる
          strbuf = ", '{1}' AS " + F_MESSAGEID1;
        }
        else if (val == F_TRANSFERTEXT)
        {
          // TRANSFERTEXT は LONG型のため あえての NULL を入れる
          strbuf = ", NULL AS " + F_TRANSFERTEXT;
        }
        else
        {
          strbuf = ", AAA." + val + " AS " + val;
        }
        sbINSAPC.AppendLine(strbuf);
      }
      sbINSAPC.AppendLine(" FROM");
      sbINSAPC.AppendLine("   " + TABLE_NAME + " AAA");
      sbINSAPC.AppendLine(" WHERE");
      sbINSAPC.AppendLine("   (AAA." + F_TRANSFERSTATUS + " = '" + SYORI_TAISYO_STATUS + "')");
      // EC02 は 指定条件を満たしたユーザ に対して絞り込みを実行する
      sbINSAPC.AppendLine("   AND (AAA." + F_MESSAGEID1 + " = '{2}')");
      // 追加条件をココに入れる
      sbINSAPC.AppendLine("{3}");

      // テーブルのカラム情報を作成する
      Make_Column();
    }

    /// <summary>
    /// 処理対象のレコードを最初に暫定ステータスに更新する
    /// </summary>
    /// <returns></returns>
    public string Get_UPDSQL()
    {
      // 処理対象のレコードを最初に暫定ステータスに更新する
      string strSQL;
      strSQL = "UPDATE " + TABLE_NAME + " SET " + F_MESSAGEID1 + " = '" + SYORI_TAISYO_MSG1 + "'";
      strSQL += " WHERE (" + F_TRANSFERSTATUS + " = '" + SYORI_TAISYO_STATUS + "') AND (" + F_MESSAGEID1 + " IS NULL)";
      return strSQL;
    }

    /// <summary>
    /// 暫定ステータスのレコードを削除する
    /// </summary>
    /// <returns></returns>
    public string Get_DELSQL()
    {
      // 暫定ステータスのレコードを削除する
      string strSQL;
      strSQL = "DELETE " + TABLE_NAME + " WHERE (" + F_MESSAGEID1 + " = '" + SYORI_TAISYO_MSG1 + "')";
      return strSQL;
    }

    /// <summary>
    /// [EC01], [EC03] データ挿入用
    /// </summary>
    /// <returns></returns>
    public string Get_INSSQL()
    {
      // ([EC01], [EC03] データ挿入用)
      return sbINSERT.ToString();
    }

    /// <summary>
    /// [EC02] データ挿入用
    /// </summary>
    /// <returns></returns>
    public string Get_INSSQL2()
    {
      // ([EC02] データ挿入用)
      return sbINSAPC.ToString();
    }

    /// <summary>
    /// 重複レコードを削除する
    /// </summary>
    /// <returns></returns>
    public List<string> Get_CUT_DUP_SQL()
    {
      List<string> lstSQL = new List<string>();
      string strSQL;
      strSQL = " UPDATE " + TABLE_NAME + " AA";
      strSQL += " SET AA." + F_MESSAGEID1 + " = 'RREC_' || AA." + F_MESSAGEID1 + "";
      strSQL += " WHERE(AA." + F_REQUESTID + " in (";
      strSQL += " SELECT MAX(BB." + F_REQUESTID + ")";
      strSQL += " FROM " + TABLE_NAME + " BB";
      strSQL += " WHERE((BB." + F_TRANSFERSTATUS + " = '" + SYORI_TAISYO_STATUS + "'";
      strSQL += " ) AND (BB." + F_TRANSFERDATE + " IS NULL";
      strSQL += " ) AND (BB." + F_TRANSFERRESULT + " IS NULL";
      strSQL += " ) AND (BB." + F_TRANSFERTEXT + " IS NULL))";
      strSQL += " GROUP BY BB." + F_DB + "";
      strSQL += ", BB." + F_APPCODE + "";
      strSQL += ", BB." + F_USERID + "";
      strSQL += ", BB." + F_HOSPITALID + "";
      strSQL += ", BB." + F_PASSWORD + "";
      strSQL += ", BB." + F_USERNAMEKANJI + "";
      strSQL += ", BB." + F_USERNAMEENG + "";
      strSQL += ", BB." + F_SECTION_ID + "";
      strSQL += ", BB." + F_TANTO_SECTION_ID + "";
      strSQL += ", BB." + F_STAFFID + "";
      strSQL += ", BB." + F_SYOKUIN_KBN + "";
      strSQL += ", BB." + F_TEL + "";
      strSQL += ", BB." + F_PASSWORDEXPIRYDATE + "";
      strSQL += ", BB." + F_PASSWORDWARNINGDATE + "";
      strSQL += ", BB." + F_USERIDVALIDITYFLAG + "";
      strSQL += ", BB." + F_REQUESTTYPE + "";
      strSQL += ", BB." + F_MESSAGEID1 + "";
      strSQL += ", BB." + F_MESSAGEID2 + "";
      strSQL += ", BB." + F_MESSAGEID3 + "";
      strSQL += "))";
      lstSQL.Add(strSQL);

      strSQL = " DELETE " + TABLE_NAME + "";
      strSQL += " WHERE((" + F_TRANSFERSTATUS + " = '" + SYORI_TAISYO_STATUS + "'";
      strSQL += " ) AND (" + F_TRANSFERDATE + " IS NULL";
      strSQL += " ) AND (" + F_TRANSFERRESULT + " IS NULL";
      strSQL += " ) AND (" + F_TRANSFERTEXT + " IS NULL";
      strSQL += " ) AND (SUBSTR(" + F_MESSAGEID1 + ", 1, 5) <> 'RREC_'))";
      lstSQL.Add(strSQL);

      strSQL = " UPDATE " + TABLE_NAME + "";
      strSQL += " SET " + F_MESSAGEID1 + " = SUBSTR(" + F_MESSAGEID1 + ", 6)";
      strSQL += " WHERE ( SUBSTR(" + F_MESSAGEID1 + ", 1, 5) = 'RREC_')";
      lstSQL.Add(strSQL);

      return lstSQL;
    }

    /// <summary>
    /// テーブル定義を持った DataTable を作成する
    /// </summary>
    /// <param name="dtTG"></param>
    public void Init_DataTable(out DataTable dtTG)
    {
      // dtTG = new DataTable();
      dtTG = dtToUsrInfo.Clone();
    }

  }
}
