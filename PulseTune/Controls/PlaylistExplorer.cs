using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal class PlaylistExplorer : ListView
    {
        // 非公開フィールド
        private readonly ListViewGroup favoritesGroup;
        private readonly ListViewGroup recentsGroup;
        private readonly ContextMenu contextMenu;
        private readonly MenuItem addPlaylistToFavoriteMenuItem;
        private readonly MenuItem addFolderToFavoriteMenuItem;
        private readonly MenuItem contextMenuSeparator1;
        private readonly MenuItem deleteItemMenuItem;

        // 公開イベント
        public event EventHandler LocationDoubleClick;

        // コンストラクタ
        public PlaylistExplorer()
        {
            this.contextMenu = new ContextMenu();
            this.addPlaylistToFavoriteMenuItem = new MenuItem();
            this.addPlaylistToFavoriteMenuItem.Text = "プレイリストをお気に入りに追加";
            this.addPlaylistToFavoriteMenuItem.Click += AddPlaylistToFavoriteMenuItem_Click;
            this.addFolderToFavoriteMenuItem = new MenuItem();
            this.addFolderToFavoriteMenuItem.Text = "フォルダをお気に入りに追加";
            this.addFolderToFavoriteMenuItem.Click += AddFolderToFavoriteMenuItem_Click;
            this.contextMenuSeparator1 = new MenuItem();
            this.contextMenuSeparator1.Text = "-";
            this.deleteItemMenuItem = new MenuItem();
            this.deleteItemMenuItem.Text = "一覧から削除";
            this.deleteItemMenuItem.Click += DeleteItemMenuItem_Click;
            this.contextMenu.Popup += OnContextMenuPopup;
            this.contextMenu.MenuItems.Add(this.addPlaylistToFavoriteMenuItem);
            this.contextMenu.MenuItems.Add(this.addFolderToFavoriteMenuItem);
            this.contextMenu.MenuItems.Add(this.contextMenuSeparator1);
            this.contextMenu.MenuItems.Add(this.deleteItemMenuItem);

            this.ContextMenu = this.contextMenu;
            this.favoritesGroup = new ListViewGroup();
            this.favoritesGroup.Header = "お気に入り";
            this.favoritesGroup.HeaderAlignment = HorizontalAlignment.Center;
            this.recentsGroup = new ListViewGroup();
            this.recentsGroup.Header = "最近開いた場所";
            this.favoritesGroup.HeaderAlignment = HorizontalAlignment.Center;

            PlaylistExplorerData.FavoriteLocationsChanged += delegate
            {
                UpdateView();
            };
            PlaylistExplorerData.RecentLocationsChanged += delegate
            {
                UpdateView();
            };

            this.ShowItemToolTips = true;
            this.DoubleClick += delegate
            {
                if (this.SelectedItems.Count != 0)
                {
                    this.LocationDoubleClick?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        /// <summary>
        /// 選択された場所
        /// </summary>
        public string SelectedLocation
        {
            get
            {
                if (this.SelectedItems.Count <= 0)
                {
                    return null;
                }

                var item = this.SelectedItems[0];

                return item.Tag.ToString();
            }
        }

        /// <summary>
        /// 表示を更新する。
        /// </summary>
        public void UpdateView()
        {
            this.Items.Clear();

            // お気に入りの場所を表示する。
            for (int i = 0; i < PlaylistExplorerData.GetFavoriteLocationsCount(); ++i)
            {
                AddToFavorite(PlaylistExplorerData.GetFavoriteLocation(i));
            }

            // 最近開いた場所を表示する。
            for (int i = 0; i < PlaylistExplorerData.GetRecentLocationsCount(); ++i)
            {
                AddToRecent(PlaylistExplorerData.GetRecentLocation(i));
            }

            // 後始末
            UpdateColumnSize();
        }

        /// <summary>
        /// 列のサイズを自動調整する。
        /// </summary>
        private void UpdateColumnSize()
        {
            for (int i = 0; i < this.Columns.Count; ++i)
            {
                this.Columns[i].Width = -2;
            }
        }

        /// <summary>
        /// 指定されたパスをお気に入りに追加する。
        /// </summary>
        /// <param name="path"></param>
        private void AddToFavorite(string path)
        {
            var name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                name = $"{Path.GetFileName(Path.GetDirectoryName(path))}\\{name}";
            }

            var item = new ListViewItem();
            item.Text = name;
            item.Tag = path;
            item.ToolTipText = path;

            if (File.Exists(path))
            {
                item.SubItems.Add("プレイリスト");
            }
            else if (Directory.Exists(path))
            {
                item.SubItems.Add("フォルダ");
            }

            this.Items.Add(item);
            this.favoritesGroup.Items.Add(item);
        }

        /// <summary>
        /// 指定されたパスを最近開いた場所に追加する。
        /// </summary>
        /// <param name="path"></param>
        private void AddToRecent(string path)
        {
            var name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                name = $"{Path.GetFileName(Path.GetDirectoryName(path))}\\{name}";
            }

            var item = new ListViewItem();
            item.Text = name;
            item.Tag = path;
            item.ToolTipText = path;

            if (File.Exists(path))
            {
                item.SubItems.Add("ファイル");
            }
            else if (Directory.Exists(path))
            {
                item.SubItems.Add("フォルダ");
            }

            this.Items.Add(item);
            this.recentsGroup.Items.Add(item);
        }

        /// <summary>
        /// コントロールのハンドルが生成された場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.View = View.Details;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.ShowGroups = true;

            this.Columns.Add(new ColumnHeader() { Text = "名前" });
            this.Columns.Add(new ColumnHeader() { Text = "種類" });
            this.Groups.Add(this.recentsGroup);
            this.Groups.Add(this.favoritesGroup);
        }

        /// <summary>
        /// お気に入りにプレイリストを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPlaylistToFavoriteMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "M3Uプレイリスト(*.m3u|*.m3u8)|*.m3u|*.m3u8";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var path in dialog.FileNames)
                    {
                        PlaylistExplorerData.AddToFavorite(path);
                    }
                }
            }
        }

        /// <summary>
        /// お気に入りにフォルダを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderToFavoriteMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.IsFolderPicker = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var path in dialog.FileNames)
                    {
                        PlaylistExplorerData.AddToFavorite(path);
                    }
                }
            }
        }

        /// <summary>
        /// 一覧からアイテムを削除するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItemMenuItem_Click(object sender, EventArgs e)
        {
            var item = this.SelectedItems[0];
            var group = item.Group;

            if (group == this.recentsGroup)
            {
                PlaylistExplorerData.RemoveFromRecent(this.SelectedLocation);
            }
            else if (group == this.favoritesGroup)
            {
                PlaylistExplorerData.RemoveFromFavorite(this.SelectedLocation);
            }
        }

        /// <summary>
        /// コンテキストメニューが開かれた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuPopup(object sender, EventArgs e)
        {
            bool visible = this.SelectedItems.Count != 0;

            this.contextMenuSeparator1.Visible = visible;
            this.deleteItemMenuItem.Visible = visible;
        }
    }
}
