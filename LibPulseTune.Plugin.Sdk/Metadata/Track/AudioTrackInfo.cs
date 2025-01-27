using System;
using System.Collections.Generic;

namespace LibPulseTune.Plugin.Sdk.Metadata.Track
{
    public class AudioTrackInfo
    {
        // 非公開フィールド
        private readonly string formatName;
        private readonly List<string> extensions;
        private readonly Type audioTrackType;

        // コンストラクタ
        public AudioTrackInfo(string formatName, Type audioTrackType)
        {
            this.formatName = formatName;
            this.extensions = new List<string>();

            if (audioTrackType == null)
            {
                throw new ArgumentNullException();
            }
            else if (!audioTrackType.IsSubclassOf(typeof(AudioTrackBase)))
            {
                throw new ArgumentException($"オーディオトラックを実装した型は、{typeof(AudioTrackBase).Name} クラスを継承する必要があります。");
            }

            this.audioTrackType = audioTrackType;
        }

        /// <summary>
        /// このクラスで読み込み可能なフォーマットのフォーマット名を取得します。
        /// </summary>
        public string FormatName
        {
            get
            {
                return this.formatName;
            }
        }

        /// <summary>
        /// オーディオトラックの型情報
        /// </summary>
        public Type AudioTrackType
        {
            get
            {
                return this.audioTrackType;
            }
        }

        /// <summary>
        /// このクラスで読み込み可能なファイルの拡張子を取得します。
        /// </summary>
        public string[] Extensions
        {
            get
            {
                return this.extensions.ToArray();
            }
        }

        /// <summary>
        /// このクラスで読み込み可能なファイルの拡張子を追加します。
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
