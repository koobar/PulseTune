using NAudio.Vorbis;
using NAudio.Wave;
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

        /// <summary>
        /// フォーマット
        /// </summary>
        public WaveFormat WaveFormat
        {
            get
            {
                return this.vorbisWaveReader.WaveFormat;
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

        /// <summary>
        /// オーディオデータを読み込む。
        /// </summary>
        /// <param name="buffer">デコード結果出力用バッファ</param>
        /// <param name="offset">デコード結果出力用バッファの書き込み開始オフセット</param>
        /// <param name="count">デコード結果出力用バッファに読み込むデータのバイト数</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int length)
        {
            return this.vorbisWaveReader.Read(buffer, offset, length);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.vorbisWaveReader.Dispose();
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.vorbisWaveReader.CurrentTime;
        }

        /// <summary>
        /// 演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            return this.vorbisWaveReader.TotalTime;
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.vorbisWaveReader.CurrentTime = time;
        }
    }
}
