using LibPulseTune.CoreAudio;
using LibPulseTune.Database;
using LibPulseTune.Engine;
using LibPulseTune.Engine.Playlists;
using LibPulseTune.Engine.Providers;
using LibPulseTune.Engine.Tracks;
using LibPulseTune.Options;
using LibPulseTune.UIControls;
using LibPulseTune.UIControls.Dialogs;
using LibPulseTune.UIControls.Utils;
using PulseTune.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PulseTune
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
        private AudioTrackBase currentTrack;

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

            // �^�X�N�o�[�̃T���l�C���Ƀ��f�B�A�R���g���[���{�^����\������B
            this.ControlPanel.ShowTaskBarThumbnailButtons(this);

            // �A�N�Z�X���X�g�̃R���e�L�X�g���j���[��ݒ�
            this.AccessListControl.ContextMenu = CreateAccessListControlContextMenu();

            // �t�H���g��ݒ�
            this.Font = SystemFonts.CaptionFont;

            AudioPlayer.AudioSourceReachEnd += OnAudioSourceReachEnd;
            AudioPlayer.PlaybackPositionChanged += OnPlaybackPositionChanged;
        }

        #region UI�����p���\�b�h

        /// <summary>
        /// �w�肳�ꂽ���f�B�A�G�N�X�v���[���R���g���[���̃R���e�L�X�g���j���[���쐬����B
        /// </summary>
        private ContextMenu CreateExplorerControlContextMenu(MediaExplorerControl control)
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
            openInExplorerMenuItem.Text = "Windows�̃G�N�X�v���[���[�ŊJ��";
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
        /// �A�N�Z�X���X�g�̃R���e�L�X�g���j���[���쐬����B
        /// </summary>
        /// <returns></returns>
        private ContextMenu CreateAccessListControlContextMenu()
        {
            var openInNewTab = new MenuItem();
            openInNewTab.Text = "�V�����^�u�ŊJ��";
            openInNewTab.Click += delegate
            {
                OpenFolderInNewTab(this.AccessListControl.SelectedLocation);
            };
            var openAsPlaylist = new MenuItem();
            openAsPlaylist.Text = "�v���C���X�g�Ƃ��ĊJ��";
            openAsPlaylist.Click += delegate
            {
                OpenFolderAsPlaylistInNewTab(this.AccessListControl.SelectedLocation);
            };
            var separator1 = new MenuItem();
            separator1.Text = "-";
            var addFolderToFavoriteMenuItem = new MenuItem();
            addFolderToFavoriteMenuItem.Text = "�t�H���_��I�����Ă��C�ɓ���ɒǉ�";
            addFolderToFavoriteMenuItem.Click += delegate
            {
                using (var dialog = new FolderPickerDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var location in dialog.FileNames)
                        {
                            PlaylistExplorerData.AddToFavorite(location);
                        }
                    }
                }
            };
            var separator2 = new MenuItem();
            separator2.Text = "-";
            var addToFavoriteMenuItem = new MenuItem();
            addToFavoriteMenuItem.Text = "���C�ɓ���ɒǉ�";
            addToFavoriteMenuItem.Click += delegate
            {
                var location = this.AccessListControl.SelectedLocation;

                if (!PlaylistExplorerData.ContainsFavorite(location))
                {
                    PlaylistExplorerData.AddToFavorite(location);
                }
            };
            var removeFolderToFavoriteMenuItem = new MenuItem();
            removeFolderToFavoriteMenuItem.Text = "���C�ɓ��肩��폜";
            removeFolderToFavoriteMenuItem.Click += delegate
            {
                PlaylistExplorerData.RemoveFromFavorite(this.AccessListControl.SelectedLocation);
            };

            var contextMenu = new ContextMenu();
            contextMenu.Popup += delegate
            {
                contextMenu.MenuItems.Clear();

                if (this.AccessListControl.SelectedItems.Count > 0)
                {
                    var containsInFavorite = PlaylistExplorerData.ContainsFavorite(this.AccessListControl.SelectedLocation);
                    addToFavoriteMenuItem.Enabled = !containsInFavorite;
                    removeFolderToFavoriteMenuItem.Enabled = containsInFavorite;

                    contextMenu.MenuItems.Add(openInNewTab);
                    contextMenu.MenuItems.Add(openAsPlaylist);
                    contextMenu.MenuItems.Add(separator1);
                    contextMenu.MenuItems.Add(addFolderToFavoriteMenuItem);
                    contextMenu.MenuItems.Add(separator2);
                    contextMenu.MenuItems.Add(addToFavoriteMenuItem);
                    contextMenu.MenuItems.Add(removeFolderToFavoriteMenuItem);
                }
                else
                {
                    contextMenu.MenuItems.Add(addFolderToFavoriteMenuItem);
                }
            };

            return contextMenu;
        }

        /// <summary>
        /// �Đ����̃g���b�N���܂܂��v���C���X�g���܂܂��^�u�y�[�W�̗v�f���擾����B
        /// �Đ����ł͂Ȃ��ꍇ�A�I�����ꂽ�^�u�̗v�f���擾����B
        /// </summary>
        /// <returns></returns>
        private IMainTabControlPageElement GetCurrentTabPageElement()
        {
            var tab = this.MainTabControl.SelectedTab;

            if (tab != null && tab.Control is IMainTabControlPageElement)
            {
                return (IMainTabControlPageElement)tab.Control;
            }

            return null;
        }

        /// <summary>
        /// �Đ����̃g���b�N���܂܂��v���C���X�g���擾����B
        /// �Đ����ł͂Ȃ��ꍇ�A�I�����ꂽ�^�u�̃v���C���X�g���擾����B
        /// </summary>
        /// <returns></returns>
        private Playlist GetCurrentTabPagePlaylist()
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
            viewer.TrackDoubleClick += OnIMainTabControlTabPageElementDoubleClick;

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
        /// ���f�B�A�G�N�X�v���[�����w�肳�ꂽ�^�C�g���̃^�u�Ƃ��ĕ\������^�u�y�[�W�𐶐�����B
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private ClosableTabPage CreateNewMediaExplorerTabPage(string title)
        {
            var control = new MediaExplorerControl();
            var page = new ClosableTabPage(title)
            {
                Control = control
            };

            control.SetFileFormatFilter(AudioSourceProvider.GetAllRegisteredFormatExtensions());
            control.ItemHeight = 25;
            control.ContextMenu = CreateExplorerControlContextMenu(control);
            control.FileDoubleClick += OnIMainTabControlTabPageElementDoubleClick;
            control.Navigated += delegate
            {
                var fileName = Path.GetFileName(control.CurrentPath);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = control.CurrentPath;
                }

                page.Text = fileName;
            };

            return page;
        }

        /// <summary>
        /// ���p�\�ȃI�[�f�B�I�o�̓f�o�C�X���j���[�̃h���b�v�_�E�����j���[���X�V����B
        /// </summary>
        private void UpdateAudioOutputDeviceMenuItems()
        {
            this.AvailableOutputDevicesPlaybackMenuItem.MenuItems.Clear();

            foreach (var device in WasapiDevice.GetAvailableDevices())
            {
                var item = new MenuItem();
                item.Text = device.ToString();
                item.Tag = device;
                item.Checked = AudioPlayer.GetOutputDevice().IsSameDevice(device);
                item.Click += delegate
                {
                    SelectDevice((WasapiDevice)item.Tag);
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
                            var playlist = GetCurrentTabPagePlaylist();

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

        /// <summary>
        /// �w�肳�ꂽ�v���C���X�g��V�����^�u�ŊJ��
        /// </summary>
        /// <param name="path"></param>
        private void OpenPlaylistInNewTab(string path)
        {
            var fileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = path;
            }

            var page = CreateNewPlaylistPage(fileName);
            var viewer = (PlaylistViewer)page.Control;

            // �v���C���X�g��ǂݍ��ށB
            var reader = PlaylistReaderProvider.GetPlaylistReader(path);
            reader.OpenFile(path, viewer.Playlist);

            // �^�u��ǉ�
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;
        }

        /// <summary>
        /// �w�肳�ꂽ�p�X�̃t�H���_��V�����^�u�̃��f�B�A�G�N�X�v���[���ŊJ���B
        /// </summary>
        /// <param name="folderPath"></param>
        private void OpenFolderInNewTab(string folderPath)
        {
            var fileName = Path.GetFileName(folderPath);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = folderPath;
            }

            var page = CreateNewMediaExplorerTabPage(fileName);
            var viewer = (MediaExplorerControl)page.Control;

            // �w�肳�ꂽ�p�X�܂ňړ�
            viewer.Navigate(folderPath);

            // �^�u��ǉ�
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;
        }

        /// <summary>
        /// �w�肳�ꂽ�t�H���_���v���C���X�g�Ƃ��ĐV�����^�u�ŊJ��
        /// </summary>
        /// <param name="folderPath"></param>
        private void OpenFolderAsPlaylistInNewTab(string folderPath)
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
        }

        #endregion

        #region �I�[�f�B�I����p���\�b�h

        /// <summary>
        /// �g�p����I�[�f�B�I�o�̓f�o�C�X��ݒ肷��B
        /// </summary>
        /// <param name="device"></param>
        private void SelectDevice(WasapiDevice device)
        {
            // �A�v���P�[�V�����ݒ葤�ɐݒ�𔽉f����B
            OptionManager.AudioOutputDeviceName = device.DeviceName;
            OptionManager.IsWASAPIEventSyncMode = device.IsWASAPIEventSyncMode;
            OptionManager.IsWASAPIExclusiveMode = device.IsWASAPIExclusiveMode;
            OptionManager.AudioOutputDeviceLatency = device.Latency;

            // �o�̓f�o�C�X���j���[�̃`�F�b�N��Ԃ��X�V����B
            foreach (MenuItem item in this.AvailableOutputDevicesPlaybackMenuItem.MenuItems)
            {
                item.Checked = ((WasapiDevice)item.Tag).IsSameDevice(device);
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
        private WasapiDevice CreateAudioOutputDeviceFromApplicationOptions()
        {
            var device = new WasapiDevice(
                OptionManager.AudioOutputDeviceName,
                OptionManager.IsWASAPIEventSyncMode,
                OptionManager.IsWASAPIExclusiveMode,
                OptionManager.AudioOutputDeviceLatency);

            device.EnableMMCSS = OptionManager.EnableMMCSS;
            device.ThreadCharacteristics = OptionManager.MmThreadCharacteristics;
            device.PlaybackThreadPriority = OptionManager.PlaybackThreadPriority;
            device.CreatePlaybackThreadAsBackgroundThread = OptionManager.CreatePlaybackThreadAsBackgroundThread;

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
            this.LevelMeterControl.Reset();

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
            this.ControlPanel.SetSeekBar(0, 0, (int)AudioPlayer.GetDuration().TotalMilliseconds);

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
            this.LevelMeterControl.Reset();

            // ��n��
            this.currentTrack = null;
        }

        /// <summary>
        /// �����߂�
        /// </summary>
        private void Backward()
        {
            var playlist = GetCurrentTabPagePlaylist();
            playlist.SelectPreviousTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Forward()
        {
            var playlist = GetCurrentTabPagePlaylist();
            playlist.SelectNextTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// �Đ����̃g���b�N�̍ŏ��ɖ߂�B
        /// </summary>
        private void MoveToTrackStart()
        {
            var playlist = GetCurrentTabPagePlaylist();
            Play(playlist.SelectedTrack);

            // ���̃R�[�h�̂ق����ēǂݍ��݂�h���Č����I�����AOpus�̃f�R�[�h�Ɏg�p���Ă��郉�C�u�����uConcentus�v�Ƒ����������A
            // Opus�����J�n����1�b��̈ʒu�ɃV�[�N����Ă��܂��s�����������B
            //AudioPlayer.GetAudioSource().SetCurrentTime(TimeSpan.FromSeconds(0));
        }

        #endregion

        /// <summary>
        /// �g�`�����_���̃X�e���I�\�����[�h��ݒ肵�AUI�⃁�j���[���ڂ̃`�F�b�N��Ԃɂ����f����B
        /// </summary>
        /// <param name="stereoViewMode"></param>
        private void SetWaveformRendererStereoViewMode(WaveformRendererStereoViewMode stereoViewMode)
        {
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

            OptionManager.WaveformRendererViewMode = stereoViewMode;
        }

        /// <summary>
        /// �g�`�����_���̕`�搸�x��ݒ肵�AUI�⃁�j���[���ڂ̃`�F�b�N��Ԃɂ����f����B
        /// </summary>
        /// <param name="precision"></param>
        private void SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision precision)
        {
            switch (precision)
            {
                case WaveformRendererRenderingPrecision.Lowest:
                    this.LowestQualityWaveformRendererPrecisionMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.RenderingPrecision = WaveformRendererRenderingPrecision.Lowest;
                    break;
                case WaveformRendererRenderingPrecision.Low:
                    this.LowQualityWaveformRendererPrecisionMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.RenderingPrecision = WaveformRendererRenderingPrecision.Low;
                    break;
                case WaveformRendererRenderingPrecision.Normal:
                    this.NormalQualityWaveformRendererPrecisionMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.RenderingPrecision = WaveformRendererRenderingPrecision.Normal;
                    break;
                case WaveformRendererRenderingPrecision.High:
                    this.HighQualityWaveformRendererPrecisionMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.RenderingPrecision = WaveformRendererRenderingPrecision.High;
                    break;
                case WaveformRendererRenderingPrecision.Highest:
                    this.HighestQualityWaveformRendererPrecisionMenuItem.CheckOnlyThisMenuItem();
                    this.WaveformRendererControl.RenderingPrecision = WaveformRendererRenderingPrecision.Highest;
                    break;
            }

            OptionManager.WaveformRendererRenderingPrecision = precision;
        }

        /// <summary>
        /// �ݒ��ǂݍ��ށB
        /// </summary>
        private void LoadOptions()
        {
            SelectDevice(CreateAudioOutputDeviceFromApplicationOptions());
            UpdateAudioOutputDeviceMenuItems();

            SetWaveformRendererStereoViewMode(OptionManager.WaveformRendererViewMode);
            SetWaveformRendererRenderingPrecision(OptionManager.WaveformRendererRenderingPrecision);

            this.TopMost = this.AlwaysTopMostViewMenuItem.Checked = OptionManager.MainWindowAlwaysTopMost;
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
                    OpenFolderInNewTab(tracks[0]);
                    Play(AudioTrackProvider.CreateFile(tracks[0]));
                }
            }
        }

        /// <summary>
        /// �Đ��R�}���h�̎���
        /// </summary>
        /// <param name="track"></param>
        private void PlayCommandImplementation(object unused, object track)
        {
            if (track == null)
            {
                var playlist = GetCurrentTabPagePlaylist();

                if (playlist == null)
                {
                    return;
                }

                if (playlist.SelectedTrack == null)
                {
                    playlist.SelectNextTrack();
                }

                track = playlist.SelectedTrack;
            }

            if (track == null)
            {
                return;
            }

            var audioTrack = (AudioTrackBase)track;

            // �V����AudioTrackBase�����Ȃ��ƁA�A���o���A�[�g���ǂݍ��܂�Ȃ��ꍇ������B
            Play(AudioTrackProvider.CreateFile(audioTrack.Path));
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

        /// <summary>
        /// �E�B���h�E�v���V�[�W��
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (MainWindowProcMgr.GetRegisteredActionCount(m) > 0)
            {
                foreach (var action in MainWindowProcMgr.GetActions(m))
                {
                    action?.Invoke(m.WParam, m.LParam);
                }
            }
        }

        /// <summary>
        /// �t�H�[�����ǂݍ��܂ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.IsDesignMode())
            {
                return;
            }

            PlaylistExplorerData.FavoriteLocationsChanged += delegate
            {
                this.AccessListControl.UpdateAvailableLocations();
            };
            this.Menu = this.MainMenuRoot;
            
            // ApplicationOptions���ł̐ݒ�𔽉f����B
            LoadOptions();

            // �f�t�H���g�̃A�[�g��\��
            ShowTrackPicture(null);

            // �R�}���h���C�������Ɏw�肳�ꂽ�t�@�C�����J���B
            ProcessCommandLineArgs(Environment.GetCommandLineArgs());

            if (this.MainTabControl.TabCount == 0)
            {
                // �}�C�~���[�W�b�N���J��
                OpenFolderInNewTab(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            }
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
        /// ���C���^�u�ɕ\������Ă���R���g���[���ŁA�g���b�N���_�u���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIMainTabControlTabPageElementDoubleClick(object sender, EventArgs e)
        {
            var control = (IMainTabControlPageElement)sender;
            this.playCommand?.Execute(control, control.Playlist.SelectedTrack);
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
            AudioPlayer.GetWaveFormat(out _, out _, out var channels, out _);
            AudioPlayer.SyncWaveformMonitor();

            if (channels >= 2)
            {
                this.WaveformRendererControl.PaintWaveform(AudioPlayer.GetWaveform(0), AudioPlayer.GetWaveform(1));
                this.LevelMeterControl.LeftMeterDecibels = AudioPlayer.GetAmplitude(0);
                this.LevelMeterControl.RightMeterDecibels  = AudioPlayer.GetAmplitude(1);
            }
            else
            {
                this.WaveformRendererControl.PaintWaveform(AudioPlayer.GetWaveform(0));

                var amp = AudioPlayer.GetAmplitude(0);
                this.LevelMeterControl.LeftMeterDecibels = amp;
                this.LevelMeterControl.RightMeterDecibels = amp;
            }

            switch (AudioPlayer.GetAudioPlayerState())
            {
                case AudioPlayer.AUDIOPLAYER_STATE_PLAY:
                case AudioPlayer.AUDIOPLAYER_STATE_PAUSE:
                    this.PlaybackTimeStatusText.Text = $"{AudioPlayer.GetCurrentTime().ToString("mm\\:ss")}";
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
            AudioPlayer.SetCurrentTime(this.ControlPanel.GetSeekBarTime());
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
            var tabSelectedControl = GetCurrentTabPageElement();

            this.AddFilesToPlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanSelectAddTrack();
            this.AddFolderToPlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanSelectAddFolder();
            this.SavePlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanExportPlaylist();
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
            using (var dialog = new FolderPickerDialog())
            {
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    OpenFolderAsPlaylistInNewTab(dialog.FileName);
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
            GetCurrentTabPageElement().SelectAddTrack();
        }

        /// <summary>
        /// �t�H���_��ǉ����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().SelectAddFolder();
        }

        /// <summary>
        /// �v���C���X�g��ۑ����郁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().ExportPlaylist();
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
            var tabSelectedControl = GetCurrentTabPageElement();

            this.ShowFindDialogFindMenuItem.Enabled = tabSelectedControl.CanShowFindDialog();
            this.FindNextFindMenuItem.Enabled = tabSelectedControl.CanFindNext();
        }

        /// <summary>
        /// �����_�C�A���O�̕\�����j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFindDialogFindMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().ShowFindDialog();
        }

        /// <summary>
        /// �����������j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNextFindMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().FindNext();
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
                dialog.ShowDialog(Program.ApplicationVersion, Program.ApplicationBuildDate, Program.BUILD_TYPE);
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
        /// WASAI��MMCSS�̐ݒ胁�j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWasapiAndMmcssOptionMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new WasapiOptionDialog();

            dialog.ShowDialog();

            dialog.Dispose();
        }

        /// <summary>
        /// �g�`�����_���̕\�����[�h���j���[���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// �A�N�Z�X���X�g���_�u���N���b�N���ꂽ�ꍇ�̏���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccessListControl_DoubleClick(object sender, EventArgs e)
        {
            var path = this.AccessListControl.SelectedLocation;

            if (File.Exists(path))
            {
                OpenPlaylistInNewTab(path);
            }
            else if (Directory.Exists(path))
            {
                var control = GetCurrentTabPageElement();

                if (control != null && control is MediaExplorerControl)
                {
                    ((MediaExplorerControl)control).Navigate(path);
                }
                else
                {
                    OpenFolderInNewTab(path);
                }
            }
        }

        private void WaveformRendererRenderingPrecisionMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == this.LowestQualityWaveformRendererPrecisionMenuItem)
            {
                SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision.Lowest);
            }
            else if (sender == this.LowQualityWaveformRendererPrecisionMenuItem)
            {
                SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision.Low);
            }
            else if (sender == this.NormalQualityWaveformRendererPrecisionMenuItem)
            {
                SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision.Normal);
            }
            else if (sender == this.HighQualityWaveformRendererPrecisionMenuItem)
            {
                SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision.High);
            }
            else if (sender == this.HighestQualityWaveformRendererPrecisionMenuItem)
            {
                SetWaveformRendererRenderingPrecision(WaveformRendererRenderingPrecision.Highest);
            }
        }
    }
}
