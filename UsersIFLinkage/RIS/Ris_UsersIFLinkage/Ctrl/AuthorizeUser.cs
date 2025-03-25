using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Ris_UsersIFLinkage.Util;



namespace Ris_UsersIFLinkage.Ctrl
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
    private const string NODE_NAME_EC03 = Util.CommonParameter.NODE_NAME_EC03;
		// Y_Higuchi --add --2017.09.28-->>
		private const string NODE_NAME_EC05 = Util.CommonParameter.NODE_NAME_EC05;
		// Y_Higuchi --add --2017.09.28--<<

    // ユーザ登録条件ファイル(XML)
    private Util.XmlUtil xmlRIS;

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
    /// USERMANAGE/USERINFO_CA 用 登録条件
    /// </summary>
    private NODE_EC ec01;

    /// <summary>
    /// USERAPPMANAGE/ATTRMANAGE 用 登録条件
    /// </summary>
    private List<NODE_EC> ec02;

    /// <summary>
    /// RRIS.SECTIONDOCTORMASTER 用 登録条件
    /// </summary>
    private NODE_EC ec03;

	 // Y_Higuchi --add --2017.09.28-->>
    /// <summary>
    /// USERMANAGE-UPD 用 登録条件
    /// </summary>
    private NODE_EC ec05;
	 // Y_Higuchi --add --2017.09.28--<<

    public AuthorizeUser()
    {
      Init_Xmlfile();
      Read_Xmlfile();
    }

    public int Copy_UsersInfo(OracleDataBase db)
    {
      // DB接続
      db.Open();
      try
      {
        // ToUserInfo_SVR : 必要なSQL, 構造 の定義
        Data.Export.Entity.ToUserInfoEntity_act tuia = new Data.Export.Entity.ToUserInfoEntity_act();

        // お掃除
        db.ExecuteQuery(tuia.Get_DELSQL());

        // コミット
        db.Commit();

        // 更新
        int intcnt = db.ExecuteQuery(tuia.Get_UPDSQL());
        if (intcnt <= 0){ return intcnt; }

        // EC01用挿入文
        string strSQL = Make_EC01(tuia, ec01, NODE_NAME_EC01);
        db.ExecuteQuery(strSQL);

        // EC03用挿入文
        strSQL = Make_EC01(tuia, ec03, NODE_NAME_EC03);
        db.ExecuteQuery(strSQL);

			// Y_Higuchi --add --2017.09.28-->>
			// EC05用挿入文
			strSQL = Make_EC01(tuia, ec05, NODE_NAME_EC05);
			db.ExecuteQuery(strSQL);
			// Y_Higuchi --add --2017.09.28--<<

        // EC02用挿入文 (EC01より後に実行)
        List<string> lstSQL = new List<string>();
        lstSQL = Make_EC02(tuia, NODE_NAME_EC01);

        // まとめて実行
        foreach (string sql in lstSQL)
        {
          db.ExecuteQuery(sql);
        }

        // お掃除
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
      // XMLファイル名の指定まで、やっておく
      xmlRIS = new Util.XmlUtil();
      xmlRIS.strFilename = Path.Combine(Application.StartupPath, AppConfigController.GetInstance().GetValueString(AppConfigParameter.AUTHUSER_RIS));
    }

    /// <summary>
    /// ユーザ登録条件ファイル(XML)の読込
    /// </summary>
    private void Read_Xmlfile()
    {
      bool blnret = Init_EC01(ref ec01, NODE_NAME_EC01);
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC01 読込エラー");
      }
      blnret = Init_EC02();
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC02 読込エラー");
      }
      blnret = Init_EC01(ref ec03, NODE_NAME_EC03);
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC03 読込エラー");
      }
		// Y_Higuchi --add --2017.09.28-->>
      blnret = Init_EC01(ref ec05, NODE_NAME_EC05);
      if (!blnret)
      {
        throw new Exception("AuthorizeUser EC05 読込エラー");
      }
		// Y_Higuchi --add --2017.09.28--<<
      InfoLog();
    }

    /// <summary>
    /// ノード[nodenm]情報をecvへ読込&初期化
    /// </summary>
    /// <returns>OK=true,ERR=false</returns>
    private bool Init_EC01(ref NODE_EC ecv, string nodenm)
    {
      try
      {
        ecv.NODE_NAME = nodenm;
        Hashtable htBuf = new Hashtable();
        bool blnret = xmlRIS.xmlRead(nodenm, htBuf);
        if (!blnret)
        {
          return blnret;
        }

        ecv.SYOKUIN_KBN = htBuf["SYOKUIN_KBN"].ToString();
        ecv.SyokuKbn = new List<string>();
        if (ecv.SYOKUIN_KBN != "")
        {
          foreach (string strval in ecv.SYOKUIN_KBN.Split(','))
          {
            ecv.SyokuKbn.Add(strval.Trim());
          }
        }

        ecv.SECTION_ID = htBuf["SECTION_ID"].ToString();
        ecv.SectId = new List<string>();
        if (ecv.SECTION_ID != "")
        {
          foreach (string strval in ecv.SECTION_ID.Split(','))
          {
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

    /// <summary>
    /// ノード[EC02]情報の読込&初期化
    /// </summary>
    /// <returns>OK=true,ERR=false</returns>
    private bool Init_EC02()
    {
      try
      {
        ec02 = new List<NODE_EC>();

        NODE_EC ecbuf = new NODE_EC();
        ecbuf.NODE_NAME = NODE_NAME_EC02;

        Hashtable htBuf = new Hashtable();
        bool blnret = xmlRIS.xmlRead(ecbuf.NODE_NAME, htBuf);
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
          ec02.Add(ecbuf);
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
          blnret = xmlRIS.xmlRead("EC02_" + ecfbuf.NODE_NAME, htfbuf);
          if (!blnret)
          {
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

          ec02.Add(ecfbuf);
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
        strinfo += "(" + flc.ToString() + ")職区=";
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

      // EC03
      strinfo = "職区=";
      foreach (string val in ec03.SyokuKbn)
      {
        strinfo += val + ",";
      }
      strinfo += "診ID=";
      foreach (string val in ec03.SectId)
      {
        strinfo += val + ",";
      }
      // Debug情報出力
      _log.Debug(ec03.NODE_NAME + strinfo);

		// Y_Higuchi --add --2017.09.28-->>
      // EC05
      strinfo = "職区=";
      foreach (string val in ec05.SyokuKbn)
      {
        strinfo += val + ",";
      }
      strinfo += "診ID=";
      foreach (string val in ec05.SectId)
      {
        strinfo += val + ",";
      }
      // Debug情報出力
      _log.Debug(ec05.NODE_NAME + strinfo);
		// Y_Higuchi --add --2017.09.28--<<

    }

    /// <summary>
    /// 職員区分, 診療ID の追加条件部分を SQL で返す
    /// </summary>
    /// <param name="ecv"></param>
    /// <returns></returns>
    private string Get_SyoKbnSectId(NODE_EC ecv)
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
        strSyoKbn = " AND (AAA.SYOKUIN_KBN in (" + strSyoKbn + "))";
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
        strSectId = " AND (AAA.SECTION_ID in (" + strSectId + "))";
      }

      // 追加条件をまとめる
      string strWhere = strSyoKbn + strSectId;

      return strWhere;
    }

    /// <summary>
    /// [EC01], [EC03] 用SQL文を返す
    /// </summary>
    /// <param name="tuia"></param>
    /// <param name="ecv"></param>
    /// <param name="ecname"></param>
    /// <returns></returns>
    private string Make_EC01(Data.Export.Entity.ToUserInfoEntity_act tuia, NODE_EC ecv, string ecname)
    {
      // 追加条件をまとめる
      string strWhere = Get_SyoKbnSectId(ecv);

      // 挿入文
      string strINSSQL = tuia.Get_INSSQL();

      // 条件展開
      string strSQL = string.Format(strINSSQL, ecname, strWhere);

      return strSQL;
    }

    /// <summary>
    /// [EC02]用SQL文を返す
    /// </summary>
    /// <param name="tuia"></param>
    /// <param name="baseusr"></param>
    /// <returns></returns>
    private List<string> Make_EC02(Data.Export.Entity.ToUserInfoEntity_act tuia, string baseusr)
    {
      List<string> lstSQL = new List<string>();

      // EC02 - APPCODE
      for (int flc = 0; flc < ec02[0].SyokuKbn.Count; flc++)
      {
        string strAppCode = ec02[0].SyokuKbn[flc];

        // 追加条件をまとめる
        string strWhere = Get_SyoKbnSectId(ec02[flc + 1]);

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
