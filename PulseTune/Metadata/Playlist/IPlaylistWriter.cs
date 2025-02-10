namespace PulseTune.Metadata.Playlist
{
    internal interface IPlaylistWriter
    {
        void OpenFile(string path);

        void Write(Playlist playlist);
    }
}
