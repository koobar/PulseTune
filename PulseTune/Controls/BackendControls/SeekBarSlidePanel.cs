using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class SeekBarSlidePanel : Control
    {
        // 非公開定数
        private const int THUMB_SIZE = 15;

        // イベント
        public event EventHandler ValueChanged;
        public event EventHandler SeekCompleted;

        // 非公開フィールド
        private readonly Color trackLineColor;
        private readonly Brush thumbBrush;
        private readonly Brush thumbHotBrush;
        private Rectangle thumbRect;
        private int maximumValue;
        private int minimumValue;
        private int value;
        private bool isHot;

        // コンストラクタ
        public SeekBarSlidePanel()
        {
            this.LineWidth = 3f;

            // 描画設定
            this.trackLineColor = Color.FromArgb(204, 206, 219);
            this.thumbBrush = new SolidBrush(Color.FromArgb(0, 122, 240));
            this.thumbHotBrush = new SolidBrush(Color.FromArgb(32, 142, 250));
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// コントロールのハンドルが生成された場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            // 何もしなければつまみの描画位置がずれるので、
            // Valueプロパティを同じ値で再設定することで、無理やり再描画を促す。
            // なお、なぜかInvalidateで再描画しても位置ずれが改善されない。
            this.Value = this.value;

            base.OnHandleCreated(e);
        }

        #region プロパティ

        /// <summary>
        /// ユーザーがマウス操作でシーク中であるかどうかを示す。
        /// </summary>
        public bool IsSeeking { private set; get; }

        /// <summary>
        /// 最大値
        /// </summary>
        public int MaximumValue
        {
            set
            {
                this.maximumValue = value;

                UpdateThumb();
                Invalidate();
            }
            get
            {
                return this.maximumValue;
            }
        }

        /// <summary>
        /// 最小値
        /// </summary>
        public int MinimumValue
        {
            set
            {
                this.minimumValue = value;

                UpdateThumb();
                Invalidate();
            }
            get
            {
                return this.minimumValue;
            }
        }

        /// <summary>
        /// 内部値
        /// </summary>
        protected int InternalValue
        {
            set
            {
                this.value = value;

                UpdateThumb();
                Invalidate();
            }
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 値
        /// </summary>
        public int Value
        {
            set
            {
                if (!this.IsSeeking)
                {
                    this.InternalValue = value;
                }
            }
            get
            {
                return this.InternalValue;
            }
        }

        /// <summary>
        /// 線の幅
        /// </summary>
        public float LineWidth { set; get; }

        #endregion

        /// <summary>
        /// つまみを更新する。
        /// </summary>
        private void UpdateThumb()
        {
            var posRatio = (double)this.Value / this.MaximumValue;
            var trackAreaSize = this.Width - THUMB_SIZE;
            var pos = (int)(trackAreaSize * posRatio);

            this.thumbRect = new Rectangle(this.DisplayRectangle.Left + pos, this.DisplayRectangle.Top, THUMB_SIZE, Math.Max(this.Height - 3, THUMB_SIZE));
        }

        /// <summary>
        /// X座標から値に変換する。
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int XPosToValue(int x)
        {
            var a = ((double)x / this.Width) * this.maximumValue;

            var value = this.MinimumValue + (int)a;
            if (value < this.MinimumValue)
            {
                value = this.MinimumValue;
            }
            else if (value > this.MaximumValue)
            {
                value = this.MaximumValue;
            }

            return value;
        }

        #region マウス関連の処理のオーバーライド

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.isHot)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.IsSeeking = true;
                }

                Invalidate();
            }
            else if (e.Button == MouseButtons.Left)
            {
                this.Value = XPosToValue(e.X);
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
                this.SeekCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.IsSeeking)
            {
                this.IsSeeking = false;
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
                this.SeekCompleted?.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.isHot = this.thumbRect.Contains(e.Location);

            if (this.IsSeeking)
            {
                this.InternalValue = XPosToValue(e.Location.X);
            }

            Invalidate(this.thumbRect);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.isHot = false;
            Invalidate();
        }

        #endregion

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // コントロールを初期化
            e.Graphics.Clear(this.BackColor);

            // トラック線を描画する。
            using (var pen = new Pen(this.trackLineColor, 1))
            {
                pen.Width = this.LineWidth;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                e.Graphics.DrawLine(
                    pen,
                    this.DisplayRectangle.X + 5,
                    this.DisplayRectangle.Y + (this.DisplayRectangle.Height / 2),
                    this.DisplayRectangle.Right - 5,
                    this.DisplayRectangle.Y + (this.DisplayRectangle.Height / 2));
            }

            // つまみを描画する。
            var brush = this.thumbBrush;
            if (this.isHot)
            {
                brush = this.thumbHotBrush;
            }
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.FillEllipse(brush, this.thumbRect);

            base.OnPaint(e);
        }
    }
}
