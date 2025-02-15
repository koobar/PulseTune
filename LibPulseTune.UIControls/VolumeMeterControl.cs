using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class VolumeMeterControl : UserControl
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
            this.amplitude = 0;
            Invalidate();
        }

        private float ToDecibels(float amplitude)
        {
            if (amplitude <= 0)
            {
                amplitude = float.Epsilon;
            }

            return 20 * (float)Math.Log10(amplitude);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float minDb = -30;
            float boxWidth = 4.0f;
            float spacing = 1.0f;
            float db = ToDecibels(this.amplitude);
            int maxCnt = (int)Math.Round(this.ClientRectangle.Width / (boxWidth + spacing));
            int cnt = maxCnt - 1;
            float cur = minDb;
            float inc = -minDb / cnt;

            // クリア
            e.Graphics.Clear(this.BackColor);

            float x = e.ClipRectangle.X + 1;
            float y = e.ClipRectangle.Y + 1;
            float w = boxWidth;
            float h = e.ClipRectangle.Height - 2;

            // 音割れしない範囲のメモリを描画
            for (int i = 0; i < cnt; i++)
            {
                if (db >= cur)
                {
                    if (cur >= -5)
                    {
                        e.Graphics.FillRectangle(Brushes.OrangeRed, x, y, w, h);
                    }
                    else if (cur >= -14)
                    {
                        e.Graphics.FillRectangle(Brushes.Gold, x, y, w, h);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.Green, x, y, w, h);
                    }
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.LightGray, x, y, w, h);
                }

                x += boxWidth;
                x += spacing;
                cur += inc;
            }

            // -0.1db以上専用のメモリを描画
            if (db >= -0.1f)
            {
                e.Graphics.FillRectangle(Brushes.Red, x, y, w, h);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.LightGray, x, y, w, h);
            }
        }

        /*private float MinMaxNorm(float val, float min, float max)
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
        }*/
    }
}
