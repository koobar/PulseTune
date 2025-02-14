using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal class WaveformRenderer : UserControl
    {
        // 非公開フィールド
        private float[] lchWaveform;
        private float[] rchWaveform;
        private float[] waveform;
        private bool isMono;
        private float lineLength = 0;
        private WaveformRendererStereoViewMode stereoViewMode;
        private bool enableAntiAlias;

        // コンストラクタ
        public WaveformRenderer()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.stereoViewMode = WaveformRendererStereoViewMode.Separated;
        }

        /// <summary>
        /// ステレオ音声の表示モード
        /// </summary>
        public WaveformRendererStereoViewMode StereoViewMode
        {
            set
            {
                this.stereoViewMode = value;
                Invalidate();
            }
            get
            {
                return this.stereoViewMode;
            }
        }

        /// <summary>
        /// 波形をアンチエイリアスして描画するかどうか
        /// </summary>
        public bool EnableWaveformAntiAlias
        {
            set
            {
                this.enableAntiAlias = value;
                Invalidate();
            }
            get
            {
                return this.enableAntiAlias;
            }
        }

        /// <summary>
        /// 指定された波形を描画する。
        /// </summary>
        /// <param name="waveform"></param>
        public void PaintWaveform(float[] waveform)
        {
            this.isMono = true;
            this.waveform = waveform;
            this.lineLength = (float)this.ClientRectangle.Width / waveform.Length;

            Invalidate();
        }

        /// <summary>
        /// 指定された2チャンネル分の波形を描画する。
        /// </summary>
        /// <param name="lch"></param>
        /// <param name="rch"></param>
        public void PaintWaveform(float[] lch, float[] rch)
        {
            this.isMono = false;
            this.lchWaveform = lch;
            this.rchWaveform = rch;
            this.lineLength = (float)this.ClientRectangle.Width / Math.Max(lch.Length, rch.Length);

            Invalidate();
        }

        /// <summary>
        /// 指定された描画ハンドルに、指定された色で指定された波形を、指定された中心線を中心として描画する。
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="color"></param>
        /// <param name="waveform"></param>
        /// <param name="center"></param>
        /// <param name="k"></param>
        protected virtual void PaintWaveform(Graphics graphics, Color color, float[] waveform, float center, float k, bool drawCenterline = true)
        {
            float x = 0, y = center, x2 = 0, y2 = 0;
            var defaultSmoothingMode = graphics.SmoothingMode;

            if (drawCenterline)
            {
                graphics.DrawLine(Pens.DarkGray, 0, center, this.ClientRectangle.Right, center);
            }

            if (waveform != null && waveform.Length >= 1)
            {
                // アンチエイリアスが有効なら描画モードをアンチエイリアスモードに設定
                if (this.enableAntiAlias)
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                }

                // 波形を太さ0.5で描画
                using (Pen pen = new Pen(color, 0.5f))
                {
                    x = 0;
                    y = center - (waveform[0] * center);

                    for (int i = 1; i < waveform.Length - 1; i++)
                    {
                        x2 = x + this.lineLength;
                        y2 = center - (waveform[i] * k);

                        graphics.DrawLine(pen, x, y, x2, y2);
                        x = x2;
                        y = y2;
                    }
                }

                // 後始末
                graphics.SmoothingMode = defaultSmoothingMode;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(this.BackColor);
            
            if (this.isMono)
            {
                float center = e.ClipRectangle.Height * 0.5f;
                PaintWaveform(e.Graphics, Color.Blue, this.waveform, center, center);
            }
            else
            {
                if (this.stereoViewMode == WaveformRendererStereoViewMode.Separated)
                {
                    float leftCenter = e.ClipRectangle.Height * 0.25f;
                    float splitter = leftCenter * 2;
                    float rightCenter = splitter + leftCenter;

                    PaintWaveform(e.Graphics, Color.Blue, this.lchWaveform, leftCenter, leftCenter);
                    e.Graphics.DrawLine(Pens.Gray, 0, splitter, this.ClientRectangle.Right, splitter);
                    PaintWaveform(e.Graphics, Color.Blue, this.rchWaveform, rightCenter, leftCenter);
                }
                else if (this.stereoViewMode == WaveformRendererStereoViewMode.Mixed)
                {
                    float center = e.ClipRectangle.Height * 0.5f;

                    PaintWaveform(e.Graphics, Color.Green, this.lchWaveform, center, center);
                    PaintWaveform(e.Graphics, Color.Red, this.rchWaveform, center, center, false);
                }
            }
        }
    }
}
