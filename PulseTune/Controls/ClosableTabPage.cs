using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal class ClosableTabPage
    {
        // コンストラクタ
        public ClosableTabPage(string text, bool isClosable = true)
        {
            this.Text = text;
            this.IsClosable = isClosable;
        }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// このタブページを閉じることができるかどうかを示す。
        /// </summary>
        public bool IsClosable { private set; get; }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor { set; get; } = SystemColors.Control;

        /// <summary>
        /// タブページに表示するコントロール
        /// </summary>
        public Control Control { set; get; }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (this.Control != null)
            {
                this.Control.Dispose();
            }
        }
    }
}
