using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static LibPulseTune.AudioSource.Cd.CDDA;
using static LibPulseTune.WinApi;

namespace LibPulseTune.AudioSource.Cd
{
    internal class DiskDrive : IDisposable
    {
        // 一度に読み込むセクタ数。
        // 一度に読み込まれるセクタ数が多すぎると、読み込み速度や転送速度が遅いドライブとの相性が悪くなる。
        // 一方で、一度に読み込まれるセクタ数を少なくすると、ディスクへのアクセスが頻繁に行われるようになるため、結局、
        // 再生時に求められるアクセス速度を実現できなくなり、音が途切れがちになる。
        // そのため、ドライブによってはこの値ではうまく動作しない場合があるため、本来はこの値を調整可能にすることが望ましい。
        // 手持ちのドライブでは、16だとすべてのドライブで動作した。32だと遅いUSB接続のドライブで動作しなかった。
        public const uint SECTORS_TO_READ = 16;

        // 非公開フィールド
        private readonly CDTrack[] tracks;
        private SafeFileHandle driveHandle;
        private bool isDisposed;

        // コンストラクタ
        public DiskDrive(char driveLetter)
        {
            var fileName = @"\\.\" + driveLetter + ":";

            this.driveHandle = CreateFile(
                fileName,
                FileAccess.Read,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);
            this.isDisposed = false;

            if (this.driveHandle.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
            }

            this.tracks = GetTracks();
        }

        /// <summary>
        /// トラック一覧
        /// </summary>
        public CDTrack[] Tracks
        {
            get
            {
                return this.tracks;
            }
        }

        /// <summary>
        /// トラックをすべて取得する。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private CDTrack[] GetTracks()
        {
            // CDROM_TOCの領域を確保
            IntPtr hToc = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(CDROM_TOC)));

            // TOCを読み込む。
            if (!DeviceIoControl(
                this.driveHandle,
                IOCTL_CDROM_READ_TOC,
                IntPtr.Zero,
                0,
                hToc,
                (uint)Marshal.SizeOf(typeof(CDROM_TOC)),
                out _,
                IntPtr.Zero))
            {
                Marshal.FreeCoTaskMem(hToc);
                throw new Exception($"トラックリストの列挙に失敗しました。{nameof(GetTracks)} 関数内で呼び出された {nameof(DeviceIoControl)} がエラーを返しました。");
            }

            // CDROM_TOCの領域のポインタからCDROM_TOCに変換
            var toc = Marshal.PtrToStructure<CDROM_TOC>(hToc);

            // トラックを列挙
            var tracks = new CDTrack[toc.LastTrack];
            for (int i = toc.FirstTrack - 1; i < toc.LastTrack; ++i)
            {
                tracks[i] = new CDTrack((uint)i, ComputeSectorFromAddress(toc.TrackData[i].Address), ComputeSectorFromAddress(toc.TrackData[i + 1].Address));
            }

            // 後始末
            Marshal.FreeCoTaskMem(hToc);

            return tracks;
        }

        /// <summary>
        /// ハンドルを取得する。
        /// </summary>
        /// <returns></returns>
        public SafeFileHandle GetHandle()
        {
            return this.driveHandle;
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

            this.driveHandle.Dispose();
            this.driveHandle = null;
            GC.SuppressFinalize(this);
            this.isDisposed = true;
        }
    }
}
