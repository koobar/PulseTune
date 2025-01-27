using LibPulseTune.AudioSource;
using LibPulseTune.Plugin.Sdk;
using PulseTune.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PulseTune
{
    public static class PluginLoader
    {
        // 非公開定数
        private const string PLUGIN_ENTRY_POINT_NAME = @"PluginMain";
        private const string NAME_OF_PLUGIN_LOCATIONS_FIELD = @"pluginLocations";
        private const string NAME_OF_REGISTERED_PLUGINS_FIELD = @"registeredPlugins";

        /// <summary>
        /// システムコールの準備を行う。
        /// </summary>
        private static void InitSystemCalls()
        {
            SystemCalls.Application.GetPulseTuneVersion = new SystemCall(() =>
            {
                return Program.ApplicationVersion;
            });
            SystemCalls.Application.ContainsCustomOptionValue = new SystemCall((object key) =>
            {
                return OptionManager.ContainsCustomOption(key.ToString());
            });
            SystemCalls.Application.SetCustomOptionValue = new SystemCall((object key, object value) =>
            {
                OptionManager.SetCustomOption(key.ToString(), value);
            });
            SystemCalls.Application.GetCustomOptionValue = new SystemCall((object key, object defaultValue, object valueType) =>
            {
                return OptionManager.GetCustomOption(key.ToString(), defaultValue, (Type)valueType);
            });

            SystemCalls.AudioSource.CreateAudioSource = new SystemCall((object path) =>
            {
                return AudioSourceProvider.CreateAudioSource(path.ToString());
            });

            SystemCalls.AudioTrack.CreateFile = new SystemCall((object path) =>
            {
                return AudioTrackProvider.CreateFile(path.ToString());
            });
            SystemCalls.AudioTrack.CreateUseCustomConstructor = new SystemCall((object type, object parameters) =>
            {
                if (!(type is Type))
                {
                    throw new ArgumentException($"このシステムコールの第1引数は、{typeof(Type).FullName} 型の値が必要です。");
                }

                if (!(parameters is object[]))
                {
                    throw new ArgumentException($"このシステムコールの第2引数は、{typeof(object).FullName} 型の一次元配列が必要です。");
                }

                var t = (Type)type;
                var args = (object[])parameters;

                return AudioTrackProvider.CreateUseCustomConstructor(t, args);
            });
            SystemCalls.AudioTrack.IsPlaybackSupportedFileFormat = new SystemCall((object path) =>
            {
                var fileName = path.ToString();

                if (File.Exists(fileName) && AudioSourceProvider.IsPlaybackSupportedFileFormat(fileName))
                {
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// 指定された型から指定された名称の非公開かつ静的なフィールドを取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static T GetStaticField<T>(Type type, string fieldName)
        {
            var field = typeof(PluginEngine).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);

            if (field == null)
            {
                throw new Exception($"プラグインの読み込みに失敗しました。プラグインSDKに含まれる型 {type.FullName} に、フィールド {fieldName} が実装されていません。");
            }
            else if (!field.IsStatic)
            {
                throw new Exception($"プラグインの読み込みに失敗しました。フィールド {fieldName} が静的ではありません。");
            }

            return (T)field.GetValue(null);
        }

        /// <summary>
        /// 登録されたすべてのプラグインを取得する。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IList<PluginInfo> GetAllRegisteredPlugins()
        {
            return GetStaticField<IList<PluginInfo>>(typeof(PluginEngine), NAME_OF_REGISTERED_PLUGINS_FIELD);
        }

        /// <summary>
        /// プラグインが配置された場所の辞書を取得する。
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, string> GetPluginLocationDictionary()
        {
            return GetStaticField<Dictionary<Guid, string>>(typeof(PluginEngine), NAME_OF_PLUGIN_LOCATIONS_FIELD);
        }

        /// <summary>
        /// プラグインとして実装された各種拡張機能を登録する。
        /// </summary>
        private static void RegisterPluginExtensions()
        {
            var plugins = GetAllRegisteredPlugins();

            foreach (var plugin in plugins)
            {
                foreach (var decoder in plugin.GetRegisteredDecoders())
                {
                    AudioSourceProvider.RegisterDecoder(decoder.FormatName, decoder.DecoderType, decoder.Extensions);
                }

                foreach (var reader in plugin.GetRegisteredPlaylistReaders())
                {
                    PlaylistReaderProvider.RegisterPlaylistReader(reader.FormatName, reader.PlaylistReaderType, reader.Extensions);
                }

                foreach (var writer in plugin.GetRegisteredPlaylistWriters())
                {
                    PlaylistWriterProvider.RegisterPlaylistWriter(writer.FormatName, writer.PlaylistWriterType, writer.Extensions);
                }

                foreach (var audioTrack in plugin.GetRegisteredAudioTrackTypes())
                {
                    AudioTrackProvider.RegisterAudioTrackType(audioTrack.FormatName, audioTrack.AudioTrackType, audioTrack.Extensions);
                }
            }
        }

        #region プラグインの読み込み用メソッド

        /// <summary>
        /// 実行環境のプロセッサのアーキテクチャを取得する。
        /// </summary>
        /// <returns></returns>
        private static ProcessorArchitecture GetProcessorArchitecture()
        {
            if (IntPtr.Size == 4)
            {
                return ProcessorArchitecture.X86;
            }
            else if (IntPtr.Size == 8)
            {
                return ProcessorArchitecture.Amd64;
            }

            return ProcessorArchitecture.X86;
        }

        /// <summary>
        /// 指定されたパスのDLLがアセンブリであるかどうかを判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsAssembly(string path)
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(path);

                // プラグインが要求するプロセッサのアーキテクチャと、実行環境のプロセッサのアーキテクチャが合致するか？
                if (asmName.ProcessorArchitecture == ProcessorArchitecture.MSIL ||
                    asmName.ProcessorArchitecture == GetProcessorArchitecture())
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 指定されたアセンブリからプラグインのエントリポイントを取得する。
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException">プラグイン内で複数のエントリポイントが定義されていた場合に発生する</exception>
        private static MethodInfo GetPluginEntryPoint(Assembly asm)
        {
            var entryPoints = new List<MethodInfo>();

            foreach (var type in asm.GetTypes())
            {
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();

                    if (method.Name == PLUGIN_ENTRY_POINT_NAME &&
                        method.IsStatic &&
                        parameters.Length == 1 &&
                        parameters[0].ParameterType == typeof(Guid) &&
                        method.ReturnType == typeof(void))
                    {
                        entryPoints.Add(method);
                    }
                }
            }

            if (entryPoints.Count == 0)
            {
                return null;
            }
            else if (entryPoints.Count == 1)
            {
                return entryPoints[0];
            }
            else
            {
                throw new InvalidProgramException("プラグインに複数のエントリポイントの候補が見つかりました。エントリポイントは、各プラグインに1つだけ存在できます。");
            }
        }

        /// <summary>
        /// 指定されたパスのプラグインを読み込んでエントリポイントを呼び出す。
        /// </summary>
        /// <param name="path"></param>
        private static void RunPlugin(string path)
        {
            if (!IsAssembly(path))
            {
                return;
            }

            // アセンブリを読み込んでプラグインのエントリポイントを取得する。
            var asm = Assembly.LoadFile(path);
            var entryPoint = GetPluginEntryPoint(asm);

            // エントリポイントが見つからない場合、プラグインではないただのアセンブリとみなす。
            if (entryPoint == null)
            {
                return;
            }

            // プラグインの固有IDを生成
            var pluginID = Guid.NewGuid();

            // プラグインの配置場所の辞書に追加
            var dict = GetPluginLocationDictionary();
            dict.Add(pluginID, path);

            // エントリポイントを呼び出す。
            try
            {
                entryPoint.Invoke(null, new object[] { pluginID });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// すべてのプラグインを読み込む。
        /// </summary>
        public static void LoadPlugins()
        {
            var pluginDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\{Program.APPLICATION_NAME}\\plug-ins";
            var dllFiles = new List<string>();

            if (!Directory.Exists(pluginDir))
            {
                Directory.CreateDirectory(pluginDir);
            }

            // プラグインフォルダ直下に配置されたDLLを列挙
            foreach (var path in Directory.GetFiles(pluginDir))
            {
                var lowerPath = path.ToLower();
                if (lowerPath.EndsWith(".dll"))
                {
                    dllFiles.Add(lowerPath);
                }
            }

            // プラグイン毎にフォルダに分けて配置されたDLLを列挙
            foreach (var path in Directory.GetDirectories(pluginDir))
            {
                foreach (var fileName in Directory.GetFiles(path))
                {
                    var lowerPath = fileName.ToLower();
                    if (lowerPath.EndsWith(".dll"))
                    {
                        dllFiles.Add(lowerPath);
                    }
                }
            }

            // プラグインSDKを初期化
            InitSystemCalls();

            // 列挙されたDLLを読み込む。
            foreach (var path in dllFiles)
            {
                RunPlugin(path);
            }

            // プラグインに実装されたデコーダを登録する。
            RegisterPluginExtensions();
        }

        #endregion
    }
}
