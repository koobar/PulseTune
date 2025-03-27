using LibPulseTune.Engine;
using NAudio.CoreAudioApi;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace LibPulseTune.CoreAudio
{
    /// <summary>
    /// WASAPI出力（koobarによるカスタマイズ版）
    /// </summary>
    public class CustomWasapiOut : IAudioPlayer
    {
        // 非公開フィールド
        private readonly bool isUsingEventSync;
        private readonly MMDevice mmDevice;
        private readonly AudioClientShareMode shareMode;
        private readonly SynchronizationContext syncContext;
        private AudioClient audioClient;
        private AudioRenderClient renderClient;
        private IWaveProvider sourceProvider;
        private int latencyMilliseconds;
        private int bufferFrameCount;
        private int bytesPerFrame;
        private EventWaitHandle frameEventWaitHandle;
        private byte[] readBuffer;
        private volatile PlaybackState playbackState;
        private Thread playbackThread;
        private bool resamplerNeeded;

        // イベント
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        #region コンストラクタ

        public CustomWasapiOut() :
            this(GetDefaultAudioEndpoint(), AudioClientShareMode.Shared, true, 200)
        {

        }

        public CustomWasapiOut(AudioClientShareMode shareMode, int latency) :
            this(GetDefaultAudioEndpoint(), shareMode, true, latency)
        {

        }

        public CustomWasapiOut(AudioClientShareMode shareMode, bool useEventSync, int latency) :
            this(GetDefaultAudioEndpoint(), shareMode, useEventSync, latency)
        {

        }

        public CustomWasapiOut(MMDevice device, AudioClientShareMode shareMode, bool useEventSync, int latency)
        {
            this.audioClient = device.AudioClient;
            this.mmDevice = device;
            this.shareMode = shareMode;
            this.isUsingEventSync = useEventSync;
            this.latencyMilliseconds = latency;
            this.syncContext = SynchronizationContext.Current;
            this.OutputWaveFormat = this.audioClient.MixFormat;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 再生状況
        /// </summary>
        public PlaybackState PlaybackState
        {
            get
            {
                return this.playbackState;
            }
        }

        /// <summary>
        /// 自動変換時のリサンプラーの品質(1～60)
        /// </summary>
        public int AutoResamplerQuality { set; get; } = 60;

        /// <summary>
        /// 再生スレッドの優先度
        /// </summary>
        public AvThreadPriority PlaybackAvThreadPriority { set; get; } = AvThreadPriority.Normal;

        /// <summary>
        /// 再生スレッドの特徴
        /// </summary>
        public MmThreadCharacteristics MmThreadCharacteristics { set; get; } = MmThreadCharacteristics.Audio;

        /// <summary>
        /// MMCSSを使用するかどうか
        /// </summary>
        public bool EnableMMCSS { set; get; }

        /// <summary>
        /// ハードウェアの出力フォーマット
        /// </summary>
        public WaveFormat OutputWaveFormat { get; private set; }

        #endregion

        /// <summary>
        /// デフォルトのオーディオエンドポイントを取得します。
        /// </summary>
        /// <returns></returns>
        private static MMDevice GetDefaultAudioEndpoint()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("WASAPI supported only on Windows Vista and above");
            }

            var enumerator = new MMDeviceEnumerator();
            return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        /// <summary>
        /// MmThreadCharacteristicsの文字列を取得する。
        /// </summary>
        /// <param name="characteristics"></param>
        /// <returns></returns>
        static string GetMmThreadCharacteristicsString(MmThreadCharacteristics characteristics)
        {
            switch (characteristics)
            {
                case MmThreadCharacteristics.Audio:
                    return "Audio";
                case MmThreadCharacteristics.Capture:
                    return "Capture";
                case MmThreadCharacteristics.DisplayPostProcess:
                    return "DisplayPostProcess";
                case MmThreadCharacteristics.Distribution:
                    return "Distribution";
                case MmThreadCharacteristics.Games:
                    return "Games";
                case MmThreadCharacteristics.Playback:
                    return "Playback";
                case MmThreadCharacteristics.ProAudio:
                    return "Pro Audio";
                case MmThreadCharacteristics.WindowManager:
                    return "Window Manager";
                default:
                    return "Audio";
            }
        }

        /// <summary>
        /// 再生スレッドでの処理
        /// </summary>
        private void PlayThread()
        {
            MediaFoundationResampler resampler = null;
            IWaveProvider playbackProvider = this.sourceProvider;
            Exception exception = null;
            IntPtr hAvrt = IntPtr.Zero;
            int taskIndex = 0;

            try
            {
                // リサンプラーが必要なら、再生するオーディオプロバイダにMediaFoudnationResamplerをかませる。
                if (this.resamplerNeeded)
                {
                    resampler = new MediaFoundationResampler(this.sourceProvider, this.OutputWaveFormat);
                    resampler.ResamplerQuality = this.AutoResamplerQuality;
                    playbackProvider = resampler;
                }

                // バッファに初回分のデータを格納する。
                this.bufferFrameCount = this.audioClient.BufferSize;
                this.bytesPerFrame = this.OutputWaveFormat.Channels * (this.OutputWaveFormat.BitsPerSample >> 3);
                this.readBuffer = BufferHelpers.Ensure(this.readBuffer, this.bufferFrameCount * this.bytesPerFrame);
                if (FillBuffer(playbackProvider, this.bufferFrameCount))
                {
                    // 初回バッファ格納の時点ですでにデータの終わりに達していれば再生せずに戻る。
                    return;
                }

                // 同期のためのWaitHandleを作成
                var waitHandles = new WaitHandle[] { this.frameEventWaitHandle };

                // AudioClientの駆動を開始
                this.audioClient.Start();

                if (this.EnableMMCSS)
                {
                    DwmapiInterop.DwmEnableMMCSS(this.EnableMMCSS);

                    // Av*関数群を使用する設定を反映
                    hAvrt = AvrtInterop.AvSetMmThreadCharacteristics(GetMmThreadCharacteristicsString(this.MmThreadCharacteristics), ref taskIndex);
                    if (hAvrt != IntPtr.Zero)
                    {
                        if (!AvrtInterop.AvSetMmThreadPriority(hAvrt, (int)this.PlaybackAvThreadPriority))
                        {
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                        }
                    }
                    else
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }
                }

                while (this.playbackState != PlaybackState.Stopped)
                {
                    if (this.isUsingEventSync)
                    {
                        // イベント同期駆動の場合、AudioClientからの通知があるまでスレッドを待機させる。
                        WaitHandle.WaitAny(waitHandles, 3 * this.latencyMilliseconds, false);
                    }
                    else
                    {
                        // レイテンシの半分の時間だけ待機する。
                        Thread.Sleep(this.latencyMilliseconds / 2);
                    }

                    // まだ再生中か？
                    if (this.playbackState == PlaybackState.Playing)
                    {
                        int numFramesPadding;

                        if (this.shareMode == AudioClientShareMode.Shared || !this.isUsingEventSync)
                        {
                            // 共有モードまたはプッシュ駆動の場合はパディングが必要
                            numFramesPadding = this.audioClient.CurrentPadding;
                        }
                        else
                        {
                            // イベント駆動の排他モードでは、常にバッファの全体にアクセスされるため、
                            // パディングは必要ない。（してはいけない）
                            numFramesPadding = 0;
                        }

                        int numFramesAvailable = this.bufferFrameCount - numFramesPadding;
                        if (numFramesAvailable > 10)        // 別に numFramesAvailable > 0 でもよい
                        {
                            if (FillBuffer(playbackProvider, numFramesAvailable))
                            {
                                // データの末端まで読み込んでいれば再生ループを抜ける。
                                break;
                            }
                        }
                    }
                }

                // まだ再生中か？
                if (this.playbackState == PlaybackState.Playing)
                {
                    // イベント駆動なら出力レイテンシと同じだけ、イベント駆動でなければ出力レイテンシの半分の時間だけスレッドを中断する。
                    Thread.Sleep(this.isUsingEventSync ? this.latencyMilliseconds : this.latencyMilliseconds / 2);
                }

                if (hAvrt != IntPtr.Zero)
                {
                    // スレッドの設定を元に戻す。
                    AvrtInterop.AvRevertMmThreadCharacteristics(hAvrt);
                    hAvrt = IntPtr.Zero;
                }

                // AudioClientの駆動を停止する。
                this.audioClient.Stop();
                
                // 後始末
                this.playbackState = PlaybackState.Stopped;
                this.audioClient.Reset();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if (resampler != null)
                {
                    resampler.Dispose();
                }

                // 停止時イベントを発生させる。
                RaisePlaybackStopped(exception);
            }
        }

        /// <summary>
        /// 停止イベントを発生させる。
        /// </summary>
        /// <param name="e"></param>
        private void RaisePlaybackStopped(Exception e)
        {
            var handler = this.PlaybackStopped;
            if (handler != null)
            {
                if (this.syncContext == null)
                {
                    handler(this, new StoppedEventArgs(e));
                }
                else
                {
                    this.syncContext.Post(state => handler(this, new StoppedEventArgs(e)), null);
                }
            }
        }

        /// <summary>
        /// デバイスのバッファにデータを満たす。データの末端まで読み込まれた場合は真を、そうでなければ偽を返す。
        /// </summary>
        /// <param name="playbackProvider"></param>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        private bool FillBuffer(IWaveProvider playbackProvider, int frameCount)
        {
            var readLength = frameCount * this.bytesPerFrame;
            int read = playbackProvider.Read(this.readBuffer, 0, readLength);
            if (read == 0)
            {
                return true;
            }

            // デバイスのバッファにデータをコピー
            var buffer = this.renderClient.GetBuffer(frameCount);
            Marshal.Copy(this.readBuffer, 0, buffer, read);

            if (this.isUsingEventSync && this.shareMode == AudioClientShareMode.Exclusive)
            {
                if (read < readLength)
                {
                    // 排他モードの場合、バッファの未使用領域をゼロ埋めしないと、未使用領域がノイズとなって再生されてしまう。
                    unsafe
                    {
                        byte* pByte = (byte*)buffer;
                        while (read < readLength)
                        {
                            pByte[read++] = 0;
                        }
                    }
                }

                this.renderClient.ReleaseBuffer(frameCount, AudioClientBufferFlags.None);
            }
            else
            {
                int actualFrameCount = read / this.bytesPerFrame;
                this.renderClient.ReleaseBuffer(actualFrameCount, AudioClientBufferFlags.None);
            }

            return false;
        }

        /// <summary>
        /// デバイスが対応していないデータ形式を再生する際など、あえて品質を下げる必要がある場合には、
        /// 品質は低いものの、再生可能なフォーマットを選択する必要がある。このとき、デバイスが対応できる最大限のフォーマットを取得する。
        /// </summary>
        /// <returns></returns>
        private WaveFormat GetFallbackFormat()
        {
            var deviceSampleRate = this.audioClient.MixFormat.SampleRate;
            var deviceChannels = this.audioClient.MixFormat.Channels;
            var sampleRatesToTry = new List<int>();

            // 再生するオーディオデータと同じサンプリング周波数を最優先として追加する。
            sampleRatesToTry.Add(this.OutputWaveFormat.SampleRate);

            // 次に、出力デバイスにおけるサンプリング周波数を2番目に優先する値として追加する。
            if (!sampleRatesToTry.Contains(deviceSampleRate))
            {
                sampleRatesToTry.Add(deviceSampleRate);
            }

            // 次に、よく使われるサンプリング周波数である、44.1Khzと48Khzを、3、4番目に優先する値として追加する。
            if (!sampleRatesToTry.Contains(44100))
            {
                sampleRatesToTry.Add(44100);
            }
            if (!sampleRatesToTry.Contains(48000))
            {
                sampleRatesToTry.Add(48000);
            }

            var channelCountsToTry = new List<int>() { this.OutputWaveFormat.Channels };
            if (!channelCountsToTry.Contains(deviceChannels)) channelCountsToTry.Add(deviceChannels);
            if (!channelCountsToTry.Contains(2)) channelCountsToTry.Add(2);

            var bitDepthsToTry = new List<int>() { this.OutputWaveFormat.BitsPerSample };
            if (!bitDepthsToTry.Contains(32)) bitDepthsToTry.Add(32);
            if (!bitDepthsToTry.Contains(24)) bitDepthsToTry.Add(24);
            if (!bitDepthsToTry.Contains(16)) bitDepthsToTry.Add(16);

            // 総当たりでデバイスがサポートするフォーマットを探索し、一番最初に見つかったフォーマットを返す。
            foreach (var sampleRate in sampleRatesToTry)
            {
                foreach (var channelCount in channelCountsToTry)
                {
                    foreach (var bitDepth in bitDepthsToTry)
                    {
                        var format = new WaveFormatExtensible(sampleRate, bitDepth, channelCount);
                        if (this.audioClient.IsFormatSupported(this.shareMode, format))
                        {
                            return format;
                        }
                    }
                }
            }

            throw new NotSupportedException("オーディオ出力デバイスで使用可能なフォーマットが見つかりません。");
        }

        /// <summary>
        /// 出力フォーマットとデバイスの間に互換性があるか確認し、互換性があれば真を、互換性がなせれば偽を返す。
        /// </summary>
        private bool CheckWaveFormat(out WaveFormat closestWaveFormat)
        {
            if (!this.audioClient.IsFormatSupported(this.shareMode, this.OutputWaveFormat, out var closestFormat))
            {
                if (closestFormat == null)
                {
                    closestWaveFormat = GetFallbackFormat();
                }
                else
                {
                    closestWaveFormat = closestFormat;
                }

                return false;
            }
            else
            {
                closestWaveFormat = null;
                return true;
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play()
        {
            if (this.playbackState != PlaybackState.Playing)
            {
                if (this.playbackState == PlaybackState.Stopped)
                {
                    this.playbackThread = new Thread(PlayThread)
                    {
                        IsBackground = true,
                    };
                    this.playbackState = PlaybackState.Playing;
                    this.playbackThread.Start();
                }
                else
                {
                    this.playbackState = PlaybackState.Playing;
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (this.playbackState != PlaybackState.Stopped)
            {
                this.playbackState = PlaybackState.Stopped;
                this.playbackThread.Join();
                this.playbackThread = null;
            }
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        public void Pause()
        {
            if (this.playbackState == PlaybackState.Playing)
            {
                this.playbackState = PlaybackState.Paused;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="waveProvider">IWaveProvider to play</param>
        public void Init(IWaveProvider waveProvider)
        {
            long latencyRefTimes = this.latencyMilliseconds * 10000L;
            var flags = AudioClientStreamFlags.None; 

            this.sourceProvider = waveProvider;
            this.OutputWaveFormat = waveProvider.WaveFormat;

            // 出力フォーマットの互換性チェック
            this.resamplerNeeded = !CheckWaveFormat(out var closestWaveFormat);
            if (this.resamplerNeeded)
            {
                this.OutputWaveFormat = closestWaveFormat;
            }
            
            if (this.isUsingEventSync)
            {
                if (this.shareMode == AudioClientShareMode.Shared)
                {
                    this.audioClient.Initialize(this.shareMode, AudioClientStreamFlags.EventCallback | flags, latencyRefTimes, 0, this.OutputWaveFormat, Guid.Empty);

                    var streamLatency = this.audioClient.StreamLatency;
                    if (streamLatency != 0)
                    {
                        // 実効レイテンシを取得する。
                        this.latencyMilliseconds = (int)(streamLatency / 10000);
                    }
                }
                else
                {
                    try
                    {
                        this.audioClient.Initialize(this.shareMode, AudioClientStreamFlags.EventCallback | flags, latencyRefTimes, latencyRefTimes, this.OutputWaveFormat, Guid.Empty);
                    }
                    catch
                    {
                        long newLatencyRefTimes = (long)(10000000.0 / this.OutputWaveFormat.SampleRate * this.audioClient.BufferSize + 0.5);

                        this.audioClient.Dispose();
                        this.audioClient = this.mmDevice.AudioClient;
                        this.audioClient.Initialize(this.shareMode, AudioClientStreamFlags.EventCallback | flags, newLatencyRefTimes, newLatencyRefTimes, this.OutputWaveFormat, Guid.Empty);
                    }
                }

                // 待機イベントハンドルを生成して設定する。
                this.frameEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                this.audioClient.SetEventHandle(this.frameEventWaitHandle.SafeWaitHandle.DangerousGetHandle());
            }
            else
            {
                this.audioClient.Initialize(this.shareMode, flags, latencyRefTimes, 0, this.OutputWaveFormat, Guid.Empty);
            }

            // AudioRenderClientを取得
            this.renderClient = this.audioClient.AudioRenderClient;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (this.audioClient != null)
            {
                Stop();

                this.audioClient.Dispose();
                this.audioClient = null;
                this.renderClient = null;
            }
        }
    }
}
