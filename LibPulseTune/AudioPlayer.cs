using LibPulseTune.AudioDevice;
using LibPulseTune.AudioSource;
using LibPulseTune.Wasapi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace LibPulseTune
{
    public static class AudioPlayer
    {
        // 公開定数
        public const int AUDIOPLAYER_NOT_READY = 0;             // オーディオデバイスの準備ができていない
        public const int AUDIOPLAYER_STATE_STOP = 1;            // デバイスと音声ソースが指定されているが再生していない
        public const int AUDIOPLAYER_STATE_PLAY = 2;            // 再生中
        public const int AUDIOPLAYER_STATE_PAUSE = 3;           // 一時停止中

        // 非公開フィールド
        private static AudioOutputDevice outputDevice;
        private static CustomWasapiOut currentDeviceInstance;
        private static IAudioSource currentAudioSource;
        private static WaveformMonitor waveformMonitor;
        private static VolumeSampleProvider volumeController;
        private static int volume;
        private static int audioPlayerState;
        private static PlaybackTimer timer;

        // イベント
        public static event EventHandler StatusChanged;
        public static event EventHandler PlaybackPositionChanged;
        public static event EventHandler AudioSourceReachEnd;

        /// <summary>
        /// 初期化
        /// </summary>
        public static void Init()
        {
            timer = new PlaybackTimer(16);
            timer.Tick += OnTimerElapsed;
        }

        /// <summary>
        /// タイマーのインターバルを取得する。
        /// </summary>
        /// <returns></returns>
        public static double GetTimerInterval()
        {
            return timer.Interval;
        }

        /// <summary>
        /// 音量を設定する。
        /// </summary>
        /// <param name="volume"></param>
        public static void SetVolume(int volume)
        {
            AudioPlayer.volume = volume;

            if (volumeController != null)
            {
                volumeController.Volume = volume / 100.0f;
            }
        }

        /// <summary>
        /// 音量を取得する。
        /// </summary>
        /// <returns></returns>
        public static int GetVolume()
        {
            return volume;
        }

        /// <summary>
        /// オーディオプレイヤーの状態を取得する。
        /// </summary>
        /// <returns></returns>
        public static int GetAudioPlayerState()
        {
            return audioPlayerState;
        }

        /// <summary>
        /// オーディオプレイヤーの状態を設定する。
        /// </summary>
        /// <param name="state"></param>
        private static void SetAudioPlayerState(int state)
        {
            audioPlayerState = state;
            StatusChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// オーディオ出力デバイスを設定する。
        /// </summary>
        /// <param name="device"></param>
        public static void SetOutputDevice(AudioOutputDevice device)
        {
            outputDevice = device;
        }

        /// <summary>
        /// オーディオ出力デバイスを取得する。
        /// </summary>
        /// <returns></returns>
        public static AudioOutputDevice GetOutputDevice()
        {
            return outputDevice;
        }

        /// <summary>
        /// オーディオソースを設定する。
        /// </summary>
        /// <param name="source"></param>
        public static void SetAudioSource(IAudioSource source)
        {
            switch (GetAudioPlayerState())
            {
                case AUDIOPLAYER_NOT_READY:
                    break;
                case AUDIOPLAYER_STATE_STOP:
                    Close();
                    break;
                case AUDIOPLAYER_STATE_PLAY:
                case AUDIOPLAYER_STATE_PAUSE:
                    Stop();
                    Close();
                    break;
            }

            currentAudioSource = source;
        }

        /// <summary>
        /// オーディオソースを取得する。
        /// </summary>
        /// <returns></returns>
        public static IAudioSource GetAudioSource()
        {
            return currentAudioSource;
        }

        public static WaveformMonitor GetWaveformMonitor()
        {
            return waveformMonitor;
        }

        private static IWaveProvider CreateWaveProvider(ISampleProvider sampleProvider, WaveFormat originalWaveformat)
        {
            if (originalWaveformat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                return new SampleToWaveProvider(sampleProvider);
            }
            else if (originalWaveformat.Encoding == WaveFormatEncoding.Pcm)
            {
                switch (originalWaveformat.BitsPerSample)
                {
                    case 16:
                        return new SampleToWaveProvider16(sampleProvider);
                    case 24:
                        return new SampleToWaveProvider24(sampleProvider);
                }
            }

            return new SampleToWaveProvider(sampleProvider);
        }

        /// <summary>
        /// オーディオ出力デバイスと音声ソースの準備をする。
        /// </summary>
        public static void Prepare()
        {
            if (outputDevice != null && currentAudioSource != null)
            {
                // 古い波形モニタがあれば削除
                if (waveformMonitor != null)
                {
                    waveformMonitor = null;
                }

                // 古い音量コントローラがあれば削除
                if (volumeController != null)
                {
                    volumeController = null;
                }

                // 波形モニタを生成
                waveformMonitor = new WaveformMonitor(currentAudioSource);

                // 音量コントローラを作成
                volumeController = new VolumeSampleProvider(waveformMonitor);
                volumeController.Volume = volume / 100.0f;

                // デバイスを初期化
                currentDeviceInstance = outputDevice.CreateDeviceInstance();
                currentDeviceInstance.Init(CreateWaveProvider(volumeController, currentAudioSource.WaveFormat));

                GC.Collect();
            }

            SetAudioPlayerState(AUDIOPLAYER_STATE_STOP);
        }

        /// <summary>
        /// 閉じる
        /// </summary>
        public static void Close()
        {
            if (currentDeviceInstance != null)
            {
                Stop();

                currentDeviceInstance.Dispose();
                currentDeviceInstance = null;
            }

            if (currentAudioSource != null)
            {
                currentAudioSource.Dispose();
                currentAudioSource = null;
            }

            SetAudioPlayerState(AUDIOPLAYER_NOT_READY);
        }

        /// <summary>
        /// 指定された時間から再生を開始する。但し、一時停止中に呼び出された場合は、再開の挙動をとる。
        /// </summary>
        /// <param name="time"></param>
        public static void PlayFrom(TimeSpan time)
        {
            switch (GetAudioPlayerState())
            {
                case AUDIOPLAYER_NOT_READY:
                    throw new Exception("オーディオ出力デバイスと音声ソースの準備ができていません。");
                case AUDIOPLAYER_STATE_STOP:
                    currentAudioSource.SetCurrentTime(time);
                    currentDeviceInstance.Play();
                    timer.Start();
                    SetAudioPlayerState(AUDIOPLAYER_STATE_PLAY);
                    break;
                case AUDIOPLAYER_STATE_PAUSE:
                    currentDeviceInstance.Play();
                    timer.Start();
                    SetAudioPlayerState(AUDIOPLAYER_STATE_PLAY);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 再生または一時停止解除
        /// </summary>
        public static void Play()
        {
            PlayFrom(TimeSpan.FromSeconds(0));
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        public static void Pause()
        {
            switch (GetAudioPlayerState())
            {
                case AUDIOPLAYER_NOT_READY:
                    throw new Exception("オーディオ出力デバイスと音声ソースの準備ができていません。");
                case AUDIOPLAYER_STATE_STOP:
                    throw new Exception("オーディオソースの再生はすでに停止されています。");
                case AUDIOPLAYER_STATE_PLAY:
                    currentDeviceInstance.Stop();
                    timer.Stop();
                    SetAudioPlayerState(AUDIOPLAYER_STATE_PAUSE);
                    break;
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            switch (GetAudioPlayerState())
            {
                case AUDIOPLAYER_STATE_PLAY:
                case AUDIOPLAYER_STATE_PAUSE:
                    currentAudioSource.SetCurrentTime(TimeSpan.FromMilliseconds(0));
                    currentDeviceInstance.Stop();
                    timer.Stop();
                    PlaybackPositionChanged?.Invoke(null, EventArgs.Empty);
                    SetAudioPlayerState(AUDIOPLAYER_STATE_STOP);
                    break;
            }
        }

        /// <summary>
        /// イベントタイマーが更新された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTimerElapsed(object sender, EventArgs e)
        {
            var currentTime = currentAudioSource.GetCurrentTime();
            var duration = currentAudioSource.GetDuration();

            if (currentTime >= duration)
            {
                AudioSourceReachEnd?.Invoke(sender, e);
            }

            PlaybackPositionChanged?.Invoke(sender, e);
        }
    }
}
