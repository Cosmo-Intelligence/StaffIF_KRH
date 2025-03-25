using System.Text;

namespace Ris_UsersIFLinkage.Util
{
    class CommonParameter
    {
        /// <summary>
        /// 文字コード
        /// </summary>
        public static Encoding CommonEnocode = Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// yyyyMMdd書式
        /// </summary>
        public const string DATEFORMAT_YYYYMMDD = "yyyyMMdd";

        /// <summary>
        /// yyyyMMddHHmmss書式
        /// </summary>
        public const string DATEFORMAT_YYYYMMDDHHMMSS = "yyyyMMddHHmmss";

        /// <summary>
        /// yyyyMMddHHmmssfff書式
        /// </summary>
        public const string DATEFORMAT_YYYYMMDDHHMMSSFFF = "yyyyMMddHHmmssfff";

        /// <summary>
        /// 処理モード：GUI
        /// </summary>
        public const string MODE_GUI = "gui";

        /// <summary>
        /// 処理モード：タスクスケジューラ
        /// </summary>
        public const string MODE_TASK = "task";

        /// <summary>
        /// 処理モード：サービス
        /// </summary>
        public const string MODE_SERVICE = "serv";

		// Y_Higuchi -- add -- from --
		public const string NODE_NAME_EC01 = "EC01";
		public const string NODE_NAME_EC02 = "EC02";
		public const string NODE_NAME_EC03 = "EC03";
		// Y_Higuchi -- add -- to --
		// Y_Higuchi --add --2017.09.28-->>
		public const string NODE_NAME_EC05 = "EC05";
		// Y_Higuchi --add --2017.09.28--<<


	}
}
