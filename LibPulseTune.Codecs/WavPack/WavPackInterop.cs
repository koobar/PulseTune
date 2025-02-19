using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.Codecs.WavPack
{
    internal static class WavPackInterop
    {
        public enum WavPackMode
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

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private delegate IntPtr DWavpackOpenFileInput(string fileName, IntPtr error, int flags, int norm_offset);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DWavpackGetSampleRate(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DWavpackGetNumSamples(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int DWavpackGetNumChannels(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int DWavpackGetBitsPerSample(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DWavpackUnpackSamples(IntPtr wpc, IntPtr buffer, uint samples);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int DWavpackSeekSample(IntPtr wpc, uint sample);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint DWavpackGetSampleIndex(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr DWavpackCloseFile(IntPtr wpc);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate WavPackMode DWavpackGetMode(IntPtr wpc);

        private const string WAVPACK_DLL_NAME = @"wavpackdll.dll";
        private static IntPtr wavPackModule;
        private static DWavpackOpenFileInput _wavPackOpenFileInput;
        private static DWavpackGetSampleRate _wavPackGetSampleRate;
        private static DWavpackGetNumSamples _wavPackGetNumSamples;
        private static DWavpackGetNumChannels _wavPackGetNumChannels;
        private static DWavpackGetBitsPerSample _wavPackGetBitsPerSample;
        private static DWavpackUnpackSamples _wavPackUnpackSamples;
        private static DWavpackSeekSample _wavPackSeekSample;
        private static DWavpackGetSampleIndex _wavPackGetSampleIndex;
        private static DWavpackCloseFile _wavPackCloseFile;
        private static DWavpackGetMode _wavPackGetMode;
        private static bool isLibraryLoaded;

        /// <summary>
        /// WavPackのDLLの場所を取得する。
        /// </summary>
        /// <returns></returns>
        public static string GetDllPath()
        {
            return $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\{WAVPACK_DLL_NAME}";
        }

        public static bool IsAvailable()
        {
            if (File.Exists(GetDllPath()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// WavPackのDLLを読み込む。
        /// </summary>
        public static void LoadLibrary()
        {
            if (isLibraryLoaded)
            {
                return;
            }

            wavPackModule = WinApi.LoadLibrary(GetDllPath());
            _wavPackOpenFileInput = WinApiHelper.LoadFunction<DWavpackOpenFileInput>(wavPackModule, "WavpackOpenFileInput");
            _wavPackGetSampleRate = WinApiHelper.LoadFunction<DWavpackGetSampleRate>(wavPackModule, "WavpackGetSampleRate");
            _wavPackGetNumSamples = WinApiHelper.LoadFunction<DWavpackGetNumSamples>(wavPackModule, "WavpackGetNumSamples");
            _wavPackGetNumChannels = WinApiHelper.LoadFunction<DWavpackGetNumChannels>(wavPackModule, "WavpackGetNumChannels");
            _wavPackGetBitsPerSample = WinApiHelper.LoadFunction<DWavpackGetBitsPerSample>(wavPackModule, "WavpackGetBitsPerSample");
            _wavPackUnpackSamples = WinApiHelper.LoadFunction<DWavpackUnpackSamples>(wavPackModule, "WavpackUnpackSamples");
            _wavPackSeekSample = WinApiHelper.LoadFunction<DWavpackSeekSample>(wavPackModule, "WavpackSeekSample");
            _wavPackGetSampleIndex = WinApiHelper.LoadFunction<DWavpackGetSampleIndex>(wavPackModule, "WavpackGetSampleIndex");
            _wavPackCloseFile = WinApiHelper.LoadFunction<DWavpackCloseFile>(wavPackModule, "WavpackCloseFile");
            _wavPackGetMode = WinApiHelper.LoadFunction<DWavpackGetMode>(wavPackModule, "WavpackGetMode");
            isLibraryLoaded = false;
        }

        public static IntPtr WavpackOpenFileInput(string fileName, IntPtr error, int flags, int norm_offset)
        {
            return _wavPackOpenFileInput(fileName, error, flags, norm_offset);
        }

        public static uint WavpackGetSampleRate(IntPtr context)
        {
            return _wavPackGetSampleRate(context);
        }

        public static uint WavpackGetNumSamples(IntPtr context)
        {
            return _wavPackGetNumSamples(context);
        }

        public static int WavpackGetNumChannels(IntPtr context)
        {
            return _wavPackGetNumChannels(context);
        }

        public static int WavpackGetBitsPerSample(IntPtr context)
        {
            return _wavPackGetBitsPerSample(context);
        }

        public static uint WavpackUnpackSamples(IntPtr context, IntPtr buffer, uint samples)
        {
            return _wavPackUnpackSamples(context, buffer, samples);
        }

        public static int WavpackSeekSample(IntPtr context, uint sample)
        {
            return _wavPackSeekSample(context, sample);
        }

        public static uint WavpackGetSampleIndex(IntPtr context)
        {
            return _wavPackGetSampleIndex(context);
        }

        public static IntPtr WavpackCloseFile(IntPtr context)
        {
            return _wavPackCloseFile(context);
        }

        public static WavPackMode WavpackGetMode(IntPtr wpc)
        {
            return _wavPackGetMode(wpc);
        }
    }
}
