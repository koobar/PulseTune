using LibPulseTune.Plugin.Sdk.Metadata.Track;
using PulseTune.Metadata.Track;
using System;
using System.Collections.Generic;
using System.IO;

namespace PulseTune.Metadata
{
    internal static class AudioTrackProvider
    {
        // 非公開フィールド
        private static readonly Dictionary<string, List<string>> audioTrackFormatDictionary = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> audioTrackTypeDictionary = new Dictionary<string, Type>();

        /// <summary>
        /// オーディオトラックの型を登録する。
        /// </summary>
        /// <param name="formatName"></param>
        /// <param name="type"></param>
        /// <param name="extensions"></param>
        public static void RegisterAudioTrackType(string formatName, Type type, params string[] extensions)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsSubclassOf(typeof(AudioTrackBase)))
            {
                throw new ArgumentException($"オーディオトラックとして登録する型は、{typeof(AudioTrackBase).Name} クラスを継承する必要があります。");
            }

            if (!audioTrackFormatDictionary.ContainsKey(formatName))
            {
                audioTrackFormatDictionary.Add(formatName, new List<string>());
            }

            foreach (var ext in extensions)
            {
                var extension = ext.ToLower();

                if (audioTrackTypeDictionary.ContainsKey(extension))
                {
                    audioTrackTypeDictionary[extension] = type;
                }
                else
                {
                    audioTrackTypeDictionary.Add(extension, type);
                }
            }
        }

        public static Type GetAudioTrackType(string extension)
        {
            extension = extension.ToLower();

            // オーディオトラックの型辞書に登録されていれば、その型のインスタンスを生成して返す。
            if (audioTrackTypeDictionary.ContainsKey(extension))
            {
                return audioTrackTypeDictionary[extension];
            }

            return null;
        }

        public static AudioTrackBase CreateFile(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var type = GetAudioTrackType(extension);

            if (type != null)
            {
                return (AudioTrackBase)Activator.CreateInstance(type, new object[] { path });
            }

            // 対応するオーディオトラックの型が見つからなかった場合は、汎用トラックで読み込んで返す。
            return new GeneralPurposeAudioTrack(path);
        }

        public static AudioTrackBase CreateFileFast(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var type = GetAudioTrackType(extension);

            if (type != null)
            {
                return (AudioTrackBase)Activator.CreateInstance(type, new object[] { path, true });
            }

            // 対応するオーディオトラックの型が見つからなかった場合は、汎用トラックで読み込んで返す。
            return new GeneralPurposeAudioTrack(path, true);
        }

        public static AudioTrackBase CreateUseCustomConstructor(object type, object[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var t = (Type)type;

            if (!t.IsSubclassOf(typeof(AudioTrackBase)))
            {
                throw new ArgumentException($"オーディオトラックとして使用する型は、{typeof(AudioTrackBase).Name} クラスを継承する必要があります。");
            }

            return (AudioTrackBase)Activator.CreateInstance(t, parameters);
        }
    }
}
