using System;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Serv_UsersIFLinkage.Data.Export;
using Serv_UsersIFLinkage.Data.Export.Entity;
using Serv_UsersIFLinkage.Util;

namespace Serv_UsersIFLinkage.Ctrl
{
  class UsersIFLinkageController
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

    // Y_Higuchi --del --
    /// <summary>
    /// 処理待機時間(単位 : ミリ秒)
    /// </summary>
    //private int interval =
    //            int.Parse(AppConfigController.GetInstance().GetValueString(AppConfigParameter.ThreadInterval));
    // Y_Higuchi --del --

    /// <summary>
    /// USER接続文字列を取得
    /// </summary>
    private static string userConn =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.USER_Conn);

    /// <summary>
    /// YOKOGAWA DB接続文字列を取得
    /// </summary>
    private static string yokoConn =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.YOKO_Conn);

    /// <summary>
    /// ARQS DB接続文字列を取得
    /// </summary>
    private static string arqsConn =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.ARQS_Conn);

    ///// <summary>
    ///// MRMS DB接続文字列を取得
    ///// </summary>
    //private static string mrmsConn =
    //        AppConfigController.GetInstance().GetValueString(AppConfigParameter.MRMS_Conn);

    ///// <summary>
    ///// RRIS DB接続文字列を取得
    ///// </summary>
    //private static string rrisConn =
    //        AppConfigController.GetInstance().GetValueString(AppConfigParameter.RRIS_Conn);

    ///// <summary>
    ///// RTRIS DB接続文字列を取得
    ///// </summary>
    //private static string trisConn =
    //        AppConfigController.GetInstance().GetValueString(AppConfigParameter.RTRIS_Conn);

    #endregion

    #region public

    /// <summary>
    /// ユーザ連携処理
    /// </summary>
    public static bool Linkage()
    {
      // TOUSERSINFO用 DBクラス
      OracleDataBase tousdb = null;

      // ユーザ情報連携I/F
      DataTable tousersDt = new DataTable();

      try
      {
        _log.Info("初期処理を実行します。");
        // 初期処理
        if (!Init())
        {
          throw new Exception("初期処理でエラーが発生しました。");
        }

        // TOUSERSINFO用 インスタンス生成
        tousdb = new OracleDataBase(userConn);

        _log.Info("ユーザ情報連携I/F処理済データ削除処理を実行します。");
        // ユーザ情報連携I/F処理済データ削除処理
        if (!ToUsersInfo.Delete(tousdb))
        {
          throw new Exception("ユーザ情報連携I/F処理済データ削除処理でエラーが発生しました。");
        }

        // Y_Higuchi -- add --
        // STATUS='00'のものを拾って必要数増殖する
        _log.Info("ユーザ情報I/Fデータ複製&更新処理を実行します。");
        AuthorizeUser auth = new AuthorizeUser();
        int intret = auth.Copy_UsersInfo(tousdb);
        if (intret < 0)
        {
          throw new Exception("ユーザ情報I/Fデータ複製&更新処理でエラーが発生しました。");
        }
        else if (intret == 0)
        {
          _log.Info("処理対象となるレコードがありません。");
          return true;
        }
        // Y_Higuchi -- add --

        _log.Info("ユーザ情報連携I/Fデータ取得処理を実行します。");

        while (true)
        {
          // ユーザ情報連携I/Fデータ取得処理
          if (!ToUsersInfo.Export(ref tousersDt, tousdb))
          {
            throw new Exception("ユーザ情報連携I/Fデータ取得でエラーが発生しました。");
          }

          // ユーザ情報連携I/Fデータが0件になったら終了
          if (tousersDt.Rows.Count == 0)
          {
            break;
          }

          // 取得した件数分処理を行う
          foreach (DataRow tousersRow in tousersDt.Rows)
          {
            // デフォルト設定
            tousersRow[ToUsersInfoEntity.F_TRANSFERSTATUS] = ToUsersInfoEntity.TRANSFERSTATUS_01;
            tousersRow[ToUsersInfoEntity.F_TRANSFERRESULT] = ToUsersInfoEntity.TRANSFERRESULT_OK;
            tousersRow[ToUsersInfoEntity.F_TRANSFERTEXT] = string.Empty;
            try
            {
              // 連携処理実行
              Execute(tousersRow);
            }
            catch (Exception ex)
            {
              // エラー発生
              tousersRow[ToUsersInfoEntity.F_TRANSFERSTATUS] = ToUsersInfoEntity.TRANSFERSTATUS_02;
              tousersRow[ToUsersInfoEntity.F_TRANSFERRESULT] = ToUsersInfoEntity.TRANSFERRESULT_NG;
              tousersRow[ToUsersInfoEntity.F_TRANSFERTEXT] = ex.Message;
            }
            finally
            {
              _log.Info("ユーザ情報連携I/Fデータ処理結果更新処理を実行します。");
              // ユーザ情報連携I/Fテーブル更新
              if (!ToUsersInfo.UpdateResult(tousersRow, tousdb))
              {
                _log.ErrorFormat("ユーザ情報連携I/Fデータ処理結果更新処理でエラーが発生しました。【送信要求番号】{0}", tousersRow[ToUsersInfoEntity.F_REQUESTID]);
              }
            }

            // 終了指示があるか判定
            if (ProcessMain.isStop)
            {
              // 終了を中断
              return true;
            }
          }

          // 初期化
          tousersDt.Clear();
        }
      }
      finally
      {
        // 破棄
        tousersDt.Clear();
        tousersDt = null;
        tousdb = null;
        GC.Collect();
      }

      return true;
    }

    #endregion

    #region メソッド、ファンクション

    /// <summary>
    /// 連携実行
    /// </summary>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    private static bool Execute(DataRow tousersRow)
    {
      // YOKOGAWA DBクラス
      OracleDataBase yokodb = null;
      // ARQS DBクラス
      OracleDataBase arqsdb = null;
      // Y_Higuchi -- del --
      //// MRMS DBクラス
      //OracleDataBase mrmsdb = null;
      //// RRIS DBクラス
      //OracleDataBase rrisdb = null;
      //// RTRIS DBクラス
      //OracleDataBase trisdb = null;
      // Y_Higuchi -- del --

      try
      {
        // Y_Higuchi -- add --
        _log.InfoFormat("連携処理を開始します。【送信要求番号】{0}", tousersRow[ToUsersInfoEntity.F_REQUESTID]);
        // Y_Higuchi -- add --
        // Y_Higuchi -- del --
        //_log.InfoFormat("連携処理を開始します。【送信要求番号】{0}, 【更新対象DB】{1}",
        //        tousersRow[ToUsersInfoEntity.F_REQUESTID],
        //        tousersRow[ToUsersInfoEntity.F_DB]);
        // Y_Higuchi -- del --

        // YOKOGAWADBインスタンス生成
        yokodb = new OracleDataBase(yokoConn);
        // ARQSDBインスタンス生成
        arqsdb = new OracleDataBase(arqsConn);
        // Y_Higuchi -- del --
        //// MRMSDBインスタンス生成
        //mrmsdb = new OracleDataBase(mrmsConn);
        //// RRISDBインスタンス生成
        //rrisdb = new OracleDataBase(rrisConn);
        //// RTRISDBインスタンス生成
        //trisdb = new OracleDataBase(trisConn);
        // Y_Higuchi -- del --

        string dbname = string.Empty;
        // Y_Higuchi -- del --
        //string db = tousersRow[ToUsersInfoEntity.F_DB].ToString().ToUpper();
        // Y_Higuchi -- del --

        // Y_Higuchi -- del --
        //// 更新対象DBが「SERV, REPORT, RIS, THERARIS」以外の場合
        //if (db != ToUsersInfoEntity.DB_SERV && db != ToUsersInfoEntity.DB_REPORT
        //    && db != ToUsersInfoEntity.DB_RIS && db != ToUsersInfoEntity.DB_THERARIS)
        //{
        //  throw new Exception(string.Format("更新対象DB設定値が処理対象外でした。【処理対象】{0}, {1}, {2}, {3} 【更新対象DB設定値】{4}",
        //          ToUsersInfoEntity.DB_SERV, ToUsersInfoEntity.DB_REPORT, ToUsersInfoEntity.DB_RIS, ToUsersInfoEntity.DB_THERARIS, db));
        //}
        // Y_Higuchi -- del --

        dbname = "【SERV】YOKOGAWA";
        _log.InfoFormat("{0}連携処理を実行します。", dbname);

        // SERV YOKOGAWA接続
        yokodb.Open();

        // SERV YOKOGAWA連携処理
        SERV_YOKOGAWA_LinkageController yokoLink = new SERV_YOKOGAWA_LinkageController(yokodb);

        // SERV YOKOGAWA連携処理実行
        if (!yokoLink.Execute(tousersRow))
        {
          throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
        }

        // Y_Higuchi -- del --
        //// 更新対象DBが「SERV」の場合
        //if (db == ToUsersInfoEntity.DB_SERV)
        //{
        // Y_Higuchi -- del --
          dbname = "【SERV】ARQS";
          _log.InfoFormat("{0}連携処理を実行します。", dbname);

          // SERV ARQS接続
          arqsdb.Open();

          // SERV ARQS連携処理
          SERV_ARQS_LinkageController arqsLink = new SERV_ARQS_LinkageController(arqsdb);

          // SERV ARQS連携処理実行
          if (!arqsLink.Execute(tousersRow))
          {
            throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
          }
        // Y_Higuchi -- del --
        //}
        // Y_Higuchi -- del --

        // A_Yoshida -- del -- 
        //// 更新対象DBが「REPORT」の場合
        //if (db == ToUsersInfoEntity.DB_REPORT)
        //{
        //	dbname = "【REPORT】MRMS";
        //	_log.InfoFormat("{0}連携処理を実行します。", dbname);

        //	// REPORT MRMS接続
        //	mrmsdb.Open();

        //	// REPORT連携処理
        //	REPORT_MRMS_LinkageController mrmsLink = new REPORT_MRMS_LinkageController(mrmsdb);

        //	// REPORT連携処理実行
        //	if (!mrmsLink.Execute(tousersRow))
        //	{
        //		throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
        //	}
        //}

        //// 更新対象DBが「RIS」の場合
        ////if (db == ToUsersInfoEntity.DB_RIS)
        ////{
        //    dbname = "【RIS】RRIS";
        //    _log.InfoFormat("{0}連携処理を実行します。", dbname);

        //    // RIS RRIS接続
        //    rrisdb.Open();

        //    // RIS連携処理
        //    RIS_RRIS_LinkageController rrisLink = new RIS_RRIS_LinkageController(rrisdb);

        //    // RIS連携処理実行
        //    if (!rrisLink.Execute(tousersRow))
        //    {
        //        throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
        //    }
        ////}

        //// 更新対象DBが「THERARIS」の場合
        ////if (db == ToUsersInfoEntity.DB_THERARIS)
        ////{
        //    dbname = "【THERARIS】RTRIS";
        //    _log.InfoFormat("{0}連携処理を実行します。", dbname);

        //    // THERARIS RTRIS接続
        //    trisdb.Open();

        //    // RTRIS連携処理
        //    THERARIS_RTRIS_LinkageController trisLink = new THERARIS_RTRIS_LinkageController(trisdb);

        //    // RTRIS連携処理実行
        //    if (!trisLink.Execute(tousersRow))
        //    {
        //        throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
        //    }
        ////}
        // A_Yoshida -- del -- 

        yokodb.Commit();
        arqsdb.Commit();
        // Y_Higuchi -- del --
        //mrmsdb.Commit();
        //rrisdb.Commit();
        //trisdb.Commit();
        // Y_Higuchi -- del --
      }
      catch (Exception ex)
      {
        yokodb.RollBack();
        arqsdb.RollBack();
        // Y_Higuchi -- del --
        //mrmsdb.RollBack();
        //rrisdb.RollBack();
        //trisdb.RollBack();
        // Y_Higuchi -- del --
        throw ex;
      }
      finally
      {
        yokodb.Close();
        arqsdb.Close();
        // Y_Higuchi -- del --
        //mrmsdb.Close();
        //rrisdb.Close();
        //trisdb.Close();
        // Y_Higuchi -- del --
        yokodb = null;
        arqsdb = null;
        // Y_Higuchi -- del --
        //mrmsdb = null;
        //rrisdb = null;
        //trisdb = null;
        // Y_Higuchi -- del --
        _log.Info("連携処理を終了します。");
      }

      return true;
    }

    /// <summary>
    /// 初期処理
    /// </summary>
    /// <returns>正常ならtrue、異常ならfalse</returns>
    private static bool Init()
    {
      // ログフォルダ削除
      Logger.Delete();

      return true;
    }

    #endregion
  }
}
