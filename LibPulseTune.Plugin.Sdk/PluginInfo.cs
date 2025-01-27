using LibPulseTune.Plugin.Sdk.AudioSource;
using LibPulseTune.Plugin.Sdk.Metadata.Playlist;
using LibPulseTune.Plugin.Sdk.Metadata.Track;
using System;
using System.Collections.Generic;

namespace LibPulseTune.Plugin.Sdk
{
    public class PluginInfo
    {
        // 非公開フィールド
        private readonly string name;
        private readonly string author;
        private readonly string description;
        private readonly Version version;
        private readonly List<DecoderInfo> registeredDecoders;
        private readonly List<PlaylistReaderInfo> registeredPlaylistReaders;
        private readonly List<PlaylistWriterInfo> registeredPlaylistWriters;
        private readonly List<AudioTrackInfo> registeredAudioTrackTypes;

        /// <summary>
        /// プラグイン情報を作成します。
        /// </summary>
        /// <param name="name">プラグインの名称</param>
        /// <param name="author">プラグインの作者</param>
        /// <param name="description">プラグインの説明</param>
        /// <param name="pluginVersion">プラグインのバージョン</param>
        public PluginInfo(string name, string author, string description, Version pluginVersion)
        {
            this.name = name;
            this.author = author;
            this.description = description;
            this.version = pluginVersion;
            this.registeredDecoders = new List<DecoderInfo>();
            this.registeredPlaylistReaders = new List<PlaylistReaderInfo>();
            this.registeredPlaylistWriters = new List<PlaylistWriterInfo>();
            this.registeredAudioTrackTypes = new List<AudioTrackInfo>();
        }

        /// <summary>
        /// プラグイン名
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// プラグインの作者名
        /// </summary>
        public string Author
        {
            get
            {
                return this.author;
            }
        }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public Version Version
        {
            get
            {
                return this.version;
            }
        }

        /// <summary>
        /// デコーダを登録します。
        /// </summary>
        /// <param name="info"></param>
        public void RegisterDecoderInfo(DecoderInfo info)
        {
            if (this.registeredDecoders.Contains(info))
            {
                return;
            }

            this.registeredDecoders.Add(info);
        }

        /// <summary>
        /// プレイリストリーダーを登録します。
        /// </summary>
        /// <param name="info"></param>
        public void RegisterPlaylistReader(PlaylistReaderInfo info)
        {
            if (this.registeredPlaylistReaders.Contains(info))
            {
                return;
            }

            this.registeredPlaylistReaders.Add(info);
        }

        /// <summary>
        /// プレイリストライターを登録します。
        /// </summary>
        /// <param name="info"></param>
        public void RegisterPlaylistWriter(PlaylistWriterInfo info)
        {
            if (this.registeredPlaylistWriters.Contains(info))
            {
                return;
            }

            this.registeredPlaylistWriters.Add(info);
        }

        /// <summary>
        /// オーディオトラックの型を登録します。
        /// </summary>
        /// <param name="info"></param>
        public void RegisterAudioTrackType(AudioTrackInfo info)
        {
            if (this.registeredAudioTrackTypes.Contains(info))
            {
                return;
            }

            this.registeredAudioTrackTypes.Add(info);
        }

        /// <summary>
        /// 登録されたデコーダをすべて取得します。
        /// </summary>
        /// <returns></returns>
        public IList<DecoderInfo> GetRegisteredDecoders()
        {
            return this.registeredDecoders;
        }

        /// <summary>
        /// 登録されたプレイリストリーダーをすべて取得します。
        /// </summary>
        /// <returns></returns>
        public IList<PlaylistReaderInfo> GetRegisteredPlaylistReaders()
        {
            return this.registeredPlaylistReaders;
        }

        /// <summary>
        /// 登録されたプレイリストライターをすべて取得します。
        /// </summary>
        /// <returns></returns>
        public IList<PlaylistWriterInfo> GetRegisteredPlaylistWriters()
        {
            return this.registeredPlaylistWriters;
        }

        /// <summary>
        /// 登録されたオーディオトラックの型をすべて取得します。
        /// </summary>
        /// <returns></returns>
        public IList<AudioTrackInfo> GetRegisteredAudioTrackTypes()
        {
            return this.registeredAudioTrackTypes;
        }
    }
}
