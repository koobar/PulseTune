using LibPulseTune.Codecs.Aiff;
using LibPulseTune.Codecs.Ape;
using LibPulseTune.Codecs.Cd;
using LibPulseTune.Codecs.MediaFoundation;
using LibPulseTune.Codecs.Opus;
using LibPulseTune.Codecs.Vorbis;
using LibPulseTune.Codecs.Wav;
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
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 2, 20);
        public static readonly Version ApplicationVersion = CreateApplicationVersion(1, 4, ApplicationBuildDate);

        private static Version CreateApplicationVersion(int major, int minor, DateTime buildDate)
        {
            return new Version(major, minor, buildDate.Year - 2000, int.Parse($"{buildDate.Month.ToString("00")}{buildDate.Day.ToString("00")}"));
        }

        /// <summary>
        /// 組み込みコーデックを読み込む。
        /// </summary>
        public static void LoadCodecs()
        {
            AudioSourceProvider.RegisterDecoder("AAC", typeof(MediaFoundationDecoder), ".aac");
            AudioSourceProvider.RegisterDecoder("FLAC", typeof(MediaFoundationDecoder), ".flac");
            AudioSourceProvider.RegisterDecoder("MP2", typeof(MediaFoundationDecoder), ".mp2");
            AudioSourceProvider.RegisterDecoder("MP3", typeof(MediaFoundationDecoder), ".mp3");
            AudioSourceProvider.RegisterDecoder("M4A", typeof(MediaFoundationDecoder), ".m4a");
            AudioSourceProvider.RegisterDecoder("Windows Media Audio", typeof(MediaFoundationDecoder), ".wma");

            AudioSourceProvider.RegisterDecoder("AIFF", typeof(AiffDecoder), ".aif", ".aiff");
            AudioSourceProvider.RegisterDecoder("Vorbis", typeof(VorbisDecoder), ".ogg");
            AudioSourceProvider.RegisterDecoder("Opus", typeof(OpusDecoder), ".opus");
            AudioSourceProvider.RegisterDecoder("WAV", typeof(WavDecoder), ".wav");
            AudioSourceProvider.RegisterDecoder("オーディオCDトラック", typeof(CDAudioDecoder), ".cda");

            // Monkey's Audioが使用可能なら登録
            if (ApeDecoder.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("Monkey's Audio", typeof(ApeDecoder), ".ape");
            }

            // WavPackが使用可能なら登録
            if (WavPackDecoder.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("WavPack", typeof(WavPackDecoder), ".wv");
            }

            // ZilophiXが使用可能なら登録
            if (ZilophiXDecoder.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("ZilophiX", typeof(ZilophiXDecoder), ".zpx");
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