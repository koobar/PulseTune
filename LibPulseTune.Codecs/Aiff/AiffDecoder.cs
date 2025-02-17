using LibPulseTune.Engine;
using NAudio.Wave;
using System;

namespace LibPulseTune.Codecs.Aiff
{
    public class AiffDecoder : IAudioSource
    {
        // 非公開フィールド
        private AiffFileReader reader;
        private bool isDisposed;

        // コンストラクタ
        public AiffDecoder(string path)
        {
            this.reader = new AiffFileReader(path);
        }

        // デストラクタ
        ~AiffDecoder()
        {
            Dispose();
        }

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.reader.WaveFormat.SampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.reader.WaveFormat.BitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.reader.WaveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.reader.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat;
            }
        }

        #endregion

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.reader.Dispose();
            this.reader = null;
            this.isDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.reader.CurrentTime;
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
        /// デコード
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.reader.Read(buffer, offset, count);
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.reader.CurrentTime = time;
        }
    }
}
