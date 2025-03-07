﻿using LibPulseTune.Engine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static LibPulseTune.Codecs.Cd.CDDA;
using static LibPulseTune.Codecs.WinApi;

namespace LibPulseTune.Codecs.Cd
{
    public class CDAudioDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly object lockObject = new object();
        private readonly CDTrack track;
        private readonly IntPtr pRawReadInfo;
        private DiskDrive diskDrive;
        private uint sectorIndex;
        private bool isDisposed;

        #region コンストラクタ・デストラクタ

        public CDAudioDecoder(DiskDrive drive, CDTrack track)
        {
            this.diskDrive = drive;
            this.track = track;
            this.sectorIndex = track.GetStartSector();
            this.isDisposed = false;

            // DeviceIoControl関数でCDからPCMデータを読み込む際の各種指令を書き込むための領域を確保。
            this.pRawReadInfo = Marshal.AllocCoTaskMem((int)SIZE_OF_RAW_READ_INFO);
        }

        public CDAudioDecoder(DiskDrive diskDrive, uint trackNumber)
            : this(diskDrive, diskDrive.Tracks[trackNumber - 1])
        {

        }

        public CDAudioDecoder(string path)
            : this(CreateDiskDrive(path), GetTrackNumber(path))
        {

        }

        ~CDAudioDecoder()
        {
            Dispose();
        }

        /// <summary>
        /// CDAファイルのパスからドライブレターを取得し、対応するDiskDriveのインスタンスを返す。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static DiskDrive CreateDiskDrive(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            return new DiskDrive(path[0]);
        }

        /// <summary>
        /// CDAファイルのファイル名からトラック番号を取得する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static uint GetTrackNumber(string path)
        {
            if (uint.TryParse(Path.GetFileNameWithoutExtension(path.Replace("Track", string.Empty)), out uint trackNumber))
            {
                return trackNumber;
            }

            return 0;
        }

        #endregion

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return 44100;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return 16;
            }
        }

        public int Channels
        {
            get
            {
                return 2;
            }
        }

        public bool IsFloat
        {
            get
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// ディスクから次のセクタを読み込み、指定されたバッファに格納する。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int ReadNextSector(byte[] buffer, int offset, int count)
        {
            var sectorCount = Math.Min(DiskDrive.SECTORS_TO_READ, this.sectorIndex - this.track.GetEndSector());

            // RAW_READ_INFO構造体領域にディスクオフセット、読み込むセクタ数、トラックのモードの指定を書き込む。
            // 当然ながら、この書き込み順は変更してはならない。
            Marshal.WriteInt64(this.pRawReadInfo, this.sectorIndex * CB_CDROM_SECTOR);
            Marshal.WriteInt32(this.pRawReadInfo, 8, (int)sectorCount);
            Marshal.WriteInt32(this.pRawReadInfo, 12, (int)CDROM_TRACK_MODE_TYPE_CDDA);

            // セクタからオーディオデータを読み込み、bufferに格納する。
            var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);                     // buffer[0] のアドレスが取得される。Cの &buffer[0] と同じ。
            var bufferPtr = bufferHandle.AddrOfPinnedObject() + (sizeof(byte) * offset);        // buffer[offset] のアドレスを計算。buffer[0]のアドレスに、データのサイズ × offsetを足せばよい。
            if (!DeviceIoControl(
                    this.diskDrive.GetHandle(),
                    IOCTL_CDROM_RAW_READ,
                    this.pRawReadInfo,
                    SIZE_OF_RAW_READ_INFO,
                    bufferPtr,
                    (uint)count,
                    out var read,
                    IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            // 後始末
            bufferHandle.Free();                    // セクタの読み込みに使用したbufferがGCで解放されるように、GCHandleを解放する。
            this.sectorIndex += sectorCount;        // 読み込んだセクタ数を加算する。

            return (int)read;
        }

        /// <summary>
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer">デコード結果出力用バッファ</param>
        /// <param name="offset">デコード結果出力用バッファの書き込み開始オフセット</param>
        /// <param name="count">デコード結果出力用バッファに読み込むデータのバイト数</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            lock (this.lockObject)
            {
                return ReadNextSector(buffer, offset, count);
            }
        }

        /// <summary>
        /// 演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            var totalSectors = this.track.GetEndSector() - this.track.GetStartSector();
            var totalBytes = totalSectors * CB_CDDA_AUDIO;
            var milliseconds = totalBytes / PCM_BYTES_PER_MILLISECOND;

            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            var sector = this.sectorIndex - this.track.GetStartSector();
            var currentPositionBytes = sector * CB_CDDA_AUDIO;
            var milliseconds = currentPositionBytes / PCM_BYTES_PER_MILLISECOND;

            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            var totalBytes = (uint)time.TotalMilliseconds * PCM_BYTES_PER_MILLISECOND;
            var sector = this.track.GetStartSector() + totalBytes / CB_CDDA_AUDIO;

            if (sector > this.track.GetEndSector())
            {
                sector = this.track.GetEndSector();
            }

            this.sectorIndex = sector;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.diskDrive.Dispose();
                this.diskDrive = null;
                Marshal.FreeCoTaskMem(this.pRawReadInfo);
                GC.SuppressFinalize(this);

                this.isDisposed = true;
            }
        }
    }
}
