using LibPulseTune.Engine;
using System;

namespace LibPulseTune.Codecs.Ape
{
    public class ApeDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly IntPtr decoderHandle;
        private readonly int sampleRate;
        private readonly int bitsPerSample;
        private readonly int channels;
        private readonly bool isFloat;
        private readonly object lockObject;
        private sbyte[] decodeBuffer;
        private bool isDisposed;

        // コンストラクタ
        public ApeDecoder(string path)
        {
            ApeInterop.LoadNativeLibrary();
            this.decoderHandle = ApeInterop.c_APEDecompress_CreateW(path, out int errorCode);
            this.sampleRate = GetInfoInt(this.decoderHandle, ApeInterop.InformationField.SampleRate);
            this.bitsPerSample = GetInfoInt(this.decoderHandle, ApeInterop.InformationField.BitsPerSample);
            this.channels = GetInfoInt(this.decoderHandle, ApeInterop.InformationField.Channels);
            this.isFloat = false;
            this.lockObject = new object();
        }

        // デストラクタ
        ~ApeDecoder()
        {
            Dispose();
        }

        #region プロパティ

        /// <summary>
        /// サンプリング周波数
        /// </summary>
        public int SampleRate
        {
            get
            {
                return this.sampleRate;
            }
        }

        /// <summary>
        /// 量子化ビット数
        /// </summary>
        public int BitsPerSample
        {
            get
            {
                return this.bitsPerSample;
            }
        }

        /// <summary>
        /// チャンネル数
        /// </summary>
        public int Channels
        {
            get
            {
                return this.channels;
            }
        }

        /// <summary>
        /// 浮動小数点のサンプルデータであるかどうかを示す。
        /// </summary>
        public bool IsFloat
        {
            get
            {
                return this.isFloat;
            }
        }

        /// <summary>
        /// ブロックのサイズ
        /// </summary>
        public int BlockAlign
        {
            get
            {
                return this.Channels * (this.BitsPerSample / 8);
            }
        }

        /// <summary>
        /// Readメソッドから読み込み可能なデータのバイト数
        /// </summary>
        public long Length
        {
            get
            {
                lock (this.lockObject)
                {
                    return GetInfoInt(this.decoderHandle, ApeInterop.InformationField.TotalBlocks) * this.BlockAlign;
                }
            }
        }

        /// <summary>
        /// バイト単位のストリームの読み込み位置
        /// </summary>
        public long Position
        {
            get
            {
                lock (this.lockObject)
                {
                    return GetInfoInt(this.decoderHandle, ApeInterop.InformationField.DecompressCurrentBlock) * this.BlockAlign;
                }
            }
            set
            {
                lock (this.lockObject)
                {
                    var blockPos = value / this.BlockAlign;
                    ApeInterop.c_APEDecompress_Seek(this.decoderHandle, (int)blockPos);
                }
            }
        }

        #endregion

        /// <summary>
        /// Monkey's AudioのDLLが存在し、このデコーダが使用可能であるかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            return ApeInterop.IsAvailable();
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

            ApeInterop.c_APEDecompress_Destroy(this.decoderHandle);

            this.decodeBuffer = null;
            this.isDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// デコード
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            lock (this.lockObject)
            {
                if (this.decodeBuffer == null || this.decodeBuffer.Length < count)
                {
                    this.decodeBuffer = new sbyte[count];
                }

                // デコード用バッファにデコード
                ApeInterop.c_APEDecompress_GetData(this.decoderHandle, this.decodeBuffer, count / this.BlockAlign, out int retrievedBlocks);

                // デコード用バッファから出力用バッファにデータを転送
                int bytesRead = retrievedBlocks * this.BlockAlign;
                Buffer.BlockCopy(this.decodeBuffer, 0, buffer, 0, bytesRead);

                return bytesRead;
            }
        }

        /// <summary>
        /// このストリームの演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            var bytesPerSecond = this.sampleRate * this.BlockAlign;
            var totalSeconds = this.Length / bytesPerSecond;

            return TimeSpan.FromSeconds(totalSeconds);
        }

        /// <summary>
        /// このストリームの再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            var bytesPerSecond = this.sampleRate * this.BlockAlign;
            var seconds = this.Position / bytesPerSecond;

            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// このストリームの再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            var bytesPerSecond = this.sampleRate * this.BlockAlign;
            var position = time.TotalSeconds * bytesPerSecond;

            this.Position = (long)position;
        }

        private int GetInfoInt(IntPtr handle, ApeInterop.InformationField info)
        {
            return ApeInterop.c_APEDecompress_GetInfo(handle, info, 0, 0).ToInt32();
        }
    }
}
