using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TheraRis_UsersIFLinkage.Frm
{
	public partial class frmNotifyIcon : Form
    {
        /// <summary>
        /// ログ出力
        /// </summary>
        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// スレッドのインスタンス
        /// </summary>
        private Thread thread = null;

        /// <summary>
        /// プロセスメインクラス
        /// </summary>
        private ProcessMain main = new ProcessMain("TaskTrayApp");

        /// <summary>
        /// 初期処理
        /// </summary>
        public frmNotifyIcon()
        {
            // コンポーネントのコンパイルされたページを読込む
            InitializeComponent();
        }

        /// <summary>
        /// 画面ロード処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmNotifyIcon_Load(object sender, EventArgs e)
        {
            _log.Info("Application Start >>>>>");

            try
            {
                // アイコンの設定
                this.notifyIcon.Icon = new System.Drawing.Icon(Path.Combine(Application.StartupPath, "Icon\\app.ico"));

                // 画面を非表示
                this.Visible = false;

                // スレッドの生成
                thread = new Thread(main.Start);

                // スレッドの開始
                thread.Start();

                // 再開を使用不可
                this.toolStripMenuItemRestart.Enabled = false;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
                _log.Info("Application Stop >>>>>");
                throw ex;
            }
        }

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void End_Click(object sender, EventArgs e)
        {
            // タスクトレイアイコンを非表示にする。
            this.notifyIcon.Visible = false;

            // タスクトレイアイコンが残らないようにする
            this.notifyIcon.Dispose();

            // 終了指示を設定する
            main.Stop();

            // スレッドが完了するまで待機
            thread.Join();

            // アプリケーション終了
            Application.Exit();

            _log.Info("Application Stop >>>>>");
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemStop_Click(object sender, EventArgs e)
        {
            // 終了指示を設定する
            main.Stop();

            // スレッドが完了するまで待機
            thread.Join();

            // 停止を使用不可
            this.toolStripMenuItemStop.Enabled = false;

            // 再開を使用可
            this.toolStripMenuItemRestart.Enabled = true;
        }

        /// <summary>
        /// 再開
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemRestart_Click(object sender, EventArgs e)
        {
            // スレッドの生成
            thread = new Thread(main.Start);

            // スレッドの開始
            thread.Start();

            // 停止を使用可
            this.toolStripMenuItemStop.Enabled = true;

            // 再開を使用不可
            this.toolStripMenuItemRestart.Enabled = false;
        }
    }
}