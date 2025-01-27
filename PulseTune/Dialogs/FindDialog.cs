using System;
using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Dialogs
{
    public partial class FindDialog : Form
    {
        // コンストラクタ
        public FindDialog()
        {
            InitializeComponent();

            this.Font = SystemFonts.CaptionFont;
        }

        /// <summary>
        /// 検索プロンプト
        /// </summary>
        public string Prompt
        {
            set
            {
                this.PromptTextBox.Text = value;
            }
            get
            {
                return this.PromptTextBox.Text;
            }
        }

        /// <summary>
        /// 大文字と小文字を区別するかどうかを示す
        /// </summary>
        public bool IgnoreCase
        {
            set
            {
                this.IgnoreCaseCheckBox.Checked = value;
            }
            get
            {
                return this.IgnoreCaseCheckBox.Checked;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnFind_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
