using LibPulseTune;
using LibPulseTune.AudioDevice;
using LibPulseTune.AudioSource;
using Microsoft.WindowsAPICodePack.Dialogs;
using PulseTune.Controls;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using PulseTune.Metadata.Track;
using PulseTune.Properties;
using PulseTune.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PulseTune.Dialogs
{
    internal partial class MainWindow : Form
    {
        // ����J�R�}���h
        private readonly Command playCommand;
        private readonly Command pauseCommand;
        private readonly Command resumeCommand;
        private readonly Command stopCommand;
        private readonly Command backwardCommand;
        private readonly Command forwardCommand;
        private readonly Command moveToStartCommand;

        // ����J�t�B�[���h
        private readonly ExplorerControl explorerViewer;    // �G�N�X�v���[���[�R���g���[��
        private readonly ClosableTabPage explorerTabPage;   // �G�N�X�v���[���[�R���g���[�����z�u���ꂽ�^�u�y�[�W
        private AudioTrackBase currentTrack;                // �Đ����̃g���b�N

        // �R���X�g���N�^
        public MainWindow()
        {
            InitializeComponent();

            // �R�}���h�̏���
            this.playCommand = new Command(PlayCommandImplementation);
            this.pauseCommand = new Command(PauseCommandImplementation);
            this.resumeCommand = new Command(ResumeCommandImplementation);
            this.stopCommand = new Command(StopCommandImplementation);
            this.backwardCommand = new Command(BackwardCommandImplementation);
            this.forwardCommand = new Command(ForwardCommandImplementation);
            this.moveToStartCommand = new Command(MoveToStartCommandImplementation);

            // ���f�B�A�R���g���[���p�l���̃R�}���h�̐ݒ�
            this.ControlPanel.PlayCommand = this.playCommand;
            this.ControlPanel.PauseCommand = this.pauseCommand;
            this.ControlPanel.ResumeCommand = this.resumeCommand;
            this.ControlPanel.StopCommand = this.stopCommand;
            this.ControlPanel.BackwardCommand = this.backwardCommand;
            this.ControlPanel.ForwardCommand = this.forwardCommand;
            this.ControlPanel.MoveToTrackStartCommand = this.moveToStartCommand;

            // �G�N�X�v���[���̃^�u�y�[�W���쐬
            this.explorerViewer = new ExplorerControl();
            this.explorerViewer.Dock = DockStyle.Fill;
            this.explorerViewer.ContextMenu = CreateExplorerControlContextMenu(this.explorerViewer);
            this.explorerViewer.FileDoubleClick += OnExplorerViewerFileDoubleClick;
            this.explorerTabPage = new ClosableTabPage("�G�N�X�v���[��", false);
            this.explorerTabPage.Control = this.explorerViewer;

            // �^�X�N�o�[�̃T���l�C���Ƀ��f�B�A�R���g���[���{�^����\������B
            this.ControlPanel.ShowTaskBarThumbnailButtons(this);

            // �t�H���g��ݒ�
            this.Font = SystemFonts.CaptionFont;

            AudioPlayer.AudioSourceReachEnd += OnAudioSourceReachEnd;
            AudioPlayer.PlaybackPositionChanged += OnPlaybackPositionChanged;
        }

        /// <summary>
        /// �Đ��R�}���h�̎���
        /// </summary>
        /// <param name="track"></param>
        private void PlayCommandImplementation(object unused, object track)
        {
            if (track == null)
            {
                if (this.TabSelectedPlaylist == null)
                {
                    return;
                }

                if (this.TabSelectedPlaylist.SelectedTrack == null)
                {
                    this.TabSelectedPlaylist.SelectNextTrack();
                }

                track = this.TabSelectedPlaylist.SelectedTrack;
            }

            if (track == null)
            {
                return;
            }

            Play((AudioTrackBase)track);
        }

        /// <summary>
        /// �ꎞ��~�R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void PauseCommandImplementation(object unused1, object unused2)
        {
            Pause();
        }

        /// <summary>
        /// �ĊJ�R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void ResumeCommandImplementation(object unused1, object unused2)
        {
            Resume();
        }

        /// <summary>
        /// ��~�R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void StopCommandImplementation(object unused1, object unused2)
        {
            Stop();
        }

        /// <summary>
        /// ������R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void ForwardCommandImplementation(object unused1, object unused2)
        {
            Forward();
        }

        /// <summary>
        /// �����߂��R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void BackwardCommandImplementation(object unused1, object unused2)
        {
            Backward();
        }

        /// <summary>
        /// �g���b�N�̊J�n�ʒu�Ɉړ�����R�}���h�̎���
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void MoveToStartCommandImplementation(object unused1, object unused2)
        {
            MoveToTrackStart();
        }

        #region �v���p�e�B

        /// <summary>
        /// �I������Ă���^�u�̃R���g���[��
        /// </summary>
        internal IMainTabControlPageElement TabSelectedControl
        {
            get
            {
                var tab = this.MainTabControl.SelectedTab;

                if (tab != null && tab.Control is IMainTabControlPageElement)
                {
                    return (IMainTabControlPageElement)tab.Control;
                }

                return null;
            }
        }

        /// <summary>
        /// �^�u�őI������Ă���v���C���X�g
        /// </summary>
        public Playlist TabSelectedPlaylist
        {
            get
            {
                var control = this.TabSelectedControl;
                if (control != null)
                {
                    return control.Playlist;
                }

                return null;
            }
        }

        #endregion

        /// <summary>
        /// �ݒ��ǂݍ��ށB
        /// </summary>
        private void LoadOptions()
        {
            SelectDevice(CreateAudioOutputDeviceFromApplicationOptions());
            UpdateAudioOutputDeviceMenuItems();

            this.TopMost =  this.AlwaysTopMostViewMenuItem.Checked = OptionManager.MainWindowAlwaysTopMost;

            switch (OptionManager.WaveformRendererViewMode)
            {
                case WaveformRendererStereoViewMode.Separated:
                    this.SeparateByChannelsWaveformRendererViewModeMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.StereoViewMode = WaveformRendererStereoViewMode.Separated;
                    break;
                case WaveformRendererStereoViewMode.Mixed:
                    this.MixedWaveformRendererViewModeMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.StereoViewMode = WaveformRendererStereoViewMode.Mixed;
                    break;
            }
        }

        /// <summary>
        /// �R�}���h���C����������������B
        /// </summary>
        public void ProcessCommandLineArgs(string[] commandLineArgs)
        {
            var tracks = new List<string>();

            // �R�}���h���C�������ɗ^����ꂽ�t�@�C����ǉ����čĐ�
            if (commandLineArgs.Length > 0)
            {
                foreach (var argument in commandLineArgs)
                {
                    if (File.Exists(argument) && AudioSourceProvider.IsPlaybackSupportedFileFormat(argument))
                    {
                        tracks.Add(argument);
                    }
                }

                if (tracks.Count > 0)
                {
                    this.explorerViewer.Navigate(tracks[0]);
                    Play(AudioTrackProvider.CreateFile(tracks[0]));
                }
            }
        }

        #region �t�@�C���E�t�H���_����

        /// <summary>
        /// �w�肳�ꂽ�v���C���X�g��V�����^�u�ŊJ��
        /// </summary>
        /// <param name="path"></param>
        private void OpenPlaylistInNewTab(string path)
        {
            var page = CreateNewPlaylistPage(Path.GetFileName(path));
            var viewer = (PlaylistViewer)page.Control;

            // �v���C���X�g��ǂݍ��ށB
            var reader = PlaylistReaderProvider.GetPlaylistReader(path);
            reader.OpenFile(path, viewer.Playlist);

            // �^�u��ǉ�
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;

            // �ŋߊJ�����ꏊ�ɒǉ�
            PlaylistExplorerData.AddToRecent(path);
        }

        /// <summary>
        /// �w�肳�ꂽ�t�H���_��V�����^�u�ŊJ��
        /// </summary>
        /// <param name="folderPath"></param>
        private void OpenFolderInNewTab(string folderPath)
        {
            var tracks = new List<AudioTrackBase>();

            foreach (string path in Directory.GetFiles(folderPath))
            {
                if (AudioSourceProvider.IsPlaybackSupportedFileFormat(path))
                {
                    var track = AudioTrackProvider.CreateFile(path);

                    if (track != null)
                    {
                        tracks.Add(track);
                    }
                }
            }

            var displayName = Path.GetFileName(folderPath);
            var page = CreateNewPlaylistPage(displayName);
            var viewer = (PlaylistViewer)page.Control;

            viewer.DisplayName = displayName;
            viewer.AddTrackToPlaylist(tracks.ToArray());

            // �^�u��ǉ�
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;

            // �ŋߊJ�����ꏊ�ɒǉ�
            PlaylistExplorerData.AddToRecent(folderPath);
        }

        #endregion

        #region UI����

        /// <summary>
        /// �w�肳�ꂽ�G�N�X�v���[���R���g���[���̃R���e�L�X�g���j���[���쐬����B
        /// </summary>
        private ContextMenu CreateExplorerControlContextMenu(ExplorerControl control)
        {
            var playbackMenuItem = new MenuItem();
            playbackMenuItem.Text = "�Đ�(&S)";
            playbackMenuItem.DefaultItem = true;
            playbackMenuItem.Click += delegate
            {
                Play(AudioTrackProvider.CreateFile(control.SelectedFileNames[0]));
            };
            var separator1 = new MenuItem() { Text = "-" };
            var updateMenuItem = new MenuItem();
            updateMenuItem.Text = "�ŐV�̏��ɍX�V";
            updateMenuItem.Click += delegate
            {
                control.Reload();
            };
            updateMenuItem.Shortcut = Shortcut.F5;
            var separator2 = new MenuItem() { Text = "-" };
            var openInExplorerMenuItem = new MenuItem();
            openInExplorerMenuItem.Text = "�G�N�X�v���[���[�ŊJ��";
            openInExplorerMenuItem.Click += delegate
            {
                ProcessUtils.OpenInExplorer(control.SelectedFileName);
            };
            var showPropertyMenuItem = new MenuItem();
            showPropertyMenuItem.Text = "�v���p�e�B";
            showPropertyMenuItem.Click += delegate
            {
                WinApi.ShowFileProperties(control.SelectedFileName);
            };

            var contextMenu = new ContextMenu();
            contextMenu.Popup += delegate
            {
                int numFilesSelected = control.SelectedFileNames.Length;
                int numFoldersSelected = control.SelectedFolders.Length;
                bool itemSelected = numFilesSelected > 0 || numFoldersSelected > 0;

                playbackMenuItem.Visible = numFilesSelected > 0;
                separator1.Visible = numFilesSelected > 0;
                separator2.Visible = itemSelected;
                openInExplorerMenuItem.Visible = itemSelected;
                showPropertyMenuItem.Visible = itemSelected;
            };
            contextMenu.MenuItems.Add(playbackMenuItem);
            contextMenu.MenuItems.Add(separator1);
            contextMenu.MenuItems.Add(updateMenuItem);
            contextMenu.MenuItems.Add(separator2);
            contextMenu.MenuItems.Add(openInExplorerMenuItem);
            contextMenu.MenuItems.Add(showPropertyMenuItem);

            return contextMenu;
        }

        /// <summary>
        /// �Đ����̃g���b�N���܂܂��v���C���X�g���܂܂��^�u�y�[�W�̗v�f���擾����B
        /// �Đ����ł͂Ȃ��ꍇ�A�I�����ꂽ�^�u�̗v�f���擾����B
        /// </summary>
        /// <returns></returns>
        private IMainTabControlPageElement GetCurrentTabPageElement()
        {
            if (this.TabSelectedControl is IMainTabControlPageElement)
            {
                return this.TabSelectedControl;
            }

            return null;
        }

        /// <summary>
        /// �Đ����̃g���b�N���܂܂��v���C���X�g���擾����B
        /// �Đ����ł͂Ȃ��ꍇ�A�I�����ꂽ�^�u�̃v���C���X�g���擾����B
        /// </summary>
        /// <returns></returns>
        private Playlist GetCurrentPlaylist()
        {
            var control = GetCurrentTabPageElement();

            if (control == null)
            {
                return null;
            }

            return control.Playlist;
        }

        /// <summary>
        /// �V�����v���C���X�g�r���[�𐶐�����B
        /// </summary>
        /// <returns></returns>
        private PlaylistViewer CreateNewPlaylistViewer()
        {
            var viewer = new PlaylistViewer();
            viewer.TrackDoubleClick += delegate
            {
                this.playCommand?.Execute(viewer, viewer.Playlist.SelectedTrack);
            };

            return viewer;
        }

        /// <summary>
        /// �v���C���X�g���w�肳�ꂽ�^�C�g���̃^�u�Ƃ��ĕ\������^�u�y�[�W�𐶐�����B
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private ClosableTabPage CreateNewPlaylistPage(string title)
        {
            var page = new ClosableTabPage(title);
            page.Control = CreateNewPlaylistViewer();

            return page;
        }

        /// <summary>
        /// ���p�\�ȃI�[�f�B�I�o�̓f�o�C�X���j���[�̃h���b�v�_�E�����j���[���X�V����B
        /// </summary>
        private void UpdateAudioOutputDeviceMenuItems()
        {
            this.AvailableOutputDevicesPlaybackMenuItem.MenuItems.Clear();

            foreach (var device in AudioOutputDevice.GetAvailableDevices())
            {
                var item = new MenuItem();
                item.Text = device.ToString();
                item.Tag = device;
                item.Checked = AudioPlayer.GetOutputDevice().IsSameDevice(device);
                item.Click += delegate
                {
                    SelectDevice((AudioOutputDevice)item.Tag);
                };

                this.AvailableOutputDevicesPlaybackMenuItem.MenuItems.Add(item);
            }
        }

        /// <summary>
        /// �I�[�f�B�I�o�̓f�o�C�X�̃��C�e���V�̐ݒ�ɉ����āA���C�e���V���j���[�̃`�F�b�N��Ԃ��X�V����B
        /// </summary>
        private void UpdateLatencyMenuCheckState()
        {
            foreach (MenuItem menu in this.OutputLatencyPlaybackMenuItem.MenuItems)
            {
                menu.Checked = Convert.ToUInt32(menu.Tag) == OptionManager.AudioOutputDeviceLatency;
            }
        }

        /// <summary>
        /// �E�B���h�E�̃^�C�g�����X�V����B
        /// </summary>
        private void UpdateWindowTitle(AudioTrackBase audioTrack)
        {
            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_STATE_STOP:
                case AudioPlayer.AUDIOPLAYER_NOT_READY:
                    this.Text = Program.APPLICATION_NAME;
                    break;
                default:
                    {
                        if (audioTrack != null)
                        {
                            this.Text = $"{Program.APPLICATION_NAME} - {audioTrack.Title}";
                        }
                        else if (this.currentTrack != null)
                        {
                            this.Text = $"{Program.APPLICATION_NAME} - {this.currentTrack.Title}";
                        }
                        else
                        {
                            var playlist = GetCurrentPlaylist();

                            if (playlist != null)
                            {
                                this.Text = $"{Program.APPLICATION_NAME} - {playlist.SelectedTrack.Title}";
                            }
                            else
                            {
                                this.Text = Program.APPLICATION_NAME;
                            }
                        }
                        
                    }
                    break;
            }
        }

        /// <summary>
        /// �g���b�N�̉摜�Ƃ��ĕ\������摜���擾����B
        /// </summary>
        /// <returns></returns>
        private Image GetTrackPicture(AudioTrackBase track)
        {
            if (track == null)
            {
                return Resources.note;
            }

            if (track.Picture == null)
            {
                if (!string.IsNullOrEmpty(track.Path))
                {
                    var dir = Path.GetDirectoryName(track.Path);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        var defaultPicPng = $"{dir}\\default.png";
                        var defaultPicJpg = $"{dir}\\default.jpg";
                        var defaultPicBmp = $"{dir}\\default.bmp";

                        if (File.Exists(defaultPicPng))
                        {
                            return Image.FromFile(defaultPicPng);
                        }
                        else if (File.Exists(defaultPicJpg))
                        {
                            return Image.FromFile(defaultPicJpg);
                        }
                        else if (File.Exists(defaultPicBmp))
                        {
                            return Image.FromFile(defaultPicBmp);
                        }
                    }
                }


                return Resources.note;
            }

            return track.Picture;
        }

        /// <summary>
        /// �g���b�N�̉摜��\������B
        /// </summary>
        private void ShowTrackPicture(AudioTrackBase track)
        {
            this.TrackPictureViewer.Picture = GetTrackPicture(track);
        }

        #endregion

        #region ���f�B�A�R���g���[��

        /// <summary>
        /// �g�p����I�[�f�B�I�o�̓f�o�C�X��ݒ肷��B
        /// </summary>
        /// <param name="device"></param>
        private void SelectDevice(AudioOutputDevice device)
        {
            // �A�v���P�[�V�����ݒ葤�ɐݒ�𔽉f����B
            OptionManager.AudioOutputDeviceName = device.DeviceName;
            OptionManager.IsWASAPIDevice = device.IsWASAPIDevice;
            OptionManager.IsWASAPIEventSyncMode = device.IsWASAPIEventSyncMode;
            OptionManager.IsWASAPIExclusiveMode = device.IsWASAPIExclusiveMode;
            OptionManager.AudioOutputDeviceLatency = device.Latency;

            // �o�̓f�o�C�X���j���[�̃`�F�b�N��Ԃ��X�V����B
            foreach (MenuItem item in this.AvailableOutputDevicesPlaybackMenuItem.MenuItems)
            {
                item.Checked = ((AudioOutputDevice)item.Tag).IsSameDevice(device);
            }

            // �o�̓��C�e���V���j���[�̃`�F�b�N��Ԃ��X�V����B
            UpdateLatencyMenuCheckState();

            // �f�o�C�X��ݒ肷��B
            AudioPlayer.SetOutputDevice(CreateAudioOutputDeviceFromApplicationOptions());
        }

        /// <summary>
        /// ApplicationOptions�ɐݒ肳�ꂽ�\���̃I�[�f�B�I�o�̓f�o�C�X������AudioOutputDevice�̃C���X�^���X�𐶐�����B
        /// </summary>
        /// <returns></returns>
        private AudioOutputDevice CreateAudioOutputDeviceFromApplicationOptions()
        {
            var device = new AudioOutputDevice(
                OptionManager.AudioOutputDeviceName,
                OptionManager.IsWASAPIDevice,
                OptionManager.IsWASAPIEventSyncMode,
                OptionManager.IsWASAPIExclusiveMode,
                OptionManager.AudioOutputDeviceLatency);

            device.EnableMMCSS = OptionManager.EnableMMCSS;
            device.ThreadCharacteristics = OptionManager.MmThreadCharacteristics;
            device.PlaybackThreadPriority = OptionManager.PlaybackThreadPriority;

            return device;
        }

        /// <summary>
        /// �I�[�f�B�I�o�̓f�o�C�X�̃��C�e���V��ݒ肷��B
        /// </summary>
        /// <param name="latency"></param>
        private void SetDeviceLatency(uint latency)
        {
            // �ݒ�𔽉f���A���j���[�̃`�F�b�N��Ԃ��X�V����B
            OptionManager.AudioOutputDeviceLatency = latency;
            UpdateLatencyMenuCheckState();
        }

        /// <summary>
        /// �w�肳�ꂽ�g���b�N���Đ�����B
        /// </summary>
        /// <param name="track"></param>
        private void Play(AudioTrackBase track)
        {
            // ���x�����[�^�[�����Z�b�g
            this.LeftChannelVolumeMeter.Reset();
            this.RightChannelVolumeMeter.Reset();

            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_NOT_READY:
                    break;
                case AudioPlayer.AUDIOPLAYER_STATE_PLAY:
                case AudioPlayer.AUDIOPLAYER_STATE_PAUSE:
                    AudioPlayer.Stop();
                    AudioPlayer.Close();
                    break;
                case AudioPlayer.AUDIOPLAYER_STATE_STOP:
                    AudioPlayer.Close();
                    break;
                default:
                    AudioPlayer.Close();
                    break;
            }

            // �t�@�C�����J��
            var source = AudioSourceProvider.CreateAudioSource(track.Path);
            AudioPlayer.SetAudioSource(source);

            // �o�̓f�o�C�X�̏���
            AudioPlayer.SetOutputDevice(CreateAudioOutputDeviceFromApplicationOptions());
            AudioPlayer.Prepare();

            // �V�[�N�o�[�̐ݒ�
            this.ControlPanel.SetSeekBar(0, 0, (int)source.GetDuration().TotalMilliseconds);

            // ���ʐݒ�𔽉f
            AudioPlayer.SetVolume(OptionManager.Volume);

            // �Đ�
            AudioPlayer.Play();

            // �g���b�N�̉摜��\������B
            ShowTrackPicture(track);

            // �E�B���h�E�̃^�C�g�����X�V����B
            UpdateWindowTitle(track);

            // ��n��
            this.currentTrack = track;
        }

        /// <summary>
        /// �ꎞ��~
        /// </summary>
        private void Pause()
        {
            AudioPlayer.Pause();

            // �E�B���h�E�̃^�C�g�����X�V����B
            UpdateWindowTitle(null);
        }

        /// <summary>
        /// �ĊJ
        /// </summary>
        private void Resume()
        {
            AudioPlayer.Play();

            // �E�B���h�E�̃^�C�g�����X�V����B
            UpdateWindowTitle(this.currentTrack);
        }

        /// <summary>
        /// ��~
        /// </summary>
        private void Stop()
        {
            AudioPlayer.Stop();
            AudioPlayer.Close();

            // �g���b�N�̉摜��\������B
            ShowTrackPicture(null);

            // �E�B���h�E�̃^�C�g�����X�V����B
            UpdateWindowTitle(null);

            // ���x�����[�^�[�����Z�b�g
            this.LeftChannelVolumeMeter.Reset();
            this.RightChannelVolumeMeter.Reset();

            // ��n��
            this.currentTrack = null;
        }

        /// <summary>
        /// �����߂�
        /// </summary>
        private void Backward()
        {
            var playlist = GetCurrentPlaylist();
            playlist.SelectPreviousTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Forward()
        {
            var playlist = GetCurrentPlaylist();
            playlist.SelectNextTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// �Đ����̃g���b�N�̍ŏ��ɖ߂�B
        /// </summary>
        private void MoveToTrackStart()
        {
            AudioPlayer.GetAudioSource().SetCurrentTime(TimeSpan.FromSeconds(0));
        }

        #endregion

        /// <summary>
        /// �t�H�[�����ǂݍ��܂ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // �f�U�C�i�\���̃t�H�[���Ń��C�����j���[��ݒ肷��ƁA
            // �f�U�C�i���J�����тɃt�H�[���̍������������Ȃ��Ă����o�O�����邽�߁A
            // �f�U�C�����[�h�ł̓��C�����j���[��ݒ肵�Ȃ��B
            if (!this.IsDesignMode())
            {
                this.Menu = this.MainMenuRoot;
            }

            // ApplicationOptions���ł̐ݒ�𔽉f����B
            LoadOptions();

            // �f�t�H���g�̃A�[�g��\��
            ShowTrackPicture(null);

            // �v���C���X�g�u���E�U�̕\�����X�V����B
            this.PlaylistBrowser.UpdateView();

            // �G�N�X�v���[���ɕ\������t�@�C���̊g���q���A�Ή����Ă���I�[�f�B�I�g���b�N�̊g���q�ɍi�荞�ށB
            this.explorerViewer.SetFileFormatFilter(AudioSourceProvider.GetAllRegisteredFormatExtensions());

            // �G�N�X�v���[���̃^�u�y�[�W��ǉ�
            this.MainTabControl.AddTabPage(this.explorerTabPage);
            this.explorerViewer.SelectDrive('C');

            // �R�}���h���C�������Ɏw�肳�ꂽ�t�@�C�����J���B
            ProcessCommandLineArgs(Environment.GetCommandLineArgs());
        }

        /// <summary>
        /// �t�H�[����������ꍇ�̏���
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            Stop();
            base.OnClosing(e);
        }

        /// <summary>
        /// �G�N�X�v���[���r���[�Ńt�@�C�����_�u���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExplorerViewerFileDoubleClick(object sender, EventArgs e)
        {
            Play(AudioTrackProvider.CreateFile(this.explorerViewer.SelectedFileName));
        }

        /// <summary>
        /// �I�[�f�B�I�\�[�X�̍Ō�܂ōĐ������ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAudioSourceReachEnd(object sender, EventArgs e)
        {
            var viewer = GetCurrentTabPageElement();
            var playlist = viewer.Playlist;

            if (OptionManager.ShuffleMode)
            {
                var random = new Random(Environment.TickCount);
                var index = random.Next(0, playlist.Count - 1);

                playlist.SelectedTrack = playlist.GetTrack(index);
                Stop();
                Play(playlist.SelectedTrack);
            }
            else if (OptionManager.RepeatMode == RepeatMode.RepeatCurrentTrackOnly)
            {
                var track = this.currentTrack;
                Stop();
                Play(track);
            }
            else if (OptionManager.RepeatMode == RepeatMode.RepeatAllTracks)
            {
                Forward();
            }
            else
            {
                if (playlist.SelectedTrack == playlist.GetTrack(playlist.Count - 1))
                {
                    Stop();
                }
                else
                {
                    Forward();
                }
            }
        }

        /// <summary>
        /// �Đ����Ԃ��ω������ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaybackPositionChanged(object sender, EventArgs e)
        {
            var source = AudioPlayer.GetWaveformMonitor();
            if (source == null)
            {
                return;
            }

            var msec = AudioPlayer.GetTimerInterval();
            source.LoadData(msec);

            if (source.WaveFormat.Channels >= 2)
            {
                this.WaveformRendererControl.PaintWaveform(source.GetWaveform(0), source.GetWaveform(1));
                this.LeftChannelVolumeMeter.Amplitude = source.GetAmplitude(0);
                this.RightChannelVolumeMeter.Amplitude = source.GetAmplitude(1);
            }
            else
            {
                this.WaveformRendererControl.PaintWaveform(source.GetWaveform(0));

                var amp = source.GetAmplitude(0);
                this.LeftChannelVolumeMeter.Amplitude = amp;
                this.RightChannelVolumeMeter.Amplitude = amp;
            }

            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_STATE_PLAY:
                case AudioPlayer.AUDIOPLAYER_STATE_PAUSE:
                    this.PlaybackTimeStatusText.Text = $"{AudioPlayer.GetAudioSource().GetCurrentTime().ToString("mm\\:ss")}";
                    break;
                default:
                    this.PlaybackTimeStatusText.Text = $"STOP";
                    break;
            }
        }

        /// <summary>
        /// �V�[�N���삪�s��ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_Seek(object sender, EventArgs e)
        {
            AudioPlayer.GetAudioSource()?.SetCurrentTime(this.ControlPanel.GetSeekBarTime());
        }

        /// <summary>
        /// ���ʂ̕ύX���삪�s��ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_VolumeChanged(object sender, EventArgs e)
        {
            AudioPlayer.SetVolume(OptionManager.Volume);
        }

        /// <summary>
        /// �t�@�C�����j���[���|�b�v�A�b�v�\�������ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenu_Popup(object sender, EventArgs e)
        {
            this.AddFilesToPlaylistFileMenuItem.Enabled = this.TabSelectedControl != null && this.TabSelectedControl.CanSelectAddTrack();
            this.AddFolderToPlaylistFileMenuItem.Enabled = this.TabSelectedControl != null && this.TabSelectedControl.CanSelectAddFolder();
            this.SavePlaylistFileMenuItem.Enabled = this.TabSelectedControl != null && this.TabSelectedControl.CanExportPlaylist();
        }

        /// <summary>
        /// �V�����v���C���X�g���쐬���郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.MainTabControl.AddTabPage(CreateNewPlaylistPage("����"));
            this.MainTabControl.SelectedIndex = this.MainTabControl.TabCount - 1;
        }

        /// <summary>
        /// �v���C���X�g���J�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "M3U �v���C���X�g(*.m3u;*.m3u8)|*.m3u;*.m3u8";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string path in dialog.FileNames)
                    {
                        OpenPlaylistInNewTab(path);
                    }
                }
            }
        }

        /// <summary>
        /// �t�H���_���J�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFolderFileMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    OpenFolderInNewTab(dialog.FileName);
                }
            }
        }

        /// <summary>
        /// �t�@�C����ǉ����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFilesToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.SelectAddTrack();
        }

        /// <summary>
        /// �t�H���_��ǉ����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.SelectAddFolder();
        }

        /// <summary>
        /// �v���C���X�g��ۑ����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.ExportPlaylist();
        }

        /// <summary>
        /// ��ɍőO�ʂɕ\�����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlwaysTopMostViewMenuItem_Click(object sender, EventArgs e)
        {
            this.AlwaysTopMostViewMenuItem.Checked = !this.AlwaysTopMostViewMenuItem.Checked;
            OptionManager.MainWindowAlwaysTopMost = this.AlwaysTopMostViewMenuItem.Checked;
            this.TopMost = this.AlwaysTopMostViewMenuItem.Checked;
        }

        /// <summary>
        /// �Đ��E�ꎞ��~���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPausePlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.ControlPanel.ExecutePlaybackCommand();
        }

        /// <summary>
        /// ��~���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.stopCommand.Execute();
        }

        /// <summary>
        /// �O�̃g���b�N���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousTrackPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.backwardCommand.Execute();
        }

        /// <summary>
        /// ���̃g���b�N���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextTrackPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.forwardCommand.Execute();
        }

        /// <summary>
        /// ���p�\�ȃI�[�f�B�I�o�̓f�o�C�X�̃h���b�v�_�E�����j���[���J���ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableOutputDevicesPlaybackMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateAudioOutputDeviceMenuItems();
        }

        /// <summary>
        /// �f�o�C�X�̃��C�e���V���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LatencyPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            uint value = Convert.ToUInt32(((MenuItem)sender).Tag);

            SetDeviceLatency(value);
        }

        /// <summary>
        /// �������j���[�̃h���b�v�_�E�����j���[���J�����ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindMenu_Popup(object sender, EventArgs e)
        {
            this.ShowFindDialogFindMenuItem.Enabled = this.TabSelectedControl.CanShowFindDialog();
            this.FindNextFindMenuItem.Enabled = this.TabSelectedControl.CanFindNext();
        }

        /// <summary>
        /// �����_�C�A���O�̕\�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFindDialogFindMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.ShowFindDialog();
        }

        /// <summary>
        /// �����������j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNextFindMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.FindNext();
        }

        /// <summary>
        /// �o�[�W������񃁃j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowVersionDialogHelpMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new VersionDialog())
            {
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// ReadMe�\�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowReadMeHelpMenuItem_Click(object sender, EventArgs e)
        {
            var info = new ProcessStartInfo();
            info.FileName = $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\README.txt";

            Process.Start(info);
        }

        /// <summary>
        /// �X�V����\�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHistoryHelpMenuItem_Click(object sender, EventArgs e)
        {
            var info = new ProcessStartInfo();
            info.FileName = $"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}\\HISTORY.txt";

            Process.Start(info);
        }

        /// <summary>
        /// �v���C���X�g�u���E�U�ŏꏊ���_�u���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistBrowser_LocationDoubleClick(object sender, EventArgs e)
        {
            var path = this.PlaylistBrowser.SelectedLocation;

            if (File.Exists(path))
            {
                OpenPlaylistInNewTab(path);
            }
            else if (Directory.Exists(path))
            {
                OpenFolderInNewTab(path);
            }
        }

        private void ShowWasapiAndMmcssOptionMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new WasapiOptionDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {

            }

            dialog.Dispose();
        }

        private void WaveformRendererViewModesMenuItem_Clicked(object sender, EventArgs e)
        {
            if (sender != null && sender is MenuItem)
            {
                // �N���b�N���ꂽ���j���[�̐e�������j���[�A�C�e���̂����A�N���b�N���ꂽ���j���[������I����Ԃɂ���B
                var clicked = sender as MenuItem;
                clicked.CheckOnlyThisMenuItem();

                if (clicked == this.SeparateByChannelsWaveformRendererViewModeMenuItem)
                {
                    this.WaveformRendererControl.StereoViewMode = WaveformRendererStereoViewMode.Separated;
                    OptionManager.WaveformRendererViewMode = WaveformRendererStereoViewMode.Separated;
                }
                else if (clicked == this.MixedWaveformRendererViewModeMenuItem)
                {
                    this.WaveformRendererControl.StereoViewMode = WaveformRendererStereoViewMode.Mixed;
                    OptionManager.WaveformRendererViewMode = WaveformRendererStereoViewMode.Mixed;
                }
            }
        }
    }
}
