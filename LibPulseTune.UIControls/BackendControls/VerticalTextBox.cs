using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class VerticalTextBox : Control
    {
        // 非公開フィールド
        private readonly CustomTextBox textBox;
        private int leftRightPadding;

        // コンストラクタ
        public VerticalTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.textBox = new CustomTextBox();
            this.textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(this.textBox);

            // 初期化
            this.Text = "";
            this.LeftRightPadding = 10;
        }

        public bool ReadOnly
        {
            set
            {
                this.textBox.ReadOnly = value;
            }
            get
            {
                return this.textBox.ReadOnly;
            }
        }
        
        public override string Text
        {
            get
            {
                return this.textBox.Text; 
            }
            set 
            { 
                this.textBox.Text = value; 
            }
        }

        public override Color ForeColor 
        { 
            get 
            { 
                return this.textBox.ForeColor; 
            } 
            set 
            {
                this.textBox.ForeColor = value; 
            } 
        }
        
        public override Color BackColor
        {
            get 
            { 
                return base.BackColor; 
            }
            set
            {
                this.textBox.BackColor = base.BackColor = value;
                Invalidate();
            }
        }

        public HorizontalAlignment TextAlign 
        { 
            get 
            { 
                return this.textBox.TextAlign; 
            } 
            set 
            {
                this.textBox.TextAlign = value; 
            } 
        }

        public uint LeftRightPadding
        {
            get 
            { 
                return Convert.ToUInt32(this.leftRightPadding); 
            }
            set
            {
                this.leftRightPadding = Convert.ToInt32(value);
                this.textBox.Location = new Point(this.leftRightPadding, this.textBox.Location.Y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            int textTop = (this.Height / 2) - (this.textBox.ClientSize.Height / 2);
            this.textBox.Location = new Point(this.leftRightPadding, textTop);
            this.textBox.Width = this.Width - (this.leftRightPadding * 2) - 2;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!this.textBox.Focused)
            {
                this.textBox.Focus();
            }
        }

        class CustomTextBox : TextBox
        {
            public CustomTextBox()
            {
                this.BorderStyle = BorderStyle.None;
            }

            protected override void OnFontChanged(EventArgs e)
            {
                base.OnFontChanged(e);

                int textTop = (this.Parent.Height / 2) - ((this.ClientSize.Height + 2) / 2);
                this.Location = new Point(this.Location.X, textTop);
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    e.Handled = true;
                }
                base.OnKeyPress(e);
            }
        }
    }

}
