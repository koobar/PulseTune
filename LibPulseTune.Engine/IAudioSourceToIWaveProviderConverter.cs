using NAudio.Wave;
using System;

namespace LibPulseTune.Engine
{
    internal class IAudioSourceToIWaveProviderConverter : IWaveProvider
    {
        // 非公開フィールド
        private IAudioSource source;
        private WaveFormat waveFormat;

        // コンストラクタ
        public IAudioSourceToIWaveProviderConverter(IAudioSource source)
        {
            this.source = source;

            if (this.source.IsFloat)
            {
                this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(this.source.SampleRate, this.source.Channels);
            }
            else
            {
                this.waveFormat = new WaveFormat(this.source.SampleRate, this.source.BitsPerSample, this.source.Channels);
            }
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return this.waveFormat;
            }
        }

        public bool IsPaused { set; get; }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (this.IsPaused)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }

            return this.source.Read(buffer, offset, count);
        }
    }
}
