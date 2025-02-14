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
        // 非公開コマンド
        private readonly Command playCommand;
        private readonly Command pauseCommand;
        private readonly Command resumeCommand;
        private readonly Command stopCommand;
        private readonly Command backwardCommand;
        private readonly Command forwardCommand;
        private readonly Command moveToStartCommand;

        // 非公開フィールド
        private readonly ExplorerControl explorerViewer;    // エクスプローラーコントロール
        private readonly ClosableTabPage explorerTabPage;   // エクスプローラーコントロールが配置されたタブページ
        private AudioTrackBase currentTrack;                // 再生中のトラック

        // コンストラクタ
        public MainWindow()
        {
            InitializeComponent();

            // コマンドの準備
            this.playCommand = new Command(PlayCommandImplementation);
            this.pauseCommand = new Command(PauseCommandImplementation);
            this.resumeCommand = new Command(ResumeCommandImplementation);
            this.stopCommand = new Command(StopCommandImplementation);
            this.backwardCommand = new Command(BackwardCommandImplementation);
            this.forwardCommand = new Command(ForwardCommandImplementation);
            this.moveToStartCommand = new Command(MoveToStartCommandImplementation);

            // メディアコントロールパネルのコマンドの設定
            this.ControlPanel.PlayCommand = this.playCommand;
            this.ControlPanel.PauseCommand = this.pauseCommand;
            this.ControlPanel.ResumeCommand = this.resumeCommand;
            this.ControlPanel.StopCommand = this.stopCommand;
            this.ControlPanel.BackwardCommand = this.backwardCommand;
            this.ControlPanel.ForwardCommand = this.forwardCommand;
            this.ControlPanel.MoveToTrackStartCommand = this.moveToStartCommand;

            // エクスプローラのタブページを作成
            this.explorerViewer = new ExplorerControl();
            this.explorerViewer.Dock = DockStyle.Fill;
            this.explorerViewer.ContextMenu = CreateExplorerControlContextMenu(this.explorerViewer);
            this.explorerViewer.FileDoubleClick += OnExplorerViewerFileDoubleClick;
            this.explorerTabPage = new ClosableTabPage("エクスプローラ", false);
            this.explorerTabPage.Control = this.explorerViewer;

            // タスクバーのサムネイルにメディアコントロールボタンを表示する。
            this.ControlPanel.ShowTaskBarThumbnailButtons(this);

            // フォントを設定
            this.Font = SystemFonts.CaptionFont;

            AudioPlayer.AudioSourceReachEnd += OnAudioSourceReachEnd;
            AudioPlayer.PlaybackPositionChanged += OnPlaybackPositionChanged;
        }

        /// <summary>
        /// 再生コマンドの実装
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
        /// 一時停止コマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void PauseCommandImplementation(object unused1, object unused2)
        {
            Pause();
        }

        /// <summary>
        /// 再開コマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void ResumeCommandImplementation(object unused1, object unused2)
        {
            Resume();
        }

        /// <summary>
        /// 停止コマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void StopCommandImplementation(object unused1, object unused2)
        {
            Stop();
        }

        /// <summary>
        /// 早送りコマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void ForwardCommandImplementation(object unused1, object unused2)
        {
            Forward();
        }

        /// <summary>
        /// 巻き戻しコマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void BackwardCommandImplementation(object unused1, object unused2)
        {
            Backward();
        }

        /// <summary>
        /// トラックの開始位置に移動するコマンドの実装
        /// </summary>
        /// <param name="unused1"></param>
        /// <param name="unused2"></param>
        private void MoveToStartCommandImplementation(object unused1, object unused2)
        {
            MoveToTrackStart();
        }

        #region プロパティ

        /// <summary>
        /// 選択されているタブのコントロール
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
        /// タブで選択されているプレイリスト
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
        /// 設定を読み込む。
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
        /// コマンドライン引数を処理する。
        /// </summary>
        public void ProcessCommandLineArgs(string[] commandLineArgs)
        {
            var tracks = new List<string>();

            // コマンドライン引数に与えられたファイルを追加して再生
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

        #region ファイル・フォルダ処理

        /// <summary>
        /// 指定されたプレイリストを新しいタブで開く
        /// </summary>
        /// <param name="path"></param>
        private void OpenPlaylistInNewTab(string path)
        {
            var page = CreateNewPlaylistPage(Path.GetFileName(path));
            var viewer = (PlaylistViewer)page.Control;

            // プレイリストを読み込む。
            var reader = PlaylistReaderProvider.GetPlaylistReader(path);
            reader.OpenFile(path, viewer.Playlist);

            // タブを追加
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;

            // 最近開いた場所に追加
            PlaylistExplorerData.AddToRecent(path);
        }

        /// <summary>
        /// 指定されたフォルダを新しいタブで開く
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

            // タブを追加
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;

            // 最近開いた場所に追加
            PlaylistExplorerData.AddToRecent(folderPath);
        }

        #endregion

        #region UI処理

        /// <summary>
        /// 指定されたエクスプローラコントロールのコンテキストメニューを作成する。
        /// </summary>
        private ContextMenu CreateExplorerControlContextMenu(ExplorerControl control)
        {
            var playbackMenuItem = new MenuItem();
            playbackMenuItem.Text = "再生(&S)";
            playbackMenuItem.DefaultItem = true;
            playbackMenuItem.Click += delegate
            {
                Play(AudioTrackProvider.CreateFile(control.SelectedFileNames[0]));
            };
            var separator1 = new MenuItem() { Text = "-" };
            var updateMenuItem = new MenuItem();
            updateMenuItem.Text = "最新の情報に更新";
            updateMenuItem.Click += delegate
            {
                control.Reload();
            };
            updateMenuItem.Shortcut = Shortcut.F5;
            var separator2 = new MenuItem() { Text = "-" };
            var openInExplorerMenuItem = new MenuItem();
            openInExplorerMenuItem.Text = "エクスプローラーで開く";
            openInExplorerMenuItem.Click += delegate
            {
                ProcessUtils.OpenInExplorer(control.SelectedFileName);
            };
            var showPropertyMenuItem = new MenuItem();
            showPropertyMenuItem.Text = "プロパティ";
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
        /// 再生中のトラックが含まれるプレイリストが含まれるタブページの要素を取得する。
        /// 再生中ではない場合、選択されたタブの要素を取得する。
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
        /// 再生中のトラックが含まれるプレイリストを取得する。
        /// 再生中ではない場合、選択されたタブのプレイリストを取得する。
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
        /// 新しいプレイリストビューを生成する。
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
        /// プレイリストを指定されたタイトルのタブとして表示するタブページを生成する。
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
        /// 利用可能なオーディオ出力デバイスメニューのドロップダウンメニューを更新する。
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
        /// オーディオ出力デバイスのレイテンシの設定に応じて、レイテンシメニューのチェック状態を更新する。
        /// </summary>
        private void UpdateLatencyMenuCheckState()
        {
            foreach (MenuItem menu in this.OutputLatencyPlaybackMenuItem.MenuItems)
            {
                menu.Checked = Convert.ToUInt32(menu.Tag) == OptionManager.AudioOutputDeviceLatency;
            }
        }

        /// <summary>
        /// ウィンドウのタイトルを更新する。
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
        /// トラックの画像として表示する画像を取得する。
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
        /// トラックの画像を表示する。
        /// </summary>
        private void ShowTrackPicture(AudioTrackBase track)
        {
            this.TrackPictureViewer.Picture = GetTrackPicture(track);
        }

        #endregion

        #region メディアコントロール

        /// <summary>
        /// 使用するオーディオ出力デバイスを設定する。
        /// </summary>
        /// <param name="device"></param>
        private void SelectDevice(AudioOutputDevice device)
        {
            // アプリケーション設定側に設定を反映する。
            OptionManager.AudioOutputDeviceName = device.DeviceName;
            OptionManager.IsWASAPIDevice = device.IsWASAPIDevice;
            OptionManager.IsWASAPIEventSyncMode = device.IsWASAPIEventSyncMode;
            OptionManager.IsWASAPIExclusiveMode = device.IsWASAPIExclusiveMode;
            OptionManager.AudioOutputDeviceLatency = device.Latency;

            // 出力デバイスメニューのチェック状態を更新する。
            foreach (MenuItem item in this.AvailableOutputDevicesPlaybackMenuItem.MenuItems)
            {
                item.Checked = ((AudioOutputDevice)item.Tag).IsSameDevice(device);
            }

            // 出力レイテンシメニューのチェック状態を更新する。
            UpdateLatencyMenuCheckState();

            // デバイスを設定する。
            AudioPlayer.SetOutputDevice(CreateAudioOutputDeviceFromApplicationOptions());
        }

        /// <summary>
        /// ApplicationOptionsに設定された構成のオーディオ出力デバイスを示すAudioOutputDeviceのインスタンスを生成する。
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
        /// オーディオ出力デバイスのレイテンシを設定する。
        /// </summary>
        /// <param name="latency"></param>
        private void SetDeviceLatency(uint latency)
        {
            // 設定を反映し、メニューのチェック状態を更新する。
            OptionManager.AudioOutputDeviceLatency = latency;
            UpdateLatencyMenuCheckState();
        }

        /// <summary>
        /// 指定されたトラックを再生する。
        /// </summary>
        /// <param name="track"></param>
        private void Play(AudioTrackBase track)
        {
            // レベルメーターをリセット
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

            // ファイルを開く
            var source = AudioSourceProvider.CreateAudioSource(track.Path);
            AudioPlayer.SetAudioSource(source);

            // 出力デバイスの準備
            AudioPlayer.SetOutputDevice(CreateAudioOutputDeviceFromApplicationOptions());
            AudioPlayer.Prepare();

            // シークバーの設定
            this.ControlPanel.SetSeekBar(0, 0, (int)source.GetDuration().TotalMilliseconds);

            // 音量設定を反映
            AudioPlayer.SetVolume(OptionManager.Volume);

            // 再生
            AudioPlayer.Play();

            // トラックの画像を表示する。
            ShowTrackPicture(track);

            // ウィンドウのタイトルを更新する。
            UpdateWindowTitle(track);

            // 後始末
            this.currentTrack = track;
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        private void Pause()
        {
            AudioPlayer.Pause();

            // ウィンドウのタイトルを更新する。
            UpdateWindowTitle(null);
        }

        /// <summary>
        /// 再開
        /// </summary>
        private void Resume()
        {
            AudioPlayer.Play();

            // ウィンドウのタイトルを更新する。
            UpdateWindowTitle(this.currentTrack);
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void Stop()
        {
            AudioPlayer.Stop();
            AudioPlayer.Close();

            // トラックの画像を表示する。
            ShowTrackPicture(null);

            // ウィンドウのタイトルを更新する。
            UpdateWindowTitle(null);

            // レベルメーターをリセット
            this.LeftChannelVolumeMeter.Reset();
            this.RightChannelVolumeMeter.Reset();

            // 後始末
            this.currentTrack = null;
        }

        /// <summary>
        /// 巻き戻し
        /// </summary>
        private void Backward()
        {
            var playlist = GetCurrentPlaylist();
            playlist.SelectPreviousTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// 早送り
        /// </summary>
        private void Forward()
        {
            var playlist = GetCurrentPlaylist();
            playlist.SelectNextTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// 再生中のトラックの最初に戻る。
        /// </summary>
        private void MoveToTrackStart()
        {
            AudioPlayer.GetAudioSource().SetCurrentTime(TimeSpan.FromSeconds(0));
        }

        #endregion

        /// <summary>
        /// フォームが読み込まれた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // デザイナ表示のフォームでメインメニューを設定すると、
            // デザイナを開くたびにフォームの高さが小さくなっていくバグがあるため、
            // デザインモードではメインメニューを設定しない。
            if (!this.IsDesignMode())
            {
                this.Menu = this.MainMenuRoot;
            }

            // ApplicationOptions側での設定を反映する。
            LoadOptions();

            // デフォルトのアートを表示
            ShowTrackPicture(null);

            // プレイリストブラウザの表示を更新する。
            this.PlaylistBrowser.UpdateView();

            // エクスプローラに表示するファイルの拡張子を、対応しているオーディオトラックの拡張子に絞り込む。
            this.explorerViewer.SetFileFormatFilter(AudioSourceProvider.GetAllRegisteredFormatExtensions());

            // エクスプローラのタブページを追加
            this.MainTabControl.AddTabPage(this.explorerTabPage);
            this.explorerViewer.SelectDrive('C');

            // コマンドライン引数に指定されたファイルを開く。
            ProcessCommandLineArgs(Environment.GetCommandLineArgs());
        }

        /// <summary>
        /// フォームが閉じられる場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            Stop();
            base.OnClosing(e);
        }

        /// <summary>
        /// エクスプローラビューでファイルがダブルクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExplorerViewerFileDoubleClick(object sender, EventArgs e)
        {
            Play(AudioTrackProvider.CreateFile(this.explorerViewer.SelectedFileName));
        }

        /// <summary>
        /// オーディオソースの最後まで再生した場合の処理
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
        /// 再生時間が変化した場合の処理
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
        /// シーク操作が行われた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_Seek(object sender, EventArgs e)
        {
            AudioPlayer.GetAudioSource()?.SetCurrentTime(this.ControlPanel.GetSeekBarTime());
        }

        /// <summary>
        /// 音量の変更操作が行われた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_VolumeChanged(object sender, EventArgs e)
        {
            AudioPlayer.SetVolume(OptionManager.Volume);
        }

        /// <summary>
        /// ファイルメニューがポップアップ表示される場合の処理
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
        /// 新しいプレイリストを作成するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateNewPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.MainTabControl.AddTabPage(CreateNewPlaylistPage("無題"));
            this.MainTabControl.SelectedIndex = this.MainTabControl.TabCount - 1;
        }

        /// <summary>
        /// プレイリストを開くメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "M3U プレイリスト(*.m3u;*.m3u8)|*.m3u;*.m3u8";
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
        /// フォルダを開くメニューがクリックされた場合の処理
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
        /// ファイルを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFilesToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.SelectAddTrack();
        }

        /// <summary>
        /// フォルダを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.SelectAddFolder();
        }

        /// <summary>
        /// プレイリストを保存するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.ExportPlaylist();
        }

        /// <summary>
        /// 常に最前面に表示するメニューがクリックされた場合の処理
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
        /// 再生・一時停止メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPausePlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.ControlPanel.ExecutePlaybackCommand();
        }

        /// <summary>
        /// 停止メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.stopCommand.Execute();
        }

        /// <summary>
        /// 前のトラックメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousTrackPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.backwardCommand.Execute();
        }

        /// <summary>
        /// 次のトラックメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextTrackPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            this.forwardCommand.Execute();
        }

        /// <summary>
        /// 利用可能なオーディオ出力デバイスのドロップダウンメニューを開く場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableOutputDevicesPlaybackMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateAudioOutputDeviceMenuItems();
        }

        /// <summary>
        /// デバイスのレイテンシメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LatencyPlaybackMenuItem_Click(object sender, EventArgs e)
        {
            uint value = Convert.ToUInt32(((MenuItem)sender).Tag);

            SetDeviceLatency(value);
        }

        /// <summary>
        /// 検索メニューのドロップダウンメニューが開かれる場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindMenu_Popup(object sender, EventArgs e)
        {
            this.ShowFindDialogFindMenuItem.Enabled = this.TabSelectedControl.CanShowFindDialog();
            this.FindNextFindMenuItem.Enabled = this.TabSelectedControl.CanFindNext();
        }

        /// <summary>
        /// 検索ダイアログの表示メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFindDialogFindMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.ShowFindDialog();
        }

        /// <summary>
        /// 次を検索メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNextFindMenuItem_Click(object sender, EventArgs e)
        {
            this.TabSelectedControl.FindNext();
        }

        /// <summary>
        /// バージョン情報メニューがクリックされた場合の処理
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
        /// ReadMe表示メニューがクリックされた場合の処理
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
        /// 更新履歴表示メニューがクリックされた場合の処理
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
        /// プレイリストブラウザで場所がダブルクリックされた場合の処理
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
                // クリックされたメニューの親が持つメニューアイテムのうち、クリックされたメニューだけを選択状態にする。
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
