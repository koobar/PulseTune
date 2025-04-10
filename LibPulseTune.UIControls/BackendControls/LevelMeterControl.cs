﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class LevelMeterControl : UserControl
    {
        // 非公開定数
        private const float METER_SCALE_WIDTH = 4.0f;               // 目盛りの幅
        private const float METER_DECIBELS_NOT_MUTE = -90.0f;       // 無音ではないと判定する基準とするデシベル（これより小さい音は無音と判定する）

        // 非公開フィールド
        private readonly float scaleWidth;
        private float decibels;
        private float minimumDecibels;
        private float scaleSpacing;

        // コンストラクタ
        public LevelMeterControl()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this.decibels = METER_DECIBELS_NOT_MUTE;
            this.minimumDecibels = -30;
            this.scaleWidth = METER_SCALE_WIDTH * Math.Max(1.0f, this.DeviceDpi / 100.0f);
            this.scaleSpacing = 1.0f;
        }

        #region プロパティ

        /// <summary>
        /// 目盛りと目盛りの間隔（ピクセル単位）
        /// </summary>
        public float ScaleSpacing
        {
            set
            {
                this.scaleSpacing = value;
                Invalidate();
            }
            get
            {
                return this.scaleSpacing;
            }
        }
        
        /// <summary>
        /// デシベル
        /// </summary>
        public float Decibels
        {
            set
            {
                this.decibels = value;
                Invalidate();
            }
            get
            {
                return this.decibels;
            }
        }

        /// <summary>
        /// デシベルの最小値
        /// </summary>
        public float MinimumDecibels
        {
            set
            {
                this.minimumDecibels = value;
                Invalidate();
            }
            get
            {
                return this.minimumDecibels;
            }
        }

        #endregion

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            this.decibels = METER_DECIBELS_NOT_MUTE;
            Refresh();
        }

        private Brush GetBrush(float scaleDb, float decibels)
        {
            if (decibels > scaleDb)
            {
                if (scaleDb >= -5.0f)
                {
                    return Brushes.OrangeRed;
                }
                else if (scaleDb >= -14.0f)
                {
                    return Brushes.Gold;
                }
                else if (scaleDb >= -30.0f)
                {
                    return Brushes.Green;
                }
                else
                {
                    return Brushes.DarkGreen;
                }
            }

            return Brushes.LightGray;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float db = this.decibels;
            float maxDb = -0.1f;
            int maxCnt = (int)Math.Round(this.ClientRectangle.Width / (this.scaleWidth + this.scaleSpacing));
            int cnt = maxCnt - 1;
            float cur = METER_DECIBELS_NOT_MUTE;
            float inc = -this.minimumDecibels / cnt;
            float x = e.ClipRectangle.X + 1;
            float y = e.ClipRectangle.Y + 1;
            float w = this.scaleWidth;
            float h = e.ClipRectangle.Height - 2;

            // クリア
            e.Graphics.Clear(this.BackColor);

            // すべての目盛りを描画
            for (int i = 0; i < cnt; i++)
            {
                if (i == 1)
                {
                    cur = this.minimumDecibels;
                }
                else if (i == cnt - 1)
                {
                    cur = maxDb;
                }

                // 目盛りを描画
                e.Graphics.FillRectangle(GetBrush(cur, db), x, y, w, h);

                // 描画位置を更新
                x += w;
                x += this.scaleSpacing;
                cur += inc;
            }

            // -0.1db以上専用の目盛りを描画
            if (db >= -0.1f)
            {
                e.Graphics.FillRectangle(Brushes.Red, x, y, w, h);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.LightGray, x, y, w, h);
            }
        }
    }
}
