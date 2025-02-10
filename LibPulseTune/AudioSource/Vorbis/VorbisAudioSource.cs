using NAudio.Vorbis;
using System;

namespace LibPulseTune.AudioSource.Vorbis
{
    public class VorbisAudioSource : IAudioSource
    {
        // 非公開フィールド
        private readonly VorbisWaveReader vorbisWaveReader;

        // コンストラクタ
        public VorbisAudioSource(string path)
        {
            this.vorbisWaveReader = new VorbisWaveReader(path);
        }

        public uint SampleRate
        {
            get
            {
                return (uint)this.vorbisWaveReader.WaveFormat.SampleRate;
            }
        }

        public uint BitsPerSample
        {
            get
            {
                return (uint)this.vorbisWaveReader.WaveFormat.BitsPerSample;
            }
        }

        public uint Channels
        {
            get
            {
                return (uint)this.vorbisWaveReader.WaveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.vorbisWaveReader.WaveFormat.Encoding == NAudio.Wave.WaveFormatEncoding.IeeeFloat;
            }
        }

        public uint LowerBitrate
        {
            get
            {
                return (uint)this.vorbisWaveReader.LowerBitrate;
            }
        }

        public uint NominalBitrate
        {
            get
            {
                return (uint)this.vorbisWaveReader.NominalBitrate;
            }
        }

        public uint UpperBitrate
        {
            get
            {
                return (uint)this.vorbisWaveReader.UpperBitrate;
            }
        }

        public int Decode(byte[] buffer, int offset, int length)
        {
            return this.vorbisWaveReader.Read(buffer, offset, length);
        }

        public void Dispose()
        {
            this.vorbisWaveReader.Dispose();
        }

        public TimeSpan GetCurrentTime()
        {
            return this.vorbisWaveReader.CurrentTime;
        }

        public TimeSpan GetDuration()
        {
            return this.vorbisWaveReader.TotalTime;
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.vorbisWaveReader.CurrentTime = time;
        }
    }
}
