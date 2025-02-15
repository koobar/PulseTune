using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class ClosableTabPage
    {
        // 非公開フィールド
        private string text;

        // イベント
        public event EventHandler TextChanged;

        // コンストラクタ
        public ClosableTabPage(string text, bool isClosable = true)
        {
            this.Text = text;
            this.IsClosable = isClosable;
        }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text
        {
            set
            {
                this.text = value;
                this.TextChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return this.text;
            }
        }

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
