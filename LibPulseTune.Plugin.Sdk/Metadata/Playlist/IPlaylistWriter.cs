namespace LibPulseTune.Plugin.Sdk.Metadata.Playlist
{
    public interface IPlaylistWriter
    {
        void OpenFile(string path);

        void Write(Playlist playlist);
    }
}
