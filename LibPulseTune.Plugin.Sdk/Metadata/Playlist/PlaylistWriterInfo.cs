using System;
using System.Collections.Generic;
using System.Linq;

namespace LibPulseTune.Plugin.Sdk.Metadata.Playlist
{
    public class PlaylistWriterInfo
    {
        // 非公開フィールド
        private readonly string formatName;
        private readonly List<string> extensions;
        private readonly Type writerType;

        // コンストラクタ
        public PlaylistWriterInfo(string formatName, Type writerType)
        {
            this.formatName = formatName;
            this.extensions = new List<string>();

            if (writerType == null)
            {
                throw new ArgumentNullException();
            }
            else if (!writerType.GetInterfaces().Contains(typeof(IPlaylistWriter)))
            {
                throw new ArgumentException($"プレイリストライターを実装した型は、{typeof(IPlaylistWriter).Name} インタフェースを継承・実装する必要があります。");
            }

            this.writerType = writerType;
        }

        /// <summary>
        /// このプレイリストライターで書き込み可能なフォーマットのフォーマット名を取得します。
        /// </summary>
        public string FormatName
        {
            get
            {
                return this.formatName;
            }
        }

        /// <summary>
        /// プレイリストライター
        /// </summary>
        public Type PlaylistWriterType
        {
            get
            {
                return this.writerType;
            }
        }

        /// <summary>
        /// このプレイリストライターで書き込み可能なファイルの拡張子を取得します。
        /// </summary>
        public string[] Extensions
        {
            get
            {
                return this.extensions.ToArray();
            }
        }

        /// <summary>
        /// このプレイリストライターで書き込み可能なファイルの拡張子を追加します。
        /// </summary>
        /// <param name="extensions"></param>
        public void AddExtensions(params string[] extensions)
        {
            foreach (string ext in extensions)
            {
                string extension = ext.ToLower();

                if (!this.extensions.Contains(extension))
                {
                    this.extensions.Add(extension);
                }
            }
        }
    }
}
