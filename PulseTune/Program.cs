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

        // �A�v���P�[�V�������̒�`
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 2, 16);
        public static readonly Version ApplicationVersion = new Version(1, 4, ToBuildNumber(ApplicationBuildDate));

        /// <summary>
        /// �r���h��������r���h�ԍ������߂�B
        /// </summary>
        /// <param name="dateOnly"></param>
        /// <returns></returns>
        private static int ToBuildNumber(DateTime dateOnly)
        {
            string result = string.Empty;

            result += dateOnly.Year.ToString().Substring(1);        // ����̉��񌅂��擾
            result += dateOnly.Month;                               // ����ǉ�
            result += dateOnly.Day;                                 // ����ǉ�

            // �����ɕϊ�
            return int.Parse(result);
        }

        /// <summary>
        /// �g�ݍ��݃R�[�f�b�N��ǂݍ��ށB
        /// </summary>
        public static void LoadCodecs()
        {
            // MediaFoundation�iOS�g�ݍ��݃f�R�[�_�j�Ńf�R�[�h����t�H�[�}�b�g��o�^
            AudioSourceProvider.RegisterDecoder("AAC", typeof(MediaFoundationAudioSource), ".aac");
            AudioSourceProvider.RegisterDecoder("AIFF", typeof(MediaFoundationAudioSource), ".aif", ".aiff");
            AudioSourceProvider.RegisterDecoder("FLAC", typeof(MediaFoundationAudioSource), ".flac");
            AudioSourceProvider.RegisterDecoder("MP2", typeof(MediaFoundationAudioSource), ".mp2");
            AudioSourceProvider.RegisterDecoder("MP3", typeof(MediaFoundationAudioSource), ".mp3");
            AudioSourceProvider.RegisterDecoder("Vorbis", typeof(VorbisAudioSource), ".ogg");
            AudioSourceProvider.RegisterDecoder("M4A", typeof(MediaFoundationAudioSource), ".m4a");
            AudioSourceProvider.RegisterDecoder("WAV", typeof(MediaFoundationAudioSource), ".wav");
            AudioSourceProvider.RegisterDecoder("Windows Media Audio", typeof(MediaFoundationAudioSource), ".wma");

            // �I�[�f�B�ICD�f�R�[�_��o�^
            AudioSourceProvider.RegisterDecoder("�I�[�f�B�ICD�g���b�N", typeof(CDAudioSource), ".cda");

            // WavPack���g�p�\�Ȃ�o�^
            if (WavPackAudioSource.IsAvailable())
            {
                AudioSourceProvider.RegisterDecoder("WavPack", typeof(WavPackAudioSource), ".wv");
            }

            // ZilophiX���g�p�\�Ȃ�o�^
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

            // �I�[�f�B�I�G���W����������
            AudioEngine.Init();
            LoadCodecs();

            // �I�[�f�B�I�g���b�N�̓ǂݍ��ݏ���
            AudioTrackProvider.RegisterGeneralPurposeAudioTrackType(typeof(GeneralPurposeAudioTrack));
            AudioTrackProvider.RegisterAudioTrackType("Vorbis", typeof(VorbisAudioTrack), ".ogg");

            // M3U�v���C���X�g�̓ǂݏ������\�ɂ���B
            PlaylistReaderProvider.RegisterPlaylistReader("M3U", typeof(M3UPlaylistReader), ".m3u", ".m3u8");
            PlaylistWriterProvider.RegisterPlaylistWriter("M3U", typeof(M3UPlaylistWriter), ".m3u", ".m3u8");

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