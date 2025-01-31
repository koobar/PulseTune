using LibPulseTune;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using System;
using System.Windows.Forms;

namespace PulseTune
{
    internal static class Program
    {
        // �A�v���P�[�V�������̒�`
        public const string APPLICATION_NAME = @"PulseTune";
        public static readonly Version ApplicationVersion = new Version(1, 2);
        public static readonly DateOnly ApplicationBuildDate = new DateOnly(2025, 2, 1);

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // �I�[�f�B�I�G���W����������
            AudioEngine.Init();

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
                PluginLoader.LoadPlugins();

                app.Run(args);

                // �A�v���P�[�V�����ݒ��ۑ�����B
                OptionManager.SaveAllOptions();
                PlaylistExplorerData.Save();
            }
        }
    }
}