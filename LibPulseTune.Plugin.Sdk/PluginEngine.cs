using System;
using System.Collections.Generic;

namespace LibPulseTune.Plugin.Sdk
{
    public static class PluginEngine
    {
        // 非公開フィールド
        private static readonly List<PluginInfo> registeredPlugins = new List<PluginInfo>();
        private static readonly Dictionary<Guid, string> pluginLocations = new Dictionary<Guid, string>();

        /// <summary>
        /// プラグインが呼び出し可能なPulseTune APIのAPIレベルを取得します。
        /// </summary>
        public static APILevel APILevel
        {
            get
            {
                return new APILevel(1, 2);
            }
        }

        /// <summary>
        /// PulseTuneにプラグインを登録します。
        /// </summary>
        /// <param name="info"></param>
        public static void RegisterPlugin(PluginInfo info)
        {
            if (registeredPlugins.Contains(info))
            {
                return;
            }

            registeredPlugins.Add(info);
        }

        /// <summary>
        /// 指定されたIDに対応するプラグインのフルパスを取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetPluginFileFullPath(Guid id)
        {
            if (pluginLocations.ContainsKey(id))
            {
                return pluginLocations[id];
            }

            throw new ArgumentException($"指定されたID {id} に関連付けられて読み込まれたプラグインはありません。");
        }

        /// <summary>
        /// PulseTuneのバージョンを取得します。
        /// </summary>
        /// <returns></returns>
        public static Version GetPulseTuneVersion()
        {
            return (Version)SystemCalls.Application.GetPulseTuneVersion.Call();
        }
    }
}
