using NAudio.Wave;

namespace LibPulseTune.AudioSource
{
    internal class AudioSourceToWaveProviderConverter : IWaveProvider
    {
        // 非公開フィールド
        private readonly IAudioSource source;

        // コンストラクタ
        public AudioSourceToWaveProviderConverter(IAudioSource source)
        {
            this.source = source;
        }

        public WaveFormat WaveFormat
        {
            get
            {
                if (this.source.IsFloat)
                {
                    return WaveFormat.CreateIeeeFloatWaveFormat((int)this.source.SampleRate, (int)this.source.Channels);
                }

                return new WaveFormat((int)this.source.SampleRate, (int)this.source.BitsPerSample, (int)this.source.Channels);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return this.source.Decode(buffer, offset, count);
        }
    }
}
