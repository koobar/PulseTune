namespace LibPulseTune.Engine
{
    public interface IAudioOutputDevice
    {
        /// <summary>
        /// 一時停止がサポートされているかどうかを示す。<br/>
        /// 一時停止がサポートされていない環境では、AudioPlayer側で、一時停止を「無音データの再生」に置き換えて再現します。
        /// </summary>
        bool IsPauseSupported { get; }

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
