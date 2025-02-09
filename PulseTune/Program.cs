using LibPulseTune;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using PulseTune.Metadata.Track;
using System;
using System.Windows.Forms;

namespace PulseTune
{
    internal static class Program
    {
        // アプリケーション情報の定義
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly DateOnly ApplicationBuildDate = new DateOnly(2025, 2, 10);
        public static readonly Version ApplicationVersion = new Version(1, 3, ToBuildNumber(ApplicationBuildDate));

        /// <summary>
        /// ビルド日時からビルド番号を求める。
        /// </summary>
        /// <param name="dateOnly"></param>
        /// <returns></returns>
        private static int ToBuildNumber(DateOnly dateOnly)
        {
            string result = string.Empty;

            result += dateOnly.Year.ToString().Substring(1);        // 西暦の下二桁を取得
            result += dateOnly.Month;                               // 月を追加
            result += dateOnly.Day;                                 // 日を追加

            // 整数に変換
            return int.Parse(result);
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // オーディオエンジンを初期化
            AudioEngine.Init();

            // Vorbis専用のオーディオトラックを登録
            AudioTrackProvider.RegisterAudioTrackType("Vorbis", typeof(VorbisAudioTrack), ".ogg");

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