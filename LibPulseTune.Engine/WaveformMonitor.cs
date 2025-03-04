using NAudio.Wave;
using System;

namespace LibPulseTune.Engine
{
    internal class WaveformMonitor : ISampleProvider
    {
        // 非公開フィールド
        private readonly ISampleProvider source;
        private readonly double samplesPerMillisecond;
        private readonly float[][] buffers;
        private readonly int[] bufferIndexes;
        private readonly float[] amplitudes;
        private readonly float[][] waveforms;
        private readonly object lockObj;

        // コンストラクタ
        public WaveformMonitor(IWaveProvider source)
        {
            this.source = source.ToSampleProvider();
            this.samplesPerMillisecond = this.source.WaveFormat.SampleRate / 1000.0;
            this.buffers = new float[source.WaveFormat.Channels][];
            this.bufferIndexes = new int[source.WaveFormat.Channels];
            this.amplitudes = new float[source.WaveFormat.Channels];
            this.waveforms = new float[source.WaveFormat.Channels][];
            this.lockObj = new object();

            for (int ch = 0; ch < source.WaveFormat.Channels; ++ch)
            {
                this.buffers[ch] = new float[128];
                this.waveforms[ch] = new float[128];
            }
        }

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
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(float[] buffer, int offset, int count)
        {
            lock (this.lockObj)
            {
                int read = this.source.Read(buffer, offset, count);
                int samplesPerCh = read / this.WaveFormat.Channels;

                // バッファのサイズとチャンネルごとのサンプル数が不一致ならバッファをリサイズ
                if (samplesPerCh != this.buffers[0].Length)
                {
                    for (int ch = 0; ch < this.WaveFormat.Channels; ++ch)
                    {
                        Array.Resize(ref this.buffers[ch], samplesPerCh);
                        Array.Clear(this.buffers[ch], 0, samplesPerCh);
                    }
                }

                // チャンネルごとにサンプルを分離してバッファに格納する。
                int index = offset;
                for (int i = 0; i < samplesPerCh; ++i)
                {
                    for (int ch = 0; ch < this.WaveFormat.Channels; ++ch)
                    {
                        this.buffers[ch][i] = buffer[index++];
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
        /// 指定されたミリ秒分のデータをバッファから読み込む。
        /// </summary>
        /// <param name="milliseconds"></param>
        public void LoadData(double milliseconds)
        {
            lock (this.lockObj)
            {
                for (int channel = 0; channel < this.source.WaveFormat.Channels; channel++)
                {
                    int take = (int)Math.Round(this.samplesPerMillisecond * milliseconds);
                    int startIndex = this.bufferIndexes[channel];
                    int endIndex = this.bufferIndexes[channel] + take;
                    int bufferMax = this.buffers[channel].Length - 1;
                    float max = 0;

                    // 読み込み終了インデックスがバッファの領域外なら領域内にとどめる。
                    if (endIndex >= bufferMax)
                    {
                        endIndex = bufferMax;
                    }

                    int length = endIndex - startIndex;
                    if (length > 0 && this.waveforms[channel].Length != length)
                    {
                        Array.Resize(ref this.waveforms[channel], length);
                    }

                    for (int i = startIndex, j = 0; i < endIndex; ++i, ++j)
                    {
                        float sample = this.buffers[channel][i];
                        float abs = Math.Abs(sample);

                        this.waveforms[channel][j] = sample;

                        if (abs > max)
                        {
                            max = abs;
                        }
                    }

                    this.amplitudes[channel] = max;
                    this.bufferIndexes[channel] = endIndex;
                }
            }
        }

        /// <summary>
        /// 読み込まれたデータから、指定されたチャンネルの最大振幅を取得する。
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public float GetAmplitude(int channel)
        {
            lock (this.lockObj)
            {
                return this.amplitudes[channel];
            }
        }

        public float GetMaximumInstantaneousDecibels(int channel)
        {
            return ToDecibels(GetAmplitude(channel));
        }

        /// <summary>
        /// 読み込まれたデータから、指定されたチャンネルの波形を取得する。
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public float[] GetWaveform(int channel)
        {
            lock (this.lockObj)
            {
                return this.waveforms[channel];
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

        private float ToDecibels(float amplitude)
        {
            if (amplitude <= 0)
            {
                amplitude = float.Epsilon;
            }

            return 20 * (float)Math.Log10(amplitude);
        }
    }
}
