using LibPulseTune;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using System;
using System.Windows.Forms;

namespace PulseTune
{
    internal static class Program
    {
        // アプリケーション情報の定義
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly Version ApplicationVersion = new Version(1, 2);
        public static readonly DateOnly ApplicationBuildDate = new DateOnly(2025, 2, 1);

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // オーディオエンジンを初期化
            AudioEngine.Init();

            // M3Uプレイリストの読み書きを可能にする。
            PlaylistReaderProvider.RegisterPlaylistReader("M3U", typeof(M3UPlaylistReader), ".m3u", ".m3u8");
            PlaylistWriterProvider.RegisterPlaylistWriter("M3U", typeof(M3UPlaylistWriter), ".m3u", ".m3u8");

            // 設定を読み込む。
            OptionManager.Init();
            OptionManager.LoadAllOptions();
            PlaylistExplorerData.Load();

            // メインウィンドウを表示し、メッセージループを開始
            using (var app = new App())
            {
                PluginLoader.LoadPlugins();

                app.Run(args);

                // アプリケーション設定を保存する。
                OptionManager.SaveAllOptions();
                PlaylistExplorerData.Save();
            }
        }
    }
}