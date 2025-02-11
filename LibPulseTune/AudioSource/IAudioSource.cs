using System;
using NAudio.Wave;

namespace LibPulseTune.AudioSource
{
    public interface IAudioSource : IWaveProvider, IDisposable
    {
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
