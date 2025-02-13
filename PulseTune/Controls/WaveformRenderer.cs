using System;
using System.Diagnostics;
using System.Drawing;
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

        // コンストラクタ
        public WaveformRenderer()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public void PaintWaveform(float[] waveform)
        {
            this.isMono = true;
            this.waveform = waveform;
            this.lineLength = (float)this.ClientRectangle.Width / waveform.Length;

            Invalidate();
        }

        public void PaintWaveform(float[] lch, float[] rch)
        {
            this.isMono = false;
            this.lchWaveform = lch;
            this.rchWaveform = rch;
            this.lineLength = (float)this.ClientRectangle.Width / Math.Max(lch.Length, rch.Length);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float vCenter = e.ClipRectangle.Height * 0.5f;
            float x = 0, y = vCenter, x2 = 0, y2 = 0;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.Clear(this.BackColor);

            if (this.isMono)
            {
                if (this.waveform == null || this.waveform.Length <= 1)
                {
                    return;
                }

                using (Pen pen = new Pen(Color.Black))
                {
                    x = 0;
                    y = vCenter - (this.waveform[0] * vCenter);

                    for (int i = 1; i < this.waveform.Length - 1; i++)
                    {
                        x2 = x + this.lineLength;
                        y2 = vCenter - (this.waveform[i] * vCenter);

                        e.Graphics.DrawLine(pen, x, y, x2, y2);
                        x = x2;
                        y = y2;
                    }
                }
            }
            else
            {
                // 左チャンネルのデータがあれば緑色で描画
                if (this.lchWaveform != null && this.lchWaveform.Length >= 1)
                {
                    x = 0;
                    y = vCenter - (this.lchWaveform[0] * vCenter);
                    using (Pen pen = new Pen(Color.Green))
                    {
                        for (int i = 1; i < this.lchWaveform.Length - 1; i++)
                        {
                            x2 = x + this.lineLength;
                            y2 = vCenter - (this.lchWaveform[i] * vCenter);

                            e.Graphics.DrawLine(pen, x, y, x2, y2);
                            x = x2;
                            y = y2;
                        }
                    }
                }

                // 初期化
                x2 = 0;
                y2 = 0;

                // 右チャンネルのデータがあれば赤色で描画
                if (this.rchWaveform != null && this.rchWaveform.Length >= 1)
                {
                    using (Pen pen = new Pen(Color.Red))
                    {
                        x = 0;
                        y = vCenter - (this.rchWaveform[0] * vCenter);
                        for (int i = 1; i < this.rchWaveform.Length - 1; i++)
                        {
                            x2 = x + this.lineLength;
                            y2 = vCenter - (this.rchWaveform[i] * vCenter);

                            e.Graphics.DrawLine(pen, x, y, x2, y2);
                            x = x2;
                            y = y2;
                        }
                    }
                }
            }

            e.Graphics.DrawLine(Pens.Blue, 0, vCenter, e.ClipRectangle.Right, vCenter);
        }
    }
}
