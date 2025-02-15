using LibPulseTune.Codecs.Cd;
using LibPulseTune.Codecs.MediaFoundation;
using LibPulseTune.Codecs.Vorbis;
using LibPulseTune.Codecs.WavPack;
using LibPulseTune.Codecs.ZilophiX;
using LibPulseTune.Database;
using LibPulseTune.Engine;
using LibPulseTune.Engine.Providers;
using LibPulseTune.Metadata.Playlist;
using LibPulseTune.Metadata.Track;
using LibPulseTune.Options;
using System;
using System.Windows.Forms;

namespace PulseTune
{
    internal static class Program
    {
#if DEBUG
        public const string BUILD_TYPE = @"DEBUG";
#else
        public const string BUILD_TYPE = @"RELEASE";
#endif

        // アプリケーション情報の定義
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 2, 16);
        public static readonly Version ApplicationVersion = new Version(1, 4, ToBuildNumber(ApplicationBuildDate));

        /// <summary>
        /// ビルド日時からビルド番号を求める。
        /// </summary>
        /// <param name="dateOnly"></param>
        /// <returns></returns>
        private static int ToBuildNumber(DateTime dateOnly)
        {
            string result = string.Empty;

            result += dateOnly.Year.ToString().Substring(1);        // 西暦の下二桁を取得
            result += dateOnly.Month;                               // 月を追加
            result += dateOnly.Day;                                 // 日を追加

            // 整数に変換
            return int.Parse(result);
        }

        /// <summary>
        /// 組み込みコーデックを読み込む。
        /// </summary>
        public static void LoadCodecs()
        {
            // MediaFoundation（OS組み込みデコーダ）でデコードするフォーマットを登録
            AudioSourceProvider.RegisterDecoder("AAC", typeof(MediaFoundationAudioSource), ".aac");
            AudioSourceProvider.RegisterDecoder("AIFF", typeof(MediaFoundationAudioSource), ".aif", ".aiff");
            AudioSourceProvider.RegisterDecoder("FLAC", typeof(MediaFoundationAudioSource), ".flac");
            AudioSourceProvider.RegisterDecoder("MP2", typeof(MediaFoundationAudioSource), ".mp2");
            AudioSourceProvider.RegisterDecoder("MP3", typeof(MediaFoundationAudioSource), ".mp3");
            AudioSourceProvider.RegisterDecoder("Vorbis", typeof(VorbisAudioSource), ".ogg");
            AudioSourceProvider.RegisterDecoder("M4A", typeof(MediaFoundationAudioSource), ".m4a");
            AudioSourceProvider.RegisterDecoder("WAV", typeof(MediaFoundationAudioSource), ".wav");
            AudioSourceProvider.RegisterDecoder("Windows Media Audio", typeof(MediaFoundationAudioSource), ".wma");

            // オーディオCDデコーダを登録
            AudioSourceProvider.RegisterDecoder("オーディオCDトラック", typeof(CDAudioSource), ".cda");

            // WavPackが使用可能なら登録
            if (WavPackAudioSource.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("WavPack", typeof(WavPackAudioSource), ".wv");
            }

            // ZilophiXが使用可能なら登録
            if (ZilophiXAudioSource.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("ZilophiX", typeof(ZilophiXAudioSource), ".zpx");
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // オーディオエンジンを初期化
            AudioEngine.Init();
            LoadCodecs();

            // オーディオトラックの読み込み準備
            AudioTrackProvider.RegisterGeneralPurposeAudioTrackType(typeof(GeneralPurposeAudioTrack));
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
                app.Run(args);

                // アプリケーション設定を保存する。
                OptionManager.SaveAllOptions();
                PlaylistExplorerData.Save();
            }
        }
    }
}