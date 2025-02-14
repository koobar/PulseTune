using System;
using System.Windows.Forms;

namespace LibPulseTune.Engine
{
    internal class PlaybackTimer
    {
        // 非公開フィール
        private readonly Timer internalTimer;

        // イベント
        public event EventHandler Tick;

        // コンストラクタ
        public PlaybackTimer(int interval)
        {
            this.internalTimer = new Timer();
            this.internalTimer.Interval = interval;
            this.internalTimer.Tick += OnTick;
        }

        public int Interval
        {
            get
            {
                return this.internalTimer.Interval;
            }
        }

        /// <summary>
        /// タイマーを開始する。
        /// </summary>
        public void Start()
        {
            this.internalTimer.Start();
        }

        /// <summary>
        /// タイマーを停止する。
        /// </summary>
        public void Stop()
        {
            this.internalTimer.Stop();
        }

        /// <summary>
        /// タイマー処理の実行タイミングが来た場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            this.Tick?.Invoke(this, EventArgs.Empty);
        }
    }
}
