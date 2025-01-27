using PulseTune.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal partial class ExplorerControlDetailViewer : UserControl
    {
        // 非公開フィールド
        private readonly DriveStateWatcher driveStateWatcher;

        // イベント
        public event EventHandler LocationSelectionChanged;

        // コンストラクタ
        public ExplorerControlDetailViewer()
        {
            InitializeComponent();

            this.driveStateWatcher = new DriveStateWatcher();
            this.driveStateWatcher.DriveStateChanged += OnDriveStateChanged;

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
                this.driveStateWatcher.Stop();

                UpdateAvailableLocations();

                foreach (ListViewGroup group in this.LocationsList.Groups)
                {
                    foreach (ListViewItem item in group.Items)
                    {
                        item.Selected = item.Tag.ToString() == value;
                    }
                }

                this.driveStateWatcher.Start();
            }
            get
            {
                ListViewItem selection = null;
                foreach (ListViewGroup group in this.LocationsList.Groups)
                {
                    foreach (ListViewItem item in group.Items)
                    {
                        if (item.Selected)
                        {
                            selection = item;
                            break;
                        }
                    }
                }

                if (selection != null)
                {
                    return selection.Tag.ToString();
                }

                return null;
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

        /// <summary>
        /// 利用可能なドライブのリストを更新する。
        /// </summary>
        private void UpdateAvailableLocations()
        {
            this.LocationsList.Items.Clear();
            this.LocationsList.Groups.Clear();

            // ドライブを追加
            var driveGroup = new ListViewGroup();
            driveGroup.Header = "ドライブ";
            foreach (var info in DriveInfo.GetDrives())
            {
                if (info.IsReady)
                {
                    var item = new ListViewItem();
                    item.Tag = info.RootDirectory;
                    item.Text = $"{info.RootDirectory.FullName} ({info.VolumeLabel})";

                    driveGroup.Items.Add(item);
                    this.LocationsList.Items.Add(item);
                }
            }

            // Windows 10以降か？
            if (Environment.OSVersion.Version.Major >= 10)
            {
                var shell = new Shell32.Shell();
                var folder = shell.NameSpace("shell:::{679F85CB-0220-4080-B29B-5540CC05AAB6}");
                var quickAccessGroup = new ListViewGroup();
                quickAccessGroup.Header = "クイックアクセス";

                foreach (Shell32.FolderItem folderItem in folder.Items())
                {
                    var item = new ListViewItem();
                    item.Tag = folderItem.Path;
                    item.Text = Path.GetFileName(folderItem.Path);

                    quickAccessGroup.Items.Add(item);
                    this.LocationsList.Items.Add(item);
                }

                this.LocationsList.Groups.Add(quickAccessGroup);
                this.LocationsList.Groups.Add(driveGroup);
            }
            else
            {
                this.LocationsList.Groups.Add(driveGroup);
            }

            this.columnHeader1.Width = -2;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.driveStateWatcher.Start();
            UpdateAvailableLocations();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.driveStateWatcher.Stop();
            base.OnHandleDestroyed(e);
        }

        private void OnLocationsListSelectionChanged(object sender, EventArgs e)
        {
            this.LocationSelectionChanged?.Invoke(sender, e);
        }

        private void OnDriveStateChanged(object sender, EventArgs e)
        {
            var location = this.SelectedLocation;

            UpdateAvailableLocations();

            this.SelectedLocation = location;
        }
    }
}
