using LibPulseTune;
using Microsoft.WindowsAPICodePack.Taskbar;
using PulseTune.Properties;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using Windows.Web.Http;

namespace PulseTune.Controls
{
    public partial class MediaControlPanel : UserControl
    {
        // イベント
        public event EventHandler Seek;
        public event EventHandler VolumeChanged;

        // コマンド
        private Command playCommand;
        private Command pauseCommand;
        private Command resumeCommand;
        private Command backwardCommand;
        private Command moveToTrackStartCommand;
        private Command forwardCommand;
        private Command stopCommand;

        // 非公開フィールド
        private readonly ThumbnailToolBarButton BackwardThumbnailButton;
        private readonly ThumbnailToolBarButton PlayPauseThumbnailButton;
        private readonly ThumbnailToolBarButton StopThumbnailButton;
        private readonly ThumbnailToolBarButton ForwardThumbnailButton;

        // コンストラクタ
        public MediaControlPanel()
        {
            InitializeComponent();

            this.BackwardThumbnailButton = new ThumbnailToolBarButton(Icon.FromHandle(Resources.backward.GetHicon()), "前のトラック");
            this.BackwardThumbnailButton.Click += BackwardButton_Click;
            this.PlayPauseThumbnailButton = new ThumbnailToolBarButton(Icon.FromHandle(Resources.play.GetHicon()), "再生");
            this.PlayPauseThumbnailButton.Click += PlayPauseButton_Click;
            this.StopThumbnailButton = new ThumbnailToolBarButton(Icon.FromHandle(Resources.stop.GetHicon()), "停止");
            this.StopThumbnailButton.Click += StopButton_Click;
            this.ForwardThumbnailButton = new ThumbnailToolBarButton(Icon.FromHandle(Resources.forward.GetHicon()), "次のトラック");
            this.ForwardThumbnailButton.Click += ForwardButton_Click;

            AudioPlayer.StatusChanged += OnAudioPlayerStateChanged;
            AudioPlayer.PlaybackPositionChanged += OnAudioPlayerPlaybackPositionChanged;
        }

        #region コマンド

        /// <summary>
        /// 再生コマンド
        /// </summary>
        public Command PlayCommand
        {
            set
            {
                this.playCommand = value;
            }
            get
            {
                return this.playCommand;
            }
        }

        /// <summary>
        /// 一時停止コマンド
        /// </summary>
        public Command PauseCommand
        {
            set
            {
                this.pauseCommand = value;
            }
            get
            {
                return this.pauseCommand;
            }
        }

        /// <summary>
        /// 再開コマンド
        /// </summary>
        public Command ResumeCommand
        {
            set
            {
                this.resumeCommand = value;
            }
            get
            {
                return this.resumeCommand;
            }
        }

        /// <summary>
        /// 停止コマンド
        /// </summary>
        public Command StopCommand
        {
            set
            {
                this.stopCommand = value;
            }
            get
            {
                return this.stopCommand;
            }
        }

        /// <summary>
        /// 巻き戻しコマンド
        /// </summary>
        public Command BackwardCommand
        {
            set
            {
                this.backwardCommand = value;
            }
            get
            {
                return this.backwardCommand;
            }
        }

        /// <summary>
        /// トラックの最初に戻るコマンド
        /// </summary>
        public Command MoveToTrackStartCommand
        {
            set
            {
                this.moveToTrackStartCommand = value;
            }
            get
            {
                return this.moveToTrackStartCommand;
            }
        }

        /// <summary>
        /// 早送りコマンド
        /// </summary>
        public Command ForwardCommand
        {
            set
            {
                this.forwardCommand = value;
            }
            get
            {
                return this.forwardCommand;
            }
        }

        #endregion

        /// <summary>
        /// 指定されたフォームのタスクバーのサムネイル表示に、メディアコントロールボタンを表示する。
        /// </summary>
        /// <param name="form"></param>
        public void ShowTaskBarThumbnailButtons(Form form)
        {
            TaskbarManager.Instance.ThumbnailToolBars.AddButtons(
                form.Handle,
                this.BackwardThumbnailButton,
                this.PlayPauseThumbnailButton,
                this.StopThumbnailButton,
                this.ForwardThumbnailButton);
        }

        /// <summary>
        /// AudioPlayerの状態に応じて、再生ボタンがクリックされた場合に実行されるべきコマンドを、「再生」「一時停止」「再開」コマンドの中から取得して返す。
        /// </summary>
        /// <param name="executePlayCommandFlag"></param>
        /// <param name="executePauseCommandFlag"></param>
        /// <param name="executeResumeCommandFlag"></param>
        private static void GetPlayPauseButtonCommandFlags(out bool executePlayCommandFlag, out bool executePauseCommandFlag, out bool executeResumeCommandFlag)
        {
            executePlayCommandFlag = false;
            executePauseCommandFlag = false;
            executeResumeCommandFlag = false;

            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_NOT_READY:
                case AudioPlayer.AUDIOPLAYER_STATE_STOP:
                    executePlayCommandFlag = true;
                    executePauseCommandFlag = false;
                    executeResumeCommandFlag = false;
                    break;
                case AudioPlayer.AUDIOPLAYER_STATE_PLAY:
                    executePlayCommandFlag = false;
                    executePauseCommandFlag = true;
                    executeResumeCommandFlag = false;
                    break;
                case AudioPlayer.AUDIOPLAYER_STATE_PAUSE:
                    executePlayCommandFlag = false;
                    executePauseCommandFlag = false;
                    executeResumeCommandFlag = true;
                    break;
            }
        }

        /// <summary>
        /// 再生コマンドを実行する。
        /// </summary>
        public void ExecutePlaybackCommand()
        {
            GetPlayPauseButtonCommandFlags(out bool play, out bool pause, out bool resume);

            if (play)
            {
                this.PlayCommand?.Execute();
            }

            if (pause)
            {
                this.PauseCommand?.Execute();
            }

            if (resume)
            {
                this.ResumeCommand?.Execute();
            }
        }

        /// <summary>
        /// シークバーを設定する。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimumValue"></param>
        /// <param name="maximumValue"></param>
        public void SetSeekBar(int value, int minimumValue, int maximumValue)
        {
            this.SeekBarControl.Minimum = minimumValue;
            this.SeekBarControl.Maximum = maximumValue;
            this.SeekBarControl.Value = value;
        }

        /// <summary>
        /// シークバーの位置を時間として取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetSeekBarTime()
        {
            return TimeSpan.FromMilliseconds(this.SeekBarControl.Value);
        }

        /// <summary>
        /// 音量ラベルの表示を更新する。
        /// </summary>
        private void UpdateVolumeLabel()
        {
            this.VolumeLabel.Text = $"音量：{OptionManager.Volume}％";
        }

        /// <summary>
        /// AudioPlayerの状態に応じてボタンのアイコンを更新する。
        /// </summary>
        private void UpdateButtonIcons()
        {
            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_NOT_READY:
                case AudioPlayer.AUDIOPLAYER_STATE_STOP:
                case AudioPlayer.AUDIOPLAYER_STATE_PAUSE:
                    this.PlayPauseButton.Image = Resources.play;

                    if (this.PlayPauseThumbnailButton.Icon != null)
                    {
                        this.PlayPauseThumbnailButton.Icon.Dispose();
                    }

                    this.PlayPauseThumbnailButton.Icon = Icon.FromHandle(Resources.play.GetHicon());
                    this.PlayPauseThumbnailButton.Tooltip = "再生";
                    break;
                case AudioPlayer.AUDIOPLAYER_STATE_PLAY:
                    this.PlayPauseButton.Image = Resources.pause;

                    if (this.PlayPauseThumbnailButton.Icon != null)
                    {
                        this.PlayPauseThumbnailButton.Icon.Dispose();
                    }

                    this.PlayPauseThumbnailButton.Icon = Icon.FromHandle(Resources.pause.GetHicon());
                    this.PlayPauseThumbnailButton.Tooltip = "一時停止";
                    break;
            }
        }

        /// <summary>
        /// リピート方法チェックボタンの表示を、ApplicationOptionsでの設定に合わせて更新する。
        /// </summary>
        private void UpdateRepeatModeCheckButtonState()
        {
            switch (OptionManager.RepeatMode)
            {
                case RepeatMode.Off:
                    this.RepeatModeCheckButton.Image = Resources.repeat_off;
                    this.RepeatModeCheckButton.CheckState = CheckState.Unchecked;
                    break;
                case RepeatMode.RepeatAllTracks:
                    this.RepeatModeCheckButton.Image = Resources.repeat;
                    this.RepeatModeCheckButton.CheckState = CheckState.Checked;
                    break;
                case RepeatMode.RepeatCurrentTrackOnly:
                    this.RepeatModeCheckButton.Image = Resources.repeat_once;
                    this.RepeatModeCheckButton.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        /// <summary>
        /// リピート方法を切り替える。
        /// </summary>
        private void SwitchRepeatMode()
        {
            switch (OptionManager.RepeatMode)
            {
                case RepeatMode.Off:
                    OptionManager.RepeatMode = RepeatMode.RepeatAllTracks;
                    break;
                case RepeatMode.RepeatAllTracks:
                    OptionManager.RepeatMode = RepeatMode.RepeatCurrentTrackOnly;
                    break;
                case RepeatMode.RepeatCurrentTrackOnly:
                    OptionManager.RepeatMode = RepeatMode.Off;
                    break;
            }

            UpdateRepeatModeCheckButtonState();
        }

        /// <summary>
        /// コントロールが読み込まれた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.VolumeTrackBar.Value = OptionManager.Volume;
            this.ShuffleCheckButton.Checked = OptionManager.ShuffleMode;

            UpdateVolumeLabel();
            UpdateRepeatModeCheckButtonState();
        }

        /// <summary>
        /// AudioPlayerの状態が変化した場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAudioPlayerStateChanged(object sender, EventArgs e)
        {
            UpdateButtonIcons();
        }

        /// <summary>
        /// AudioPlayerで再生中のオーディオソースの再生位置が変化した場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAudioPlayerPlaybackPositionChanged(object sender, EventArgs e)
        {
            var source = AudioPlayer.GetAudioSource();

            if (source != null)
            {
                this.SeekBarControl.Value = (int)source.GetCurrentTime().TotalMilliseconds;
            }
        }

        /// <summary>
        /// 再生・一時停止ボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            ExecutePlaybackCommand();
        }

        /// <summary>
        /// 巻き戻しボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackwardButton_Click(object sender, EventArgs e)
        {
            if (AudioPlayer.GetAudioPlayerState() == AudioPlayer.AUDIOPLAYER_STATE_PLAY && this.MoveToTrackStartCommand != null)
            {
                if (AudioPlayer.GetAudioSource().GetCurrentTime().TotalSeconds >= 3)
                {
                    this.MoveToTrackStartCommand.Execute();
                }
                else
                {
                    this.BackwardCommand?.Execute();
                }
            }
            else
            {
                this.BackwardCommand?.Execute();
            }
        }

        /// <summary>
        /// 停止ボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, EventArgs e)
        {
            this.StopCommand?.Execute();
        }

        /// <summary>
        /// 早送りボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardButton_Click(object sender, EventArgs e)
        {
            this.ForwardCommand?.Execute();
        }

        /// <summary>
        /// シーク操作がされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SeekBarControl_Seek(object sender, EventArgs e)
        {
            this.Seek?.Invoke(sender, e);
        }

        /// <summary>
        /// 音量トラックバーの値が変更された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            OptionManager.Volume = this.VolumeTrackBar.Value;
            UpdateVolumeLabel();

            this.VolumeChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// リピート方法チェックボタンがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RepeatModeCheckButton_Click(object sender, EventArgs e)
        {
            SwitchRepeatMode();
        }

        /// <summary>
        /// シャッフルモードチェックボタンのチェック状態が変更された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShuffleCheckButton_CheckedChanged(object sender, EventArgs e)
        {
            OptionManager.ShuffleMode = this.ShuffleCheckButton.Checked;
        }
    }
}
