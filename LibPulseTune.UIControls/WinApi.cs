using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LibPulseTune.UIControls
{
    internal static class WinApi
    {
        // 非公開定数
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const int GWL_STYLE = -16;
        private const int WS_VSCROLL = 0x00200000;
        private const int WS_HSCROLL = 0x00100000;

        #region 構造体

        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        #endregion

        #region 列挙型

        public enum ExtractIconSize : uint
        {
            Large = SHGFI_LARGEICON,
            Small = SHGFI_SMALLICON
        }

        #endregion

        #region ラッパー関数の定義

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool DestroyIcon(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion

        /// <summary>
        /// 指定されたパスのファイルまたはフォルダのアイコンを取得する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Icon ExtractIconFromPath(string path, ExtractIconSize size)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | (uint)size);
            return Icon.FromHandle(shinfo.hIcon);
        }

        /// <summary>
        /// 指定されたハンドルのコントロールの、垂直方向のスクロールバーが表示されているかどうかを判定する。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool GetVScrollBarVisible(IntPtr hWnd)
        {
            int style = GetWindowLong(hWnd, GWL_STYLE);

            return (style & WS_VSCROLL) != 0;
        }

        /// <summary>
        /// 指定されたハンドルのコントロールの、水平方向のスクロールバーが表示されているかどうかを判定する。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool GetHScrollBarVisible(IntPtr hWnd)
        {
            int style = GetWindowLong(hWnd, GWL_STYLE);

            return (style & WS_HSCROLL) != 0;
        }
    }
}
