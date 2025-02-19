using LibPulseTune.Engine;
using NAudio.Wave;
using System;
using static LibPulseTune.Codecs.ZilophiX.ZilophiXInterop;

namespace LibPulseTune.Codecs.ZilophiX
{
    public class ZilophiXDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly IntPtr decoder;
        private readonly WaveFormat waveFormat;
        private readonly uint numTotalSamples;
        private readonly int bytesPerSample;
        private bool isSeeking;
        private bool isDisposed;

        // コンストラクタ
        public ZilophiXDecoder(string path)
        {
            LoadLibrary();

            this.decoder = ZpXCreateDecoder(path);
            this.numTotalSamples = ZpXGetNumTotalSamples(this.decoder);
            this.waveFormat = new WaveFormat((int)ZpXGetSampleRate(this.decoder), (int)ZpXGetBitsPerSample(this.decoder), (int)ZpXGetChannels(this.decoder));
            this.bytesPerSample = this.waveFormat.BitsPerSample >> 3;
            this.isSeeking = false;
        }

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.waveFormat.SampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.waveFormat.BitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.waveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.waveFormat.Encoding == WaveFormatEncoding.IeeeFloat;
            }
        }

        private double SamplesPerMilliseconds
        {
            get
            {
                return (this.waveFormat.SampleRate * this.waveFormat.Channels) / 1000.0;
            }
        }

        #endregion

        /// <summary>
        /// ZilophiXのデコーダが使用可能であるかどうか判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsAvailable()
        {
            return ZilophiXInterop.IsAvailable();
        }

        public TimeSpan GetDuration()
        {
            var result = TimeSpan.FromMilliseconds(ZpXGetNumTotalSamples(this.decoder) / this.SamplesPerMilliseconds);

            return result;
        }

        public TimeSpan GetCurrentTime()
        {
            var pos = ZpXGetSampleOffset(this.decoder);
            var result = TimeSpan.FromMilliseconds(pos / this.SamplesPerMilliseconds);

            return result;
        }

        public void SetCurrentTime(TimeSpan time)
        {
            this.isSeeking = true;

            ZpXSeekTo(this.decoder, (uint)time.TotalMilliseconds);

            this.isSeeking = false;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            ZpXCloseFile(this.decoder);
            ZpXFreeDecoder(this.decoder);
            this.isDisposed = true;
        }

        /// <summary>
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer">デコード結果出力用バッファ</param>
        /// <param name="offset">デコード結果出力用バッファの書き込み開始オフセット</param>
        /// <param name="count">デコード結果出力用バッファに読み込むデータのバイト数</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            // シーク中にデータの読み込みを行うと、保護されたメモリ領域からPCMデータを
            // 読み込もうとして例外が発生するため、シーク中は無音データを返すこととする。
            if (this.isSeeking)
            {
                // 無音データを格納
                for (int i = offset; i < buffer.Length; ++i)
                {
                    buffer[i] = 0;
                }

                return count;
            }

            int result = offset;

            while (result < count)
            {
                if (ZpXGetSampleOffset(this.decoder) >= this.numTotalSamples)
                {
                    break;
                }

                int sample = ZpXReadSample(this.decoder);

                unsafe
                {
                    fixed(void* pBuffer = &buffer[offset])
                    {
                        Buffer.MemoryCopy(&sample, pBuffer, buffer.Length, this.bytesPerSample);
                    }
                }

                // 後始末
                offset += this.bytesPerSample;
                result += this.bytesPerSample;
            }

            return result;
        }
    }
}
