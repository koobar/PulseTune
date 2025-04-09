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

        // �A�v���P�[�V�������̒�`
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 4, 9);
        public static readonly Version ApplicationVersion = CreateApplicationVersion(1, 4, ApplicationBuildDate);

        private static Version CreateApplicationVersion(int major, int minor, DateTime buildDate)
        {
            return new Version(major, minor, buildDate.Year - 2000, int.Parse($"{buildDate.Month.ToString("00")}{buildDate.Day.ToString("00")}"));
        }

        /// <summary>
        /// �g�ݍ��݃R�[�f�b�N��ǂݍ��ށB
        /// </summary>
        public static void LoadCodecs()
        {
            // Media Foundation�iOS�g�ݍ��݃R�[�f�b�N�j�Ńf�R�[�h����t�H�[�}�b�g��o�^
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_AAC, "AAC", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".aac");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_FLAC, "FLAC", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".flac");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_MP2, "MP2", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".mp2");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_MP3, "MP3", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".mp3");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_M4A, "M4A", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".m4a");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WMA, "Windows Media Audio", typeof(MediaFoundationDecoder), null, typeof(GeneralPurposeAudioTrack), ".wma");

            // LibPulseTune.Codecs���C�u�����Ɋ܂܂��f�R�[�_�Ńf�R�[�h����t�H�[�}�b�g��o�^
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_AIFF, "AIFF", typeof(AiffDecoder), null, typeof(GeneralPurposeAudioTrack), ".aif", ".aiff");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_VORBIS, "Vorbis", typeof(VorbisDecoder), null, typeof(VorbisAudioTrack), ".ogg");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_OPUS, "Opus", typeof(OpusDecoder), null, typeof(GeneralPurposeAudioTrack), ".opus");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WAV, "WAV", typeof(WavDecoder), null, typeof(GeneralPurposeAudioTrack), ".wav");
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_CDA, "�I�[�f�B�ICD�g���b�N", typeof(CDAudioDecoder), null, typeof(GeneralPurposeAudioTrack), ".cda");

            // Monkey's Audio���g�p�\�Ȃ�o�^
            if (ApeDecoder.IsAvailable())
            {
                FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_APE, "Monkey's Audio", typeof(VorbisDecoder), null, typeof(GeneralPurposeAudioTrack), ".ape");
            }

            // WavPack���g�p�\�Ȃ�o�^
            if (WavPackDecoder.IsAvailable())
            {
                FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_WV, "WavPack", typeof(VorbisDecoder), null, typeof(GeneralPurposeAudioTrack), ".wv");
            }

            // ZilophiX���g�p�\�Ȃ�o�^
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

            // �I�[�f�B�I�G���W����������
            AudioEngine.Init();
            LoadCodecs();

            // M3U�v���C���X�g�̓ǂݏ������\�ɂ���B
            FileFormatProvider.RegisterFileFormat(FormatCodes.CODE_M3U, "M3U", typeof(M3UPlaylistReader), typeof(M3UPlaylistWriter), null, ".m3u", ".m3u8");

            // �ݒ��ǂݍ��ށB
            OptionManager.Init();
            OptionManager.LoadAllOptions();
            PlaylistExplorerData.Load();

            // ���C���E�B���h�E��\�����A���b�Z�[�W���[�v���J�n
            using (var app = new App())
            {
                app.Run(args);

                // �A�v���P�[�V�����ݒ��ۑ�����B
                OptionManager.SaveAllOptions();
                PlaylistExplorerData.Save();
            }
        }
    }
}