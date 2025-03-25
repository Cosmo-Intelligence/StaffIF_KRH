
namespace Report_UsersIFLinkage.Data.Import.Entity
{
    /// <summary>
    /// 【REPORT】MRMS.REQUESTDOCTORMASTER:依頼医マスタ
    /// </summary>
    class REPORT_MRMS_RequestDoctorMasterEntity
    {
        #region const

        /// <summary>
        /// テーブル物理名：論理名
        /// </summary>
        public const string EntityName = "【REPORT】MRMS.REQUESTDOCTORMASTER:依頼医マスタ";

        /// <summary>
        /// テーブル物理名
        /// </summary>
        public const string Entity = "MRMS.REQUESTDOCTORMASTER";

        #endregion

        #region get set

        /// <summary>
        /// 医師ID
        /// </summary>
        private string id = null;

        /// <summary>
        /// 医師ID
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 医師名
        /// </summary>
        private string name = null;

        /// <summary>
        /// 医師名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value.Trim(); }
        }

        /// <summary>
        /// 診療科ID
        /// </summary>
        private string attribute = null;

        /// <summary>
        /// 診療科ID
        /// </summary>
        public string Attribute
        {
            get { return attribute; }
            set { attribute = value.Trim(); }
        }

        /// <summary>
        /// 表示順序
        /// </summary>
        private string showorder = null;

        /// <summary>
        /// 表示順序
        /// </summary>
        public string Showorder
        {
            get { return showorder; }
            set { showorder = value; }
        }

        #endregion

    }
}
