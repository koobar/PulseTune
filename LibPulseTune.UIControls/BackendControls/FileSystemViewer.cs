using LibPulseTune.Engine.Providers;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class FileSystemViewer : ExplorerLikeListView
    {
        // 非公開定数
        private const string COLUMN_HEADER_FILENAME = @"名前";
        private const string COLUMN_HEADER_FORMAT = @"種類";
        private const string COLUMN_HEADER_SIZE = @"サイズ";
        private const string COLUMN_HEADER_LASTWRITE = @"更新日時";
        private const string COLUMN_HEADER_READONLY = @"読み取り専用";

        // アイコンの定義
        private readonly StockIcon FolderIcon;
        private readonly StockIcon AudioFileIcon;
        private readonly int IconWidth;

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
            this.FolderIcon = new StockIcon(StockIconIdentifier.Folder, StockIconSize.Small, false, false);
            this.AudioFileIcon = new StockIcon(StockIconIdentifier.AudioFiles, StockIconSize.Small, false, false);
            this.IconWidth = Math.Max(FolderIcon.Icon.Width, AudioFileIcon.Icon.Width);

            this.fileFormatFilterExtensions = new List<string>();
            this.forwardStack = new Stack<string>();
            this.itemsSource = new List<FileSystemViewerItem>();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.OwnerDraw = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.AutoArrange = false;
            this.AllowColumnReorder = false;
            this.FullRowSelect = true;
            this.Columns.Add(new ColumnHeader() { Text = COLUMN_HEADER_FILENAME });
            this.Columns.Add(new ColumnHeader() { Text = COLUMN_HEADER_FORMAT });
            this.Columns.Add(new ColumnHeader() { Text = COLUMN_HEADER_SIZE });
            this.Columns.Add(new ColumnHeader() { Text = COLUMN_HEADER_LASTWRITE });
            this.Columns.Add(new ColumnHeader() { Text = COLUMN_HEADER_READONLY });
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
        }

        /// <summary>
        /// 指定されたパスのディレクトリを開く。
        /// </summary>
        /// <param name="path"></param>
        private void InternalNavigate(string path)
        {
            this.currentPath = path;

            UpdateView();

            this.Navigated?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// 指定されたフォルダを示すアイテムを生成する。
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns></returns>
        private FileSystemViewerItem CreateDirectoryItem(DirectoryInfo dirInfo)
        {
            var item = new FileSystemViewerItem(dirInfo.FullName, this.FolderIcon.Icon, true);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                string text = string.Empty;

                switch (this.Columns[i].Text)
                {
                    case COLUMN_HEADER_FILENAME:
                        text = dirInfo.Name;
                        break;
                    case COLUMN_HEADER_FORMAT:
                        text = "フォルダ";
                        break;
                    case COLUMN_HEADER_SIZE:
                        text = string.Empty;
                        break;
                    case COLUMN_HEADER_LASTWRITE:
                        text = dirInfo.LastAccessTime.ToString();
                        break;
                    case COLUMN_HEADER_READONLY:
                        text = dirInfo.Attributes.HasFlag(FileAttributes.ReadOnly) ? "はい" : "いいえ";
                        break;
                }

                if (i == 0)
                {
                    item.Text = text;
                }
                else
                {
                    item.SubItems.Add(text);
                }
            }

            return item;
        }

        /// <summary>
        /// 指定されたファイルを示すアイテムを生成する。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private FileSystemViewerItem CreateFileItem(FileInfo fileInfo)
        {
            var item = new FileSystemViewerItem(fileInfo.FullName, this.AudioFileIcon.Icon, false);

            for (int i = 0; i < this.Columns.Count; i++)
            {
                string text = string.Empty;

                switch (this.Columns[i].Text)
                {
                    case COLUMN_HEADER_FILENAME:
                        text = fileInfo.Name;
                        break;
                    case COLUMN_HEADER_FORMAT:
                        text = AudioSourceProvider.GetFormatNameFromExtension(fileInfo.Extension);
                        break;
                    case COLUMN_HEADER_SIZE:
                        text = Math.Round(fileInfo.Length / 1024.0 / 1024.0, 2) + "MiB";
                        break;
                    case COLUMN_HEADER_LASTWRITE:
                        text = fileInfo.LastAccessTime.ToString();
                        break;
                    case COLUMN_HEADER_READONLY:
                        text = fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly) ? "はい" : "いいえ";
                        break;
                }

                if (i == 0)
                {
                    item.Text = text;
                }
                else
                {
                    item.SubItems.Add(text);
                }
            }

            return item;
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

                this.itemsSource.Add(CreateDirectoryItem(folders.Current));
            }

            // ファイルを列挙
            while (files.MoveNext())
            {
                if (this.fileFormatFilterExtensions.Count == 0 || this.fileFormatFilterExtensions.Contains(files.Current.Extension.ToLower()))
                {
                    this.itemsSource.Add(CreateFileItem(files.Current));
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
            UpdateColumnHeaderSize();
        }

        /// <summary>
        /// 列ヘッダのサイズを更新する。
        /// </summary>
        private void UpdateColumnHeaderSize()
        {
            const int spacing = 3;
            int maxFileNameWidth = 0;

            foreach (FileSystemViewerItem item in this.itemsSource)
            {
                if (item == null)
                {
                    continue;
                }

                var textSize = TextRenderer.MeasureText(item.SubItems[0].Text, this.Font);
                var fileNameWidth = this.IconWidth + SPACING + textSize.Width;

                if (maxFileNameWidth < fileNameWidth)
                {
                    maxFileNameWidth = fileNameWidth + (spacing * 5);
                }
            }

            this.Columns[0].Width = maxFileNameWidth;
            for (int i = 1; i < this.Columns.Count; i++)
            {
                var headerTextWidth = TextRenderer.MeasureText(this.Columns[i].Text, this.Font).Width + spacing + spacing;
                var maximumContentTextWidth = -1;

                foreach (var item in this.itemsSource)
                {
                    int contentTextWidth = TextRenderer.MeasureText(item.SubItems[i].Text, this.Font).Width + spacing + spacing;

                    if (maximumContentTextWidth < contentTextWidth)
                    {
                        maximumContentTextWidth = contentTextWidth;
                    }
                }

                this.Columns[i].Width = Math.Max(headerTextWidth, maximumContentTextWidth);
                this.Columns[i].TextAlign = HorizontalAlignment.Center;

                // これを使うと、Windows 11環境で不要な水平スクロールバーが出現する場合がある。
                //this.Columns[i].Width = -2;
            }
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
