using NAudio.Wave;
using NVorbis;
using System;

namespace LibPulseTune.Codecs.Vorbis
{
    internal class NVorbisReader : ISampleProvider
    {
        // 非公開定数
        private const int INITIAL_DECODE_BUFFER_SIZE = 128;

        // 非公開フィールド
        private readonly WaveFormat waveFormat;
        private readonly object lockObject;
        private float[] decodeBuffer;
        private int decodeBufferAvailable;
        private int decodeBufferIndex;
        private VorbisReader reader;
        

        // コンストラクタ
        public NVorbisReader(string path)
        {
            this.lockObject = new object();
            this.reader = new VorbisReader(path);
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(this.reader.SampleRate, this.reader.Channels);
            this.decodeBuffer = new float[INITIAL_DECODE_BUFFER_SIZE];
        }

        #region プロパティ

        /// <summary>
        /// フォーマット
        /// </summary>
        public WaveFormat WaveFormat
        {
            get
            {
                return this.waveFormat;
            }
        }

        /// <summary>
        /// ストリームの下限ビットレート
        /// </summary>
        public uint LowerBitrate
        {
            get
            {
                return (uint)this.reader.LowerBitrate;
            }
        }

        /// <summary>
        /// ストリームのビットレート
        /// </summary>
        public uint NominalBitrate
        {
            get
            {
                return (uint)this.reader.NominalBitrate;
            }
        }

        /// <summary>
        /// ストリームの上限ビットレート
        /// </summary>
        public uint UpperBitrate
        {
            get
            {
                return (uint)this.reader.UpperBitrate;
            }
        }

        #endregion

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.reader.Dispose();
            this.reader = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ストリームをデコードし、デコード用バッファにサンプルデータを格納する。
        /// </summary>
        private void FillBuffer()
        {
            lock (this.lockObject)
            {
                int numSamples = this.reader.ReadSamples(this.decodeBuffer, 0, this.decodeBuffer.Length);

                this.decodeBufferAvailable = numSamples;
                this.decodeBufferIndex = 0;
            }
        }

        /// <summary>
        /// デコード用バッファからサンプルデータを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int ReadFromBuffer(float[] buffer, ref int offset, int count)
        {
            lock (this.lockObject)
            {
                // デコード用バッファのサイズが不足していればリサイズ
                if (this.decodeBuffer.Length < count)
                {
                    Array.Resize(ref this.decodeBuffer, count);
                }

                // デコード可能なサンプル数を求める。
                int numCopySamples = count;
                if (this.decodeBufferAvailable < count)
                {
                    numCopySamples = this.decodeBufferAvailable;
                }

                // デコード用バッファから出力用バッファにサンプルデータを転送
                Buffer.BlockCopy(this.decodeBuffer, this.decodeBufferIndex * sizeof(float), buffer, offset * sizeof(float), numCopySamples * sizeof(float));

                // オフセットを更新
                this.decodeBufferIndex += numCopySamples;
                this.decodeBufferAvailable -= numCopySamples;
                offset += numCopySamples;

                // バッファの読み込み可能サンプル数の境界チェック
                if (this.decodeBufferAvailable < 0)
                {
                    this.decodeBufferAvailable = 0;
                }

                return numCopySamples;
            }
        }

        /// <summary>
        /// サンプルデータを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(float[] buffer, int offset, int count)
        {
            // これでも動くには動くが、オーディオ出力デバイスによっては少々不安定な挙動となる場合がある。
            //return this.reader.ReadSamples(buffer, offset, count);

            lock (this.lockObject)
            {
                int result = 0;

                while (count > 0)
                {
                    if (this.reader.IsEndOfStream)
                    {
                        break;
                    }

                    if (this.decodeBufferAvailable == 0)
                    {
                        FillBuffer();
                    }

                    // デコード用バッファからサンプルを読み込む。
                    int numSamples = ReadFromBuffer(buffer, ref offset, count);
                    count -= numSamples;
                    result += numSamples;
                }

                return result;
            }
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.reader.TimePosition;
        }

        /// <summary>
        /// 演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            return this.reader.TotalTime;
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.reader.TimePosition = time;
        }
    }
}
