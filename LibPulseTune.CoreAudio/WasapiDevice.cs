﻿using LibPulseTune.Engine;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Text;

namespace LibPulseTune.CoreAudio
{
    public class WasapiDevice : IAudioOutputDevice
    {
        // 非公開フィールド
        private readonly string deviceName;
        private readonly bool isWasapiEventSync;
        private readonly bool isWasapiExclusiveMode;
        private readonly uint latency;

        // コンストラクタ
        public WasapiDevice(
            string deviceName, 
            bool isWasapiEventSync,
            bool isWasapiExclusiveMode,
            uint latency)
        {
            this.deviceName = deviceName;
            this.isWasapiEventSync = isWasapiEventSync;
            this.isWasapiExclusiveMode = isWasapiExclusiveMode;
            this.latency = latency;
        }

        #region プロパティ
        
        /// <summary>
        /// デバイス名
        /// </summary>
        public string DeviceName
        {
            get
            {
                return this.deviceName;
            }
        }

        /// <summary>
        /// WASAPI排他モードで使用されるデバイスであるかどうかを示す。
        /// </summary>
        public bool IsWASAPIExclusiveMode
        {
            get
            {
                return this.isWasapiExclusiveMode;
            }
        }

        /// <summary>
        /// WASAPIイベントモードであるかどうかを示す。
        /// </summary>
        public bool IsWASAPIEventSyncMode
        {
            get
            {
                return this.isWasapiEventSync;
            }
        }

        /// <summary>
        /// 出力レイテンシ
        /// </summary>
        public uint Latency
        {
            get
            {
                return this.latency;
            }
        }

        /// <summary>
        /// MMCSSを使用するかどうか
        /// </summary>
        public bool EnableMMCSS { set; get; }

        /// <summary>
        /// 再生スレッドの特徴
        /// </summary>
        public MmThreadCharacteristics ThreadCharacteristics { set; get; }

        /// <summary>
        /// 再生スレッドの優先度
        /// </summary>
        public AvThreadPriority PlaybackThreadPriority { set; get; }

        #endregion

        /// <summary>
        /// 利用可能なデバイスをすべて取得する。
        /// </summary>
        /// <returns></returns>
        public static WasapiDevice[] GetAvailableDevices()
        {
            var result = new List<WasapiDevice>();
            
            // WASAPIで利用可能なデバイスをすべて取得する。
            using (var enumerator = new MMDeviceEnumerator())
            {
                // 共有モード（イベント駆動）
                foreach (var mmdevice in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    result.Add(new WasapiDevice(mmdevice.FriendlyName, true, false, AudioEngine.GetAudioOutputDeviceLatency()));
                }

                // 共有モード（プッシュ駆動）
                foreach (var mmdevice in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    result.Add(new WasapiDevice(mmdevice.FriendlyName, false, false, AudioEngine.GetAudioOutputDeviceLatency()));
                }

                // 排他モード（イベント駆動）
                foreach (var mmdevice in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    result.Add(new WasapiDevice(mmdevice.FriendlyName, true, true, AudioEngine.GetAudioOutputDeviceLatency()));
                }

                // 排他モード（プッシュ駆動）
                foreach (var mmdevice in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    result.Add(new WasapiDevice(mmdevice.FriendlyName, false, true, AudioEngine.GetAudioOutputDeviceLatency()));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// デフォルトデバイスを取得する。
        /// </summary>
        /// <returns></returns>
        public static WasapiDevice GetDefaultDevice()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                return new WasapiDevice(device.FriendlyName, true, false, AudioEngine.GetAudioOutputDeviceLatency());
            }
        }

        /// <summary>
        /// 指定された名前のMMDeviceを取得する。
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private MMDevice GetMMDevice(string deviceName)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    if (device.FriendlyName == deviceName)
                    {
                        return device;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// デバイスのインスタンスを生成する。
        /// </summary>
        /// <returns></returns>
        public IAudioPlayer CreateDeviceInstance()
        {
            var shareMode = this.isWasapiExclusiveMode ? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared;
            var device = GetMMDevice(this.deviceName);

            if (device != null)
            {
                var result = new CustomWasapiOut(device, shareMode, this.isWasapiEventSync, (int)this.latency);
                result.EnableMMCSS = this.EnableMMCSS;
                result.MmThreadCharacteristics = this.ThreadCharacteristics;
                result.PlaybackAvThreadPriority = this.PlaybackThreadPriority;

                return result;
            }

            return null;
        }

        /// <summary>
        /// このデバイスが指定されたデバイスと同一であるかどうか判定する。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool IsSameDevice(IAudioOutputDevice device)
        {
            if (device is WasapiDevice)
            {
                var wasapi = device as WasapiDevice;

                if (this.deviceName == wasapi.deviceName &&
                    this.isWasapiEventSync == wasapi.isWasapiEventSync &&
                    this.isWasapiExclusiveMode == wasapi.isWasapiExclusiveMode)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!this.isWasapiExclusiveMode)
            {
                sb.Append("[共有モード] ");
            }
            else if (this.isWasapiExclusiveMode)
            {
                sb.Append("[排他モード] ");
            }

            sb.Append(this.deviceName);

            if (this.isWasapiEventSync)
            {
                sb.Append(" (Event)");
            }
            else
            {
                sb.Append(" (Push)");
            }

            return sb.ToString();
        }
    }
}
