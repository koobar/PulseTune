﻿using LibPulseTune.Engine;
using NAudio.Wave;
using System;

namespace LibPulseTune.Codecs.MediaFoundation
{
    public class MediaFoundationDecoder : IAudioSource
    {
        // 非公開フィールド
        private readonly MediaFoundationReader source;

        // コンストラクタ
        public MediaFoundationDecoder(string path)
        {
            this.source = new MediaFoundationReader(path);
        }

        #region プロパティ

        public int SampleRate
        {
            get
            {
                return this.source.WaveFormat.SampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.source.WaveFormat.BitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.source.WaveFormat.Channels;
            }
        }

        public bool IsFloat
        {
            get
            {
                return this.source.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat;
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
            return this.source.Read(buffer, offset, count);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            this.source.Dispose();
        }

        /// <summary>
        /// 再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetCurrentTime()
        {
            return this.source.CurrentTime;
        }

        /// <summary>
        /// 演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetDuration()
        {
            return this.source.TotalTime;
        }

        /// <summary>
        /// 再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        public void SetCurrentTime(TimeSpan time)
        {
            this.source.CurrentTime = time;
        }
    }
}
