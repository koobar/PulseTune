using PulseTune.Metadata.Playlist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PulseTune.Metadata
{
    internal static class PlaylistReaderProvider
    {
        // 非公開フィールド
        private static readonly Dictionary<string, List<string>> playlistFormatDictionary = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Type> playlistReaderTypeDictionary = new Dictionary<string, Type>();

        /// <summary>
        /// プレイリストリーダーの型を登録する。
        /// </summary>
        /// <param name="formatName"></param>
        /// <param name="readerType"></param>
        /// <param name="extensions"></param>
        public static void RegisterPlaylistReader(string formatName, Type readerType, params string[] extensions)
        {
            if (readerType == null)
            {
                throw new ArgumentNullException(nameof(readerType));
            }

            if (!readerType.GetInterfaces().Contains(typeof(IPlaylistReader)))
            {
                throw new ArgumentException("プレイリストリーダーとして登録する型は、IPlaylistReaderインタフェースを継承・実装する必要があります。");
            }

            if (!playlistFormatDictionary.ContainsKey(formatName))
            {
                playlistFormatDictionary.Add(formatName, new List<string>());
            }

            foreach (var ext in extensions)
            {
                var extension = ext.ToLower();

                if (playlistReaderTypeDictionary.ContainsKey(extension))
                {
                    playlistReaderTypeDictionary[extension] = readerType; 
                }
                else
                {
                    playlistReaderTypeDictionary.Add(extension, readerType);
                }
            }
        }

        /// <summary>
        /// 指定されたパスのプレイリストを読み込むことができるIPlaylistReaderのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IPlaylistReader GetPlaylistReader(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            
            if (playlistReaderTypeDictionary.ContainsKey(extension))
            {
                return (IPlaylistReader)Activator.CreateInstance(playlistReaderTypeDictionary[extension]);
            }

            throw new Exception("指定されたパスのプレイリストはサポートされていない形式です。");
        }
    }
}
