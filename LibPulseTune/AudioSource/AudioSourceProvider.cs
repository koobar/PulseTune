using LibPulseTune.Plugin.Sdk.AudioSource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibPulseTune.AudioSource
{
    public static class AudioSourceProvider
    {
        // 非公開フィールド
        private static readonly Dictionary<string, List<string>> registeredFormats = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> registeredDecoderTypes = new Dictionary<string, Type>();

        /// <summary>
        /// デコーダを登録する。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="extensions"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void RegisterDecoder(string name, Type type, params string[] extensions)
        {
            if (!type.GetInterfaces().Contains(typeof(IAudioSource)))
            {
                throw new InvalidOperationException("IAudioSourceインタフェースを実装していない型をデコーダとして登録しようとしました。");
            }

            if (!registeredFormats.ContainsKey(name))
            {
                registeredFormats.Add(name, new List<string>());
            }

            foreach (var ext in extensions)
            {
                var extension = ext.ToLower();
                
                if (registeredDecoderTypes.ContainsKey(extension))
                {
                    registeredDecoderTypes[extension] = type;
                }
                else
                {
                    registeredDecoderTypes.Add(extension, type);
                }

                // 拡張子を追加
                registeredFormats[name].Add(extension);
            }
        }

        /// <summary>
        /// 指定された拡張子に対応するフォーマット名を取得する。
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetFormatNameFromExtension(string extension)
        {
            extension = extension.ToLower();

            foreach (var name in registeredFormats.Keys)
            {
                if (registeredFormats[name].Contains(extension))
                {
                    return name;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定された名前のフォーマットに対応するファイル名拡張子をすべて取得する。
        /// フォーマット名の大文字と小文字の区別は行わない。
        /// </summary>
        /// <param name="formatName"></param>
        /// <returns></returns>
        public static IList<string> GetRegisteredExtensions(string formatName)
        {
            formatName = formatName.ToLower();

            foreach (var name in registeredFormats.Keys)
            {
                if (name.ToLower() == formatName)
                {
                    return registeredFormats[name];
                }
            }

            return null;
        }

        /// <summary>
        /// デコーダが登録されているフォーマットのフォーマット名をすべて取得する。
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetRegisteredFormatNames()
        {
            return registeredFormats.Keys.ToArray();
        }

        /// <summary>
        /// デコーダが登録されているすべてのフォーマットの拡張子を取得する。
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetAllRegisteredFormatExtensions()
        {
            var result = new List<string>();

            foreach (var formatName in GetRegisteredFormatNames())
            {
                foreach (var extension in GetRegisteredExtensions(formatName))
                {
                    if (!result.Contains(extension))
                    {
                        result.Add(extension);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 指定されたパスのファイルの再生がサポートされているかどうかを判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPlaybackSupportedFileFormat(string path)
        {
            string extension = Path.GetExtension(path).ToLower();

            return registeredDecoderTypes.ContainsKey(extension);
        }

        /// <summary>
        /// 指定されたパスのファイルを開いたIAudioSourceのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IAudioSource CreateAudioSource(string path)
        {
            if (!IsPlaybackSupportedFileFormat(path))
            {
                return null;
            }

            var extension = Path.GetExtension(path).ToLower();
            var type = registeredDecoderTypes[extension];
            var instance = Activator.CreateInstance(type, new object[1] { path });

            return (IAudioSource)instance;
        }
    }
}
