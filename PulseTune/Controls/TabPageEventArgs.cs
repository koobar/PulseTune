using System;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal class TabPageEventArgs : EventArgs
    {
        // 非公開フィールド
        private readonly ClosableTabPage tabPage;

        // コンストラクタ
        public TabPageEventArgs(ClosableTabPage tabPage)
        {
            this.tabPage = tabPage;
        }

        public ClosableTabPage TabPage
        {
            get
            {
                return this.tabPage;
            }
        }
    }
}
