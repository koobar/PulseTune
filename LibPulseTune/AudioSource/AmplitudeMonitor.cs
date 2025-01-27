using LibPulseTune.Plugin.Sdk.AudioSource;
using System;

namespace LibPulseTune.AudioSource
{
    public class AmplitudeMonitor : IAudioSource
    {
        // 非公開フィールド
        private readonly IAudioSource source;           // 入力ソース
        private readonly int samplesPerMillisecond;     // 1ミリ秒あたりのサンプル数
        private readonly float[][] buffers;             // チャンネルごとの振幅バッファ
        private readonly int[] bufferIndexes;           // チャンネルごとの振幅バッファのアクセスインデックス
        private readonly object lockObj;                // 共有オブジェクトのロック用オブジェクト

        // コンストラクタ
        public AmplitudeMonitor(IAudioSource source)
        {
            this.source = source;
            this.samplesPerMillisecond = (int)Math.Round(this.source.SampleRate / 1000.0);
            this.buffers = new float[source.Channels][];
            this.bufferIndexes = new int[source.Channels];
            this.lockObj = new object();

            for (int ch = 0; ch < source.Channels; ++ch)
            {
                this.buffers[ch] = new float[128];
            }
        }

        #region IAudioSourceの実装

        public uint SampleRate
        {
            get
            {
                return this.source.SampleRate;
            }
        }

        public uint BitsPerSample
        {
            get
            {
                return this.source.BitsPerSample;
            }
        }

        public uint Channels
        {
            get
            {
                return this.source.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.source.IsFloat;
            }
        }

        public int Decode(byte[] buffer, int offset, int length)
        {
            lock (this.lockObj)
            {
                int read = this.source.Decode(buffer, offset, length);
                var readSapmles = read / (this.BitsPerSample / 8);
                int samplesPerCh = (int)(readSapmles / this.Channels);

                // バッファのサイズとチャンネルごとのサンプル数が不一致ならバッファをリサイズ
                if (samplesPerCh != this.buffers[0].Length)
                {
                    for (int ch = 0; ch < this.Channels; ++ch)
                    {
                        Array.Resize(ref this.buffers[ch], samplesPerCh);
                        Array.Clear(this.buffers[ch], 0, samplesPerCh);
                    }
                }

                int index = offset;
                for (int i = 0; i < samplesPerCh; ++i)
                {
                    for (int ch = 0; ch < this.Channels; ++ch)
                    {
                        this.buffers[ch][i] = Math.Abs(ReadSampleFromBuffer(buffer, ref index));
                    }
                }

                // 各チャンネルのバッファへのアクセスインデックスを初期化
                for (int ch = 0; ch < this.Channels; ++ch)
                {
                    this.bufferIndexes[ch] = 0;
                }

                return read;
            }
        }

        public void Dispose()
        {
            this.source.Dispose();
        }

        public TimeSpan GetCurrentTime()
        {
            return this.source.GetCurrentTime();
        }

        public TimeSpan GetDuration()
        {
            return this.source.GetDuration();
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.source.SetCurrentTime(time);
        }

        #endregion

        /// <summary>
        /// 指定されたバッファの指定されたオフセット以降から次の1サンプルを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private float ReadSampleFromBuffer(byte[] buffer, ref int offset)
        {
            int sample = 0;
            int m = 0;

            switch (this.BitsPerSample)
            {
                case 8:
                    m = 128;
                    sample = buffer[offset++] - 128;
                    break;
                case 16:
                    m = 32767;
                    sample = (sbyte)buffer[offset + 1] << 8 | buffer[offset];
                    offset += 2;
                    break;
                case 24:
                    m = 8388607;
                    sample = (sbyte)buffer[offset + 2] << 16 | (buffer[offset + 1] << 8) | buffer[offset];
                    offset += 3;
                    break;
                case 32:
                    if (this.IsFloat)
                    {
                        return BitConverter.ToSingle(new byte[] { buffer[offset++], buffer[offset++], buffer[offset++], buffer[offset++] }, 0);
                    }
                    else
                    {
                        m = int.MaxValue;
                        sample = BitConverter.ToInt32(new byte[] { buffer[offset++], buffer[offset++], buffer[offset++], buffer[offset++] }, 0);
                        offset += 4;
                    }
                    break;
            }

            return sample / (float)m;
        }

        /// <summary>
        /// 指定されたチャンネルのバッファ済みサンプルデータのうち、指定されたミリ秒だけ進んだ時間までの最大振幅を取得する。
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public float GetAmplitude(int channel, double milliseconds)
        {
            lock (this.lockObj)
            {
                int take = (int)Math.Round(this.samplesPerMillisecond * milliseconds);
                int endIndex = this.bufferIndexes[channel] + take;
                int bufferMax = this.buffers[channel].Length - 1;
                float max = 0;

                // 読み込み終了インデックスがバッファの領域外なら領域内にとどめる。
                if (endIndex >= bufferMax)
                {
                    endIndex = bufferMax;
                }

                for (int i = this.bufferIndexes[channel]; i < endIndex; ++i)
                {
                    if (this.buffers[channel][i] > max)
                    {
                        max = this.buffers[channel][i];
                        if (max == 1.0f)
                        {
                            break;
                        }
                    }
                }

                this.bufferIndexes[channel] = endIndex;
                return max;
            }
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            for (int ch = 0; ch < this.source.Channels; ++ch)
            {
                Array.Clear(this.buffers[ch], 0, this.buffers[ch].Length);
            }
        }
    }
}
