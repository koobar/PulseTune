using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.Dialogs
{
    public partial class VersionDialog : Form
    {
        // 非公開フィールド
        private Version version;
        private DateTime buildDate;
        private string buildType;

        // コンストラクタ
        public VersionDialog()
        {
            InitializeComponent();

            this.Font = SystemFonts.CaptionFont;
        }

        public new DialogResult ShowDialog()
        {
            throw new NotImplementedException();
        }

        public DialogResult ShowDialog(Version applicationVersion, DateTime buildDate, string buildType)
        {
            this.version = applicationVersion;
            this.buildDate = buildDate;
            this.buildType = buildType;

            return base.ShowDialog();
        }

        /// <summary>
        /// フォームが読み込まれた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var license_doc = $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\LICENSE";

            var sb = new StringBuilder();
            sb.AppendLine("バージョン：" + this.version.ToString() + " (" + this.buildType + ")");
            sb.AppendLine("ビルド日時：" + this.buildDate.ToLongDateString());
            sb.AppendLine("ランタイム：.NET " + Environment.Version.ToString());
            sb.AppendLine("====================");
            sb.AppendLine(File.ReadAllText(license_doc, Encoding.GetEncoding(932)));

            this.InformationTextBox.Text = sb.ToString();
        }

        /// <summary>
        /// PulseTuneのページへのリンクがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PulseTunePageLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var info = new ProcessStartInfo();
            info.UseShellExecute = true;
            info.FileName = @"https://sites.google.com/view/pulsetune/HOME";

            Process.Start(info);
        }

        /// <summary>
        /// OKボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
