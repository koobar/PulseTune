using LibPulseTune.Engine;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.Codecs.WavPack
{
    public class WavPackDecoder : IAudioSource
    {
        enum WavPackMode
        {
            Wvc = 0x1,
            Lossless = 0x2,
            Hybrid = 0x4,
            Float = 0x8,
            ValidTag = 0x10,
            High = 0x20,
            Fast = 0x40,
            Extra = 0x80,
            ApeTag = 0x100,
            Sfx = 0x200,
            VeryHigh = 0x400,
            Md5 = 0x800,
            XMode = 0x7000,
            Dns = 0x8000,
        }

        // 非公開定数
        private const string WAVPACK_DLL_NAME = @"wavpackdll.dll";
        private const int WAVPACK_BYTES_PER_SAMPLE = 4;
        private const int OPEN_WVC = 1;
        private const int OPEN_2CH_MAX = 0x08;
        private const int OPEN_NORMALIZE = 0x10;
        private const int SAMPLES_TO_READ = 16;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private delegate IntPtr InternalWavpackOpenFileInput(string fileName, IntPtr error, int flags, int norm_offset);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackGetSampleRate(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackGetNumSamples(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int InternalWavpackGetNumChannels(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int InternalWavpackGetBitsPerSample(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackUnpackSamples(IntPtr wpc, IntPtr buffer, uint samples);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int InternalWavpackSeekSample(IntPtr wpc, uint sample);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackGetSampleIndex(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr InternalWavpackCloseFile(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate WavPackMode InternalWavpackGetMode(IntPtr wpc);

        // 非公開フィールド
        private static readonly string WavPackDllPath = $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\{WAVPACK_DLL_NAME}";
        private readonly IntPtr wavPackModule;
        private readonly InternalWavpackOpenFileInput _wavPackOpenFileInput;
        private readonly InternalWavpackGetSampleRate _wavPackGetSampleRate;
        private readonly InternalWavpackGetNumSamples _wavPackGetNumSamples;
        private readonly InternalWavpackGetNumChannels _wavPackGetNumChannels;
        private readonly InternalWavpackGetBitsPerSample _wavPackGetBitsPerSample;
        private readonly InternalWavpackUnpackSamples _wavPackUnpackSamples;
        private readonly InternalWavpackSeekSample _wavPackSeekSample;
        private readonly InternalWavpackGetSampleIndex _wavPackGetSampleIndex;
        private readonly InternalWavpackCloseFile _wavPackCloseFile;
        private readonly InternalWavpackGetMode _wavPackGetMode;
        private readonly IntPtr wavPackContext;
        private readonly WaveFormat waveFormat;
        private readonly int bytesPerSample;
        private readonly IntPtr readBuffer;
        private readonly int readBufferSize;
        private bool isDisposed;

        // コンストラクタ
        public WavPackDecoder(string path)
        {
            if (!IsAvailable())
            {
                throw new DllNotFoundException("wavpack.dll が見つかりません。");
            }

            // WavPackのDLLを読み込む。
            this.wavPackModule = WinApi.LoadLibrary(WavPackDllPath);
            if (this.wavPackModule == IntPtr.Zero)
            {
                throw new InvalidProgramException($"{WAVPACK_DLL_NAME} が見つかりません。");
            }

            // DLLから関数を取得し、delegateに変換したものを保持する。
            this._wavPackOpenFileInput = WinApiHelper.LoadFunction<InternalWavpackOpenFileInput>(this.wavPackModule, "WavpackOpenFileInput");
            this._wavPackGetSampleRate = WinApiHelper.LoadFunction<InternalWavpackGetSampleRate>(this.wavPackModule, "WavpackGetSampleRate");
            this._wavPackGetNumSamples = WinApiHelper.LoadFunction<InternalWavpackGetNumSamples>(this.wavPackModule, "WavpackGetNumSamples");
            this._wavPackGetNumChannels = WinApiHelper.LoadFunction<InternalWavpackGetNumChannels>(this.wavPackModule, "WavpackGetNumChannels");
            this._wavPackGetBitsPerSample = WinApiHelper.LoadFunction<InternalWavpackGetBitsPerSample>(this.wavPackModule, "WavpackGetBitsPerSample");
            this._wavPackUnpackSamples = WinApiHelper.LoadFunction<InternalWavpackUnpackSamples>(this.wavPackModule, "WavpackUnpackSamples");
            this._wavPackSeekSample = WinApiHelper.LoadFunction<InternalWavpackSeekSample>(this.wavPackModule, "WavpackSeekSample");
            this._wavPackGetSampleIndex = WinApiHelper.LoadFunction<InternalWavpackGetSampleIndex>(this.wavPackModule, "WavpackGetSampleIndex");
            this._wavPackCloseFile = WinApiHelper.LoadFunction<InternalWavpackCloseFile>(this.wavPackModule, "WavpackCloseFile");
            this._wavPackGetMode = WinApiHelper.LoadFunction<InternalWavpackGetMode>(this.wavPackModule, "WavpackGetMode");

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

        /// <summary>
        /// WavPackのDLLが存在し、WavPackのデコードが可能であるかどうかを確認する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            if (File.Exists(WavPackDllPath))
            {
                return true;
            }

            return false;
        }

        #region ラッパー関数

        private IntPtr WavpackOpenFileInput(string fileName, IntPtr error, int flags, int norm_offset)
        {
            return this._wavPackOpenFileInput(fileName, error, flags, norm_offset);
        }

        private uint WavpackGetSampleRate(IntPtr context)
        {
            return this._wavPackGetSampleRate(context);
        }

        private uint WavpackGetNumSamples(IntPtr context)
        {
            return this._wavPackGetNumSamples(context);
        }

        private int WavpackGetNumChannels(IntPtr context)
        {
            return this._wavPackGetNumChannels(context);
        }

        private int WavpackGetBitsPerSample(IntPtr context)
        {
            return this._wavPackGetBitsPerSample(context);
        }

        private uint WavpackUnpackSamples(IntPtr context, IntPtr buffer, uint samples)
        {
            return this._wavPackUnpackSamples(context, buffer, samples);
        }

        private int WavpackSeekSample(IntPtr context, uint sample)
        {
            return this._wavPackSeekSample(context, sample);
        }

        private uint WavpackGetSampleIndex(IntPtr context)
        {
            return this._wavPackGetSampleIndex(context);
        }

        private IntPtr WavpackCloseFile(IntPtr context)
        {
            return this._wavPackCloseFile(context);
        }

        private WavPackMode WavpackGetMode(IntPtr wpc)
        {
            return this._wavPackGetMode(wpc);
        }

        #endregion

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
            WinApi.FreeLibrary(this.wavPackModule);
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
