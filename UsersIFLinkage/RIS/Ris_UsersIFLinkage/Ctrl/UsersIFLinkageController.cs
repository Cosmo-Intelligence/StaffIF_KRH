using System;
using System.Data;
using System.Reflection;
using Ris_UsersIFLinkage.Data.Export;
using Ris_UsersIFLinkage.Data.Export.Entity;
using Ris_UsersIFLinkage.Util;

namespace Ris_UsersIFLinkage.Ctrl
{
  class UsersIFLinkageController
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// USER接続文字列を取得
    /// </summary>
    private static string userConn =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.USER_Conn);

    /// <summary>
    /// RRIS DB接続文字列を取得
    /// </summary>
    private static string rrisConn =
                AppConfigController.GetInstance().GetValueString(AppConfigParameter.RRIS_Conn);

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
      // RRIS DBクラス
      OracleDataBase rrisdb = null;

      try
      {
        _log.InfoFormat("連携処理を開始します。【送信要求番号】{0}, 【更新対象DB】{1}",
            tousersRow[ToUsersInfoEntity.F_REQUESTID],
            tousersRow[ToUsersInfoEntity.F_DB]);

        // RRISDBインスタンス生成
        rrisdb = new OracleDataBase(rrisConn);

        string dbname = string.Empty;
        string db = tousersRow[ToUsersInfoEntity.F_DB].ToString().ToUpper();

        dbname = "【RIS】RRIS";
        _log.InfoFormat("{0}連携処理を実行します。", dbname);

        // RIS RRIS接続
        rrisdb.Open();

        // RIS連携処理
        RIS_RRIS_LinkageController rrisLink = new RIS_RRIS_LinkageController(rrisdb);

        // RIS連携処理実行
        if (!rrisLink.Execute(tousersRow))
        {
          throw new Exception(string.Format("{0}連携処理でエラーが発生しました。", dbname));
        }

        rrisdb.Commit();
      }
      catch (Exception ex)
      {
        rrisdb.RollBack();
        throw ex;
      }
      finally
      {
        rrisdb.Close();
        rrisdb = null;
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
