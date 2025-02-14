using System.IO;
using System.Text;

namespace LibPulseTune.Metadata.Playlist
{
    public class M3UPlaylistWriter : IPlaylistWriter
    {
        // 非公開フィールド
        private StreamWriter outputStreamWriter;

        public void OpenFile(string path)
        {
            this.outputStreamWriter = CreateStreamWriter(path);
        }

        public void Write(Playlist playlist)
        {
            // #EXTM3Uフラグを書き込む。
            this.outputStreamWriter.WriteLine("#EXTM3U");

            // 各トラックを書き込む。
            for (int i = 0; i < playlist.Count; ++i)
            {
                var track = playlist.GetTrack(i);

                this.outputStreamWriter.WriteLine($"#EXTINF:{(uint)track.Duration.TotalSeconds}, {track.Artist} - {track.Title}");
                this.outputStreamWriter.WriteLine(track.Path);
            }

            // 後始末
            this.outputStreamWriter.Flush();
            this.outputStreamWriter.Close();
        }

        /// <summary>
        /// 指定されたパスにプレイリストを書き込むためのStreamWriterのインスタンスを生成する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static StreamWriter CreateStreamWriter(string path)
        {
            StreamWriter writer = null;
            string extension = Path.GetExtension(path).ToLower();

            if (extension == ".m3u8")
            {
                writer = new StreamWriter(path, false, Encoding.UTF8);
            }
            else
            {
                writer = new StreamWriter(path, false, Encoding.GetEncoding(932));
            }

            return writer;
        }
    }
}
