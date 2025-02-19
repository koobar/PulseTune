using LibPulseTune.Engine;
using NAudio.Wave;
using System;
using System.Runtime.InteropServices;
using static LibPulseTune.Codecs.WavPack.WavPackInterop;

namespace LibPulseTune.Codecs.WavPack
{
    public class WavPackDecoder : IAudioSource
    {
        // 非公開定数
        private const int WAVPACK_BYTES_PER_SAMPLE = 4;
        private const int OPEN_WVC = 1;
        private const int OPEN_2CH_MAX = 0x08;
        private const int OPEN_NORMALIZE = 0x10;
        private const int SAMPLES_TO_READ = 16;

        // 非公開フィールド
        private readonly IntPtr wavPackContext;
        private readonly WaveFormat waveFormat;
        private readonly int bytesPerSample;
        private readonly IntPtr readBuffer;
        private readonly int readBufferSize;
        private bool isDisposed;

        // コンストラクタ
        public WavPackDecoder(string path)
        {
            LoadLibrary();

            // エラーメッセージを格納する領域を確保
            var error = Marshal.AllocHGlobal(1024);

            // ファイルを開く。
            var context = WavpackOpenFileInput(path, error, OPEN_WVC | OPEN_2CH_MAX | OPEN_NORMALIZE, 0);

            // 無効なコンテキストが返されていればエラーメッセージで例外を投げる。
            if (context == IntPtr.Zero)
            {
                throw new Exception(Marshal.PtrToStringAnsi(error));
            }

            // エラーメッセージ領域を解放
            Marshal.FreeHGlobal(error);

            // フォーマットを取得
            if (WavpackGetMode(context).HasFlag(WavPackMode.Float))
            {
                this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)WavpackGetSampleRate(context), WavpackGetBitsPerSample(context));
            }
            else
            {
                this.waveFormat = new WaveFormat((int)WavpackGetSampleRate(context), WavpackGetBitsPerSample(context), WavpackGetNumChannels(context));
            }
            this.bytesPerSample = this.waveFormat.BitsPerSample >> 3;

            // デコード用バッファを確保
            this.readBufferSize = WAVPACK_BYTES_PER_SAMPLE * SAMPLES_TO_READ * this.waveFormat.Channels;
            this.readBuffer = Marshal.AllocHGlobal(this.readBufferSize);

            // 後始末
            this.wavPackContext = context;
        }

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.waveFormat.SampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.waveFormat.BitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.waveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.waveFormat.Encoding == WaveFormatEncoding.IeeeFloat;
            }
        }

        #endregion

        public static bool IsAvailable()
        {
            return WavPackInterop.IsAvailable();
        }

        /// <summary>
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer">デコード結果出力用バッファ</param>
        /// <param name="offset">デコード結果出力用バッファの書き込み開始オフセット</param>
        /// <param name="count">デコード結果出力用バッファに読み込むデータのバイト数</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            int sampleCount = count / this.bytesPerSample;
            int samplesPerLoop = this.waveFormat.Channels * SAMPLES_TO_READ;

            for (int i = 0; i < sampleCount; i += samplesPerLoop)
            {
                IntPtr pReadBuffer = this.readBuffer;

                // 各チャンネルからサンプルをデコード
                uint read = WavpackUnpackSamples(this.wavPackContext, this.readBuffer, SAMPLES_TO_READ);

                // デコードされたサンプル数が0サンプルであれば読み込み終了。
                if (read == 0)
                {
                    break;
                }

                for (int src = 0; src < this.readBufferSize; src += WAVPACK_BYTES_PER_SAMPLE)
                {
                    // デコード結果出力用バッファのオフセットが境界外なら読み込み終了。
                    if (offset >= buffer.Length)
                    {
                        break;
                    }

                    // 読み込み用バッファからデコード結果出力用バッファにデータを転送
                    Marshal.Copy(pReadBuffer, buffer, offset, bytesPerSample);
                    
                    // 各種更新
                    pReadBuffer += WAVPACK_BYTES_PER_SAMPLE;
                    offset += bytesPerSample;
                    totalBytesRead += bytesPerSample;
                }
            }

            return totalBytesRead;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            WavpackCloseFile(this.wavPackContext);
            Marshal.FreeHGlobal(this.readBuffer);

            this.isDisposed = true;
        }

        public TimeSpan GetCurrentTime()
        {
            return TimeSpan.FromMilliseconds(WavpackGetSampleIndex(this.wavPackContext) / ((this.waveFormat.SampleRate / 1000.0) * this.waveFormat.Channels));
        }

        public TimeSpan GetDuration()
        {
            return TimeSpan.FromMilliseconds(WavpackGetNumSamples(this.wavPackContext) / ((this.waveFormat.SampleRate / 1000.0) * this.waveFormat.Channels));
        }

        public void SetCurrentTime(TimeSpan time)
        {
            var sampleIndex = time.TotalMilliseconds * ((this.waveFormat.SampleRate / 1000.0) * this.waveFormat.Channels);

            WavpackSeekSample(this.wavPackContext, (uint)Math.Round(sampleIndex));
        }
    }
}
