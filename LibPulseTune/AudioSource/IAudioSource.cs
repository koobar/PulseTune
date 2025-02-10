using System;

namespace LibPulseTune.AudioSource
{
    public interface IAudioSource : IDisposable
    {
        /// <summary>
        /// サンプリング周波数
        /// </summary>
        uint SampleRate { get; }

        /// <summary>
        /// PCMとしてデコードした結果の量子化ビット数
        /// </summary>
        uint BitsPerSample { get; }

        /// <summary>
        /// チャンネル数
        /// </summary>
        uint Channels { get; }

        /// <summary>
        /// 浮動小数点データであるかどうか
        /// </summary>
        bool IsFloat { get; }

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
        /// 指定された時間に再生位置を移動する。
        /// </summary>
        /// <param name="time"></param>
        void SetCurrentTime(TimeSpan time);

        /// <summary>
        /// PCM形式にデコードしたデータを読み込む。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        int Decode(byte[] buffer, int offset, int length);
    }
}
