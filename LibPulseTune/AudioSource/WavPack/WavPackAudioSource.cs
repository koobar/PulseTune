using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.AudioSource.WavPack
{
    internal class WavPackAudioSource : IAudioSource
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

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private delegate IntPtr InternalWavpackOpenFileInput(string fileName, IntPtr error, int flags, int norm_offset);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackGetSampleRate(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackGetNumSamples(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int InternalWavpackGetNumChannels(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int InternalWavpackGetBitsPerSample(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint InternalWavpackUnpackSamples(IntPtr wpc, byte[] buffer, uint samples);
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
        private byte[] readBuffer;

        // コンストラクタ
        public WavPackAudioSource(string path)
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

            // 後始末
            this.wavPackContext = context;
            this.readBuffer = null;
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

        private uint WavpackGetSampleRate()
        {
            return this._wavPackGetSampleRate(this.wavPackContext);
        }

        private uint WavpackGetNumSamples()
        {
            return this._wavPackGetNumSamples(this.wavPackContext);
        }

        private int WavpackGetNumChannels()
        {
            return this._wavPackGetNumChannels(this.wavPackContext);
        }

        private int WavpackGetBitsPerSample()
        {
            return this._wavPackGetBitsPerSample(this.wavPackContext);
        }

        private uint WavpackUnpackSamples(byte[] buffer, uint samples)
        {
            return this._wavPackUnpackSamples(this.wavPackContext, buffer, samples);
        }

        private int WavpackSeekSample(uint sample)
        {
            return this._wavPackSeekSample(this.wavPackContext, sample);
        }

        private uint WavpackGetSampleIndex()
        {
            return this._wavPackGetSampleIndex(this.wavPackContext);
        }

        private IntPtr WavpackCloseFile()
        {
            return this._wavPackCloseFile(this.wavPackContext);
        }

        private WavPackMode WavpackGetMode(IntPtr wpc)
        {
            return this._wavPackGetMode(wpc);
        }

        #endregion

        public uint SampleRate
        {
            get
            {
                return WavpackGetSampleRate();
            }
        }

        public uint BitsPerSample
        {
            get
            {
                return (uint)WavpackGetBitsPerSample();
            }
        }

        public uint Channels
        {
            get
            {
                return (uint)WavpackGetNumChannels();
            }
        }

        public bool IsFloat
        {
            get
            {
                return WavpackGetMode(this.wavPackContext).HasFlag(WavPackMode.Float);
            }
        }

        public int Decode(byte[] buffer, int offset, int length)
        {
            if (this.readBuffer == null)
            {
                // バッファがnullなら、各チャンネルのサンプルを1つ格納できるだけのバッファを確保する。
                //
                // WavPackでは、サンプルの量子化ビット数にかかわらず、常に4バイトでデコードされた
                // サンプルが返されるため、必要とされるバッファのバイト数は次の式で計算できる：
                //    4 × 各チャンネルからデコードするサンプル数 × チャンネル数
                //
                // ここでは、各チャンネルから1サンプルだけをデコードすることとする。
                this.readBuffer = new byte[WAVPACK_BYTES_PER_SAMPLE * 1 * this.Channels];
            }

            int totalBytesRead = 0;
            uint bytesPerSample = this.BitsPerSample / 8;
            uint sampleCount = (uint)(length / bytesPerSample);

            for (uint i = 0; i < sampleCount; i += this.Channels)
            {
                // 各チャンネルから1サンプルデコード
                uint read = WavpackUnpackSamples(this.readBuffer, 1);

                // デコードされたサンプル数が0サンプルであるか、または、デコード結果用バッファのオフセットが境界外ならループを抜ける。
                if (read == 0 || offset >= buffer.Length)
                {
                    break;
                }

                switch (bytesPerSample)
                {
                    case 1:
                        {
                            // 各チャンネルのサンプル（4バイトで返されている）から、下位1バイトを取得してデコード結果用バッファに格納
                            for (int src = 0; src < this.readBuffer.Length; src += WAVPACK_BYTES_PER_SAMPLE)
                            {
                                buffer[offset++] = this.readBuffer[src];
                            }
                        }
                        break;
                    case 2:
                        {
                            // 各チャンネルのサンプル（4バイトで返されている）から、下位2バイトを取得してデコード結果用バッファに格納
                            for (int src = 0; src < this.readBuffer.Length; src += WAVPACK_BYTES_PER_SAMPLE)
                            {
                                buffer[offset++] = this.readBuffer[src];
                                buffer[offset++] = this.readBuffer[src + 1];
                            }
                        }
                        break;
                    case 3:
                        {
                            // 各チャンネルのサンプル（4バイトで返されている）から、下位3バイトを取得してデコード結果用バッファに格納
                            for (int src = 0; src < this.readBuffer.Length; src += WAVPACK_BYTES_PER_SAMPLE)
                            {
                                buffer[offset++] = this.readBuffer[src];
                                buffer[offset++] = this.readBuffer[src + 1];
                                buffer[offset++] = this.readBuffer[src + 2];
                            }
                        }
                        break;
                    case 4:
                        {
                            // 各チャンネルのサンプルを取得してデコード結果用バッファに格納
                            // 返されたサンプルのバイト数と求められるサンプルのバイト数が一致するため、そのままコピーでOK
                            for (int src = 0; src < this.readBuffer.Length; src += WAVPACK_BYTES_PER_SAMPLE)
                            {
                                buffer[offset++] = this.readBuffer[src];
                                buffer[offset++] = this.readBuffer[src + 1];
                                buffer[offset++] = this.readBuffer[src + 2];
                                buffer[offset++] = this.readBuffer[src + 3];
                            }
                        }
                        break;
                }

                totalBytesRead += (int)(bytesPerSample * this.Channels);
            }

            return totalBytesRead;
        }

        public void Dispose()
        {
            WavpackCloseFile();
            WinApi.FreeLibrary(this.wavPackModule);
        }

        public TimeSpan GetCurrentTime()
        {
            var sampleIndex = WavpackGetSampleIndex();

            return TimeSpan.FromSeconds(sampleIndex / (this.SampleRate * this.Channels));
        }

        public TimeSpan GetDuration()
        {
            return TimeSpan.FromSeconds(WavpackGetNumSamples() / (this.SampleRate * this.Channels));
        }

        public void SetCurrentTime(TimeSpan time)
        {
            var sampleIndex = time.TotalSeconds * (this.SampleRate * this.Channels);

            WavpackSeekSample((uint)sampleIndex);
        }
    }
}
