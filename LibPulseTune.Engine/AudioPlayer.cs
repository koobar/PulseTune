using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace LibPulseTune.Engine
{
    public static class AudioPlayer
    {
        // 公開定数
        public const int AUDIOPLAYER_NOT_READY = 0;             // オーディオデバイスの準備ができていない
        public const int AUDIOPLAYER_STATE_STOP = 1;            // デバイスと音声ソースが指定されているが再生していない
        public const int AUDIOPLAYER_STATE_PLAY = 2;            // 再生中
        public const int AUDIOPLAYER_STATE_PAUSE = 3;           // 一時停止中

        // 非公開フィールド
        private static IAudioOutputDevice outputDevice;
        private static IAudioPlayer currentDeviceInstance;
        private static IAudioSource currentAudioSource;
        private static IAudioSourceToIWaveProviderConverter currentAudioSourceConverter;
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
        public static void SetOutputDevice(IAudioOutputDevice device)
        {
            outputDevice = device;
        }

        /// <summary>
        /// オーディオ出力デバイスを取得する。
        /// </summary>
        /// <returns></returns>
        public static IAudioOutputDevice GetOutputDevice()
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

        /// <summary>
        /// ISampleProviderを、元のフォーマットを維持したままIWaveProviderに変換する。
        /// </summary>
        /// <param name="sampleProvider"></param>
        /// <param name="originalWaveformat"></param>
        /// <returns></returns>
        private static IWaveProvider ConvertToWaveProvider(ISampleProvider sampleProvider, WaveFormat originalWaveformat)
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

                currentAudioSourceConverter = new IAudioSourceToIWaveProviderConverter(currentAudioSource);

                // 波形モニタを生成
                waveformMonitor = new WaveformMonitor(currentAudioSourceConverter);

                // 音量コントローラを作成
                volumeController = new VolumeSampleProvider(waveformMonitor);
                volumeController.Volume = volume / 100.0f;

                // デバイスを初期化
                currentDeviceInstance = outputDevice.CreateDeviceInstance();
                currentDeviceInstance.Init(ConvertToWaveProvider(volumeController, currentAudioSourceConverter.WaveFormat));

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
        /// 再生または一時停止解除
        /// </summary>
        public static void Play()
        {
            if (currentDeviceInstance == null)
            {
                return;
            }

            if (currentAudioSource == null)
            {
                return;
            }

            if (currentAudioSourceConverter == null)
            {
                return;
            }

            currentDeviceInstance.Play();
            timer.Start();
            currentAudioSourceConverter.IsPaused = false;
            SetAudioPlayerState(AUDIOPLAYER_STATE_PLAY);
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
                    timer.Stop();
                    if (outputDevice.IsPauseSupported)
                    {
                        currentAudioSourceConverter.IsPaused = true;
                    }
                    else
                    {
                        currentDeviceInstance.Pause();
                    }
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
                    currentAudioSourceConverter.IsPaused = false;
                    PlaybackPositionChanged?.Invoke(null, EventArgs.Empty);
                    SetAudioPlayerState(AUDIOPLAYER_STATE_STOP);
                    break;
            }
        }

        /// <summary>
        /// 再生中のオーディオソースのフォーマットを取得する。
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="bitsPerSample"></param>
        /// <param name="channels"></param>
        /// <param name="isFloat"></param>
        public static void GetWaveFormat(out int sampleRate, out int bitsPerSample, out int channels, out bool isFloat)
        {
            if (currentAudioSource == null)
            {
                sampleRate = bitsPerSample = channels = 0;
                isFloat = false;
                return;
            }

            sampleRate = currentAudioSource.SampleRate;
            bitsPerSample = currentAudioSource.BitsPerSample;
            channels = currentAudioSource.Channels;
            isFloat = currentAudioSource.IsFloat;
        }

        /// <summary>
        /// 再生中のオーディオソースの演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetDuration()
        {
            return currentAudioSource.GetDuration();
        }

        /// <summary>
        /// 再生中のオーディオソースの再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetCurrentTime()
        {
            return currentAudioSource.GetCurrentTime();
        }

        /// <summary>
        /// 再生中のオーディオソースの再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public static void SetCurrentTime(TimeSpan time)
        {
            currentAudioSource.SetCurrentTime(time);
        }

        /// <summary>
        /// 波形モニタリング用のデータを再生位置と同期する。
        /// </summary>
        public static void SyncWaveformMonitor()
        {
            waveformMonitor.LoadData(GetTimerInterval());
        }

        /// <summary>
        /// 指定されたチャンネルの振幅を取得する。
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static float GetAmplitude(int channel)
        {
            return waveformMonitor.GetMaximumInstantaneousDecibels(channel);
        }

        /// <summary>
        /// 指定されたチャンネルの波形を取得する。
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static float[] GetWaveform(int channel)
        {
            return waveformMonitor.GetWaveform(channel);
        }

        public static uint GetOutputSampleRate()
        {
            if (currentAudioSource == null)
            {
                return 0;
            }

            return (uint)currentAudioSource.SampleRate;
        }

        public static uint GetOutputBitsPerSample()
        {
            if (currentAudioSource == null)
            {
                return 0;
            }

            return (uint)currentAudioSource.BitsPerSample;
        }

        public static uint GetOutputChannels()
        {
            if (currentAudioSource == null)
            {
                return 0;
            }

            return (uint)currentAudioSource.Channels;
        }

        public static bool GetOutputIsFloat()
        {
            if (currentAudioSource == null)
            {
                return false;
            }

            return currentAudioSource.IsFloat;
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
