using LibPulseTune.UIControls.BackendControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class SeekBar : UserControl
    {
        // 非公開フィールド
        private readonly SeekBarSlidePanel slidePanel;

        // イベント
        public event EventHandler MaximumValueChanged;
        public event EventHandler MinimumValueChanged;
        public event EventHandler ValueChanged;
        public event EventHandler Seek;

        // コンストラクタ
        public SeekBar()
        {
            this.slidePanel = new SeekBarSlidePanel();

            this.Size = new Size(100, 20);
            this.Maximum = 100;
            this.Minimum = 0;
            this.Value = 0;

            this.slidePanel.Dock = DockStyle.Fill;
            this.slidePanel.Parent = this;
            this.slidePanel.ValueChanged += OnValueChanged;
            this.slidePanel.SeekCompleted += OnSeekCompleted;
        }

        #region プロパティ

        /// <summary>
        /// マウスホイールの回転による値の変更を許可するかどうか
        /// </summary>
        public bool AllowMouseWheelValueChange { set; get; }

        /// <summary>
        /// 減少ステップ数
        /// </summary>
        public int DecrementStep { set; get; } = 3;

        /// <summary>
        /// 増加ステップ数
        /// </summary>
        public int IncrementStep { set; get; } = 3;

        /// <summary>
        /// パーセント
        /// </summary>
        public int Percent
        {
            get
            {
                var p = ((double)this.Value - this.Minimum) / ((double)this.Maximum - this.Minimum) * 100;

                return (int)Math.Round(p);
            }
        }

        /// <summary>
        /// 最大値
        /// </summary>
        public int Maximum
        {
            set
            {
                this.slidePanel.MaximumValue = value;

                OnMaximumValueChanged();
            }
            get
            {
                return this.slidePanel.MaximumValue;
            }
        }

        /// <summary>
        /// 最小値
        /// </summary>
        public int Minimum
        {

            set
            {
                this.slidePanel.MinimumValue = value;

                OnMinimumValueChanged();
            }
            get
            {
                return this.slidePanel.MinimumValue;
            }
        }

        /// <summary>
        /// 値
        /// </summary>
        public int Value
        {

            set
            {
                this.slidePanel.Value = value;
            }
            get
            {
                return this.slidePanel.Value;
            }
        }

        /// <summary>
        /// シーク操作中であるかどうかを示す。
        /// </summary>
        public bool IsSeeking
        {
            get
            {
                return this.slidePanel.IsSeeking;
            }
        }

        #endregion

        /// <summary>
        /// マウスのホイールが回転した場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (this.AllowMouseWheelValueChange)
            {
                int value = this.Value;

                value += e.Delta < 0 ? this.IncrementStep : -this.DecrementStep;
                if (value < this.Minimum)
                {
                    value = this.Minimum;
                }
                else if (value > this.Maximum)
                {
                    value = this.Maximum;
                }

                this.Value = value;
            }

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// 最大値が変更された場合の処理
        /// </summary>
        protected virtual void OnMaximumValueChanged()
        {
            this.MaximumValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 最小値が変更された場合の処理
        /// </summary>
        protected virtual void OnMinimumValueChanged()
        {
            this.MinimumValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 値が変更された場合の処理
        /// </summary>
        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            this.ValueChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// シーク操作が終了した場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSeekCompleted(object sender, EventArgs e)
        {
            this.Seek?.Invoke(sender, e);
        }
    }
}
