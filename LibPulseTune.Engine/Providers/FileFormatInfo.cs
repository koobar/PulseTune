using System;
using System.Collections.Generic;
using System.IO;

namespace LibPulseTune.Engine.Providers
{
    internal class FileFormatInfo
    {
        // 非公開フィールド
        private readonly uint formatCode;                           // フォーマットコード
        private readonly string formatName;                         // フォーマット名
        private readonly Type readerType;                           // 読み込み用クラスの型情報
        private readonly Type writerType;                           // 書き込み用クラスの型情報
        private readonly Type metadataReaderType;                   // メタデータ読み込み用クラスの型情報
        private readonly List<string> fileNameExtensions;           // 対応するファイル名拡張子一覧

        // コンストラクタ
        public FileFormatInfo(uint formatCode, string formatName, Type readerType, Type writerType, Type metadataReaderType)
        {
            this.fileNameExtensions = new List<string>();
            this.formatCode = formatCode;
            this.formatName = formatName;
            this.readerType = readerType;
            this.writerType = writerType;
            this.metadataReaderType = metadataReaderType;
         }

        #region プロパティ

        /// <summary>
        /// フォーマットコード
        /// </summary>
        public uint FormatCode
        {
            get
            {
                return this.formatCode;
            }
        }

        /// <summary>
        /// フォーマット名
        /// </summary>
        public string FormatName
        {
            get
            {
                return this.formatName;
            }
        }

        /// <summary>
        /// ファイルの読み込みに使用されるクラスの型情報
        /// </summary>
        public Type ReaderType
        {
            get
            {
                return this.readerType;
            }
        }

        /// <summary>
        /// ファイルの書き込みに使用されるクラスの型情報
        /// </summary>
        public Type WriterType
        {
            get
            {
                return this.writerType;
            }
        }

        /// <summary>
        /// ファイルのメタデータの読み込みに使用されるクラスの型情報
        /// </summary>
        public Type MetadataReaderType
        {
            get
            {
                return this.metadataReaderType;
            }
        }

        /// <summary>
        /// 関連付けられたファイル名拡張子の一覧
        /// </summary>
        public string[] Extensions
        {
            get
            {
                return this.fileNameExtensions.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// 指定されたファイル名拡張子が、この登録情報に関連付けられた拡張子であるかどうかを判定する。
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool ContainsFileNameExtension(string extension)
        {
            if (extension == null)
            {
                return false;
            }

            extension = extension.ToLower();

            return this.fileNameExtensions.Contains(extension);
        }

        /// <summary>
        /// 指定されたパスのファイルが、この登録情報で登録された処理用クラスで処理可能かどうか判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsSupportedFileFormat(string path)
        {
            var extension = Path.GetExtension(path).ToLower();

            return this.fileNameExtensions.Contains(extension);
        }

        /// <summary>
        /// 対応する拡張子を追加する。
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(string extension)
        {
            extension = extension.ToLower();

            if (this.fileNameExtensions.Contains(extension))
            {
                return;
            }

            this.fileNameExtensions.Add(extension);
        }
    }
}
