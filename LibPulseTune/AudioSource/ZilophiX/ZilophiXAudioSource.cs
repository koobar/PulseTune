using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using LibPulseTune.Helpers;

namespace LibPulseTune.AudioSource.ZilophiX
{
    internal class ZilophiXAudioSource : IAudioSource
    {
        // デリゲート
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)] private delegate IntPtr DCreateDecoder(string path);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void DZpXFreeDecoder(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void DZpXCloseFile(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetSampleRate(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetChannels(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetBitsPerSample(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetNumTotalSamples(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int DZpXReadSample(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetSampleOffset(IntPtr pDecoder);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void DZpXSeekTo(IntPtr pDecoder, uint msec);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DZpXGetDurationMsec(IntPtr pDecoder);

        // 非公開フィールド
        private static readonly string ZilophiXDecoderDllPath = $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\zilophixdec.dll";
        private readonly IntPtr pDll;
        private readonly DCreateDecoder _ZpXCreateDecoder;
        private readonly DZpXFreeDecoder _ZpXFreeDecoder;
        private readonly DZpXCloseFile _ZpXCloseFile;
        private readonly DZpXGetSampleRate _ZpXGetSampleRate;
        private readonly DZpXGetChannels _ZpXGetChannels;
        private readonly DZpXGetBitsPerSample _ZpXGetBitsPerSample;
        private readonly DZpXGetNumTotalSamples _ZpXGetNumTotalSamples;
        private readonly DZpXReadSample _ZpXReadSample;
        private readonly DZpXGetSampleOffset _ZpXGetSampleOffset;
        private readonly DZpXSeekTo _ZpXSeekTo;
        private readonly DZpXGetDurationMsec _ZpXGetDurationMsec;
        private readonly IntPtr decoder;
        private readonly WaveFormat waveFormat;
        private readonly uint numTotalSamples;
        private readonly int bytesPerSample;
        private bool isSeeking;
        private bool isDisposed;

        // コンストラクタ
        public ZilophiXAudioSource(string path)
        {
            this.pDll = WinApi.LoadLibrary(ZilophiXDecoderDllPath);
            if (this.pDll == IntPtr.Zero)
            {
                throw new InvalidProgramException("zilophixdec.dll が見つかりません");
            }

            this._ZpXCreateDecoder = WinApiHelper.LoadFunction<DCreateDecoder>(this.pDll, "ZpXCreateDecoderW");
            this._ZpXFreeDecoder = WinApiHelper.LoadFunction<DZpXFreeDecoder>(this.pDll, "ZpXFreeDecoder");
            this._ZpXCloseFile = WinApiHelper.LoadFunction<DZpXCloseFile>(this.pDll, "ZpXCloseFile");
            this._ZpXGetSampleRate = WinApiHelper.LoadFunction<DZpXGetSampleRate>(this.pDll, "ZpXGetSampleRate");
            this._ZpXGetChannels = WinApiHelper.LoadFunction<DZpXGetChannels>(this.pDll, "ZpXGetChannels");
            this._ZpXGetBitsPerSample = WinApiHelper.LoadFunction<DZpXGetBitsPerSample>(this.pDll, "ZpXGetBitsPerSample");
            this._ZpXGetNumTotalSamples = WinApiHelper.LoadFunction<DZpXGetNumTotalSamples>(this.pDll, "ZpXGetNumTotalSamples");
            this._ZpXReadSample = WinApiHelper.LoadFunction<DZpXReadSample>(this.pDll, "ZpXReadSample");
            this._ZpXGetSampleOffset = WinApiHelper.LoadFunction<DZpXGetSampleOffset>(this.pDll, "ZpXGetSampleOffset");
            this._ZpXSeekTo = WinApiHelper.LoadFunction<DZpXSeekTo>(this.pDll, "ZpXSeekTo");
            this._ZpXGetDurationMsec = WinApiHelper.LoadFunction<DZpXGetDurationMsec>(this.pDll, "ZpXGetDurationMsec");

            this.decoder = ZpXCreateDecoder(path);
            this.numTotalSamples = ZpXGetNumTotalSamples(this.decoder);
            this.waveFormat = new WaveFormat((int)ZpXGetSampleRate(this.decoder), (int)ZpXGetBitsPerSample(this.decoder), (int)ZpXGetChannels(this.decoder));
            this.bytesPerSample = this.waveFormat.BitsPerSample >> 3;
            this.isSeeking = false;
        }

        /// <summary>
        /// フォーマット
        /// </summary>
        public WaveFormat WaveFormat
        {
            get
            {
                return this.waveFormat;
            }
        }

        private double SamplesPerMilliseconds
        {
            get
            {
                return (this.waveFormat.SampleRate * this.waveFormat.Channels) / 1000.0;
            }
        }

        #region ラッパー関数

        public IntPtr ZpXCreateDecoder(string path)
        {
            return this._ZpXCreateDecoder(path);
        }

        public void ZpXFreeDecoder(IntPtr pDecoder)
        {
            this._ZpXFreeDecoder(pDecoder);
        }

        public void ZpXCloseFile(IntPtr pDecoder)
        {
            this._ZpXCloseFile(pDecoder);
        }

        public uint ZpXGetSampleRate(IntPtr pDecoder)
        {
            return this._ZpXGetSampleRate(pDecoder);
        }

        public uint ZpXGetChannels(IntPtr pDecoder)
        {
            return this._ZpXGetChannels(pDecoder);
        }

        public uint ZpXGetBitsPerSample(IntPtr pDecoder)
        {
            return this._ZpXGetBitsPerSample(pDecoder);
        }

        public uint ZpXGetNumTotalSamples(IntPtr pDecoder)
        {
            return this._ZpXGetNumTotalSamples(pDecoder);
        }

        public int ZpXReadSample(IntPtr pDecoder)
        {
            return this._ZpXReadSample(pDecoder);
        }

        public uint ZpXGetSampleOffset(IntPtr pDecoder)
        {
            return this._ZpXGetSampleOffset(pDecoder);
        }

        public void ZpXSeekTo(IntPtr pDecoder, uint msec)
        {
            this._ZpXSeekTo(pDecoder, msec);
        }

        public uint ZpXGetDurationMsec(IntPtr pDecoder)
        {
            return this._ZpXGetDurationMsec(pDecoder);
        }

        #endregion

        /// <summary>
        /// ZilophiXのデコーダが使用可能であるかどうか判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            if (File.Exists(ZilophiXDecoderDllPath))
            {
                return true;
            }

            return false;
        }

        public TimeSpan GetDuration()
        {
            var result = TimeSpan.FromMilliseconds(ZpXGetNumTotalSamples(this.decoder) / this.SamplesPerMilliseconds);

            return result;
        }

        public TimeSpan GetCurrentTime()
        {
            var pos = ZpXGetSampleOffset(this.decoder);
            var result = TimeSpan.FromMilliseconds(pos / this.SamplesPerMilliseconds);

            return result;
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.isSeeking = true;

            ZpXSeekTo(this.decoder, (uint)time.TotalMilliseconds);

            this.isSeeking = false;
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

            ZpXCloseFile(this.decoder);
            ZpXFreeDecoder(this.decoder);
            WinApi.FreeLibrary(this.pDll);
            this.isDisposed = true;
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
            // シーク中にデータの読み込みを行うと、保護されたメモリ領域からPCMデータを
            // 読み込もうとして例外が発生するため、シーク中は無音データを返すこととする。
            if (this.isSeeking)
            {
                // 無音データを格納
                for (int i = offset; i < buffer.Length; ++i)
                {
                    buffer[i] = 0;
                }

                return count;
            }

            int result = offset;

            while (result < count)
            {
                if (ZpXGetSampleOffset(this.decoder) >= this.numTotalSamples)
                {
                    break;
                }

                int sample = ZpXReadSample(this.decoder);

                unsafe
                {
                    fixed(void* pBuffer = &buffer[offset])
                    {
                        Buffer.MemoryCopy(&sample, pBuffer, buffer.Length, this.bytesPerSample);
                    }
                }

                // 後始末
                offset += this.bytesPerSample;
                result += this.bytesPerSample;
            }

            return result;
        }
    }
}
