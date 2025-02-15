using LibPulseTune.Database;
using LibPulseTune.UIControls.BackendControls;
using LibPulseTune.UIControls.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class AccessList : ExplorerLikeListView
    {
        // 非公開定数
        private const string SHELL_NAMESPACE_QUICKACCESS = @"shell:::{679F85CB-0220-4080-B29B-5540CC05AAB6}";

        // 非公開フィールド
        private readonly DriveStateWatcher driveStateWatcher;

        // イベント
        public event EventHandler LocationSelectionChanged;

        // コンストラクタ
        public AccessList()
        {
            this.driveStateWatcher = new DriveStateWatcher();
            this.driveStateWatcher.DriveStateChanged += OnDriveStateChanged;

            this.View = View.Details;
            this.HeaderStyle = ColumnHeaderStyle.None;
            this.Columns.Add(new ColumnHeader() { Text = "場所" });
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        /// <summary>
        /// 選択された場所
        /// </summary>
        public string SelectedLocation
        {
            set
            {
                if (this.IsDesignMode())
                {
                    return;
                }

                this.driveStateWatcher.Stop();
                UpdateAvailableLocations();

                foreach (ListViewGroup group in this.Groups)
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
                foreach (ListViewGroup group in this.Groups)
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
        /// 指定されたパスのフォルダのアイコンを取得する。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap GetFolderIcon(string path, int width, int height)
        {
            var result = new Bitmap(width, height);
            var original = WinApi.ExtractIconFromPath(path, WinApi.ExtractIconSize.Small).ToBitmap();
            var g = Graphics.FromImage(result);

            g.DrawImage(original, 0, 0, width, height);
            g.Dispose();
            original.Dispose();

            return result;
        }

        /// <summary>
        /// ドライブを読み込む。
        /// </summary>
        private void LoadDrives()
        {
            var driveGroup = new ListViewGroup();
            driveGroup.Header = "ドライブ";
            foreach (var info in DriveInfo.GetDrives())
            {
                if (info.IsReady)
                {
                    var item = new ExplorerLikeListViewItem();
                    item.Tag = info.RootDirectory;
                    item.Icon = GetFolderIcon(info.RootDirectory.FullName, 16, 16);
                    item.Text = $"{info.RootDirectory.FullName} ({info.VolumeLabel})";

                    driveGroup.Items.Add(item);
                    this.Items.Add(item);
                }
            }
            this.Groups.Add(driveGroup);
        }

        /// <summary>
        /// クイックアクセスを読み込む。
        /// </summary>
        private void LoadQuickAccess()
        {
            // Windows 10以降か？
            if (Environment.OSVersion.Version.Major >= 10)
            {
                var shell = new Shell32.Shell();
                var folder = shell.NameSpace(SHELL_NAMESPACE_QUICKACCESS);
                var quickAccessGroup = new ListViewGroup();
                quickAccessGroup.Header = "クイックアクセス";

                foreach (Shell32.FolderItem folderItem in folder.Items())
                {
                    var item = new ExplorerLikeListViewItem();
                    item.Tag = folderItem.Path;
                    item.Icon = GetFolderIcon(folderItem.Path, 16, 16);
                    item.Text = Path.GetFileName(folderItem.Path);

                    quickAccessGroup.Items.Add(item);
                    this.Items.Add(item);
                }

                this.Groups.Insert(0, quickAccessGroup);
            }
        }

        /// <summary>
        /// お気に入りを読み込む。
        /// </summary>
        private void LoadFavorites()
        {
            var favoriteGroup = new ListViewGroup();
            favoriteGroup.Header = "お気に入り";
            
            for (int i = 0; i < PlaylistExplorerData.GetFavoriteLocationsCount(); i++)
            {
                var location = PlaylistExplorerData.GetFavoriteLocation(i);
                var item = new ExplorerLikeListViewItem();
                item.Tag = location;
                item.Icon = GetFolderIcon(location, 16, 16);
                item.Text = Path.GetFileName(location);

                favoriteGroup.Items.Add(item);
                this.Items.Add(item);
            }

            this.Groups.Insert(0, favoriteGroup);
        }

        /// <summary>
        /// 利用可能なドライブのリストを更新する。
        /// </summary>
        public void UpdateAvailableLocations()
        {
            // 古いアイテムとグループを初期化
            this.Items.Clear();
            this.Groups.Clear();

            LoadDrives();
            LoadQuickAccess();
            LoadFavorites();

            this.Columns[0].Width = -2;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.IsDesignMode())
            {
                return;
            }

            this.driveStateWatcher.Start();
            UpdateAvailableLocations();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (this.IsDesignMode())
            {
                return;
            }

            this.driveStateWatcher.Stop();
        }

        private void OnDriveStateChanged(object sender, EventArgs e)
        {
            if (this.IsDesignMode())
            {
                return;
            }

            var location = this.SelectedLocation;

            UpdateAvailableLocations();
            this.SelectedLocation = location;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.LocationSelectionChanged?.Invoke(sender, e);
        }
    }
}
