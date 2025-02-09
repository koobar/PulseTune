using System;
using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal class VolumeMeterControl : UserControl
    {
        // 非公開フィールド
        private float amplitude;

        // コンストラクタ
        public VolumeMeterControl()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.amplitude = 0;
        }

        /// <summary>
        /// 減衰をシミュレーションするかどうか（バッファが多い場合にメーターの応答が高速に見える）
        /// </summary>
        public bool EnableAttenuationEmulation { set; get; }

        /// <summary>
        /// 振幅
        /// </summary>
        public float Amplitude
        {
            set
            {
                this.amplitude = value;
                Invalidate();
            }
            get
            {
                return this.amplitude;
            }
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            this.Amplitude = 0;
        }

        private float ToDecibels(float amplitude)
        {
            if (amplitude <= 0)
            {
                amplitude = float.Epsilon;
            }

            return 20 * (float)Math.Log10(amplitude);
        }

        private float MinMaxNorm(float val, float min, float max)
        {
            return (val - min) / (max - min);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float db = ToDecibels(this.amplitude);
            float a = e.ClipRectangle.Width * MinMaxNorm(db, -40, 0);
            
            // 描画処理
            e.Graphics.Clear(this.BackColor);
            e.Graphics.FillRectangle(Brushes.Black, new Rectangle(e.ClipRectangle.X + 1, e.ClipRectangle.Y + 1, (int)(a - 2.0f), e.ClipRectangle.Height - 2));
        }
    }
}
