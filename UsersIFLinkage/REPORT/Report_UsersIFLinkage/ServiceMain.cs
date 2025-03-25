using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Report_UsersIFLinkage.Ctrl;

namespace Report_UsersIFLinkage
{
    public partial class ServiceMain : ServiceBase
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// プロセスクラス
        /// </summary>
        ProcessMain proc = null;

        /// <summary>
        /// スレッドクラス
        /// </summary>
        Thread th = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();

            // ソースが存在していない時は、作成する
            if (!System.Diagnostics.EventLog.SourceExists(this.ServiceName))
            {
                // ログ名を空白にすると、"Application"となる
                System.Diagnostics.EventLog.CreateEventSource(this.ServiceName, "");
            }

            // ログにプロセスIDを出すため
            log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;

            // 例外処理
            Thread.GetDomain().UnhandledException += Application_UnhandledException;
        }

        /// <summary>
        /// サービス開始
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _log.Info("Service Start >>>>>");

            proc = new ProcessMain(this.ServiceName);

            th = new Thread(new ThreadStart(proc.Start));

            th.Start();
        }

        /// <summary>
        /// サービス停止
        /// </summary>
        protected override void OnStop()
        {
            if (proc != null)
            {
                proc.Stop();
                th.Join();
            }

            _log.Info("Service Stop <<<<<");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                _log.Error("Error", ex);
                EventLog.WriteEntry(this.ServiceName, ex.Message, EventLogEntryType.Error);
                _log.Info("Service Stop On Error <<<<<");
                System.Environment.Exit(1);
            }
        }
    }
}
