using LibPulseTune.Metadata.Track;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LibPulseTune.Metadata
{
    public static class AudioTrackProvider
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

        /// <summary>
        /// 指定された拡張子に対応するオーディオトラックの型を取得する。
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
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

        /// <summary>
        /// オーディオトラックのインスタンスを作成するためのコンストラクタを取得する。
        /// </summary>
        /// <param name="typeOfAudioTrack"></param>
        /// <param name="ctorNormal"></param>
        /// <param name="ctorFast"></param>
        private static void GetConstructor(Type typeOfAudioTrack, out ConstructorInfo ctorNormal, out ConstructorInfo ctorFast)
        {
            var ctors = typeOfAudioTrack.GetConstructors();

            ctorNormal = null;
            ctorFast = null;

            foreach (var ctor in ctors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                {
                    ctorNormal = ctor;
                }
                else if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(bool))
                {
                    ctorFast = ctor;
                }
            }
        }

        /// <summary>
        /// 指定されたパスのオーディオトラックのインスタンスを作成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioTrackBase CreateFile(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var type = GetAudioTrackType(extension);

            if (type != null)
            {
                GetConstructor(type, out var normalModeConstructor, out _);

                if (normalModeConstructor != null)
                {
                    return (AudioTrackBase)normalModeConstructor.Invoke(new object[] { path });
                }

                return (AudioTrackBase)Activator.CreateInstance(type, new object[] { path });
            }

            // 対応するオーディオトラックの型が見つからなかった場合は、汎用トラックで読み込んで返す。
            return new GeneralPurposeAudioTrack(path);
        }

        /// <summary>
        /// 指定されたパスのオーディオトラックのインスタンスを高速に作成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioTrackBase CreateFileFast(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var type = GetAudioTrackType(extension);
            
            if (type != null)
            {
                GetConstructor(type, out var normalModeConstructor, out var fastModeConstructor);

                if (fastModeConstructor != null)
                {
                    return (AudioTrackBase)fastModeConstructor.Invoke(new object[] { path, true });
                }

                if (normalModeConstructor != null)
                {
                    return (AudioTrackBase)normalModeConstructor.Invoke(new object[] { path });
                }

                return (AudioTrackBase)Activator.CreateInstance(type, new object[] { path, true });
            }

            // 対応するオーディオトラックの型が見つからなかった場合は、汎用トラックで読み込んで返す。
            return new GeneralPurposeAudioTrack(path, true);
        }

        /// <summary>
        /// カスタムコンストラクタを使用してオーディオトラックのインスタンスを生成する。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
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
