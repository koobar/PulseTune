using LibPulseTune.Plugin.Sdk.AudioSource;
using NAudio.Wave;
using System;

namespace LibPulseTune.AudioSource.MediaFoundation
{
    public class MediaFoundationAudioSource : IAudioSource
    {
        // 非公開フィールド
        private readonly MediaFoundationReader source;

        // コンストラクタ
        public MediaFoundationAudioSource(string path)
        {
            this.source = new MediaFoundationReader(path);
        }

        public uint SampleRate
        {
            get
            {
                return (uint)this.source.WaveFormat.SampleRate;
            }
        }

        public uint BitsPerSample
        {
            get
            {
                return (uint)this.source.WaveFormat.BitsPerSample;
            }
        }

        public uint Channels
        {
            get
            {
                return (uint)this.source.WaveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                if (this.source.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    return true;
                }

                return false;
            }
        }

        public int Decode(byte[] buffer, int offset, int length)
        {
            return this.source.Read(buffer, offset, length);
        }

        public void Dispose()
        {
            this.source.Dispose();
        }

        public TimeSpan GetCurrentTime()
        {
            return this.source.CurrentTime;
        }

        public TimeSpan GetDuration()
        {
            return this.source.TotalTime;
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.source.CurrentTime = time;
        }
    }
}
