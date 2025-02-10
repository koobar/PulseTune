using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

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
        private readonly uint numTotalSamples;
        private readonly uint sampleRate;
        private readonly uint bitsPerSample;
        private readonly uint channels;
        private bool isSeeking;

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
            this.sampleRate = ZpXGetSampleRate(this.decoder);
            this.bitsPerSample = ZpXGetBitsPerSample(this.decoder);
            this.channels = ZpXGetChannels(this.decoder);

            this.isSeeking = false;
        }

        #region プロパティ

        public uint SampleRate
        {
            get
            {
                return this.sampleRate;
            }
        }

        public uint BitsPerSample
        {
            get
            {
                return this.bitsPerSample;
            }
        }

        public uint Channels
        {
            get
            {
                return this.channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return false;
            }
        }

        private int SampleSize
        {
            get
            {
                return (int)this.BitsPerSample / 8;
            }
        }

        private double SamplesPerMilliseconds
        {
            get
            {
                return (this.SampleRate * this.Channels) / 1000.0;
            }
        }

        #endregion

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

        public void Dispose()
        {
            ZpXCloseFile(this.decoder);
            ZpXFreeDecoder(this.decoder);
            WinApi.FreeLibrary(this.pDll);
        }

        public int Decode(byte[] buffer, int offset, int count)
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

                if (this.BitsPerSample >= 8)
                {
                    buffer[offset++] = (byte)(sample & 0xFF);
                }

                if (this.BitsPerSample >= 16)
                {
                    buffer[offset++] = (byte)((sample >> 8) & 0xFF);
                }

                if (this.BitsPerSample >= 24)
                {
                    buffer[offset++] = (byte)((sample >> 16) & 0xFF);
                }

                // 後始末
                result += this.SampleSize;
            }

            return result;
        }
    }
}
