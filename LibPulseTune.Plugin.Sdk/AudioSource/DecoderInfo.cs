using System;
using System.Collections.Generic;
using System.Linq;

namespace LibPulseTune.Plugin.Sdk.AudioSource
{
    public class DecoderInfo
    {
        // 非公開フィールド
        private readonly string formatName;
        private readonly List<string> extensions;
        private readonly Type decoderType;

        // コンストラクタ
        public DecoderInfo(string formatName, Type decoderType)
        {
            this.formatName = formatName;
            this.extensions = new List<string>();

            if (decoderType == null)
            {
                throw new ArgumentNullException();
            }
            else if (!decoderType.GetInterfaces().Contains(typeof(IAudioSource)))
            {
                throw new ArgumentException($"デコーダを実装した型は、{typeof(IAudioSource).Name} インタフェースを継承・実装する必要があります。");
            }

            this.decoderType = decoderType;
        }

        /// <summary>
        /// このデコーダでデコード可能なフォーマットのフォーマット名を取得します。
        /// </summary>
        public string FormatName
        {
            get
            {
                return this.formatName;
            }
        }

        /// <summary>
        /// デコーダの型情報
        /// </summary>
        public Type DecoderType
        {
            get
            {
                return this.decoderType;
            }
        }

        /// <summary>
        /// このデコーダでデコード可能なファイルの拡張子を取得します。
        /// </summary>
        public string[] Extensions
        {
            get
            {
                return this.extensions.ToArray();
            }
        }

        /// <summary>
        /// このデコーダでデコード可能なファイルの拡張子を追加します。
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
