using LibPulseTune.AudioSource;
using NAudio.Wave;
using System;
using static LibPulseTune.Helpers.BufferHelper;

namespace LibPulseTune
{
    public class AmplitudeMonitor : IAudioSource
    {
        // 非公開フィールド
        private readonly IAudioSource source;           // 入力ソース
        private readonly int samplesPerMillisecond;     // 1ミリ秒あたりのサンプル数
        private readonly float[][] buffers;             // チャンネルごとの振幅バッファ
        private readonly int[] bufferIndexes;           // チャンネルごとの振幅バッファのアクセスインデックス
        private readonly object lockObj;                // 共有オブジェクトのロック用オブジェクト
        private readonly int bytesPerSample;

        // コンストラクタ
        public AmplitudeMonitor(IAudioSource source)
        {
            this.source = source;
            this.samplesPerMillisecond = (int)Math.Round(this.source.WaveFormat.SampleRate / 1000.0);
            this.buffers = new float[source.WaveFormat.Channels][];
            this.bufferIndexes = new int[source.WaveFormat.Channels];
            this.lockObj = new object();
            this.bytesPerSample = this.WaveFormat.BitsPerSample >> 3;

            for (int ch = 0; ch < source.WaveFormat.Channels; ++ch)
            {
                this.buffers[ch] = new float[128];
            }
        }

        #region IAudioSourceの実装

        /// <summary>
        /// フォーマット
        /// </summary>
        public WaveFormat WaveFormat
        {
            get
            {
                return this.source.WaveFormat;
            }
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            lock (this.lockObj)
            {
                int read = this.source.Read(buffer, offset, count);
                var readSamples = read / this.bytesPerSample;
                int samplesPerCh = readSamples / this.WaveFormat.Channels;

                // バッファのサイズとチャンネルごとのサンプル数が不一致ならバッファをリサイズ
                if (samplesPerCh != this.buffers[0].Length)
                {
                    for (int ch = 0; ch < this.WaveFormat.Channels; ++ch)
                    {
                        Array.Resize(ref this.buffers[ch], samplesPerCh);
                        Array.Clear(this.buffers[ch], 0, samplesPerCh);
                    }
                }

                int index = offset;
                for (int i = 0; i < samplesPerCh; ++i)
                {
                    for (int ch = 0; ch < this.WaveFormat.Channels; ++ch)
                    {
                        this.buffers[ch][i] = Math.Abs(ReadSampleFromBuffer(buffer, ref index, this.WaveFormat));
                    }
                }

                // 各チャンネルのバッファへのアクセスインデックスを初期化
                for (int ch = 0; ch < this.WaveFormat.Channels; ++ch)
                {
                    this.bufferIndexes[ch] = 0;
                }

                return read;
            }
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.source.Dispose();
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.source.GetCurrentTime();
        }

        /// <summary>
        /// 曲の長さを取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            return this.source.GetDuration();
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.source.SetCurrentTime(time);
        }

        #endregion

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
            for (int ch = 0; ch < this.source.WaveFormat.Channels; ++ch)
            {
                Array.Clear(this.buffers[ch], 0, this.buffers[ch].Length);
            }
        }
    }
}
