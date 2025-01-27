using System;
using System.Collections.Generic;
using System.Linq;

namespace LibPulseTune.Plugin.Sdk.Metadata.Playlist
{
    public class PlaylistReaderInfo
    {
        // 非公開フィールド
        private readonly string formatName;
        private readonly List<string> extensions;
        private readonly Type readerType;

        // コンストラクタ
        public PlaylistReaderInfo(string formatName, Type readerType)
        {
            this.formatName = formatName;
            this.extensions = new List<string>();

            if (readerType == null)
            {
                throw new ArgumentNullException();
            }
            else if (!readerType.GetInterfaces().Contains(typeof(IPlaylistReader)))
            {
                throw new ArgumentException($"プレイリストリーダーを実装した型は、{typeof(IPlaylistReader).Name} インタフェースを継承・実装する必要があります。");
            }

            this.readerType = readerType;
        }

        /// <summary>
        /// このプレイリストリーダーで読み込み可能なフォーマットのフォーマット名を取得します。
        /// </summary>
        public string FormatName
        {
            get
            {
                return this.formatName;
            }
        }

        /// <summary>
        /// プレイリストリーダーの型情報
        /// </summary>
        public Type PlaylistReaderType
        {
            get
            {
                return this.readerType;
            }
        }

        /// <summary>
        /// このプレイリストリーダーで読み込み可能なファイルの拡張子を取得します。
        /// </summary>
        public string[] Extensions
        {
            get
            {
                return this.extensions.ToArray();
            }
        }

        /// <summary>
        /// このプレイリストリーダーで読み込み可能なファイルの拡張子を追加します。
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
