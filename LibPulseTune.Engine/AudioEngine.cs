namespace LibPulseTune.Engine
{
    public class AudioEngine
    {
        // 非公開定数
        private const int AUDIO_OUTPUT_DEVICE_DEFAULT_LATENCY = 256;

        // 非公開フィールド
        private static uint audioOutputDeviceLatency;

        /// <summary>
        /// オーディオ出力デバイスのレイテンシを取得する。
        /// </summary>
        /// <returns></returns>
        public static uint GetAudioOutputDeviceLatency()
        {
            return audioOutputDeviceLatency;
        }

        /// <summary>
        /// オーディオ出力デバイスのレイテンシを設定する。
        /// </summary>
        /// <param name="latency"></param>
        public static void SetAudioOutputDeviceLatency(uint latency)
        {
            if (latency == 0)
            {
                latency = AUDIO_OUTPUT_DEVICE_DEFAULT_LATENCY;
            }

            audioOutputDeviceLatency = latency;
        }

        /// <summary>
        /// ライブラリを初期化する。
        /// </summary>
        public static void Init()
        {
            SetAudioOutputDeviceLatency(0);

            AudioPlayer.Init();
        }
    }
}
