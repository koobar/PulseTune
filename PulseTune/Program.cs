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
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 4, 9);
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
            // Media Foundation（OS組み込みコーデック）でデコードするフォーマットを登録
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_AAC, "AAC", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".aac");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_FLAC, "FLAC", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".flac");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_MP2, "MP2", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".mp2");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_MP3, "MP3", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".mp3");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_M4A, "M4A", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".m4a");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WMA, "Windows Media Audio", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".wma");

            // LibPulseTune.Codecsライブラリに含まれるデコーダでデコードするフォーマットを登録
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_AIFF, "AIFF", typeof(AiffDecoder), null, typeof(GeneralPurposeAudioTrack), ".aif", ".aiff");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_VORBIS, "Vorbis", typeof(VorbisDecoder), null, typeof(VorbisAudioTrack), ".ogg");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_OPUS, "Opus", typeof(OpusDecoder), null, typeof(GeneralPurposeAudioTrack), ".opus");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WAV, "WAV", typeof(WavDecoder), null, typeof(GeneralPurposeAudioTrack), ".wav");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_CDA, "オーディオCDトラック", typeof(CDAudioDecoder), null, typeof(GeneralPurposeAudioTrack), ".cda");

            // Monkey's Audioが使用可能なら登録
            if (ApeDecoder.IsAvailable())
            {
                FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_APE, "Monkey's Audio", typeof(VorbisDecoder), null, typeof(GeneralPurposeAudioTrack), ".ape");
            }

            // WavPackが使用可能なら登録
            if (WavPackDecoder.IsAvailable())
            {
                FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WV, "WavPack", typeof(VorbisDecoder), null, typeof(GeneralPurposeAudioTrack), ".wv");
            }

            // ZilophiXが使用可能なら登録
            if (ZilophiXDecoder.IsAvailable())
            {
                FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_ZPX, "ZilophiX", typeof(ZilophiXDecoder), null, typeof(GeneralPurposeAudioTrack), ".zpx");
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

            // M3Uプレイリストの読み書きを可能にする。
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_M3U, "M3U", typeof(M3UPlaylistReader), typeof(M3UPlaylistWriter), null, ".m3u", ".m3u8");

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