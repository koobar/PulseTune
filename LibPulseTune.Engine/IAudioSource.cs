using System;

namespace LibPulseTune.Engine
{
    public interface IAudioSource : IDisposable
    {
        /// <summary>
        /// Readメソッドで読み込まれるオーディオデータのサンプリング周波数を示す。
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        /// Readメソッドで読み込まれるオーディオデータの量子化ビット数を示す。
        /// </summary>
        int BitsPerSample { get; }

        /// <summary>
        /// Readメソッドで読み込まれるオーディオデータのチャンネル数を示す。
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// Readメソッドで読み込まれるオーディオデータが浮動小数点数のサンプルデータであるかどうかを示す。
        /// </summary>
        bool IsFloat { get; }

        /// <summary>
        /// デコード
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// このストリームの演奏時間を取得する。
        /// </summary>
        /// <returns></returns>
        TimeSpan GetDuration();

        /// <summary>
        /// このストリームの再生位置を取得する。
        /// </summary>
        /// <returns></returns>
        TimeSpan GetCurrentTime();

        /// <summary>
        /// 指定された時間に再生位置を設定する。
        /// </summary>
        /// <param name="time"></param>
        void SetCurrentTime(TimeSpan time);
    }
}
