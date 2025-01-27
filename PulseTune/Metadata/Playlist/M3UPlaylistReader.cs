using LibPulseTune.Plugin.Sdk;
using LibPulseTune.Plugin.Sdk.Metadata.Playlist;
using LibPulseTune.Plugin.Sdk.Metadata.Track;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PulseTune.Metadata.Playlist
{
    internal class M3UPlaylistReader : IPlaylistReader
    {
        /// <summary>
        /// 指定されたパスのプレイリストを読み込むためのStreamReaderのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private StreamReader CreateStreamReader(string path)
        {
            var extension = Path.GetExtension(path).ToLower();

            if (extension == ".m3u8")
            {
                return new StreamReader(path, Encoding.UTF8);
            }
            else
            {
                return new StreamReader(path, Encoding.GetEncoding(932));
            }
        }

        public void OpenFile(string path, LibPulseTune.Plugin.Sdk.Metadata.Playlist.Playlist playlist)
        {
            var tracks = new List<AudioTrackBase>();

            // プレイリストを初期化
            playlist.SuspendEvents();
            playlist.Init();
            playlist.Path = path;

            using (var reader = CreateStreamReader(path))
            {
                while (reader.Peek() > -1)
                {
                    string line = reader.ReadLine();

                    if (!line.StartsWith("#"))
                    {
                        if ((bool)SystemCalls.AudioTrack.IsPlaybackSupportedFileFormat.Call(line))
                        {
                            tracks.Add((AudioTrackBase)SystemCalls.AudioTrack.CreateFile.Call(line));
                        }
                        else if (Directory.Exists(line))
                        {
                            foreach (string fileName in Directory.GetFiles(line))
                            {
                                if ((bool)SystemCalls.AudioTrack.IsPlaybackSupportedFileFormat.Call(fileName))
                                {
                                    var track = SystemCalls.AudioTrack.CreateFile.Call(fileName);

                                    if (track != null)
                                    {
                                        tracks.Add((AudioTrackBase)track);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string fullPath = $"{Path.GetDirectoryName(path)}\\{line}";

                            if ((bool)SystemCalls.AudioTrack.IsPlaybackSupportedFileFormat.Call(fullPath))
                            {
                                var track = SystemCalls.AudioTrack.CreateFile.Call(fullPath);

                                if (track != null)
                                {
                                    tracks.Add((AudioTrackBase)track);
                                }
                            }
                        }
                    }
                }

                // ストリームを閉じる。
                reader.Close();
            }

            // 読み込まれたトラックをプレイリストに追加する。
            playlist.Add(tracks.ToArray());

            // 後始末
            playlist.IsEdited = false;
            playlist.ResumeEvents();

            // 変更時のイベントを強制的に発生させる。
            playlist.ForceInvokeChangedEvent();
        }
    }
}
