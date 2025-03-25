
namespace Serv_UsersIFLinkage.Data.Import.Entity
{
    /// <summary>
    /// 【THERARIS】RRIS.SECTIONDOCTORMASTER:診療科医師マスタ
    /// </summary>
    class THERARIS_RRIS_SectionDoctorMasterEntity
    {
        #region const

        /// <summary>
        /// テーブル物理名：論理名
        /// </summary>
        public const string EntityName = "【THERARIS】RRIS.SECTIONDOCTORMASTER:診療科医師マスタ";

        /// <summary>
        /// テーブル物理名
        /// </summary>
        public const string Entity = "RRIS.SECTIONDOCTORMASTER";

        /// <summary>
        /// カラム数
        /// </summary>
        private const int fields = 12;

        #endregion

        #region get set

        /// <summary>
        /// 医師ID
        /// </summary>
        private string doctor_id = null;

        /// <summary>
        /// 医師ID
        /// </summary>
        public string Doctor_id
        {
            get { return doctor_id; }
            set { doctor_id = value; }
        }

        /// <summary>
        /// 医師名
        /// </summary>
        private string doctor_name = null;

        /// <summary>
        /// 医師名
        /// </summary>
        public string Doctor_name
        {
            get { return doctor_name; }
            set { doctor_name = value; }
        }

        /// <summary>
        /// 医師名(英語)
        /// </summary>
        private string doctor_english_name = null;

        /// <summary>
        /// 医師名(英語)
        /// </summary>
        public string Doctor_english_name
        {
            get { return doctor_english_name; }
            set { doctor_english_name = value; }
        }

        /// <summary>
        /// 診療科ID
        /// </summary>
        private string section_id = null;

        /// <summary>
        /// 診療科ID
        /// </summary>
        public string Section_id
        {
            get { return section_id; }
            set { section_id = value; }
        }

        /// <summary>
        /// PHS番号
        /// </summary>
        private string doctor_tel = null;

        /// <summary>
        /// PHS番号
        /// </summary>
        public string Doctor_tel
        {
            get { return doctor_tel; }
            set { doctor_tel = value; }
        }

        /// <summary>
        /// 担当科
        /// </summary>
        private string tanto_section_id = null;

        /// <summary>
        /// 担当科
        /// </summary>
        public string Tanto_section_id
        {
            get { return tanto_section_id; }
            set { tanto_section_id = value; }
        }

        /// <summary>
        /// 表示順序
        /// </summary>
        private object showorder = null;

        /// <summary>
        /// 表示順序
        /// </summary>
        public object Showorder
        {
            get { return showorder; }
            set { showorder = value; }
        }

        /// <summary>
        /// 職員区分
        /// </summary>
        private string syokuin_kbn;

        /// <summary>
        /// 職員区分
        /// </summary>
        public string Syokuin_kbn
        {
            get { return syokuin_kbn; }
            set { syokuin_kbn = value; }
        }
 
        /// <summary>
        /// 使用可否ﾌﾗｸﾞ　１：使用する　-1：使用中止
        /// </summary>
        private int? useflag = null;

        /// <summary>
        /// 使用可否ﾌﾗｸﾞ　１：使用する　-1：使用中止
        /// </summary>
        public int? Useflag
        {
            get { return useflag; }
            set { useflag = value; }
        }

        /// <summary>
        /// 物理士業務フラグ
        /// </summary>
        private int? physicist_flg = null;

        /// <summary>
        /// 物理士業務フラグ
        /// </summary>
        public int? Physicist_flg
        {
            get { return physicist_flg; }
            set { physicist_flg = value; }
        }

        /// <summary>
        /// プラン確定権限フラグ(0:プラン確定不可,1:プラン確定可)
        /// </summary>
        private int? plan_decision_authority_flg = null;

        /// <summary>
        /// プラン確定権限フラグ(0:プラン確定不可,1:プラン確定可)
        /// </summary>
        public int? Plan_decision_authority_flg
        {
            get { return plan_decision_authority_flg; }
            set { plan_decision_authority_flg = value; }
        }

        /// <summary>
        /// デフォルト選択メニューグループ(1:医師, 2:技師、物理士, 3:看護師, 4:一般)
        /// </summary>
        private int? default_menu_group = null;

        /// <summary>
        /// デフォルト選択メニューグループ(1:医師, 2:技師、物理士, 3:看護師, 4:一般)
        /// </summary>
        public int? Default_menu_group
        {
            get { return default_menu_group; }
            set { default_menu_group = value; }
        }

        #endregion

        #region filed定数

        /// <summary>
        /// 有効フラグ 1：有効
        /// </summary>
        public const int USEFLAG_TRUE = 1;

        /// <summary>
        /// 有効フラグ -1：無効
        /// </summary>
        public const int USEFLAG_FALSE = -1;

        #endregion

        #region ログ出力用

        /// <summary>
        /// 文字列として出力する
        /// </summary>
        /// <returns>各データ</returns>
        public override string ToString()
        {
            string strText = "[診療科医師マスタ]";

            strText += " doctor_id=" + doctor_id;
            strText += " doctor_name=" + doctor_name;
            strText += " doctor_english_name=" + doctor_english_name;
            strText += " section_id=" + section_id;
            strText += " doctor_tel=" + doctor_tel;
            strText += " tanto_section_id=" + tanto_section_id;
            strText += " showorder=" + showorder;
            strText += " syokuin_kbn=" + syokuin_kbn;
            strText += " useflag=" + useflag;
            strText += " physicist_flg=" + physicist_flg;
            strText += " plan_decision_authority_flg=" + plan_decision_authority_flg;
            strText += " default_menu_group=" + default_menu_group;

            return strText;
        }

        #endregion

        #region メソッド、ファンクション

        /// <summary>
        /// 配列化
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            object[] obj = new object[fields];

            obj[0] = doctor_id;
            obj[1] = doctor_name;
            obj[2] = doctor_english_name;
            obj[3] = section_id;
            obj[4] = doctor_tel;
            obj[5] = tanto_section_id;
            obj[6] = showorder;
            obj[7] = syokuin_kbn;
            obj[8] = useflag;
            obj[9] = physicist_flg;
            obj[10] = plan_decision_authority_flg;
            obj[11] = default_menu_group;

            return obj;
        }

        #endregion
    }
}
