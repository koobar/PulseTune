namespace LibPulseTune.Engine
{
    public interface IAudioOutputDevice
    {
        /// <summary>
        /// デバイスのインスタンスを生成する。
        /// </summary>
        /// <returns></returns>
        IAudioPlayer CreateDeviceInstance();

        /// <summary>
        /// このデバイスと指定されたデバイスが同一であるかどうかを判定する。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        bool IsSameDevice(IAudioOutputDevice device);
    }
}
