namespace PulseTune.Metadata.Playlist
{
    internal interface IPlaylistWriter
    {
        /// <summary>
        /// 指定されたパスのファイルを開く。
        /// </summary>
        /// <param name="path"></param>
        void OpenFile(string path);

        /// <summary>
        /// 指定されたプレイリストを書き込む。
        /// </summary>
        /// <param name="playlist"></param>
        void Write(Playlist playlist);
    }
}
