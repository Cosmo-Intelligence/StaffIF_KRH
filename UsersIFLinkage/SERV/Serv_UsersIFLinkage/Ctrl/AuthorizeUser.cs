using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Serv_UsersIFLinkage.Util;



namespace Serv_UsersIFLinkage.Ctrl
{
  class AuthorizeUser
  {
    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
        MethodBase.GetCurrentMethod().DeclaringType);

    // ノード名称(固定部分)
    private const string NODE_NAME_EC01 = Util.CommonParameter.NODE_NAME_EC01;
    private const string NODE_NAME_EC02 = Util.CommonParameter.NODE_NAME_EC02;
    private const string NODE_NAME_EC04 = Util.CommonParameter.NODE_NAME_EC04;

    // ユーザ登録条件ファイル(XML)
    private Util.XmlUtil xmlServ;
    private Util.XmlUtil xmlRIS;
    private Util.XmlUtil xmlTheraRIS;
    private Util.XmlUtil xmlReport;

    // 登録条件用 構造体
    private struct NODE_EC
    {
      public string NODE_NAME;
      public string SYOKUIN_KBN;
      public string SECTION_ID;
      public List<string> SyokuKbn;
      public List<string> SectId;
    }

    /// <summary>
    /// Serv.ARQS 用 登録条件
    /// </summary>
    private NODE_EC ec01;

    /// <summary>
    /// Serv.YOKOGAWA.USERAPPMANAGE/ATTRMANAGE 用 登録条件
    /// </summary>
    private List<NODE_EC> ec02;
    private List<NODE_EC> ec02Ris;
    private List<NODE_EC> ec02TheraRis;
    private List<NODE_EC> ec02Report;

    /// <summary>
    /// Serv.YOKOGAWA.USERMANAGE 用 登録条件
    /// </summary>
    private NODE_EC ec04;
    private List<NODE_EC> ec04Child;

    public AuthorizeUser()
    {
      Init_Xmlfile();
      Read_Xmlfile();
    }

    public int Copy_UsersInfo(OracleDataBase db)
    {
      try
      {
        // DB接続
        db.Open();

        // ToUserInfo_SRV : 必要な構造&SQLの定義
        Data.Export.Entity.ToUserInfoEntity_act tuia = new Data.Export.Entity.ToUserInfoEntity_act();

        // お掃除
        db.ExecuteQuery(tuia.Get_DELSQL());

        // コミット
        db.Commit();

        // 更新
        int intcnt = db.ExecuteQuery(tuia.Get_UPDSQL());
        if (intcnt <= 0) { return intcnt; }

        // EC01用挿入文
        string strSQL = Make_EC01(tuia, ec01, NODE_NAME_EC01);
        db.ExecuteQuery(strSQL);

        // EC04用挿入文
        strSQL = Make_EC01(tuia, ec04, NODE_NAME_EC04);
        db.ExecuteQuery(strSQL);

        // 各APゲートウェイの条件で EC04挿入文
        foreach (NODE_EC ecv in ec04Child)
        {
          strSQL = Make_EC01(tuia, ecv, ecv.NODE_NAME);
          db.ExecuteQuery(strSQL);
        }

        // EC02用挿入文 (EC04より後に実行)
        List<string> lstSQL = new List<string>();
        lstSQL = Make_EC02(ec02, tuia, NODE_NAME_EC04);
        foreach (string sql in lstSQL)
        {
          db.ExecuteQuery(sql);
        }

        // 各APゲートウェイの条件で EC02挿入文
        if (ec02Ris.Count > 0)
        {
          lstSQL.Clear();
          lstSQL = Make_EC02(ec02Ris, tuia, NODE_NAME_EC04 + "-RIS");
          foreach (string sql in lstSQL)
          {
            db.ExecuteQuery(sql);
          }
          // EC04-RIS を EC04 に変更
          strSQL = tuia.Get_CONDUPDSQL();
          strSQL = String.Format(strSQL, NODE_NAME_EC04, NODE_NAME_EC04 + "-RIS");
          db.ExecuteQuery(strSQL);
        }
        if (ec02TheraRis.Count > 0)
        {
          lstSQL.Clear();
          lstSQL = Make_EC02(ec02TheraRis, tuia, NODE_NAME_EC04 + "-TheraRIS");
          foreach (string sql in lstSQL)
          {
            db.ExecuteQuery(sql);
          }
          // EC04-TheraRIS を EC04 に変更
          strSQL = tuia.Get_CONDUPDSQL();
          strSQL = String.Format(strSQL, NODE_NAME_EC04, NODE_NAME_EC04 + "-TheraRIS");
          db.ExecuteQuery(strSQL);
        }
        if (ec02Report.Count > 0)
        {
          lstSQL.Clear();
          lstSQL = Make_EC02(ec02Report, tuia, NODE_NAME_EC04 + "-Report");
          foreach (string sql in lstSQL)
          {
            db.ExecuteQuery(sql);
          }
          // EC04-Report を EC04 に変更
          strSQL = tuia.Get_CONDUPDSQL();
          strSQL = String.Format(strSQL, NODE_NAME_EC04, NODE_NAME_EC04 + "-Report");
          db.ExecuteQuery(strSQL);
        }

        // 削除
        db.ExecuteQuery(tuia.Get_DELSQL());

        // 重複レコードを削除する
        lstSQL.Clear();
        lstSQL = tuia.Get_CUT_DUP_SQL();
        foreach (string sql in lstSQL)
        {
          db.ExecuteQuery(sql);
        }

        // コミット
        db.Commit();
      }
      catch (Exception ex)
      {
        _log.Error(ex.Message);
        // ロールバック
        db.RollBack();
        return -1;
      }
      finally
      {
        // DB切断
        db.Close();
      }

      return 1;
    }

    /// <summary>
    /// ユーザ登録条件ファイル(XML)取得の設定初期化
    /// </summary>
    private void Init_Xmlfile()
    {
      // パス設定
      string appfile = Application.StartupPath;

      // XMLファイル名の指定まで、やっておく
      xmlServ = new Util.XmlUtil();
      string strFileNm = AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_Serv);
      xmlServ.strFilename = Path.Combine(appfile, strFileNm);

      // RIS は指定されているか？
      strFileNm = AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_RIS);
      if (strFileNm != "")
      {
        xmlRIS = new Util.XmlUtil();
        xmlRIS.strFilename = Path.Combine(appfile, strFileNm);
      }
      else
      {
        xmlRIS = null;
      }

      // TheraRIS は指定されているか？
      strFileNm = AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_TheraRIS);
      if (strFileNm != "")
      {
        xmlTheraRIS = new Util.XmlUtil();
        xmlTheraRIS.strFilename = Path.Combine(appfile, strFileNm);
      }
      else
      {
        xmlTheraRIS = null;
      }

      // Report は指定されているか？
      strFileNm = AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_Report);
      if (strFileNm != "")
      {
        xmlReport = new Util.XmlUtil();
        xmlReport.strFilename = Path.Combine(appfile, strFileNm);
      }
      else
      {
        xmlReport = null;
      }

    }

    /// <summary>
    /// ユーザ登録条件ファイル(XML)の読込
    /// </summary>
    private void Read_Xmlfile()
    {
      bool blnret = Pllus_EC01(xmlServ, ref ec01, NODE_NAME_EC01);
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC01 読込エラー");
      }
      blnret = Init_EC02();
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC02 読込エラー");
      }
      blnret = Init_EC04();
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC04 読込エラー");
      }
      InfoLog();
    }

    /// <summary>
    /// EC02 の定義を読込む
    /// </summary>
    /// <param name="lstec">読込んだ定義を設定する NODE_EC リスト</param>
    /// <param name="xmlR">ユーザ登録条件ファイル(XML)定義</param>
    /// <param name="strNODE">読込 NODE 名</param>
    /// <returns></returns>
    private bool Init_EC02_a(ref List<NODE_EC> lstec, XmlUtil xmlR, string strNODE)
    {
      // 貯める用の構造体バッファ
      NODE_EC ecbuf = new NODE_EC();
      ecbuf.NODE_NAME = strNODE;

      Hashtable htBuf = new Hashtable();
      bool blnret = xmlR.xmlRead(ecbuf.NODE_NAME, htBuf);
      if (!blnret)
      {
        return blnret;
      }

      // 配列[0] : APPCODE 定義
      ecbuf.SYOKUIN_KBN = htBuf["APPCODE"].ToString();
      ecbuf.SyokuKbn = new List<string>();
      if (ecbuf.SYOKUIN_KBN != "")
      {
        foreach (string strval in ecbuf.SYOKUIN_KBN.Split(','))
        {
          ecbuf.SyokuKbn.Add(strval.Trim());
        }
        lstec.Add(ecbuf);
      }
      else
      {
        // APPCODE 定義が無いとエラー
        return false;
      }

      // 配列[1]～ : 各APPCODE の条件
      foreach (string apnm in ecbuf.SyokuKbn)
      {
        // 各APPCODE 条件を持つ NODE_EC構造体変数
        NODE_EC ecfbuf = new NODE_EC();
        ecfbuf.NODE_NAME = apnm; // NODE_NAME = APPCODE を設定

        // 条件NODEを参照
        Hashtable htfbuf = new Hashtable();
        blnret = xmlR.xmlRead("EC02_" + ecfbuf.NODE_NAME, htfbuf);
        if (!blnret)
        {
          _log.ErrorFormat("APPCODE = {0} の 条件が定義されていません。", apnm);
          return blnret;
        }

        // 職員区分
        ecfbuf.SYOKUIN_KBN = htfbuf["SYOKUIN_KBN"].ToString();
        ecfbuf.SyokuKbn = new List<string>();
        if (ecfbuf.SYOKUIN_KBN != "")
        {
          foreach (string strval in ecfbuf.SYOKUIN_KBN.Split(','))
          {
            ecfbuf.SyokuKbn.Add(strval.Trim());
          }
        }

        // 診療科ID
        ecfbuf.SECTION_ID = htfbuf["SECTION_ID"].ToString();
        ecfbuf.SectId = new List<string>();
        if (ecfbuf.SECTION_ID != "")
        {
          foreach (string strval in ecfbuf.SECTION_ID.Split(','))
          {
            ecfbuf.SectId.Add(strval.Trim());
          }
        }

        lstec.Add(ecfbuf);
      }

      return true;
    }

    /// <summary>
    /// ノード[EC02]情報の読込&初期化
    /// </summary>
    /// <returns>OK=true,ERR=false</returns>
    private bool Init_EC02()
    {
      try
      {
        // 各リストを初期化
        ec02 = new List<NODE_EC>();
        ec02Ris = new List<NODE_EC>();
        ec02TheraRis = new List<NODE_EC>();
        ec02Report = new List<NODE_EC>();

        // EC02 の読込
        bool blnret = Init_EC02_a(ref ec02, xmlServ, NODE_NAME_EC02);
        if (!blnret)
        {
          return blnret;
        }

        // ----- RIS -----
        if (xmlRIS != null)
        {
          blnret = Init_EC02_a(ref ec02Ris, xmlRIS, NODE_NAME_EC02);
          if (!blnret)
          {
            return blnret;
          }
        }

        // ----- TheraRIS -----
        if (xmlTheraRIS != null)
        {
          blnret = Init_EC02_a(ref ec02TheraRis, xmlTheraRIS, NODE_NAME_EC02);
          if (!blnret)
          {
            return blnret;
          }
        }

        // ----- Report -----
        if (xmlReport != null)
        {
          blnret = Init_EC02_a(ref ec02Report, xmlReport, NODE_NAME_EC02);
          if (!blnret)
          {
            return blnret;
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    /// <summary>
    /// ノード[EC04]情報の読込&初期化
    /// </summary>
    /// <returns>OK=true,ERR=false</returns>
    private bool Init_EC04()
    {
      try
      {
        // ----- YOKOGAWA ----- 条件読込
        bool blnret = Pllus_EC01(xmlServ, ref ec04, NODE_NAME_EC04);
        if (!blnret)
        {
          return blnret;
        }

        // ec04 : 子条件のリスト
        ec04Child = new List<NODE_EC>();

        // ----- ARQS ----- 条件追加
        NODE_EC ecBuf01 = new NODE_EC();
        ecBuf01.NODE_NAME = NODE_NAME_EC04;
        blnret = Pllus_EC01(xmlServ, ref ecBuf01, NODE_NAME_EC01);
        if (!blnret)
        {
          return blnret;
        }
        ec04Child.Add(ecBuf01);

        // ----- RIS ----- 条件追加
        if (xmlRIS != null)
        {
          NODE_EC ecBuf = new NODE_EC();
          ecBuf.NODE_NAME = NODE_NAME_EC04 + "-RIS";
          blnret = Pllus_EC01(xmlRIS, ref ecBuf, NODE_NAME_EC01);
          if (!blnret)
          {
            return blnret;
          }
          ec04Child.Add(ecBuf);
        }

        // ----- TheraRIS ----- 条件追加
        if (xmlTheraRIS != null)
        {
          NODE_EC ecBuf = new NODE_EC();
          ecBuf.NODE_NAME = NODE_NAME_EC04 + "-TheraRIS";
          blnret = Pllus_EC01(xmlTheraRIS, ref ecBuf, NODE_NAME_EC01);
          if (!blnret)
          {
            return blnret;
          }
          ec04Child.Add(ecBuf);
        }

        // ----- Report ----- 条件追加
        if (xmlReport != null)
        {
          NODE_EC ecBuf = new NODE_EC();
          ecBuf.NODE_NAME = NODE_NAME_EC04 + "-Report";
          blnret = Pllus_EC01(xmlReport, ref ecBuf, NODE_NAME_EC01);
          if (!blnret)
          {
            return blnret;
          }
          ec04Child.Add(ecBuf);
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private bool Pllus_EC01(Util.XmlUtil xmlR, ref NODE_EC ecv, string strnode)
    {
      try
      {
        // 指定されたXMLから指定NODEを読み出す
        Hashtable htbuf = new Hashtable();
        bool blnret = xmlR.xmlRead(strnode, htbuf);
        if (!blnret)
        {
          return blnret;
        }

        // 職員区分 条件
        string strbuf = htbuf["SYOKUIN_KBN"].ToString();
        ecv.SYOKUIN_KBN = strbuf;
        ecv.SyokuKbn = new List<string>();
        if (strbuf != "") 
        {
          foreach (string strval in strbuf.Split(','))
          {
            // strnodeに指定されている職員区分の条件をecvへ追加する
            ecv.SyokuKbn.Add(strval.Trim());
          }
        }

        // 診療科ID 条件
        strbuf = htbuf["SECTION_ID"].ToString();
        ecv.SECTION_ID = strbuf;
        ecv.SectId = new List<string>();
        if (strbuf != "")
        {
          foreach (string strval in strbuf.Split(','))
          {
            // strnodeに指定されている診療科IDの条件をecvへ追加する
            ecv.SectId.Add(strval.Trim());
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private void InfoLog()
    {
      // EC01
      string strinfo = "職区=";
      foreach (string val in ec01.SyokuKbn)
      {
        strinfo += val + ",";
      }
      strinfo += "診ID=";
      foreach (string val in ec01.SectId)
      {
        strinfo += val + ",";
      }
      // Debug情報出力
      _log.Debug(ec01.NODE_NAME + strinfo);

      // EC02
      strinfo = "";
      foreach (string val in ec02[0].SyokuKbn)
      {
        strinfo += val + ",";
      }
      for (int flc = 1; flc < ec02.Count; flc++)
      {
        strinfo += "職区=";
        foreach (string val in ec02[flc].SyokuKbn)
        {
          strinfo += val + ",";
        }
        strinfo += "診ID=";
        foreach (string val in ec02[flc].SectId)
        {
          strinfo += val + ",";
        }
      }
      // Debug情報出力
      _log.Debug("APPCODE:" + strinfo);

      // EC02 - RIS
      strinfo = "";
      if (ec02Ris.Count > 0)
      {
        foreach (string val in ec02Ris[0].SyokuKbn)
        {
          strinfo += val + ",";
        }
        for (int flc = 1; flc < ec02Ris.Count; flc++)
        {
          strinfo += "RIS-職区=";
          foreach (string val in ec02Ris[flc].SyokuKbn)
          {
            strinfo += val + ",";
          }
          strinfo += "診ID=";
          foreach (string val in ec02Ris[flc].SectId)
          {
            strinfo += val + ",";
          }
        }
        // Debug情報出力
        _log.Debug("APPCODE:" + strinfo);
      }

      // EC02 - TheraRIS
      strinfo = "";
      if (ec02TheraRis.Count > 0)
      {
        foreach (string val in ec02TheraRis[0].SyokuKbn)
        {
          strinfo += val + ",";
        }
        for (int flc = 1; flc < ec02TheraRis.Count; flc++)
        {
          strinfo += "TheraRIS-職区=";
          foreach (string val in ec02TheraRis[flc].SyokuKbn)
          {
            strinfo += val + ",";
          }
          strinfo += "診ID=";
          foreach (string val in ec02TheraRis[flc].SectId)
          {
            strinfo += val + ",";
          }
        }
        // Debug情報出力
        _log.Debug("APPCODE:" + strinfo);
      }

      // EC02 - Report
      strinfo = "";
      if (ec02Report.Count > 0)
      {
        foreach (string val in ec02Report[0].SyokuKbn)
        {
          strinfo += val + ",";
        }
        for (int flc = 1; flc < ec02Report.Count; flc++)
        {
          strinfo += "Report-職区=";
          foreach (string val in ec02Report[flc].SyokuKbn)
          {
            strinfo += val + ",";
          }
          strinfo += "診ID=";
          foreach (string val in ec02Report[flc].SectId)
          {
            strinfo += val + ",";
          }
        }
        // Debug情報出力
        _log.Debug("APPCODE:" + strinfo);
      }

      // EC04
      strinfo = "職区=";
      foreach (string val in ec04.SyokuKbn)
      {
        strinfo += val + ",";
      }
      strinfo += "診ID=";
      foreach (string val in ec04.SectId)
      {
        strinfo += val + ",";
      }
      // Debug情報出力
      _log.Debug(ec04.NODE_NAME + strinfo);

      int intChild = 1;
      foreach (NODE_EC ecc in ec04Child)
      {
        strinfo = "(" + intChild.ToString() + "-子)職区=";
        foreach (string val in ecc.SyokuKbn)
        {
          strinfo += val + ",";
        }
        strinfo += "診ID=";
        foreach (string val in ecc.SectId)
        {
          strinfo += val + ",";
        }
        // Debug情報出力
        _log.Debug(ec04.NODE_NAME + strinfo);
        intChild++;
      }

    }

    private string Make_SyokuKbn_SectID(NODE_EC ecv)
    {
      // 職員区分
      string strSyoKbn = "";
      foreach (string val in ecv.SyokuKbn)
      {
        strSyoKbn += "'" + val + "',";
      }
      if (strSyoKbn != "")
      {
        strSyoKbn = strSyoKbn.Substring(0, strSyoKbn.Length - 1);
        strSyoKbn = " (AAA.SYOKUIN_KBN in (" + strSyoKbn + "))";
      }

      // 診療ID
      string strSectId = "";
      foreach (string val in ecv.SectId)
      {
        strSectId += "'" + val + "',";
      }
      if (strSectId != "")
      {
        strSectId = strSectId.Substring(0, strSectId.Length - 1);
        strSectId = " (AAA.SECTION_ID in (" + strSectId + "))";
      }

      // 条件をまとめる
      string strWhere = "";
      if ((strSyoKbn != "") && (strSectId != ""))
      {
        strWhere = "(" + strSyoKbn + " AND " + strSectId + ")";
      }
      else if ((strSyoKbn != ""))
      {
        strWhere = strSyoKbn;
      }
      else if ((strSectId != ""))
      {
        strWhere = strSectId;
      }

      // 出来た条件文(WHERE)を返す
      return strWhere;
    }

    private string Make_EC01(Data.Export.Entity.ToUserInfoEntity_act tuia, NODE_EC ecv, string ecname)
    {
      // 追加条件をまとめる
      string strWhere = Make_SyokuKbn_SectID(ecv);
      if (strWhere != "")
      {
        strWhere = " AND " + strWhere;
      }

      // 挿入文
      string strINSSQL = tuia.Get_INSSQL();

      // 条件展開
      string strSQL = string.Format(strINSSQL, ecname, strWhere);

      return strSQL;
    }

    /// <summary>
    /// EC02 挿入用SQL文を生成する
    /// </summary>
    /// <param name="tuia"></param>
    /// <param name="baseusr"></param>
    /// <returns></returns>
    private List<string> Make_EC02(List<NODE_EC> lstec, Data.Export.Entity.ToUserInfoEntity_act tuia, string baseusr)
    {
      List<string> lstSQL = new List<string>();

      // EC02 - APPCODE
      for (int flc = 0; flc < lstec[0].SyokuKbn.Count; flc++)
      {
        string strAppCode = lstec[0].SyokuKbn[flc];

        // EC02 - 職員区分
        string strSyoKbn = "";
        foreach (string val in lstec[flc + 1].SyokuKbn)
        {
          strSyoKbn += "'" + val + "',";
        }
        if (strSyoKbn != "")
        {
          strSyoKbn = strSyoKbn.Substring(0, strSyoKbn.Length - 1);
          strSyoKbn = " AND (AAA.SYOKUIN_KBN in (" + strSyoKbn + "))";
        }

        // EC02 - 診療ID
        string strSectId = "";
        foreach (string val in lstec[flc + 1].SectId)
        {
          strSectId += "'" + val + "',";
        }
        if (strSectId != "")
        {
          strSectId = strSectId.Substring(0, strSectId.Length - 1);
          strSectId = " AND (AAA.SECTION_ID in (" + strSectId + "))";
        }

        // 追加条件をまとめる
        string strWhere = strSyoKbn + strSectId;

        // EC02 - 挿入文
        string strINSSQL = tuia.Get_INSSQL2();

        // 条件展開	(APPCODE, TRANSFERRESULT, WhereのTRANSFERRESULT, 追加条件)
        string strSQL = string.Format(strINSSQL, strAppCode, NODE_NAME_EC02, baseusr, strWhere);

        // リストへ追加
        lstSQL.Add(strSQL);
      }
      return lstSQL;
    }

  }
}
