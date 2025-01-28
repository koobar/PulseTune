using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PulseTune.Dialogs
{
    public partial class VersionDialog : Form
    {
        // コンストラクタ
        public VersionDialog()
        {
            InitializeComponent();

            this.Font = SystemFonts.CaptionFont;
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
            sb.AppendLine("バージョン：" + Program.ApplicationVersion.ToString());
            sb.AppendLine("ビルド日時：" + Program.ApplicationBuildDate.ToString());
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
