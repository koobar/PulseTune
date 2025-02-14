namespace LibPulseTune.Engine
{
    public interface IAudioOutputDevice
    {
        /// <summary>
        /// デバイスのインスタンスを生成する。
        /// </summary>
        /// <returns></returns>
        IAudioPlayer CreateDeviceInstance();

        bool IsSameDevice(IAudioOutputDevice device);
    }
}
