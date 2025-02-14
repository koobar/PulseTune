using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LibPulseTune.Codecs
{
    internal static class WinApi
    {
        // 公開定数
        public const uint IOCTL_CDROM_READ_TOC = 0x00024000;
        public const uint IOCTL_CDROM_RAW_READ = 0x0002403E;
        public const uint CDROM_TRACK_MODE_TYPE_CDDA = 2;
        public const uint SIZE_OF_RAW_READ_INFO = 16;

        // 非公開定数
        private const int MAXIMUM_NUMBER_TRACKS = 100;

        // ntddcdrm.hを参考
        [StructLayout(LayoutKind.Sequential)]
        internal struct TRACK_DATA
        {
            public byte Reserved;

            // ntddcdrm.hの定義では、ビットフィールドを用いて4ビットで分割し、それぞれをControl、Adrとして定義されているが、C#にはビットフィールドがない...
            // どうせこの値は使用しないため、とりあえず8ビットの変数を宣言しておいて、データのレイアウトが狂わないようにする。
            public byte Control;

            public byte TrackNumber;
            public byte Reserved1;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Address;
        }

        // ntddcdrm.hを参考
        [StructLayout(LayoutKind.Sequential)]
        internal struct CDROM_TOC
        {
            public ushort Length;
            public byte FirstTrack;
            public byte LastTrack;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_NUMBER_TRACKS)]
            public TRACK_DATA[] TrackData;
        }

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public extern static SafeFileHandle CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flags,
            IntPtr hTemplateFile);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped
        );

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string dllFilePath);

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr module, string functionName);

        [DllImport("kernel32")]
        public static extern bool FreeLibrary(IntPtr module);
    }
}
