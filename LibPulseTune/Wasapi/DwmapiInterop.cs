using System.Runtime.InteropServices;

namespace LibPulseTune.Wasapi
{
    internal static class DwmapiInterop
    {
        [DllImport("dwmapi.dll")]
        internal static extern int DwmEnableMMCSS(bool fEnable);
    }
}
