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
        // 非公開コマンド
        private readonly Command playCommand;
        private readonly Command pauseCommand;
        private readonly Command resumeCommand;
        private readonly Command stopCommand;
        private readonly Command backwardCommand;
        private readonly Command forwardCommand;
        private readonly Command moveToStartCommand;

        // 非公開フィールド
        private AudioTrackBase currentTrack;

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

            // タスクバーのサムネイルにメディアコントロールボタンを表示する。
            this.ControlPanel.ShowTaskBarThumbnailButtons(this);

            // アクセスリストのコンテキストメニューを設定
            this.AccessListControl.ContextMenu = CreateAccessListControlContextMenu();

            // フォントを設定
            this.Font = SystemFonts.CaptionFont;

            AudioPlayer.AudioSourceReachEnd += OnAudioSourceReachEnd;
            AudioPlayer.PlaybackPositionChanged += OnPlaybackPositionChanged;
        }

        #region UI処理用メソッド

        /// <summary>
        /// 指定されたメディアエクスプローラコントロールのコンテキストメニューを作成する。
        /// </summary>
        private ContextMenu CreateExplorerControlContextMenu(MediaExplorerControl control)
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
            openInExplorerMenuItem.Text = "Windowsのエクスプローラーで開く";
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
        /// アクセスリストのコンテキストメニューを作成する。
        /// </summary>
        /// <returns></returns>
        private ContextMenu CreateAccessListControlContextMenu()
        {
            var openInNewTab = new MenuItem();
            openInNewTab.Text = "新しいタブで開く";
            openInNewTab.Click += delegate
            {
                OpenFolderInNewTab(this.AccessListControl.SelectedLocation);
            };
            var openAsPlaylist = new MenuItem();
            openAsPlaylist.Text = "プレイリストとして開く";
            openAsPlaylist.Click += delegate
            {
                OpenFolderAsPlaylistInNewTab(this.AccessListControl.SelectedLocation);
            };
            var separator1 = new MenuItem();
            separator1.Text = "-";
            var addFolderToFavoriteMenuItem = new MenuItem();
            addFolderToFavoriteMenuItem.Text = "フォルダを選択してお気に入りに追加";
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
            addToFavoriteMenuItem.Text = "お気に入りに追加";
            addToFavoriteMenuItem.Click += delegate
            {
                var location = this.AccessListControl.SelectedLocation;

                if (!PlaylistExplorerData.ContainsFavorite(location))
                {
                    PlaylistExplorerData.AddToFavorite(location);
                }
            };
            var removeFolderToFavoriteMenuItem = new MenuItem();
            removeFolderToFavoriteMenuItem.Text = "お気に入りから削除";
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
        /// 再生中のトラックが含まれるプレイリストが含まれるタブページの要素を取得する。
        /// 再生中ではない場合、選択されたタブの要素を取得する。
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
        /// 再生中のトラックが含まれるプレイリストを取得する。
        /// 再生中ではない場合、選択されたタブのプレイリストを取得する。
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
        /// 新しいプレイリストビューを生成する。
        /// </summary>
        /// <returns></returns>
        private PlaylistViewer CreateNewPlaylistViewer()
        {
            var viewer = new PlaylistViewer();
            viewer.TrackDoubleClick += OnIMainTabControlTabPageElementDoubleClick;

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
        /// メディアエクスプローラを指定されたタイトルのタブとして表示するタブページを生成する。
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
        /// 利用可能なオーディオ出力デバイスメニューのドロップダウンメニューを更新する。
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

        /// <summary>
        /// 指定されたプレイリストを新しいタブで開く
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

            // プレイリストを読み込む。
            var reader = PlaylistReaderProvider.GetPlaylistReader(path);
            reader.OpenFile(path, viewer.Playlist);

            // タブを追加
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;
        }

        /// <summary>
        /// 指定されたパスのフォルダを新しいタブのメディアエクスプローラで開く。
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

            // 指定されたパスまで移動
            viewer.Navigate(folderPath);

            // タブを追加
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;
        }

        /// <summary>
        /// 指定されたフォルダをプレイリストとして新しいタブで開く
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

            // タブを追加
            this.MainTabControl.AddTabPage(page);
            this.MainTabControl.SelectedTab = page;
        }

        #endregion

        #region オーディオ制御用メソッド

        /// <summary>
        /// 使用するオーディオ出力デバイスを設定する。
        /// </summary>
        /// <param name="device"></param>
        private void SelectDevice(WasapiDevice device)
        {
            // アプリケーション設定側に設定を反映する。
            OptionManager.AudioOutputDeviceName = device.DeviceName;
            OptionManager.IsWASAPIEventSyncMode = device.IsWASAPIEventSyncMode;
            OptionManager.IsWASAPIExclusiveMode = device.IsWASAPIExclusiveMode;
            OptionManager.AudioOutputDeviceLatency = device.Latency;

            // 出力デバイスメニューのチェック状態を更新する。
            foreach (MenuItem item in this.AvailableOutputDevicesPlaybackMenuItem.MenuItems)
            {
                item.Checked = ((WasapiDevice)item.Tag).IsSameDevice(device);
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

            // ファイルを開く
            var source = AudioSourceProvider.CreateAudioSource(track.Path);
            AudioPlayer.SetAudioSource(source);

            // 出力デバイスの準備
            AudioPlayer.SetOutputDevice(CreateAudioOutputDeviceFromApplicationOptions());
            AudioPlayer.Prepare();

            // シークバーの設定
            this.ControlPanel.SetSeekBar(0, 0, (int)AudioPlayer.GetDuration().TotalMilliseconds);

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
            this.LevelMeterControl.Reset();

            // 後始末
            this.currentTrack = null;
        }

        /// <summary>
        /// 巻き戻し
        /// </summary>
        private void Backward()
        {
            var playlist = GetCurrentTabPagePlaylist();
            playlist.SelectPreviousTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// 早送り
        /// </summary>
        private void Forward()
        {
            var playlist = GetCurrentTabPagePlaylist();
            playlist.SelectNextTrack();

            Play(playlist.SelectedTrack);
        }

        /// <summary>
        /// 再生中のトラックの最初に戻る。
        /// </summary>
        private void MoveToTrackStart()
        {
            var playlist = GetCurrentTabPagePlaylist();
            Play(playlist.SelectedTrack);

            // このコードのほうが再読み込みを防げて効率的だが、Opusのデコードに使用しているライブラリ「Concentus」と相性が悪く、
            // Opusだけ開始から1秒後の位置にシークされてしまう不具合が発生する。
            //AudioPlayer.GetAudioSource().SetCurrentTime(TimeSpan.FromSeconds(0));
        }

        #endregion

        /// <summary>
        /// 波形レンダラのステレオ表示モードを設定し、UIやメニュー項目のチェック状態にも反映する。
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
        /// 波形レンダラの描画精度を設定し、UIやメニュー項目のチェック状態にも反映する。
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
        /// 設定を読み込む。
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
                    OpenFolderInNewTab(tracks[0]);
                    Play(AudioTrackProvider.CreateFile(tracks[0]));
                }
            }
        }

        /// <summary>
        /// 再生コマンドの実装
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

            // 新しくAudioTrackBaseを作らないと、アルバムアートが読み込まれない場合がある。
            Play(AudioTrackProvider.CreateFile(audioTrack.Path));
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

        /// <summary>
        /// ウィンドウプロシージャ
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
        /// フォームが読み込まれた場合の処理
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
            
            // ApplicationOptions側での設定を反映する。
            LoadOptions();

            // デフォルトのアートを表示
            ShowTrackPicture(null);

            // コマンドライン引数に指定されたファイルを開く。
            ProcessCommandLineArgs(Environment.GetCommandLineArgs());

            if (this.MainTabControl.TabCount == 0)
            {
                // マイミュージックを開く
                OpenFolderInNewTab(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            }
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
        /// メインタブに表示されているコントロールで、トラックがダブルクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIMainTabControlTabPageElementDoubleClick(object sender, EventArgs e)
        {
            var control = (IMainTabControlPageElement)sender;
            this.playCommand?.Execute(control, control.Playlist.SelectedTrack);
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
        /// シーク操作が行われた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlPanel_Seek(object sender, EventArgs e)
        {
            AudioPlayer.SetCurrentTime(this.ControlPanel.GetSeekBarTime());
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
            var tabSelectedControl = GetCurrentTabPageElement();

            this.AddFilesToPlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanSelectAddTrack();
            this.AddFolderToPlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanSelectAddFolder();
            this.SavePlaylistFileMenuItem.Enabled = tabSelectedControl != null && tabSelectedControl.CanExportPlaylist();
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
        /// ファイルを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFilesToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().SelectAddTrack();
        }

        /// <summary>
        /// フォルダを追加するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderToPlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().SelectAddFolder();
        }

        /// <summary>
        /// プレイリストを保存するメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePlaylistFileMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().ExportPlaylist();
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
            var tabSelectedControl = GetCurrentTabPageElement();

            this.ShowFindDialogFindMenuItem.Enabled = tabSelectedControl.CanShowFindDialog();
            this.FindNextFindMenuItem.Enabled = tabSelectedControl.CanFindNext();
        }

        /// <summary>
        /// 検索ダイアログの表示メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowFindDialogFindMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().ShowFindDialog();
        }

        /// <summary>
        /// 次を検索メニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindNextFindMenuItem_Click(object sender, EventArgs e)
        {
            GetCurrentTabPageElement().FindNext();
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
                dialog.ShowDialog(Program.ApplicationVersion, Program.ApplicationBuildDate, Program.BUILD_TYPE);
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
        /// WASAIとMMCSSの設定メニューがクリックされた場合の処理
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
        /// 波形レンダラの表示モードメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// アクセスリストがダブルクリックされた場合の処理
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
