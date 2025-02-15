using LibPulseTune.CoreAudio;
using System;

namespace LibPulseTune.Options
{
    public static class OptionManager
    {
        // 非公開フィールド
        private static readonly string ApplicationOptionFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PulseTune\\config.conf";
        private static readonly OptionCollection applicationOptions = new OptionCollection();

        #region オーディオデバイス設定

        /// <summary>
        /// オーディオ出力デバイス名
        /// </summary>
        public static string AudioOutputDeviceName
        {
            set
            {
                applicationOptions.SetValue(nameof(AudioOutputDeviceName), value);
            }
            get
            {
                return applicationOptions.GetValue<string>(nameof(AudioOutputDeviceName), null);
            }
        }

        /// <summary>
        /// オーディオ出力デバイスのレイテンシ
        /// </summary>
        public static uint AudioOutputDeviceLatency
        {
            set
            {
                applicationOptions.SetValue(nameof(AudioOutputDeviceLatency), value);
            }
            get
            {
                return applicationOptions.GetValue<uint>(nameof(AudioOutputDeviceLatency), 256);
            }
        }

        /// <summary>
        /// WASAPI排他モードで扱われるデバイスであるかどうかを示す。
        /// </summary>
        public static bool IsWASAPIExclusiveMode
        {
            set
            {
                applicationOptions.SetValue(nameof(IsWASAPIExclusiveMode), value);
            }
            get
            {
                return applicationOptions.GetValue<bool>(nameof(IsWASAPIExclusiveMode), false);
            }
        }

        /// <summary>
        /// WASAPIのイベント同期モードで駆動させるかどうかを示す。
        /// </summary>
        public static bool IsWASAPIEventSyncMode
        {
            set
            {
                applicationOptions.SetValue(nameof(IsWASAPIEventSyncMode), value);
            }
            get
            {
                return applicationOptions.GetValue<bool>(nameof(IsWASAPIEventSyncMode), false);
            }
        }

        /// <summary>
        /// MMCSSを使用するかどうか
        /// </summary>
        public static bool EnableMMCSS
        {
            set
            {
                applicationOptions.SetValue(nameof(EnableMMCSS), value);
            }
            get
            {
                return applicationOptions.GetValue<bool>(nameof(EnableMMCSS), false);
            }
        }

        /// <summary>
        /// 再生スレッドの優先度
        /// </summary>
        public static AvThreadPriority PlaybackThreadPriority
        {
            set
            {
                applicationOptions.SetValue(nameof(PlaybackThreadPriority), value);
            }
            get
            {
                return applicationOptions.GetValue(nameof(PlaybackThreadPriority), AvThreadPriority.Normal);
            }
        }

        /// <summary>
        /// 再生スレッドの特徴
        /// </summary>
        public static MmThreadCharacteristics MmThreadCharacteristics
        {
            set
            {
                applicationOptions.SetValue(nameof(MmThreadCharacteristics), value);
            }
            get
            {
                return applicationOptions.GetValue(nameof(MmThreadCharacteristics), MmThreadCharacteristics.Playback);
            }
        }

        #endregion

        #region UI設定

        /// <summary>
        /// メインウィンドウを常に最前面に表示するかどうかの設定
        /// </summary>
        public static bool MainWindowAlwaysTopMost
        {
            set
            {
                applicationOptions.SetValue(nameof(MainWindowAlwaysTopMost), value);
            }
            get
            {
                return applicationOptions.GetValue<bool>(nameof(MainWindowAlwaysTopMost), false);
            }
        }

        /// <summary>
        /// 波形レンダラの描画モード
        /// </summary>
        public static WaveformRendererStereoViewMode WaveformRendererViewMode
        {
            set
            {
                applicationOptions.SetValue(nameof(WaveformRendererViewMode), value);
            }
            get
            {
                return applicationOptions.GetValue(nameof(WaveformRendererViewMode), WaveformRendererStereoViewMode.Separated);
            }
        }

        #endregion

        #region 再生設定

        /// <summary>
        /// リピートモード
        /// </summary>
        public static RepeatMode RepeatMode
        {
            set
            {
                applicationOptions.SetValue(nameof(RepeatMode), value);
            }
            get
            {
                return applicationOptions.GetValue(nameof(RepeatMode), RepeatMode.Off);
            }
        }

        /// <summary>
        /// 音量
        /// </summary>
        public static int Volume
        {
            set
            {
                applicationOptions.SetValue(nameof(Volume), value);
            }
            get
            {
                return applicationOptions.GetValue<int>(nameof(Volume), 90);
            }
        }

        /// <summary>
        /// シャッフルモード
        /// </summary>
        public static bool ShuffleMode
        {
            set
            {
                applicationOptions.SetValue(nameof(ShuffleMode), value);
            }
            get
            {
                return applicationOptions.GetValue<bool>(nameof(ShuffleMode), false);
            }
        }

        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        public static void Init()
        {
            var defaultDevice = WasapiDevice.GetDefaultDevice();

            // デバイス設定
            AudioOutputDeviceName = defaultDevice.DeviceName;
            IsWASAPIEventSyncMode = defaultDevice.IsWASAPIEventSyncMode;
            IsWASAPIExclusiveMode = defaultDevice.IsWASAPIExclusiveMode;
            AudioOutputDeviceLatency = defaultDevice.Latency;

            // UI設定
            MainWindowAlwaysTopMost = false;

            // 再生設定
            RepeatMode = RepeatMode.Off;
            Volume = 90;
            ShuffleMode = false;
        }

        /// <summary>
        /// すべての設定を保存する。
        /// </summary>
        public static void SaveAllOptions()
        {
            applicationOptions.Save(ApplicationOptionFilePath);
        }

        /// <summary>
        /// すべての設定を読み込む。
        /// </summary>
        public static void LoadAllOptions()
        {
            applicationOptions.Load(ApplicationOptionFilePath);
        }
    }
}
