using LibPulseTune.AudioSource;
using LibPulseTune.Helpers;
using NAudio.Wave;
using System;

namespace LibPulseTune
{
    internal class VolumeController : IAudioSource
    {
        // 非公開フィールド
        private readonly IAudioSource source;
        private readonly int bytesPerSample;
        private int volumeValue;
        private float volumeScale;

        // コンストラクタ
        public VolumeController(IAudioSource source)
        {
            this.source = source;
            this.bytesPerSample = this.WaveFormat.BitsPerSample >> 3;
        }

        /// <summary>
        /// 音量（パーセント）
        /// </summary>
        public int Volume
        {
            set
            {
                this.volumeValue = value;
                this.volumeScale = value / 100.0f;
            }
            get
            {
                return this.volumeValue;
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

        public int Read(byte[] buffer, int offset, int count)
        {
            int read = this.source.Read(buffer, offset, count);
            var readSamples = read / this.bytesPerSample;
            int readIndex = offset;
            int writeIndex = offset;

            for (int i = 0; i < readSamples; i++)
            {
                // バッファからサンプルを浮動小数点数として取得
                float sample = BufferHelper.ReadSampleFromBuffer(buffer, ref readIndex, this.WaveFormat);

                // サンプルに音量のスケールを掛ける
                sample = sample * this.volumeScale;

                // 浮動小数点数のサンプルを元のフォーマットでバッファに書き込む。
                BufferHelper.WriteSampleToBuffer(buffer, ref writeIndex, sample, this.WaveFormat);
            }

            return read;
        }

        public void Dispose()
        {
            this.source.Dispose();
            GC.SuppressFinalize(this);
        }

        public TimeSpan GetDuration()
        {
            return this.source.GetDuration();
        }

        public TimeSpan GetCurrentTime()
        {
            return this.source.GetCurrentTime();
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.source.SetCurrentTime(time);
        }
    }
}
