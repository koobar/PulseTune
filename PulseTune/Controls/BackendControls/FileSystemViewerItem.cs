using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class FileSystemViewerItem : ListViewItem
    {
        // 非公開フィールド
        private readonly string path;
        private readonly Image icon;
        private readonly bool isFolder;

        // コンストラクタ
        public FileSystemViewerItem(string path, Image icon, bool isFolder) : base()
        {
            this.path = path;
            this.icon = icon;
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

        public Image Icon
        {
            get
            {
                return this.icon;
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
