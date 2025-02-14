using NAudio.Wave;
using System;

namespace LibPulseTune.Engine
{
    public interface IAudioPlayer : IDisposable
    {
        PlaybackState PlaybackState { get; }

        WaveFormat OutputWaveFormat { get; }

        void Play();
        void Stop();
        void Pause();
        void Init(IWaveProvider waveProvider);
    }
}
