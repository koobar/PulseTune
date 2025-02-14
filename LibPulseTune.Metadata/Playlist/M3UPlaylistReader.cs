using LibPulseTune.Codecs;
using LibPulseTune.Metadata.Track;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibPulseTune.Metadata.Playlist
{
    public class M3UPlaylistReader : IPlaylistReader
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

        public void OpenFile(string path, Playlist playlist)
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
                        if (AudioSourceProvider.IsPlaybackSupportedFileFormat(line))
                        {
                            tracks.Add(AudioTrackProvider.CreateFileFast(line));
                        }
                        else if (Directory.Exists(line))
                        {
                            foreach (string fileName in Directory.GetFiles(line))
                            {
                                if (AudioSourceProvider.IsPlaybackSupportedFileFormat(fileName))
                                {
                                    var track = AudioTrackProvider.CreateFile(fileName);

                                    if (track != null)
                                    {
                                        tracks.Add(track);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string fullPath = $"{Path.GetDirectoryName(path)}\\{line}";

                            if (AudioSourceProvider.IsPlaybackSupportedFileFormat(fullPath))
                            {
                                var track = AudioTrackProvider.CreateFile(fullPath);

                                if (track != null)
                                {
                                    tracks.Add(track);
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
