using LibPulseTune.Engine;
using NAudio.Wave;
using System;

namespace LibPulseTune.Codecs.Vorbis
{
    public class VorbisDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly NVorbisReader reader;
        private readonly IWaveProvider waveProviderConverter;

        // コンストラクタ
        public VorbisDecoder(string path)
        {
            this.reader = new NVorbisReader(path);
            this.waveProviderConverter = this.reader.ToWaveProvider();
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

        /// <summary>
        /// 下限ビットレート
        /// </summary>
        public uint LowerBitrate
        {
            get
            {
                return this.reader.LowerBitrate;
            }
        }

        /// <summary>
        /// 公称ビットレート
        /// </summary>
        public uint NominalBitrate
        {
            get
            {
                return this.reader.NominalBitrate;
            }
        }

        /// <summary>
        /// 上限ビットレート
        /// </summary>
        public uint UpperBitrate
        {
            get
            {
                return this.reader.UpperBitrate;
            }
        }

        #endregion

        /// <summary>
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer">デコード結果出力用バッファ</param>
        /// <param name="offset">デコード結果出力用バッファの書き込み開始オフセット</param>
        /// <param name="count">デコード結果出力用バッファに読み込むデータのバイト数</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.waveProviderConverter.Read(buffer, offset, count);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.reader.Dispose();
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.reader.GetCurrentTime();
        }

        /// <summary>
        /// 演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            return this.reader.GetDuration();
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.reader.SetCurrentTime(time);
        }
    }
}
