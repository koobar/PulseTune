using LibPulseTune.Codecs;
using LibPulseTune.Database;
using LibPulseTune.Engine;
using LibPulseTune.Metadata;
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
        public static readonly DateTime ApplicationBuildDate = new DateTime(2025, 2, 15);
        public static readonly Version ApplicationVersion = new Version(1, 3, ToBuildNumber(ApplicationBuildDate));

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

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // �I�[�f�B�I�G���W����������
            AudioEngine.Init();
            AudioSourceProvider.Init();

            // Vorbis��p�̃I�[�f�B�I�g���b�N��o�^
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