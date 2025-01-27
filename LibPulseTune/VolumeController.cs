using NAudio.Wave;

namespace LibPulseTune
{
    internal class VolumeController : ISampleProvider
    {
        // 非公開フィールド
        private ISampleProvider source;
        private int volumeValue;
        private float volumeScale;

        /// <summary>
        /// ソース
        /// </summary>
        public ISampleProvider Source
        {
            set
            {
                this.source = value;
            }
            get
            {
                return this.source;
            }
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

        public int Read(float[] buffer, int offset, int count)
        {
            int read = this.source.Read(buffer, offset, count);

            for (int i = 0; i < read; ++i)
            {
                buffer[i] = buffer[i] * this.volumeScale;
            }

            return read;
        }
    }
}
