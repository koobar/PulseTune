using LibPulseTune.Engine.Playlists;
using LibPulseTune.Engine.Tracks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LibPulseTune.Engine.Providers
{
    public static class FileFormatProvider
    {
        // 非公開フィールド
        private static readonly List<FileFormatInfo> registeredFormats = new List<FileFormatInfo>();

        /// <summary>
        /// 指定された型情報がデコーダの型情報であり、再生可能なフォーマットとして登録されたものであるかどうか判定する。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsPlaybackSupportedType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.GetInterfaces().Contains(typeof(IAudioSource)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定された型情報がプレイリストの型情報であり、読み込み可能なフォーマットとして登録されたものであるかどうか判定する。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsReadablePlaylistFileType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.GetInterfaces().Contains(typeof(IPlaylistReader)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定された型情報がプレイリストの型情報であり、書き込み可能なフォーマットとして登録されたものであるかどうか判定する。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsWriteablePlaylistFileType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.GetInterfaces().Contains(typeof(IPlaylistWriter)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定された型情報がデコーダの型情報であり、再生可能なフォーマットとして登録されたものであるかどうか判定する。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsMetadataReaderType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsSubclassOf(typeof(AudioTrackBase)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定されたフォーマットコードに対応する登録情報を取得する。
        /// </summary>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        private static FileFormatInfo GetFileFormatInfo(uint formatCode)
        {
            foreach (var info in registeredFormats)
            {
                if (info.FormatCode == formatCode)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定されたフォーマットコードに対応するフォーマットが登録されているかどうか判定する。
        /// </summary>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static bool IsFormatRegistered(uint formatCode)
        {
            foreach (var info in registeredFormats)
            {
                if (info.FormatCode == formatCode)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ファイルフォーマット情報を登録する。
        /// </summary>
        /// <param name="formatCode"></param>
        /// <param name="formatName"></param>
        /// <param name="readerType"></param>
        /// <param name="writerType"></param>
        /// <param name="extensions"></param>
        public static void RegisterFileFormat(uint formatCode, string formatName, Type readerType, Type writerType, Type metadataReaderType, params string[] extensions)
        {
            var info = new FileFormatInfo(formatCode, formatName, readerType, writerType, metadataReaderType);

            foreach (var ext in extensions)
            {
                info.AddExtension(ext);
            }

            registeredFormats.Add(info);
        }

        /// <summary>
        /// 指定されたファイル名拡張子に対応するフォーマットコードを取得する。
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static uint GetFormatCode(string extension)
        {
            foreach (var info in registeredFormats)
            {
                if (info.Extensions.Contains(extension))
                {
                    return info.FormatCode;
                }
            }

            return 0;
        }

        /// <summary>
        /// 指定された拡張子に対応するフォーマット名を取得する。
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetFormatNameFromExtension(string extension)
        {
            var formatCode = GetFormatCode(extension);

            if (formatCode == 0)
            {
                return null;
            }

            return GetFileFormatInfo(formatCode).FormatName;
        }

        /// <summary>
        /// 指定されたフォーマットコードに対応するファイル名拡張子をすべて取得する。
        /// </summary>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static IList<string> GetRegisteredExtensions(uint formatCode)
        {
            foreach (var info in registeredFormats)
            {
                if (info.FormatCode == formatCode)
                {
                    return info.Extensions;
                }
            }

            return null;
        }

        /// <summary>
        /// 登録済みのフォーマットコードをすべて取得する。
        /// </summary>
        /// <returns></returns>
        public static IList<uint> GetRegisteredFormatCodes()
        {
            var result = new List<uint>();

            foreach (var info in registeredFormats)
            {
                if (!result.Contains(info.FormatCode))
                {
                    result.Add(info.FormatCode);
                }
            }

            return result;
        }

        /// <summary>
        /// 再生可能なファイルとして登録されたフォーマットに対応するフォーマットコードをすべて取得する。
        /// </summary>
        /// <returns></returns>
        public static IList<uint> GetRegisteredPlaybackSupportedFormatCodes()
        {
            var result = new List<uint>();

            foreach (var info in registeredFormats)
            {
                if (IsPlaybackSupportedType(info.ReaderType))
                {
                    if (!result.Contains(info.FormatCode))
                    {
                        result.Add(info.FormatCode);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 指定されたフォーマットコードに対応するフォーマット名を取得する。
        /// </summary>
        /// <param name="formatCode"></param>
        /// <returns></returns>
        public static string GetFormatName(uint formatCode)
        {
            return GetFileFormatInfo(formatCode).FormatName;
        }

        /// <summary>
        /// 登録されたファイルフォーマットのうち、再生可能なフォーマットのファイル名拡張子をすべて取得する。
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetAllPlaybackSupportedFileFormatExtensions()
        {
            var result = new List<string>();

            foreach (var code in GetRegisteredPlaybackSupportedFormatCodes())
            {
                var info = GetFileFormatInfo(code);

                foreach (var ext in info.Extensions)
                {
                    if (!result.Contains(ext))
                    {
                        result.Add(ext);
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
            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());

            if (formatCode == 0)
            {
                return false;
            }

            var info = GetFileFormatInfo(formatCode);
            return IsPlaybackSupportedType(info.ReaderType);
        }

        /// <summary>
        /// 指定されたパスのファイルがプレイリストであり、かつ読み込み可能なフォーマットであるかどうかを判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsReadablePlaylistFileFormat(string path)
        {
            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());

            if (formatCode == 0)
            {
                return false;
            }

            var info = GetFileFormatInfo(formatCode);
            return IsReadablePlaylistFileType(info.ReaderType);
        }

        /// <summary>
        /// 指定されたパスのファイルがプレイリストであり、かつ書き込み可能なフォーマットであるかどうかを判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsWriteablePlaylistFileFormat(string path)
        {
            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());

            if (formatCode == 0)
            {
                return false;
            }

            var info = GetFileFormatInfo(formatCode);
            return IsWriteablePlaylistFileType(info.WriterType);
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

            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());
            var info = GetFileFormatInfo(formatCode);
            var instance = Activator.CreateInstance(info.ReaderType, new object[1] { path });

            return (IAudioSource)instance;
        }

        /// <summary>
        /// 指定されたパスのプレイリストを読み込むことができるIPlaylistReaderのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IPlaylistReader CreatePlaylistReader(string path)
        {
            if (!IsReadablePlaylistFileFormat(path))
            {
                return null;
            }

            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());
            var info = GetFileFormatInfo(formatCode);
            var instance = Activator.CreateInstance(info.ReaderType);

            return (IPlaylistReader)instance;
        }

        /// <summary>
        /// 指定されたパスのプレイリストを読み込むことができるIPlaylistReaderのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IPlaylistWriter CreatePlaylistWriter(string path)
        {
            if (!IsWriteablePlaylistFileFormat(path))
            {
                return null;
            }

            var formatCode = GetFormatCode(Path.GetExtension(path).ToLower());
            var info = GetFileFormatInfo(formatCode);
            var instance = Activator.CreateInstance(info.WriterType);

            return (IPlaylistWriter)instance;
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
        public static AudioTrackBase CreateAudioTrackFromFile(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var formatCode = GetFormatCode(extension);

            if (formatCode == 0)
            {
                return null;
            }

            var info = GetFileFormatInfo(formatCode);
            
            if (IsMetadataReaderType(info.MetadataReaderType))
            {
                GetConstructor(info.MetadataReaderType, out var normalModeConstructor, out _);

                if (normalModeConstructor != null)
                {
                    return (AudioTrackBase)normalModeConstructor.Invoke(new object[] { path });
                }

                return (AudioTrackBase)Activator.CreateInstance(info.MetadataReaderType, new object[] { path });
            }

            return null;
        }

        /// <summary>
        /// 指定されたパスのオーディオトラックのインスタンスを高速に作成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioTrackBase CreateAudioTrackFromFileFast(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var formatCode = GetFormatCode(extension);

            if (formatCode == 0)
            {
                return null;
            }

            var info = GetFileFormatInfo(formatCode);

            if (IsMetadataReaderType(info.MetadataReaderType))
            {
                GetConstructor(info.MetadataReaderType, out var normalModeConstructor, out var fastModeConstructor);

                if (fastModeConstructor != null)
                {
                    return (AudioTrackBase)fastModeConstructor.Invoke(new object[] { path, true });
                }

                if (normalModeConstructor != null)
                {
                    return (AudioTrackBase)normalModeConstructor.Invoke(new object[] { path });
                }

                return (AudioTrackBase)Activator.CreateInstance(info.MetadataReaderType, new object[] { path, true });
            }

            return null;
        }

        /// <summary>
        /// カスタムコンストラクタを使用してオーディオトラックのインスタンスを生成する。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static AudioTrackBase CreateAudioTrackWithCustomConstructor(object type, object[] parameters)
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
