﻿using System.Drawing;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class FileSystemViewerItem : ExplorerLikeListViewItem
    {
        // 非公開フィールド
        private readonly string path;
        private readonly bool isFolder;

        // コンストラクタ
        public FileSystemViewerItem(string path, Icon icon, bool isFolder) : base()
        {
            this.path = path;
            this.Icon = icon;
            this.isFolder = isFolder;
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
