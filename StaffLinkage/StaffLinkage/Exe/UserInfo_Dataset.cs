#define pwdcnv_dll
//#undef pwdcnv_dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;
#if pwdcnv_dll
using ExBSecurity;
#endif
using KanjiConversion;

namespace StaffLinkage.Exe
{
  /// <summary>
  /// ユーザ情報データセット
  /// </summary>
  class UserInfo_Dataset
  {
    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
        MethodBase.GetCurrentMethod().DeclaringType);

#if pwdcnv_dll
    /// <summary>
    /// 暗号化/復号化ツール
    /// </summary>
    private OutProc outproc = new OutProc();
#endif

    /// <summary>
    /// 文字エンコード
    /// </summary>
    private Encoding Enc = Util.CommonParameter.CommonEnocode;

    /// <summary>
    /// データが存在しなかった場合に返す値
    /// </summary>
    private string strNoData = Util.CommonParameter.tgmNO_DATA;

    /// <summary>
    /// 準備完了フラグ OK=true, ERR=false
    /// </summary>
    private bool blnReady;

    /// <summary>
    /// 更新日付始端日
    /// </summary>
    private int updymd_from;

    /// <summary>
    /// 更新日付終端日
    /// </summary>
    private int updymd_to;

    /// <summary>
    /// 項目名
    /// </summary>
    private List<string> lstName;

    /// <summary>
    /// ToUsersInfoテーブルのフィールド名
    /// </summary>
    private List<string> lstField;

    /// <summary>
    /// フィールド名に対応する項目名
    /// </summary>
    private List<List<string>> lstFName;

    /// <summary>
    /// データ開始位置[Byte]
    /// </summary>
    private List<int> intPos;

    /// <summary>
    /// データ長[Byte]
    /// </summary>
    private List<int> intLen;

    /// <summary>
    /// 出力対象フラグ(出力=0,更新日により出力対象外=-1,エラーにより出力対象外=-2)
    /// </summary>
    private List<int> lstOutflg;

    /// <summary>
    /// 読込んだユーザ情報リスト(電文オリジナル情報)
    /// </summary>
    private List<List<string>> arrlstOrg;

    /// <summary>
    /// 読込んだユーザ情報リスト(Trim(), 各種特殊仕様による変換後の情報)
    /// </summary>
    private List<List<string>> arrlstNew;

    /// <summary>
    /// 読込んだユーザ情報数
    /// </summary>
    public int RowCount
    {
      get; private set;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UserInfo_Dataset()
    {
      blnReady = false;
      lstName = new List<string>();
      lstField = new List<string>();
      lstFName = new List<List<string>>();
      intPos = new List<int>();
      intLen = new List<int>();
      lstOutflg = new List<int>();
      arrlstOrg = new List<List<string>>();
      arrlstNew = new List<List<string>>();
      RowCount = 0;
      updymd_from = 0;
      updymd_to = 0;
    }

    /// <summary>
    /// 利用者情報の構成を設定する
    /// </summary>
    /// <param name="hDenb">TelegramMap.xml から読込んだ構成を表す Hashtable </param>
    /// <returns>OK=true, ERR=false</returns>
    public bool Set_Structure(Hashtable hDenb, DateTime lastday_from, DateTime lastday_to, Hashtable hTui)
    {
      Reset_Structure();  // 利用者情報定義リセット
      try
      {
        // 更新日付：対象期間
        if(lastday_from <= lastday_to)
        {
          updymd_from = int.Parse(lastday_from.ToString(Util.CommonParameter.YYYYMMDD));
          updymd_to = int.Parse(lastday_to.ToString(Util.CommonParameter.YYYYMMDD));
        }
        else
        {
          _log.ErrorFormat("差分取得用制御ファイルの定義が不正です。{0}");
          return false;
        }

        bool blnRet = true; // 処理結果
        foreach(DictionaryEntry de in hDenb)
        {
          // Name(=Key) を List へ入れる
          lstName.Add(de.Key.ToString());

          try
          {
            // 位置 | 長さ [Byte]
            string[] strPosLen = de.Value.ToString().Split('|');
            int iPos = int.Parse(strPosLen[0]) - 1;
            int iLen = int.Parse(strPosLen[1]);
            if((iPos >= 0) && (iLen >= 1))
            {
              intPos.Add(iPos);
              intLen.Add(iLen);
            }
            else
            {
              _log.ErrorFormat("電文マッピングファイル - {0} の定義が不正です。", de.Key.ToString());
              blnRet = false;
              break;
            }
          }
          catch(Exception ex)
          {
            _log.ErrorFormat("電文マッピングファイル - {0} の定義が不正です。{1}", de.Key.ToString(), ex.Message);
            blnRet = false;
            break;
          }
        }

        if(blnRet)
        {
          // フィールドのマッピング情報を設定する
          blnRet = Set_ToUI_Field(hTui);
        }

        // 処理結果に従って、準備完了フラグを設定する
        blnReady = blnRet;
      }
      catch(Exception ex2)
      {
        _log.ErrorFormat("電文マッピングファイル 定義読込中にエラーが発生しました。{0}", ex2.Message);
        blnReady = false;
      }

      if(!blnReady)
      {
        Reset_Structure();   // 利用者情報定義リセット
      }
      // 準備完了フラグ値を返す
      return blnReady;
    }

    /// <summary>
    /// １行読込
    /// </summary>
    /// <param name="strLine">電文ファイルの１行分の文字列</param>
    /// <returns></returns>
    public bool Read_Line(string strLine)
    {
      if(!blnReady)
      {
        // 準備完了フラグがOFF
        _log.Error("電文マッピングファイル の定義が読込まれていません。");
        return false;
      }

      try
      {
        List<string> lstVal = new List<string>();
        List<string> lstNew = new List<string>();
        for(int intArrno = 0; intArrno < lstName.Count; intArrno++)
        {
          // データを取り出す
          string strVal = Enc.GetString(Enc.GetBytes(strLine), intPos[intArrno], intLen[intArrno]);
          // Value を List<string> へ入れる
          lstVal.Add(strVal);
          lstNew.Add(strVal.Trim());
        }

        // データ行単位で List<string> の List へ投入する
        arrlstOrg.Add(lstVal);
        arrlstNew.Add(lstNew);
        // 出力対象フラグ = 出力
        lstOutflg.Add(0);
        // 行カウンタをインクリメント
        RowCount++;
        return true;
      }
      catch(Exception ex)
      {
        // エラーを返す
        _log.ErrorFormat("電文ファイルの読込中にエラーが発生したため、この行は読み込まれませんでした。{0} ({1})", ex.Message, strLine);
        return false;
      }
    }

    /// <summary>
    /// 特別仕様
    /// </summary>
    /// <returns>OK=0, ERR=負値</returns>
    public int Special_Specifications(string Kjconv, string Pwconv)
    {
      int intRet = 0;

      // 更新日付による出力対象判定
      intRet = ID_UPD_YMD();
      if(intRet < 0)
      {
        return intRet;
      }

      // 設定ファイルによる職種コード変換
      intRet = ID_SYOKUKBN();
      if(intRet < 0)
      {
        return intRet;
      }

      if(Pwconv == "1")
      {
        // パスワード仕様
        intRet = ID_PASSWORD();
        if(intRet < 0)
        {
          return intRet;
        }
      }

      if(Kjconv == "1")
      {
        // 利用者漢字氏名の旧字変換
        intRet = ID_NAMEN();
        if(intRet < 0)
        {
          return intRet;
        }
      }

      return intRet;
    }

    /// <summary>
    /// フィールドにセットする値リストを取得する
    /// </summary>
    /// <param name="rowno">データ数目</param>
    /// <param name="fldname">フィールド名</param>
    /// <returns>フィールドにセットする値リスト</returns>
    public List<string> Get_DataL(int rowno, string fldname)
    {
      List<string> lstRet = new List<string>();
      List<string> lstSort = new List<string>();
      try
      {
        // 該当するフィールド名の項目リストを取得する
        int arrno = lstField.IndexOf(fldname);
        lstSort = lstFName[arrno];

        // フィールド名順に並べる
        lstSort.Sort();

        // 定義された項目名を取得する
        foreach(string strname in lstSort)
        {
          // 該当する項目値を取得する
          string strRet = Get_Data(rowno, strname);
          // 返す用のリストへ追加
          lstRet.Add(strRet);
        }

        // 取得したリストを返す
        return lstRet;
      }
      catch(Exception ex)
      {
        lstRet.Clear();
        _log.ErrorFormat("{0} データ目 フィールド {1} 項目の読込でエラーが発生しました。{2}", rowno, fldname, ex.Message);
        return lstRet;
      }
    }

    /// <summary>
    /// フィールドにセットする値を返す
    /// </summary>
    /// <param name="rowno">データ数目</param>
    /// <param name="fldname">フィールド名</param>
    /// <returns>フィールドにセットする値</returns>
    public string Get_DataV(int rowno, string fldname)
    {
      try
      {
        // 該当するフィールド名の項目リストを取得する
        int arrno = lstField.IndexOf(fldname);
        if(arrno < 0)
        {
          return strNoData;
        }
        // 定義された先頭の項目名を取得する
        string strname = lstFName[arrno][0];
        // 該当する項目値を取得する
        string strRet = Get_Data(rowno, strname);
        return strRet;
      }
      catch(Exception ex)
      {
        //_log.ErrorFormat("{0} データ目 フィールド {1} 項目の読込でエラーが発生しました。{2}", rowno, fldname, ex.Message);
        return strNoData;
      }
    }

    /// <summary>
    /// 指定行(rowno)の該当項目(name)のデータを返す
    /// </summary>
    /// <param name="rowno">指定行</param>
    /// <param name="name">項目名</param>
    /// <returns>OK=データ, ERR=Util.CommonParameter.telNO_DATA</returns>
    public string Get_Data(int rowno, string name, bool newlist = true)
    {
      try
      {
        int arrno = lstName.IndexOf(name);
        string strRet;
        if(newlist)
        {
          strRet = arrlstNew[rowno][arrno];
        }
        else
        {
          strRet = arrlstOrg[rowno][arrno];
        }
        return strRet;
      }
      catch(Exception ex)
      {
        //_log.ErrorFormat("電文ファイル {0} データ目 {1} 項目の読込でエラーが発生しました。{2}", rowno, name, ex.Message);
        return strNoData;
      }
    }

    /// <summary>
    /// 出力フラグ 参照/設定
    /// (出力=0,更新日により出力対象外=-1,エラーにより出力対象外=-2)
    /// </summary>
    /// <param name="rowno">データ数目</param>
    /// <param name="set_val">0, -1, -2 の時、指定値をセットする</param>
    /// <returns>出力フラグ</returns>
    public int intOutflg(int rowno, int set_val = -999)
    {
      if((set_val == 0) || (set_val == -1) || (set_val == -2))
      {
        lstOutflg[rowno] = set_val;
      }
      return lstOutflg[rowno];
    }

    /// <summary>
    /// 指定ユーザ情報をまとめて１文字列に出力する
    /// </summary>
    /// <param name="rowno">データ数目</param>
    /// <param name="newlist">true=オリジナルデータ, false=変換後データ</param>
    /// <returns></returns>
    public string strUsrInfo(int rowno, bool newlist = true)
    {
      string strbuf = "";
      foreach(string nm in lstName)
      {
        strbuf += "[" + nm + "](" + Get_Data(rowno, nm, newlist) + ");";
      }
      return strbuf;
    }

    /// <summary>
    /// 利用者情報の定義をリセットする
    /// </summary>
    private void Reset_Structure()
    {
      blnReady = false;
      lstName.Clear();
      lstField.Clear();
      lstFName.Clear();
      intPos.Clear();
      intLen.Clear();
      lstOutflg.Clear();
      arrlstOrg.Clear();
      arrlstNew.Clear();
      RowCount = 0;
      updymd_from = 0;
      updymd_to = 0;
    }

    /// <summary>
    /// 指定行(rowno)の該当項目(name)のデータへ値(value)を書き込む
    /// </summary>
    /// <param name="rowno">指定行</param>
    /// <param name="name">項目名</param>
    /// <param name="value">値</param>
    private void Set_Data(int rowno, string name, string value, bool newlist = true)
    {
      try
      {
        int arrno = lstName.IndexOf(name);
        if(newlist)
        {
          arrlstNew[rowno][arrno] = value;
        }
        else
        {
          arrlstOrg[rowno][arrno] = value;
        }
      }
      catch(Exception ex)
      {
        _log.ErrorFormat("電文ファイル {0} データ目 {1} 項目の書込みでエラーが発生しました。{2}", rowno, name, ex.Message);
      }
    }

    /// <summary>
    /// フィールドのマッピング情報を設定する
    /// </summary>
    /// <param name="strDenbNm">電文の項目名</param>
    /// <param name="hTui">ToUsersInfoのマッピング情報</param>
    /// <returns>OK=true,ERR=false</returns>
    private bool Set_ToUI_FieldName(string strDenbNm, Hashtable hTui)
    {
      // 指定電文項目が存在しているか？
      if(lstName.IndexOf(strDenbNm) >= 0)
      {
        try
        {
          // 該当のXML定義を取り出す
          string strSpec = hTui[strDenbNm].ToString();

          // 全ての値をフィールド名称として登録する
          string[] strFns = strSpec.Split('|');
          foreach(string strfn in strFns)
          {
            // フィールドのリスト内で検索
            int arrno = lstField.IndexOf(strfn);
            if(arrno >= 0)
            {
              // 既に登録済みのフィールド
              if(lstFName[arrno].IndexOf(strDenbNm) >= 0)
              {
                throw new Exception("フィールド名の定義に重複項目があります。");
              }
              else
              {
                // 既存の項目名リストに追加する
                lstFName[arrno].Add(strDenbNm);
              }
            }
            else
            {
              // 未登録のフィールド
              lstField.Add(strfn);
              List<string> lstbuf = new List<string>();
              lstbuf.Add(strDenbNm);
              lstFName.Add(lstbuf);
            }
          }
          return true;
        }
        catch(Exception ex)
        {
          // エラー
          _log.ErrorFormat("電文マッピングファイル ToUsersInfo - {0} の定義が不正です。{1}", strDenbNm, ex.Message);
          return false;
        }
      }
      else
      {
        // 指定の電文項目が無いためToUsersInfoへのマッピングは行わない
        return true;
      }
    }

    /// <summary>
    /// フィールドのマッピング情報を設定する
    /// </summary>
    /// <param name="hTui"></param>
    /// <returns></returns>
    private bool Set_ToUI_Field(Hashtable hTui)
    {
      bool blnRet = true;
      foreach(DictionaryEntry de in hTui)
      {
        // 電文項目目(=Key) を 取り出す
        string DenbName = de.Key.ToString();
        // フィールドリストを登録する
        blnRet = Set_ToUI_FieldName(DenbName, hTui);
        if(!blnRet)
        {
          return blnRet;
        }
      }
      return blnRet;
    }

    /// <summary>
    /// 設定ファイルによる職種コード変換
    /// </summary>
    /// <returns>OK=0, ERR=-400</returns>
    private int ID_SYOKUKBN()
    {
      int intRet = -400; // 職種コード変換エラー
      try
      {
        // 職種コード 項目は、定義に存在するか？
        int arrno = lstName.IndexOf(Util.CommonParameter.xmlID_SYOKUKBN);
        if(arrno >= 0)
        {
          // 職種コードデフォルト値
          string strDefault = Util.AppConfigController.GetInstance().GetValueString(Util.AppConfigParameter.DEFAULT);

          // 全データを対象に
          for(int intRow = 0; intRow < RowCount; intRow++)
          {
            if(lstOutflg[intRow] != 0)
            {
              // 出力対象外のデータは変換しない
              continue;
            }
            try
            {
              string strBuf = Get_Data(intRow, Util.CommonParameter.xmlID_SYOKUKBN);
              if(strBuf == Util.CommonParameter.tgmNO_DATA)
              {
                throw new Exception("職種コードのデータが参照できません。");
              }
              // 設定ファイルに従って、職種コード変換
              strBuf = ConfigurationManager.AppSettings[strBuf];
              if(string.IsNullOrEmpty(strBuf))
              {
                strBuf = strDefault;
              }
              Set_Data(intRow, Util.CommonParameter.xmlID_SYOKUKBN, strBuf);
            }
            catch(Exception ex2)
            {
              // 出力対象フラグ = エラーにより出力対象外
              lstOutflg[intRow] = -2;
              _log.ErrorFormat("電文ファイル {0} データ目 職種コード項目の変換でエラーが発生しました。{1}", intRow, ex2.Message);
            }
          }
          // 職種コード 変換OK
          intRet = 0;
        }
        else
        {
          // 職種コード 項目は、存在しない
          intRet = 0;
        }
      }
      catch(Exception ex)
      {
        _log.ErrorFormat("電文ファイル 職種コード項目の変換でエラーが発生しました。{0}", ex.Message);
        return intRet;
      }
      return intRet;
    }

    /// <summary>
    /// 利用者漢字氏名の旧字変換
    /// </summary>
    /// <returns>OK=0, ERR=-300</returns>
    private int ID_NAMEN()
    {
      int intRet = -300; // 旧字変換エラー
      try
      {
        // 利用者漢字氏名 項目は、定義に存在するか？
        int arrno = lstName.IndexOf(Util.CommonParameter.xmlID_NAMEN);
        if(arrno >= 0)
        {
          // 全データを対象に
          for(int intRow = 0; intRow < RowCount; intRow++)
          {
            if(lstOutflg[intRow] != 0)
            {
              // 出力対象外のデータは変換しない
              continue;
            }
            try
            {
              string strBuf = Get_Data(intRow, Util.CommonParameter.xmlID_NAMEN);
              if(strBuf == Util.CommonParameter.tgmNO_DATA)
              {
                throw new Exception("利用者漢字氏名のデータが参照できません。");
              }
              // 旧字変換
              strBuf = clsKanjiConversion.Convert(strBuf);
              Set_Data(intRow, Util.CommonParameter.xmlID_NAMEN, strBuf);
            }
            catch(Exception ex2)
            {
              // 出力対象フラグ = エラーにより出力対象外
              lstOutflg[intRow] = -2;
              _log.ErrorFormat("電文ファイル {0} データ目 利用者漢字氏名項目の旧字変換でエラーが発生しました。{1}", intRow, ex2.Message);
            }
          }
          // 利用者漢字氏名 旧字変換OK
          intRet = 0;
        }
        else
        {
          // 利用者漢字氏名 項目は、存在しない
          intRet = 0;
        }
      }
      catch(Exception ex)
      {
        _log.ErrorFormat("電文ファイル 利用者漢字氏名項目の変換でエラーが発生しました。{0}", ex.Message);
        return intRet;
      }
      return intRet;
    }

    /// <summary>
    /// 更新日付による出力対象判定
    /// </summary>
    /// <returns>OK=0, ERR=-100</returns>
    private int ID_UPD_YMD()
    {
      int intRet = -100; // 出力対象判定エラー
      try
      {
        // 更新日付 項目は、定義に存在するか？
        int arrno = lstName.IndexOf(Util.CommonParameter.xmlID_UPD_YMD);
        if(arrno >= 0)
        {
          // 全データを対象に
          for(int intRow = 0; intRow < RowCount; intRow++)
          {
            if(lstOutflg[intRow] != 0)
            {
              // 出力対象外のデータは判定しない
              continue;
            }
            try
            {
              string strBuf = Get_Data(intRow, Util.CommonParameter.xmlID_UPD_YMD);
              if(strBuf == Util.CommonParameter.tgmNO_DATA)
              {
                throw new Exception("更新日付のデータが参照できません。");
              }
              int intBuf = int.Parse(strBuf);
              // 期間中に更新日付が入っているか？
              if((updymd_from <= intBuf) && (intBuf <= updymd_to))
              {
                // 出力対象のまま
              }
              else
              {
                // 出力対象フラグ = 更新日により出力対象外
                lstOutflg[intRow] = -1;
              }
            }
            catch(Exception ex2)
            {
              // 出力対象フラグ = エラーにより出力対象外
              lstOutflg[intRow] = -2;
              _log.ErrorFormat("電文ファイル {0} データ目 更新日付による出力判定でエラーが発生しました。{1}", intRow, ex2.Message);
            }
          }
          // 更新日付 判定OK
          intRet = 0;
        }
        else
        {
          // 更新日付 項目は、存在しない
          _log.Error("更新日付 項目が電文マッピングファイルに定義されていません。");
        }
      }
      catch(Exception ex)
      {
        _log.ErrorFormat("更新日付 項目による出力判定でエラーが発生しました。{0}", ex.Message);
        return intRet;
      }
      return intRet;
    }

    /// <summary>
    /// パスワード仕様
    /// </summary>
    /// <returns>OK=0, ERR=-200</returns>
    private int ID_PASSWORD()
    {
      int intRet = -200; // パスワード変換エラー
#if pwdcnv_dll
      try
      {
        // パスワード 項目は、定義に存在するか？
        int arrno = lstName.IndexOf(Util.CommonParameter.xmlID_PASSWORD);
        if(arrno >= 0)
        {
          // 全データを対象に
          for(int intRow = 0; intRow < RowCount; intRow++)
          {
            if(lstOutflg[intRow] != 0)
            {
              // 出力対象外のデータは変換しない
              continue;
            }
            try
            {
              string strBuf = Get_Data(intRow, Util.CommonParameter.xmlID_PASSWORD);
              if(strBuf == Util.CommonParameter.tgmNO_DATA)
              {
                throw new Exception("パスワードのデータが参照できません。");
              }
              // パスワード復号化
              strBuf = Decrypt(strBuf);
              Set_Data(intRow, Util.CommonParameter.xmlID_PASSWORD, strBuf);
            }
            catch(Exception ex2)
            {
              // 出力対象フラグ = エラーにより出力対象外
              lstOutflg[intRow] = -2;
              _log.ErrorFormat("電文ファイル {0} データ目 パスワード項目の変換でエラーが発生しました。{1}", intRow, ex2.Message);
            }
          }
          // パスワード 変換OK
          intRet = 0;
        }
        else
        {
          // パスワード 項目は、存在しない
          intRet = 0;
        }
      }
      catch(Exception ex)
      {
        _log.ErrorFormat("電文ファイル パスワード項目の変換でエラーが発生しました。{0}", ex.Message);
        return intRet;
      }
#else
			intRet = 0;
#endif
      return intRet;
    }

#if pwdcnv_dll
    /// <summary>
    /// パスワード複合化
    /// </summary>
    /// <param name="strpassword">暗号化パスワード文字列</param>
    /// <returns>複合化したパスワード</returns>
    private string Decrypt(string strpassword)
    {
      int ret = outproc.DecodeSecretCode(ref strpassword);
      if(ret == -1)
      {
        throw new Exception("複合化に失敗しました。");
      }
      return strpassword;
    }
#endif

  }
}
