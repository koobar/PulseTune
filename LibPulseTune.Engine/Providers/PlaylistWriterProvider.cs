using LibPulseTune.Engine.Playlists;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibPulseTune.Engine.Providers
{
    public static class PlaylistWriterProvider
    {
        // 非公開フィールド
        private static readonly Dictionary<string, List<string>> playlistFormatDictionary = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> playlistWriterTypeDictionary = new Dictionary<string, Type>();

        /// <summary>
        /// プレイリストライターの型を登録する。
        /// </summary>
        /// <param name="formatName"></param>
        /// <param name="readerType"></param>
        /// <param name="extensions"></param>
        public static void RegisterPlaylistWriter(string formatName, Type readerType, params string[] extensions)
        {
            if (readerType == null)
            {
                throw new ArgumentNullException(nameof(readerType));
            }

            if (!readerType.GetInterfaces().Contains(typeof(IPlaylistWriter)))
            {
                throw new ArgumentException("プレイリストライターとして登録する型は、IPlaylistWriterインタフェースを継承・実装する必要があります。");
            }

            if (!playlistFormatDictionary.ContainsKey(formatName))
            {
                playlistFormatDictionary.Add(formatName, new List<string>());
            }

            foreach (var ext in extensions)
            {
                var extension = ext.ToLower();

                if (playlistWriterTypeDictionary.ContainsKey(extension))
                {
                    playlistWriterTypeDictionary[extension] = readerType; 
                }
                else
                {
                    playlistWriterTypeDictionary.Add(extension, readerType);
                }
            }
        }

        public static IPlaylistWriter GetPlaylistReader(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            
            if (playlistWriterTypeDictionary.ContainsKey(extension))
            {
                return (IPlaylistWriter)Activator.CreateInstance(playlistWriterTypeDictionary[extension]);
            }

            throw new Exception("指定されたパスのプレイリストはサポートされていない形式です。");
        }
    }
}
