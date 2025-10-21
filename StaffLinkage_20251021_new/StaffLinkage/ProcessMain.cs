using System;
using System.Reflection;
using System.Threading;
using StaffLinkage.Util;

namespace StaffLinkage
{
      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
  class ProcessMain
  {
    #region private

    /// <summary>
    /// ログ出力
    /// </summary>
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// 処理待機時間(単位 : ミリ秒)
    /// </summary>
    private int interval = 3000;

    /// <summary>
    /// AP終了指示フラグ(true : 終了指示)
    /// </summary>
    private bool isStop = false;

    /// <summary>
    /// サービス名
    /// </summary>
    private string ServiceName = string.Empty;

    #endregion

    #region public

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="serviceName"></param>
    public ProcessMain(string serviceName)
    {
      this.ServiceName = serviceName;
    }

    /// <summary>
    /// 処理開始
    /// </summary>
    public void Start()
    {
      isStop = false;
      _log.Info("Process Start");

      Main();

      _log.Info("Process End");
    }

    /// <summary>
    /// 処理停止
    /// </summary>
    public void Stop()
    {
      _log.Info("Stopping...");
      isStop = true;
    }

    #endregion

    #region メソッド、ファンクション

    /// <summary>
    /// メイン処理
    /// </summary>
    public void Main()
    {
      // 処理待機時間
      string strInterval = AppConfigController.GetInstance().GetValueString(AppConfigParameter.ThreadInterval);
      if (int.TryParse(strInterval, out interval))
      {
        if ((interval < 300) || (interval > 600000))
        {
          // 0.3秒～10分の範囲に指定されていなければ、標準=3秒に訂正
          interval = 3000;
        }
      }
      else
      {
        interval = 3000;
      }

      // 終了指示があるか判定
      while (!this.isStop)
      {
        _log.Debug("ループを開始します。");

        // 処理実行
        this.Execute();

        // 待機処理
        this.WaitApplication();
      }
    }

    /// <summary>
    /// 処理実行
    /// </summary>
    private void Execute()
    {
      try
      {
        // Y_Higuchi -- change -- 参考=sa_相模原協同病院 --
        //////// 職員連携処理
        //////StaffLinkageController.Linkage();
        // Y_Higuchi -- change -- 参考=sa_相模原協同病院 --

        // Controllerクラスを新規生成
        StaffLinkageController staff = new StaffLinkageController();
        // 開始
        staff.Execute();
      }
      catch(Exception ex)
      {
        _log.Error(ex.Message);
      }
    }

    /// <summary>
    /// スレッド待機処理
    /// </summary>
    private void WaitApplication()
    {
      _log.Debug("スレッド待機処理を開始します。");

      // 現在日時にスレッド待機時間を加算し、スレッド待機日時を取得
      DateTime sleepDateTime = DateTime.Now.AddMilliseconds(interval);

      _log.DebugFormat("現在日時 : {0}、スレッド待機日時 : {1}", DateTime.Now, sleepDateTime);

      // 現在日時をスレッド待機日時が上回っているか判定
      while(DateTime.Now < sleepDateTime)
      {
        // 終了指示があるか判定
        if(this.isStop)
        {
          // 現在日時とスレッド待機日時を比較しているループを終了
          break;
        }
        // スレッドを1秒間待機
        Thread.Sleep(1000);
      }
    }

    #endregion
  }
      // Y_Higuchi -- add -- 参考=sa_相模原協同病院 --
}
