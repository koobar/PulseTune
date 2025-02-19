using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.Codecs.ZilophiX
{
    internal static class ZilophiXInterop
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)] private delegate IntPtr DZpXCreateDecoder(string path);
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
        private static IntPtr pDll;
        private static DZpXCreateDecoder _ZpXCreateDecoder;
        private static DZpXFreeDecoder _ZpXFreeDecoder;
        private static DZpXCloseFile _ZpXCloseFile;
        private static DZpXGetSampleRate _ZpXGetSampleRate;
        private static DZpXGetChannels _ZpXGetChannels;
        private static DZpXGetBitsPerSample _ZpXGetBitsPerSample;
        private static DZpXGetNumTotalSamples _ZpXGetNumTotalSamples;
        private static DZpXReadSample _ZpXReadSample;
        private static DZpXGetSampleOffset _ZpXGetSampleOffset;
        private static DZpXSeekTo _ZpXSeekTo;
        private static DZpXGetDurationMsec _ZpXGetDurationMsec;
        private static bool isLibraryLoaded;

        /// <summary>
        /// ZilophiXのDLLの場所を取得する。
        /// </summary>
        /// <returns></returns>
        public static string GetDllPath()
        {
            return $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\zilophixdec.dll";
        }

        /// <summary>
        /// ZilophiXのDLLが使用可能であるかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            return File.Exists(GetDllPath());
        }

        /// <summary>
        /// ZilophiXのDLLを読み込む。
        /// </summary>
        public static void LoadLibrary()
        {
            if (isLibraryLoaded)
            {
                return;
            }

            pDll = WinApi.LoadLibrary(GetDllPath());
            _ZpXCreateDecoder = WinApiHelper.LoadFunction<DZpXCreateDecoder>(pDll, "ZpXCreateDecoderW");
            _ZpXFreeDecoder = WinApiHelper.LoadFunction<DZpXFreeDecoder>(pDll, "ZpXFreeDecoder");
            _ZpXCloseFile = WinApiHelper.LoadFunction<DZpXCloseFile>(pDll, "ZpXCloseFile");
            _ZpXGetSampleRate = WinApiHelper.LoadFunction<DZpXGetSampleRate>(pDll, "ZpXGetSampleRate");
            _ZpXGetChannels = WinApiHelper.LoadFunction<DZpXGetChannels>(pDll, "ZpXGetChannels");
            _ZpXGetBitsPerSample = WinApiHelper.LoadFunction<DZpXGetBitsPerSample>(pDll, "ZpXGetBitsPerSample");
            _ZpXGetNumTotalSamples = WinApiHelper.LoadFunction<DZpXGetNumTotalSamples>(pDll, "ZpXGetNumTotalSamples");
            _ZpXReadSample = WinApiHelper.LoadFunction<DZpXReadSample>(pDll, "ZpXReadSample");
            _ZpXGetSampleOffset = WinApiHelper.LoadFunction<DZpXGetSampleOffset>(pDll, "ZpXGetSampleOffset");
            _ZpXSeekTo = WinApiHelper.LoadFunction<DZpXSeekTo>(pDll, "ZpXSeekTo");
            _ZpXGetDurationMsec = WinApiHelper.LoadFunction<DZpXGetDurationMsec>(pDll, "ZpXGetDurationMsec");
            isLibraryLoaded = true;
        }

        public static IntPtr ZpXCreateDecoder(string path)
        {
            return _ZpXCreateDecoder(path);
        }

        public static void ZpXFreeDecoder(IntPtr pDecoder)
        {
            _ZpXFreeDecoder(pDecoder);
        }

        public static void ZpXCloseFile(IntPtr pDecoder)
        {
            _ZpXCloseFile(pDecoder);
        }

        public static uint ZpXGetSampleRate(IntPtr pDecoder)
        {
            return _ZpXGetSampleRate(pDecoder);
        }

        public static uint ZpXGetChannels(IntPtr pDecoder)
        {
            return _ZpXGetChannels(pDecoder);
        }

        public static uint ZpXGetBitsPerSample(IntPtr pDecoder)
        {
            return _ZpXGetBitsPerSample(pDecoder);
        }

        public static uint ZpXGetNumTotalSamples(IntPtr pDecoder)
        {
            return _ZpXGetNumTotalSamples(pDecoder);
        }

        public static int ZpXReadSample(IntPtr pDecoder)
        {
            return _ZpXReadSample(pDecoder);
        }

        public static uint ZpXGetSampleOffset(IntPtr pDecoder)
        {
            return _ZpXGetSampleOffset(pDecoder);
        }

        public static void ZpXSeekTo(IntPtr pDecoder, uint msec)
        {
            _ZpXSeekTo(pDecoder, msec);
        }

        public static uint ZpXGetDurationMsec(IntPtr pDecoder)
        {
            return _ZpXGetDurationMsec(pDecoder);
        }
    }
}
