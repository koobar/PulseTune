using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    internal partial class ExplorerControlDetailViewer : UserControl
    {
        // イベント
        public event EventHandler LocationSelectionChanged;

        // コンストラクタ
        public ExplorerControlDetailViewer()
        {
            InitializeComponent();

            this.Font = SystemFonts.CaptionFont;
            this.DetailsTextBox.Font = new Font(new FontFamily("ＭＳ ゴシック"), 9.5f, FontStyle.Regular);
        }

        /// <summary>
        /// 選択された場所
        /// </summary>
        public string SelectedLocation
        {
            set
            {
                this.accessList1.SelectedLocation = value;
            }
            get
            {
                return this.accessList1.SelectedLocation;
            }
        }

        /// <summary>
        /// 指定されたパスのファイルの種類を取得する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetFileType(string path)
        {
            var ext = Path.GetExtension(path).ToLower();

            return ext.Substring(1, ext.Length - 1);
        }

        /// <summary>
        /// 指定されたディレクトリに含まれるコンテンツ数を取得する。
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private int GetDirectoryContentCount(DirectoryInfo info)
        {
            var numOfContents = 0;

            foreach (var dir in info.GetDirectories())
            {
                if (!dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    numOfContents++;
                }
            }

            foreach (var file in info.GetFiles())
            {
                if (!file.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    numOfContents++;
                }
            }

            return numOfContents;
        }

        /// <summary>
        /// 指定されたパスのディレクトリの詳細情報を表示する。
        /// </summary>
        /// <param name="path"></param>
        private void ShowDirectoryDetails(string path)
        {
            var info = new DirectoryInfo(path);
            var sb = new StringBuilder();

            sb.AppendLine($"名　　前：{info.Name}");
            sb.AppendLine($"種　　類：フォルダ");
            sb.AppendLine($"作成日時：{info.CreationTime}");
            sb.AppendLine($"最終更新：{info.LastWriteTime}");
            sb.AppendLine($"アクセス：{info.LastAccessTime}");
            sb.AppendLine($"{GetDirectoryContentCount(info)} 個のコンテンツ");

            this.DetailsTextBox.Text = sb.ToString();
        }

        /// <summary>
        /// 指定されたパスのファイルの詳細情報を表示する。
        /// </summary>
        /// <param name="path"></param>
        private void ShowFileDetails(string path)
        {
            var info = new FileInfo(path);
            var sb = new StringBuilder();

            sb.AppendLine($"名　　前：{info.Name}");
            sb.AppendLine($"種　　類：{GetFileType(path)}");
            sb.AppendLine($"サ イ ズ：{Math.Round(info.Length / 1024.0 / 1024.0, 2)}MiB");
            sb.AppendLine($"作成日時：{info.CreationTime}");
            sb.AppendLine($"最終更新：{info.LastWriteTime}");
            sb.AppendLine($"アクセス：{info.LastAccessTime}");
            sb.AppendLine($"読取専用：{(info.IsReadOnly ? "はい" : "いいえ")}");

            this.DetailsTextBox.Text = sb.ToString();
        }

        /// <summary>
        /// 指定されたパスのコンテンツの詳細情報を表示する。
        /// </summary>
        /// <param name="path"></param>
        public void ShowDetails(string path)
        {
            if (Directory.Exists(path))
            {
                ShowDirectoryDetails(path);
            }
            else if (File.Exists(path))
            {
                ShowFileDetails(path);
            }
        }

        private void accessList1_LocationSelectionChanged(object sender, EventArgs e)
        {
            this.LocationSelectionChanged?.Invoke(sender, e);
        }
    }
}
