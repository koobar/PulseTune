using LibPulseTune.Codecs.Utils;
using LibPulseTune.Engine;
using NAudio.Utils;
using System;
using System.IO;

namespace LibPulseTune.Codecs.Aiff
{
    public class AiffDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly long dataChunkOffset;
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
        public AiffDecoder(string path)
        {
            this.streamReader = new BinaryReader(File.OpenRead(path));
            this.lockObject = new object();

            ReadCOMMChunk();

            if (this.streamReader.MoveToChunk("SSND"))
            {
                this.length = this.streamReader.ReadBigEndianUInt32();
                this.dataChunkOffset = this.streamReader.BaseStream.Position;
            }
            else
            {
                throw new InvalidDataException("dataチャンクが見つかりませんでした。");
            }
        }

        // デストラクタ
        ~AiffDecoder()
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
        /// COMMチャンクを読み込む。
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private void ReadCOMMChunk()
        {
            if (this.streamReader.MoveToChunk("COMM"))
            {
                this.streamReader.BaseStream.Position += sizeof(uint);

                this.channels = this.streamReader.ReadBigEndianInt16();
                this.streamReader.BaseStream.Position += sizeof(uint);
                this.bitsPerSample = this.streamReader.ReadBigEndianInt16();
                this.sampleRate = (int)IEEE.ConvertFromIeeeExtended(this.streamReader.ReadBytes(10));
                this.isFloat = false;
                this.blockSize = (short)(this.channels * (this.bitsPerSample / 8));
                this.averageBytesPerSecond = this.sampleRate * this.blockSize;
            }
            else
            {
                throw new InvalidDataException("COMMチャンクが見つかりません。");
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

                // AIFFでは、PCMデータがビッグエンディアンで記録されている。
                // IAudioSourceはリトルエンディアンのデータを必要とするため、
                // リトルエンディアンの順となるように並び替える。
                byte[] bigEndian = new byte[count];
                int read = this.streamReader.Read(bigEndian, offset, count);

                int bytesPerSample = this.bitsPerSample / 8;
                for (int i = 0; i < read; i += bytesPerSample)
                {
                    if (this.BitsPerSample == 8)
                    {
                        buffer[i] = bigEndian[i];
                    }
                    else if (this.bitsPerSample == 16)
                    {
                        buffer[i + 0] = bigEndian[i + 1];
                        buffer[i + 1] = bigEndian[i];
                    }
                    else if (this.bitsPerSample == 24)
                    {
                        buffer[i + 0] = bigEndian[i + 2];
                        buffer[i + 1] = bigEndian[i + 1];
                        buffer[i + 2] = bigEndian[i + 0];
                    }
                    else if (this.bitsPerSample == 32)
                    {
                        buffer[i + 0] = bigEndian[i + 3];
                        buffer[i + 1] = bigEndian[i + 2];
                        buffer[i + 2] = bigEndian[i + 1];
                        buffer[i + 3] = bigEndian[i + 0];
                    }
                    else
                    {
                        throw new FormatException("サポートされていないフォーマットです。");
                    }
                }

                return read;
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
                var result = this.streamReader.BaseStream.Position - this.dataChunkOffset;

                return result;
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

                this.streamReader.BaseStream.Position = this.dataChunkOffset + pos;
            }
        }
    }
}
