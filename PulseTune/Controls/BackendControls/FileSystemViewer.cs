using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class FileSystemViewer : ExplorerLikeListView
    {
        // アイコンの定義
        private static readonly Bitmap FolderIcon = new StockIcon(StockIconIdentifier.Folder, StockIconSize.Small, false, false).Bitmap;
        private static readonly Bitmap AudioFileIcon = new StockIcon(StockIconIdentifier.AudioFiles, StockIconSize.Small, false, false).Bitmap;
        private static readonly int IconWidth = Math.Max(FolderIcon.Width, AudioFileIcon.Width);

        // 非公開フィールド
        private readonly List<string> fileFormatFilterExtensions;
        private readonly Stack<string> forwardStack;
        private readonly List<FileSystemViewerItem> itemsSource;
        private string currentPath;

        // イベント
        public event EventHandler Navigated;
        public event EventHandler FileDoubleClick;

        // コンストラクタ
        public FileSystemViewer()
        {
            this.fileFormatFilterExtensions = new List<string>();
            this.forwardStack = new Stack<string>();
            this.itemsSource = new List<FileSystemViewerItem>();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.OwnerDraw = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HeaderStyle = ColumnHeaderStyle.None;
            this.AutoArrange = false;
            this.AllowColumnReorder = false;
            this.Columns.Add(new ColumnHeader() { Text = "名前" });
        }

        #region プロパティ

        /// <summary>
        /// 現在の場所
        /// </summary>
        public string CurrentPath
        {
            get
            {
                return this.currentPath;
            }
        }

        /// <summary>
        /// 表示されているファイル名一覧
        /// </summary>
        public string[] FileNames
        {
            get
            {
                var result = new List<string>();

                for (int i = 0; i < this.Items.Count; ++i)
                {
                    var item = (FileSystemViewerItem)this.Items[i];

                    if (!item.IsFolder)
                    {
                        result.Add(item.Path);
                    }
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// 表示されているフォルダ一覧
        /// </summary>
        public string[] Folders
        {
            get
            {
                var result = new List<string>();

                for (int i = 0; i < this.Items.Count; ++i)
                {
                    var item = (FileSystemViewerItem)this.Items[i];

                    if (item.IsFolder)
                    {
                        result.Add(item.Path);
                    }
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// 選択されたファイル名一覧
        /// </summary>
        public string[] SelectedFileNames
        {
            get
            {
                var result = new List<string>();

                for (int i = 0; i < this.SelectedItems.Count; ++i)
                {
                    var item = (FileSystemViewerItem)this.SelectedItems[i];

                    if (!item.IsFolder)
                    {
                        result.Add(item.Path);
                    }
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// 選択されたフォルダ名一覧
        /// </summary>
        public string[] SelectedFolders
        {
            get
            {
                var result = new List<string>();

                for (int i = 0; i < this.SelectedItems.Count; ++i)
                {
                    var item = (FileSystemViewerItem)this.SelectedItems[i];

                    if (item.IsFolder)
                    {
                        result.Add(item.Path);
                    }
                }

                return result.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// GoBackメソッドが使用可能であるかどうか判定する。
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack()
        {
            var parentDirectory = Path.GetDirectoryName(this.currentPath);

            return !string.IsNullOrEmpty(parentDirectory);
        }

        /// <summary>
        /// GoForwardメソッドが使用可能であるかどうか判定する。
        /// </summary>
        /// <returns></returns>
        public bool CanGoForward()
        {
            return this.forwardStack.Count > 0;
        }

        /// <summary>
        /// 一つ上の階層のディレクトリに戻る。
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack())
            {
                this.forwardStack.Push(this.currentPath);
                InternalNavigate(Path.GetDirectoryName(this.currentPath));
            }
        }

        /// <summary>
        /// GoBack操作を行う前のディレクトリに戻る。
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward())
            {
                var dir = this.forwardStack.Pop();
                InternalNavigate(dir);
            }
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload()
        {
            InternalNavigate(this.currentPath);
        }

        /// <summary>
        /// 指定されたパスのディレクトリを開く。
        /// </summary>
        /// <param name="path"></param>
        public void Navigate(string path)
        {
            this.forwardStack?.Clear();

            InternalNavigate(path);
        }

        /// <summary>
        /// 指定されたインデックスのアイテムを選択状態に設定する。
        /// </summary>
        /// <param name="index"></param>
        public void SelectIndex(IList<int> index)
        {
            // すべてのアイテムの選択状態をリセット
            for (int i = 0; i < this.Items.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.Items[i];
                item.Selected = false;
            }

            for (int i = 0; i < this.Items.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.Items[i];

                foreach (int j in index)
                {
                    if (i == j)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            Invalidate();
        }

        /// <summary>
        /// 現在のディレクトリに存在する、指定された名前のファイルを選択する。
        /// </summary>
        /// <param name="fileName"></param>
        public void SelectFile(string fileName)
        {
            fileName = fileName.ToLower();

            for (int i = 0; i < this.Items.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.Items[i];
                item.Selected = item.FileName.ToLower() == fileName;
            }

            Invalidate();
        }

        /// <summary>
        /// 表示するファイルの拡張子の一覧を指定する。
        /// </summary>
        /// <param name="extensions"></param>
        public void SetFileFilter(IEnumerable<string> extensions)
        {
            this.fileFormatFilterExtensions.Clear();

            foreach (var ext in extensions)
            {
                this.fileFormatFilterExtensions.Add(ext.ToLower());
            }

            UpdateView();
            UpdateColumnHeaderSize();
        }

        /// <summary>
        /// 指定されたパスのディレクトリを開く。
        /// </summary>
        /// <param name="path"></param>
        private void InternalNavigate(string path)
        {
            this.currentPath = path;

            UpdateView();
            UpdateColumnHeaderSize();

            this.Navigated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 現在のディレクトリを読み込む。
        /// </summary>
        private void LoadCurrentDirectory()
        {
            if (string.IsNullOrEmpty(this.currentPath))
            {
                return;
            }

            this.itemsSource.Clear();

            var info = new DirectoryInfo(this.currentPath);
            var folders = info.EnumerateDirectories().GetEnumerator();
            var files = info.EnumerateFiles().GetEnumerator();

            // フォルダを列挙
            while (folders.MoveNext())
            {
                if (folders.Current.Attributes.HasFlag(FileAttributes.Hidden) || folders.Current.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    continue;
                }

                this.itemsSource.Add(new FileSystemViewerItem(folders.Current.FullName, FolderIcon, true));
            }

            // ファイルを列挙
            while (files.MoveNext())
            {
                if (this.fileFormatFilterExtensions.Count == 0 || this.fileFormatFilterExtensions.Contains(files.Current.Extension.ToLower()))
                {
                    this.itemsSource.Add(new FileSystemViewerItem(files.Current.FullName, AudioFileIcon, false));
                }
            }
        }

        /// <summary>
        /// 表示を更新する。
        /// </summary>
        private void UpdateView()
        {
            this.Items.Clear();

            if (string.IsNullOrEmpty(this.currentPath))
            {
                return;
            }

            LoadCurrentDirectory();
            this.Items.AddRange(this.itemsSource.ToArray());
        }

        /// <summary>
        /// 列ヘッダのサイズを更新する。
        /// </summary>
        private void UpdateColumnHeaderSize()
        {
            const int spacing = 3;
            int maxWidth = 0;

            using (var g = CreateGraphics())
            {
                foreach (FileSystemViewerItem item in this.itemsSource)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var textSize = g.MeasureString(item.Text, this.Font);
                    var width = IconWidth + spacing + textSize.Width;

                    if (maxWidth < width)
                    {
                        maxWidth = (int)Math.Round(width) + spacing;
                    }
                }
            }

            this.Columns[0].Width = maxWidth;
        }

        /// <summary>
        /// ダブルクリックされた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.SelectedItems.Count <= 0)
            {
                base.OnDoubleClick(e);
                return;
            }

            var firstSelectedItem = (FileSystemViewerItem)this.SelectedItems[0];

            if (firstSelectedItem.IsFolder)
            {
                Navigate(firstSelectedItem.Path);
            }

            bool fileSelected = false;
            for(int i = 0; i < this.SelectedItems.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.SelectedItems[i];
                if (!item.IsFolder)
                {
                    fileSelected = true;
                    break;
                }
            }

            if (fileSelected)
            {
                this.FileDoubleClick?.Invoke(this, EventArgs.Empty);
            }

            base.OnDoubleClick(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateColumnHeaderSize();
        }
    }
}
