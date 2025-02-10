using LibPulseTune.AudioSource;
using LibPulseTune.AudioSource.Cd;
using LibPulseTune.AudioSource.MediaFoundation;
using LibPulseTune.AudioSource.Vorbis;
using LibPulseTune.AudioSource.WavPack;
using LibPulseTune.AudioSource.ZilophiX;
using System.Collections.Generic;
using System.Text;

namespace LibPulseTune
{
    public class AudioEngine
    {
        // 非公開定数
        private const int AUDIO_OUTPUT_DEVICE_DEFAULT_LATENCY = 256;

        // 非公開フィールド
        private static uint audioOutputDeviceLatency;

        /// <summary>
        /// 指定された拡張子のフィルタ文字列を生成する。
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private static string MakeFilterExtensionString(IList<string> extensions)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < extensions.Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(";");
                }

                sb.Append("*");
                sb.Append(extensions[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 組み込みコーデックを読み込む。
        /// </summary>
        private static void LoadBuiltinCodecs()
        {
            AudioSourceProvider.RegisterDecoder("Vorbis", typeof(VorbisAudioSource), ".ogg");
            AudioSourceProvider.RegisterDecoder("Audio CD Track", typeof(CDAudioSource), ".cda");

            AudioSourceProvider.RegisterDecoder("WAV", typeof(MediaFoundationAudioSource), ".wav");
            AudioSourceProvider.RegisterDecoder("AIFF", typeof(MediaFoundationAudioSource), ".aif", ".aiff");
            AudioSourceProvider.RegisterDecoder("Free Lossless Audio Codec", typeof(MediaFoundationAudioSource), ".flac");
            AudioSourceProvider.RegisterDecoder("MPEG-1 Audio Layer-2", typeof(MediaFoundationAudioSource), ".mp2");
            AudioSourceProvider.RegisterDecoder("MPEG-1 Audio Layer-3", typeof(MediaFoundationAudioSource), ".mp3");
            AudioSourceProvider.RegisterDecoder("M4A", typeof(MediaFoundationAudioSource), ".m4a");
            AudioSourceProvider.RegisterDecoder("Windows Media Audio", typeof(MediaFoundationAudioSource), ".wma");

            // WavPackが使用可能なら登録
            if (WavPackAudioSource.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("WavPack", typeof(WavPackAudioSource), ".wv");
            }

            // ZilophiXが使用可能なら登録
            if (ZilophiXAudioSource.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("ZilophiX", typeof(ZilophiXAudioSource), ".zpx");
            }
        }

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
            LoadBuiltinCodecs();
            SetAudioOutputDeviceLatency(0);

            AudioPlayer.Init();
        }

        /// <summary>
        /// 再生がサポートされているファイルをファイル選択ダイアログで選択するためのフィルタ文字列を取得する。
        /// </summary>
        /// <returns></returns>
        public static string GetSupportedPlaybackFileFormatsDialogFilterString()
        {
            var sb = new StringBuilder();
            var allTypes = MakeFilterExtensionString(AudioSourceProvider.GetAllRegisteredFormatExtensions());

            sb.Append("対応するすべての形式(");
            sb.Append(allTypes);
            sb.Append(")|");
            sb.Append(allTypes);

            foreach (var formatName in AudioSourceProvider.GetRegisteredFormatNames())
            {
                var filter = MakeFilterExtensionString(AudioSourceProvider.GetRegisteredExtensions(formatName));

                sb.Append("|");
                sb.Append(formatName);
                sb.Append("(");
                sb.Append(filter);
                sb.Append(")|");
                sb.Append(filter);
            }

            sb.Append("|");
            sb.Append("すべてのファイル(*.*)|*.*");

            return sb.ToString();
        }
    }
}
