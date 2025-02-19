using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.Codecs.Ape
{
    internal static class ApeInterop
    {
        public enum InformationField
        {
            FileVersion = 1000,
            CompressionLevel = 1001,
            FormatFlags = 1002,
            SampleRate = 1003,
            BitsPerSample = 1004,
            BytesPerSample = 1005,
            Channels = 1006,
            BlockAlignment = 1007,
            BlocksPerFrame = 1008,
            FinalFrameBlocks = 1009,
            TotalFrames = 1010,
            WavHeaderBytes = 1011,
            WavTerminatingBytes = 1012,
            WavDataBytes = 1013,
            WavTotalBytes = 1014,
            ApeTotalBytes = 1015,
            TotalBlocks = 1016,
            LengthInMs = 1017,
            AverageBitrate = 1018,
            FrameBitrate = 1019,
            DecompressedBitrate = 1020,
            PeakLevel = 1021,
            SeekBit = 1022,
            SeekByte = 1023,
            WavHeaderData = 1024,
            WavTerminatingData = 1025,
            WaveFormatEx = 1026,
            IOSource = 1027,
            FrameBytes = 1028,
            FrameBlocks = 1029,
            Tag = 1030,
            DecompressCurrentBlock = 2000,
            DecompressCurrentMs = 2001,
            DecompressTotalBlocks = 2002,
            DecompressLengthMs = 2003,
            DecompressCurrentBitrate = 2004,
            DecompressAverageBitrate = 2005,
            DecompressCurrentFrame = 2006,
            InternalInfo = 3000
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)] private delegate IntPtr DAPEDecompressCreate(string fileName, out int errorCode);
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)] private delegate IntPtr DAPEDecompressCreateW(string fileName, out int errorCode);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void DAPEDecompressDestroy(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void DAPEDecompressGetData(IntPtr handle, sbyte[] buffer, int numBlocks, out int blocksRetrieved);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void DAPEDecompressSeek(IntPtr handle, int blockOffset);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate IntPtr DAPEDecompressGetInfo(IntPtr handle, InformationField field, int nParam1, int nParam2);

        // 非公開フィールド
        private static IntPtr pDll;
        private static DAPEDecompressCreate dApeDecompressCreate;
        private static DAPEDecompressCreateW dApeDecompressCreateW;
        private static DAPEDecompressDestroy dAPEDecompressDestroy;
        private static DAPEDecompressGetData dAPEDecompressGetData;
        private static DAPEDecompressSeek dAPEDecompressSeek;
        private static DAPEDecompressGetInfo dAPEDecompressGetInfo;
        private static bool isLibraryLoaded;

        /// <summary>
        /// プロセスのビット数に応じたDLLの名前を取得する。
        /// </summary>
        /// <returns></returns>
        private static string GetDllName()
        {
            if (IntPtr.Size == 4)
            {
                return "MACDLL.dll";
            }
            else if (IntPtr.Size == 8)
            {
                return "MACDLL64.dll";
            }

            return null;
        }

        /// <summary>
        /// Monkey's AudioのネイティブDLLの場所を取得する。
        /// </summary>
        /// <returns></returns>
        private static string GetNativeLibraryPath()
        {
            return $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\{GetDllName()}";
        }

        /// <summary>
        /// Monkey's AudioのDLLが存在し、このデコーダが使用可能であるかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            return File.Exists(GetNativeLibraryPath());
        }

        /// <summary>
        /// Monkey's AudioのDLLを読み込む。
        /// </summary>
        public static void LoadNativeLibrary()
        {
            if (isLibraryLoaded)
            {
                return;
            }

            pDll = WinApi.LoadLibrary(GetNativeLibraryPath());
            dApeDecompressCreate = WinApiHelper.LoadFunction<DAPEDecompressCreate>(pDll, @"c_APEDecompress_Create");
            dApeDecompressCreateW = WinApiHelper.LoadFunction<DAPEDecompressCreateW>(pDll, @"c_APEDecompress_CreateW");
            dAPEDecompressDestroy = WinApiHelper.LoadFunction<DAPEDecompressDestroy>(pDll, @"c_APEDecompress_Destroy");
            dAPEDecompressGetData = WinApiHelper.LoadFunction<DAPEDecompressGetData>(pDll, @"c_APEDecompress_GetData");
            dAPEDecompressSeek = WinApiHelper.LoadFunction<DAPEDecompressSeek>(pDll, @"c_APEDecompress_Seek");
            dAPEDecompressGetInfo = WinApiHelper.LoadFunction<DAPEDecompressGetInfo>(pDll, @"c_APEDecompress_GetInfo");
            isLibraryLoaded = true;
        }

        public static IntPtr c_APEDecompress_Create(string path, out int errorCode)
        {
            return dApeDecompressCreate(path, out errorCode);
        }

        public static IntPtr c_APEDecompress_CreateW(string path, out int errorCode)
        {
            return dApeDecompressCreateW(path, out errorCode);
        }

        public static void c_APEDecompress_Destroy(IntPtr handle)
        {
            dAPEDecompressDestroy(handle);
        }

        public static void c_APEDecompress_GetData(IntPtr handle, sbyte[] buffer, int numBlocks, out int blocksRetrieved)
        {
            dAPEDecompressGetData(handle, buffer, numBlocks, out blocksRetrieved);
        }

        public static void c_APEDecompress_Seek(IntPtr handle, int blockOffset)
        {
            dAPEDecompressSeek(handle, blockOffset);
        }

        public static IntPtr c_APEDecompress_GetInfo(IntPtr handle, InformationField info, int nParam1, int nParam2)
        {
            return dAPEDecompressGetInfo(handle, info, nParam1, nParam2);
        }
    }
}
