using NAudio.Wave;
using System;

namespace LibPulseTune.Engine
{
    public interface IAudioPlayer : IDisposable
    {
        /// <summary>
        /// 再生状況
        /// </summary>
        PlaybackState PlaybackState { get; }

        /// <summary>
        /// 出力フォーマット
        /// </summary>
        WaveFormat OutputWaveFormat { get; }

        /// <summary>
        /// 再生
        /// </summary>
        void Play();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 一時停止
        /// </summary>
        void Pause();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="waveProvider"></param>
        void Init(IWaveProvider waveProvider);
    }
}
