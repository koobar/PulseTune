using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class FileSystemViewer : ListView
    {
        // 非公開定数
        private const uint SIID_FOLDER = 3;
        private const uint SIID_AUDIOFILES = 71;
        private const uint SHGSI_ICON = 0x000000100;
        private const uint SHGSI_SMALLICON = 0x000000001;

        // 色とブラシの定義
        private static readonly Color SelectedItemBackColor = Color.FromArgb(205, 232, 255);
        private static readonly Color HotItemBackColor = Color.FromArgb(229, 243, 255);
        private static readonly Color TextColor = Color.Black;
        private static readonly Brush SelectedItemBrush = new SolidBrush(SelectedItemBackColor);
        private static readonly Brush HotItemBrush = new SolidBrush(HotItemBackColor);

        // アイコンの定義
        private static readonly Bitmap FolderIcon = GetStockIcon(SIID_FOLDER, SHGSI_SMALLICON).ToBitmap();
        private static readonly Bitmap AudioFileIcon = GetStockIcon(SIID_AUDIOFILES, SHGSI_SMALLICON).ToBitmap();
        private static readonly int IconWidth = Math.Max(FolderIcon.Width, AudioFileIcon.Width);

        // 非公開フィールド
        private readonly List<string> fileFormatFilterExtensions;
        private readonly Stack<string> forwardStack;
        private string currentPath;
        private Point mousePoint;

        // イベント
        public event EventHandler Navigated;
        public event EventHandler FileDoubleClick;

        // コンストラクタ
        public FileSystemViewer()
        {
            this.fileFormatFilterExtensions = new List<string>();
            this.forwardStack = new Stack<string>();

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.OwnerDraw = true;
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
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

        #region WinAPI

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysIconIndex;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        private static Icon GetStockIcon(uint type, uint size)
        {
            var info = new SHSTOCKICONINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);

            SHGetStockIconInfo(type, SHGSI_ICON | size, ref info);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone(); // Get a copy that doesn't use the original handle
            DestroyIcon(info.hIcon); // Clean up native icon to prevent resource leak

            return icon;
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
        /// アイテムを更新する。
        /// </summary>
        private void UpdateView()
        {
            var folders = new List<FileSystemViewerItem>();
            var files = new List<FileSystemViewerItem>();

            this.Items.Clear();
            
            if (string.IsNullOrEmpty(this.currentPath))
            {
                return;
            }

            foreach (var folder in Directory.EnumerateDirectories(this.currentPath))
            {
                var info = new DirectoryInfo(folder);
                if (info.Attributes.HasFlag(FileAttributes.Hidden) || info.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    continue;
                }

                folders.Add(new FileSystemViewerItem(folder, FolderIcon, true));
            }

            foreach (var file in Directory.EnumerateFiles(this.currentPath))
            {
                if (this.fileFormatFilterExtensions.Count == 0 || this.fileFormatFilterExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    files.Add(new FileSystemViewerItem(file, AudioFileIcon, false));
                }
            }

            this.Items.AddRange(folders.ToArray());
            this.Items.AddRange(files.ToArray());
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
                foreach (FileSystemViewerItem item in this.Items)
                {
                    var textSize = TextRenderer.MeasureText(item.Text, this.Font);
                    var width = IconWidth + spacing + textSize.Width;

                    if (maxWidth < width)
                    {
                        maxWidth = width;
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

        /// <summary>
        /// アイテムの描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(SelectedItemBrush, e.Bounds);
            }
            else if (e.Item.GetBounds(ItemBoundsPortion.ItemOnly).Contains(this.mousePoint))
            {
                e.Graphics.FillRectangle(HotItemBrush, e.Bounds);
            }

            var item = (FileSystemViewerItem)e.Item;
            var spacing = 3;
            int iconX = e.Bounds.X + spacing, iconY = e.Bounds.Y + spacing / 2, iconWidth = item.Icon.Width, iconHeight = item.Icon.Height;
            int textX = iconX + iconWidth + spacing, textY = e.Bounds.Y, textWidth = e.Bounds.Width, textHeight = e.Bounds.Height;

            if (item.Icon != null)
            {
                e.Graphics.DrawImage(item.Icon, iconX, iconY, iconWidth, iconHeight);
            }

            TextRenderer.DrawText(e.Graphics, item.Text, this.Font, new Rectangle(textX, textY, textWidth, textHeight), TextColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mousePoint = e.Location;

            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateColumnHeaderSize();
        }
    }
}
