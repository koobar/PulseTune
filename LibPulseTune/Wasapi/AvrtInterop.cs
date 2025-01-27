using System;
using System.Runtime.InteropServices;

namespace LibPulseTune.Wasapi
{
    internal static class AvrtInterop
    {
        public const int AVRT_PRIORITY_CRITICAL = 2;
        public const int AVRT_PRIORITY_HIGH = 1;
        public const int AVRT_PRIORITY_LOW = -1;
        public const int AVRT_PRIORITY_NORMAL = 0;

        [DllImport("Avrt.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr AvSetMmThreadCharacteristics([MarshalAs(UnmanagedType.LPWStr)] string proAudio, [Out, In] ref int taskIndex);

        [DllImport("Avrt.dll")]
        internal static extern bool AvRevertMmThreadCharacteristics(IntPtr avrtHandle);

        [DllImport("Avrt.dll")]
        internal static extern bool AvSetMmThreadPriority(IntPtr avrtHandle, int priority);
    }
}
