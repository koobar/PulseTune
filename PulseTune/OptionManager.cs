using LibPulseTune.AudioDevice;
using LibPulseTune.Wasapi;
using System;

namespace PulseTune
{
    internal static class OptionManager
    {
        // 非公開フィールド
        private static readonly string ApplicationOptionFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{Program.APPLICATION_NAME}\\config.conf";
        private static readonly string CustomOptionFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{Program.APPLICATION_NAME}\\custom.conf";
        private static readonly OptionCollection applicationOptions = new OptionCollection();
        private static readonly OptionCollection customOptions = new OptionCollection();

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
        /// WASAPIで扱われるデバイスであるかどうかを示す。
        /// </summary>
        public static bool IsWASAPIDevice
        {
            set
            {
                applicationOptions.SetValue(nameof(IsWASAPIDevice), value);
            }
            get
            {
                return applicationOptions.GetValue(nameof(IsWASAPIDevice), false);
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
                return applicationOptions.GetValue<AvThreadPriority>(nameof(PlaybackThreadPriority), AvThreadPriority.Normal);
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
                return applicationOptions.GetValue<MmThreadCharacteristics>(nameof(MmThreadCharacteristics), MmThreadCharacteristics.Playback);
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
            var defaultDevice = AudioOutputDevice.GetDefaultDevice();

            // デバイス設定
            AudioOutputDeviceName = defaultDevice.DeviceName;
            IsWASAPIDevice= defaultDevice.IsWASAPIDevice;
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
        /// カスタムオプションに指定されたキーの値が存在するかどうか判定する。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsCustomOption(string key)
        {
            return customOptions.Contains(key);
        }

        /// <summary>
        /// カスタムオプションの値を設定する。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCustomOption(string key, object value)
        {
            customOptions.SetValue(key, value);
        }

        /// <summary>
        /// カスタムオプションの値を取得する。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static object GetCustomOption(string key, object defaultValue, Type valueType)
        {
            return customOptions.GetValue(key, defaultValue, valueType);
        }

        public static void SaveAllOptions()
        {
            applicationOptions.Save(ApplicationOptionFilePath);
            customOptions.Save(CustomOptionFilePath);
        }

        public static void LoadAllOptions()
        {
            applicationOptions.Load(ApplicationOptionFilePath);
            customOptions.Load(CustomOptionFilePath);
        }
    }
}
