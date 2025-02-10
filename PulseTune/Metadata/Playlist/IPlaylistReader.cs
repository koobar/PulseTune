namespace PulseTune.Metadata.Playlist
{
    internal interface IPlaylistReader
    {
        /// <summary>
        /// 指定されたパスのプレイリストファイルを指定されたプレイリストに読み込みます。
        /// </summary>
        /// <param name="path"></param>
        void OpenFile(string path, Playlist playlist);
    }
}
