namespace LibPulseTune.Engine.Playlists
{
    public interface IPlaylistReader
    {
        /// <summary>
        /// 指定されたパスのプレイリストファイルを指定されたプレイリストに読み込む。
        /// </summary>
        /// <param name="path"></param>
        void OpenFile(string path, Playlist playlist);
    }
}
