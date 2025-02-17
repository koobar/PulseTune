using LibPulseTune.Codecs.Utils;
using LibPulseTune.Engine;
using System;
using System.IO;

namespace LibPulseTune.Codecs.Wav
{
    public class WavDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly long dataChunkStartOffset;
        private readonly long length;
        private readonly object lockObject;
        private BinaryReader streamReader;
        private int sampleRate;
        private int bitsPerSample;
        private int channels;
        private int averageBytesPerSecond;
        private int blockSize;
        private bool isFloat;
        private bool isDisposed;

        // コンストラクタ
        public WavDecoder(string path)
        {
            this.streamReader = new BinaryReader(File.OpenRead(path));
            this.lockObject = new object();

            ReadFmtChunk();

            if (this.streamReader.MoveToChunk("data"))
            {
                this.length = this.streamReader.ReadUInt32();
                this.dataChunkStartOffset = this.streamReader.BaseStream.Position;
            }
            else
            {
                throw new InvalidDataException("dataチャンクが見つかりませんでした。");
            }
        }

        // デストラクタ
        ~WavDecoder()
        {
            Dispose();
        }

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.sampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.bitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.isFloat;
            }
        }

        #endregion

        /// <summary>
        /// fmt チャンクを読み込む。
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private void ReadFmtChunk()
        {
            if (this.streamReader.MoveToChunk("fmt "))
            {
                this.streamReader.BaseStream.Position += 4;
                var audioFormat = this.streamReader.ReadUInt16();

                if (audioFormat == 0x0001 || audioFormat == 0xFFFE)
                {
                    this.channels = (int)this.streamReader.ReadUInt16();
                    this.sampleRate = (int)this.streamReader.ReadUInt32();
                    this.averageBytesPerSecond = (int)this.streamReader.ReadUInt32();
                    this.blockSize = (int)this.streamReader.ReadUInt16();
                    this.bitsPerSample = (int)this.streamReader.ReadUInt16();
                    this.isFloat = false;
                }
                else if (audioFormat == 0x0003)
                {
                    this.channels = (int)this.streamReader.ReadUInt16();
                    this.sampleRate = (int)this.streamReader.ReadUInt32();
                    this.averageBytesPerSecond = (int)this.streamReader.ReadUInt32();
                    this.blockSize = (int)this.streamReader.ReadUInt16();
                    this.bitsPerSample = (int)this.streamReader.ReadUInt16();
                    this.isFloat = true;
                }
            }
            else
            {
                throw new InvalidDataException("fmt チャンクが見つかりません。");
            }
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
                var position = GetPosition();

                if (position + count > this.length)
                {
                    count = (int)this.length - (int)position;
                }

                return this.streamReader.Read(buffer, offset, count);
            }
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

            this.streamReader.Dispose();
            this.streamReader = null;
            this.isDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// PCMデータのバイトオフセットを取得する。
        /// </summary>
        /// <returns></returns>
        public long GetPosition()
        {
            lock (this.lockObject)
            {
                return this.streamReader.BaseStream.Position - this.dataChunkStartOffset;
            }
        }

        /// <summary>
        /// このストリームの演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            lock (this.lockObject)
            {
                return TimeSpan.FromSeconds(this.length / this.averageBytesPerSecond);
            }
        }

        /// <summary>
        /// このストリームの再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            lock (this.lockObject)
            {
                return TimeSpan.FromSeconds(GetPosition() / this.averageBytesPerSecond);
            }
        }

        /// <summary>
        /// このストリームの再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            lock (this.lockObject)
            {
                var pos = (int)(time.TotalSeconds * this.averageBytesPerSecond);
                pos -= (pos % this.blockSize);

                this.streamReader.BaseStream.Position = this.dataChunkStartOffset + pos;
            }
        }
    }
}
