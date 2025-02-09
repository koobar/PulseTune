using System.Drawing;

namespace PulseTune.Controls.BackendControls
{
    internal class FileSystemViewerItem : ExplorerLikeListViewItem
    {
        // 非公開フィールド
        private readonly string path;
        private readonly bool isFolder;

        // コンストラクタ
        public FileSystemViewerItem(string path, Bitmap icon, bool isFolder) : base()
        {
            if (icon != null)
            {
                icon.MakeTransparent();
            }

            this.path = path;
            this.Icon = icon;
            this.isFolder = isFolder;
            this.Text = System.IO.Path.GetFileName(path);
        }

        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(this.Path);
            }
        }

        public bool IsFolder
        {
            get
            {
                return this.isFolder;
            }
        }
    }
}
