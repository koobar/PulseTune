using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class VisualStyleIconButton : UserControl
    {
        // 非公開フィールド
        private readonly VisualStyleRenderer renderer;
        private readonly VisualStyleElement normalElement;
        private readonly VisualStyleElement hotElement;
        private readonly VisualStyleElement pressedElement;
        private readonly VisualStyleElement disabledElement;
        private bool isMouseContains;
        private bool isMouseDown;

        // コンストラクタ
        public VisualStyleIconButton(
            VisualStyleElement normal,
            VisualStyleElement hot,
            VisualStyleElement pressed,
            VisualStyleElement disabled)
        {
            this.normalElement = normal;
            this.hotElement = hot;
            this.pressedElement = pressed;
            this.disabledElement = disabled;
            this.renderer = new VisualStyleRenderer(this.normalElement);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.Size = new Size(50, 50);
        }

        public new bool Enabled
        {
            set
            {
                base.Enabled = value;
                this.isMouseDown = false;
            }
            get
            {
                return base.Enabled;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.isMouseContains = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.isMouseContains = false;
            Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            Invalidate();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.isMouseDown = false;
            Invalidate();

            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(SystemPens.ActiveBorder, e.ClipRectangle);

            if (this.Enabled)
            {
                if (this.isMouseDown)
                {
                    this.renderer.SetParameters(this.pressedElement);
                }
                else if (this.isMouseContains)
                {
                    this.renderer.SetParameters(this.hotElement);
                }
                else
                {
                    this.renderer.SetParameters(this.normalElement);
                }
            }
            else
            {
                this.renderer.SetParameters(this.disabledElement);
            }

            this.renderer.DrawBackground(e.Graphics, e.ClipRectangle);

            base.OnPaint(e);
        }
    }
}
