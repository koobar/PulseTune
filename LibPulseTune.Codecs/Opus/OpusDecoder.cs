using Concentus;
using Concentus.Oggfile;
using LibPulseTune.Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace LibPulseTune.Codecs.Opus
{
    public sealed class OpusDecoder : IAudioSource
    {
        // 非公開定数
        private const int SAMPLE_RATE = 48000;
        private const int CHANNELS = 2;
        private const int BITS_PER_SAMPLE = 16;
        private const int BYTES_PER_SAMPLE = BITS_PER_SAMPLE / 8;

        // 非公開フィールド
        private readonly Stream inputStream;
        private readonly IOpusDecoder decoder;
        private readonly OpusOggReadStream streamReader;
        private readonly Queue<short> decodeBuffer;
        private readonly long length;
        private readonly object lockObject;
        private readonly TimeSpan startTime;
        private long position;

        #region コンストラクタ

        public OpusDecoder(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("ストリームが読み込み不可能です。", nameof(stream));
            }

            this.inputStream = stream;
            this.decodeBuffer = new Queue<short>();
            this.decoder = OpusCodecFactory.CreateDecoder(SAMPLE_RATE, CHANNELS);
            this.streamReader = new OpusOggReadStream(this.decoder, stream);
            this.lockObject = new object();
            this.startTime = this.streamReader.CurrentTime;
            this.length = Convert.ToInt64(this.streamReader.TotalTime.TotalSeconds * this.SampleRate * this.Channels * BYTES_PER_SAMPLE);
        }

        public OpusDecoder(string path) : this(File.OpenRead(path))
        {
            
        }

        #endregion

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.decoder.SampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return BITS_PER_SAMPLE;
            }
        }

        public int Channels
        {
            get
            {
                return this.decoder.NumChannels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// シーク可能かどうか
        /// </summary>
        public bool CanSeek
        {
            get
            {
                return this.inputStream.CanSeek;
            }
        }

        /// <summary>
        /// ストリームの位置
        /// </summary>
        public long Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (!this.CanSeek)
                {
                    throw new InvalidOperationException("このストリームはシーク操作が許可されていません。");
                }

                if (value < 0 || value > this.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                // 新しく指定された再生位置までシーク
                var time = TimeSpan.FromSeconds((double)value / (this.SampleRate * this.Channels * BYTES_PER_SAMPLE));
                this.streamReader.SeekTo(time);

                // バイト単位の位置情報を更新
                this.position = Convert.ToInt64(this.streamReader.CurrentTime.TotalSeconds * this.SampleRate * this.Channels * BYTES_PER_SAMPLE);
            }
        }

        /// <summary>
        /// ストリームの長さ
        /// </summary>
        public long Length
        {
            get
            {
                return this.length;
            }
        }

        #endregion

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.decoder.Dispose();
            this.inputStream.Dispose();
            this.decodeBuffer.Clear();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 次のパケットをデコードする。
        /// </summary>
        private void DecodeNextPacket()
        {
            short[] packet = this.streamReader.DecodeNextPacket();

            foreach (var sample in packet)
            {
                this.decodeBuffer.Enqueue(sample);
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
                int result = 0;

                while (this.streamReader.HasNextPacket && result != count)
                {
                    if (this.decodeBuffer.Count == 0)
                    {
                        DecodeNextPacket();
                    }

                    var sample = this.decodeBuffer.Dequeue();
                    var bs = BitConverter.GetBytes(sample);
                    buffer[offset++] = bs[0];
                    buffer[offset++] = bs[1];
                    result += sizeof(short);
                }

                return result;
            }
        }

        public TimeSpan GetDuration()
        {
            return this.streamReader.TotalTime;
        }

        public TimeSpan GetCurrentTime()
        {
            return this.streamReader.CurrentTime;
        }

        public void SetCurrentTime(TimeSpan time)
        {
            if (time < this.startTime)
            {
                time = this.startTime;
            }

            this.streamReader.SeekTo(time);
        }
    }
}
