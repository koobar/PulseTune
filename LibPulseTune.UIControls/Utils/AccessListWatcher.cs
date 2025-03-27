using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.Utils
{
    public static class AccessListWatcher
    {
        // 非公開定数
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;           // USBの挿入
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // USBの取り外し
        private const int DBT_DEVTYP_VOLUME = 0x00000002;       // デバイスの種類がボリューム

        // 公開イベント
        public static event EventHandler NewDriveConnected;
        public static event EventHandler NewDriveDisconnected;

        public static bool DoWndProc(ref Message message)
        {
            if (message.Msg == WM_DEVICECHANGE)
            {
                if (message.LParam == IntPtr.Zero)
                {
                    return false;
                }

                long wp = message.WParam.ToInt64();
                int lp = Marshal.ReadInt32(message.LParam, 4);

                if (lp == DBT_DEVTYP_VOLUME)
                {
                    switch (wp)
                    {
                        case DBT_DEVICEARRIVAL:
                            NewDriveConnected?.Invoke(null, EventArgs.Empty);
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            NewDriveDisconnected?.Invoke(null, EventArgs.Empty);
                            break;
                    }
                }

                /*switch (wp)
                {
                    case DBT_DEVICEARRIVAL:
                        NewUSBDriveConnected?.Invoke(null, EventArgs.Empty);
                        break;
                    case DBT_DEVICEREMOVECOMPLETE:
                        NewUSBDriveDisconnected?.Invoke(null, EventArgs.Empty);
                        break;
                }*/
            }
            return false;
        }
    }
}
