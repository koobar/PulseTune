namespace PulseTune
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.PlaybackTimeStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.WaveformRendererControl = new LibPulseTune.UIControls.WaveformRenderer();
            this.LeftChannelVolumeMeter = new LibPulseTune.UIControls.VolumeMeterControl();
            this.RightChannelVolumeMeter = new LibPulseTune.UIControls.VolumeMeterControl();
            this.TrackPictureViewer = new LibPulseTune.UIControls.PictureViewer();
            this.PlaylistBrowser = new LibPulseTune.UIControls.PlaylistExplorer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.MainMenuRoot = new System.Windows.Forms.MainMenu(this.components);
            this.FileMenu = new System.Windows.Forms.MenuItem();
            this.CreateNewPlaylistFileMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.OpenPlaylistFileMenuItem = new System.Windows.Forms.MenuItem();
            this.OpenFolderFileMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.SavePlaylistFileMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.AddFilesToPlaylistFileMenuItem = new System.Windows.Forms.MenuItem();
            this.AddFolderToPlaylistFileMenuItem = new System.Windows.Forms.MenuItem();
            this.ViewMenu = new System.Windows.Forms.MenuItem();
            this.AlwaysTopMostViewMenuItem = new System.Windows.Forms.MenuItem();
            this.ViewMenuSeparator1 = new System.Windows.Forms.MenuItem();
            this.WaveformRendererViewModesMenuItem = new System.Windows.Forms.MenuItem();
            this.SeparateByChannelsWaveformRendererViewModeMenuItem = new System.Windows.Forms.MenuItem();
            this.MixedWaveformRendererViewModeMenuItem = new System.Windows.Forms.MenuItem();
            this.FindMenu = new System.Windows.Forms.MenuItem();
            this.ShowFindDialogFindMenuItem = new System.Windows.Forms.MenuItem();
            this.FindNextFindMenuItem = new System.Windows.Forms.MenuItem();
            this.PlayMenu = new System.Windows.Forms.MenuItem();
            this.PlayPausePlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.StopPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.PreviousTrackPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.NextTrackPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.OptionMenu = new System.Windows.Forms.MenuItem();
            this.AvailableOutputDevicesPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.OutputLatencyPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency16msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency32msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency64msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency128msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency256msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.Latency512msLatencyMenuItem = new System.Windows.Forms.MenuItem();
            this.ShowWasapiAndMmcssOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpMenu = new System.Windows.Forms.MenuItem();
            this.ShowReadMeHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.ShowHistoryHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.ShowVersionDialogHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.MainTabControl = new LibPulseTune.UIControls.ClosableTabControl();
            this.ControlPanel = new LibPulseTune.UIControls.MediaControlPanel();
            this.StatusBar.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackPictureViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText,
            this.PlaybackTimeStatusText});
            this.StatusBar.Location = new System.Drawing.Point(0, 618);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(984, 22);
            this.StatusBar.TabIndex = 4;
            this.StatusBar.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(118, 17);
            this.StatusText.Text = "PulseTune by koobar.";
            // 
            // PlaybackTimeStatusText
            // 
            this.PlaybackTimeStatusText.Name = "PlaybackTimeStatusText";
            this.PlaybackTimeStatusText.Size = new System.Drawing.Size(851, 17);
            this.PlaybackTimeStatusText.Spring = true;
            this.PlaybackTimeStatusText.Text = "00:00";
            this.PlaybackTimeStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.WaveformRendererControl);
            this.panel1.Controls.Add(this.LeftChannelVolumeMeter);
            this.panel1.Controls.Add(this.RightChannelVolumeMeter);
            this.panel1.Controls.Add(this.TrackPictureViewer);
            this.panel1.Controls.Add(this.PlaylistBrowser);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 548);
            this.panel1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(159, 316);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "0db";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(3, 316);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "-40db";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(49, 255);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "WAVEFORM";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(49, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "LEVEL METER";
            // 
            // WaveformRendererControl
            // 
            this.WaveformRendererControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WaveformRendererControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WaveformRendererControl.EnableWaveformAntiAlias = true;
            this.WaveformRendererControl.Location = new System.Drawing.Point(3, 270);
            this.WaveformRendererControl.Name = "WaveformRendererControl";
            this.WaveformRendererControl.Size = new System.Drawing.Size(174, 43);
            this.WaveformRendererControl.StereoViewMode = LibPulseTune.Options.WaveformRendererStereoViewMode.Separated;
            this.WaveformRendererControl.TabIndex = 12;
            // 
            // LeftChannelVolumeMeter
            // 
            this.LeftChannelVolumeMeter.Amplitude = 0F;
            this.LeftChannelVolumeMeter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LeftChannelVolumeMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LeftChannelVolumeMeter.EnableAttenuationEmulation = false;
            this.LeftChannelVolumeMeter.Location = new System.Drawing.Point(3, 331);
            this.LeftChannelVolumeMeter.Name = "LeftChannelVolumeMeter";
            this.LeftChannelVolumeMeter.Size = new System.Drawing.Size(174, 17);
            this.LeftChannelVolumeMeter.TabIndex = 11;
            // 
            // RightChannelVolumeMeter
            // 
            this.RightChannelVolumeMeter.Amplitude = 0F;
            this.RightChannelVolumeMeter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RightChannelVolumeMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RightChannelVolumeMeter.EnableAttenuationEmulation = false;
            this.RightChannelVolumeMeter.Location = new System.Drawing.Point(3, 351);
            this.RightChannelVolumeMeter.Name = "RightChannelVolumeMeter";
            this.RightChannelVolumeMeter.Size = new System.Drawing.Size(174, 17);
            this.RightChannelVolumeMeter.TabIndex = 10;
            // 
            // TrackPictureViewer
            // 
            this.TrackPictureViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackPictureViewer.Location = new System.Drawing.Point(3, 371);
            this.TrackPictureViewer.Name = "TrackPictureViewer";
            this.TrackPictureViewer.Picture = null;
            this.TrackPictureViewer.Size = new System.Drawing.Size(174, 174);
            this.TrackPictureViewer.TabIndex = 9;
            this.TrackPictureViewer.TabStop = false;
            // 
            // PlaylistBrowser
            // 
            this.PlaylistBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlaylistBrowser.FullRowSelect = true;
            this.PlaylistBrowser.HideSelection = false;
            this.PlaylistBrowser.Location = new System.Drawing.Point(3, 3);
            this.PlaylistBrowser.MultiSelect = false;
            this.PlaylistBrowser.Name = "PlaylistBrowser";
            this.PlaylistBrowser.ShowItemToolTips = true;
            this.PlaylistBrowser.Size = new System.Drawing.Size(174, 249);
            this.PlaylistBrowser.TabIndex = 8;
            this.PlaylistBrowser.UseCompatibleStateImageBehavior = false;
            this.PlaylistBrowser.View = System.Windows.Forms.View.Details;
            this.PlaylistBrowser.LocationDoubleClick += new System.EventHandler(this.PlaylistBrowser_LocationDoubleClick);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(180, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 548);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // MainMenuRoot
            // 
            this.MainMenuRoot.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileMenu,
            this.ViewMenu,
            this.FindMenu,
            this.PlayMenu,
            this.OptionMenu,
            this.HelpMenu});
            // 
            // FileMenu
            // 
            this.FileMenu.Index = 0;
            this.FileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.CreateNewPlaylistFileMenuItem,
            this.menuItem7,
            this.OpenPlaylistFileMenuItem,
            this.OpenFolderFileMenuItem,
            this.menuItem11,
            this.SavePlaylistFileMenuItem,
            this.menuItem13,
            this.AddFilesToPlaylistFileMenuItem,
            this.AddFolderToPlaylistFileMenuItem});
            this.FileMenu.Text = "ファイル(&F)";
            this.FileMenu.Popup += new System.EventHandler(this.FileMenu_Popup);
            // 
            // CreateNewPlaylistFileMenuItem
            // 
            this.CreateNewPlaylistFileMenuItem.Index = 0;
            this.CreateNewPlaylistFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.CreateNewPlaylistFileMenuItem.Text = "新規プレイリスト(&N)";
            this.CreateNewPlaylistFileMenuItem.Click += new System.EventHandler(this.CreateNewPlaylistFileMenuItem_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 1;
            this.menuItem7.Text = "-";
            // 
            // OpenPlaylistFileMenuItem
            // 
            this.OpenPlaylistFileMenuItem.Index = 2;
            this.OpenPlaylistFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.OpenPlaylistFileMenuItem.Text = "プレイリストを開く(&O)...";
            this.OpenPlaylistFileMenuItem.Click += new System.EventHandler(this.OpenPlaylistFileMenuItem_Click);
            // 
            // OpenFolderFileMenuItem
            // 
            this.OpenFolderFileMenuItem.Index = 3;
            this.OpenFolderFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftO;
            this.OpenFolderFileMenuItem.Text = "フォルダを開く(&F)...";
            this.OpenFolderFileMenuItem.Click += new System.EventHandler(this.OpenFolderFileMenuItem_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 4;
            this.menuItem11.Text = "-";
            // 
            // SavePlaylistFileMenuItem
            // 
            this.SavePlaylistFileMenuItem.Index = 5;
            this.SavePlaylistFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.SavePlaylistFileMenuItem.Text = "プレイリストを保存(&S)";
            this.SavePlaylistFileMenuItem.Click += new System.EventHandler(this.SavePlaylistFileMenuItem_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 6;
            this.menuItem13.Text = "-";
            // 
            // AddFilesToPlaylistFileMenuItem
            // 
            this.AddFilesToPlaylistFileMenuItem.Index = 7;
            this.AddFilesToPlaylistFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftA;
            this.AddFilesToPlaylistFileMenuItem.Text = "ファイルを追加...";
            this.AddFilesToPlaylistFileMenuItem.Click += new System.EventHandler(this.AddFilesToPlaylistFileMenuItem_Click);
            // 
            // AddFolderToPlaylistFileMenuItem
            // 
            this.AddFolderToPlaylistFileMenuItem.Index = 8;
            this.AddFolderToPlaylistFileMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftF;
            this.AddFolderToPlaylistFileMenuItem.Text = "フォルダを追加...";
            this.AddFolderToPlaylistFileMenuItem.Click += new System.EventHandler(this.AddFolderToPlaylistFileMenuItem_Click);
            // 
            // ViewMenu
            // 
            this.ViewMenu.Index = 1;
            this.ViewMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AlwaysTopMostViewMenuItem,
            this.ViewMenuSeparator1,
            this.WaveformRendererViewModesMenuItem});
            this.ViewMenu.Text = "表示(&V)";
            // 
            // AlwaysTopMostViewMenuItem
            // 
            this.AlwaysTopMostViewMenuItem.Index = 0;
            this.AlwaysTopMostViewMenuItem.Text = "常に最前面に表示";
            this.AlwaysTopMostViewMenuItem.Click += new System.EventHandler(this.AlwaysTopMostViewMenuItem_Click);
            // 
            // ViewMenuSeparator1
            // 
            this.ViewMenuSeparator1.Index = 1;
            this.ViewMenuSeparator1.Text = "-";
            // 
            // WaveformRendererViewModesMenuItem
            // 
            this.WaveformRendererViewModesMenuItem.Index = 2;
            this.WaveformRendererViewModesMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SeparateByChannelsWaveformRendererViewModeMenuItem,
            this.MixedWaveformRendererViewModeMenuItem});
            this.WaveformRendererViewModesMenuItem.Text = "波形表示モード";
            // 
            // SeparateByChannelsWaveformRendererViewModeMenuItem
            // 
            this.SeparateByChannelsWaveformRendererViewModeMenuItem.Index = 0;
            this.SeparateByChannelsWaveformRendererViewModeMenuItem.Text = "チャンネル毎に分離して表示";
            this.SeparateByChannelsWaveformRendererViewModeMenuItem.Click += new System.EventHandler(this.WaveformRendererViewModesMenuItem_Clicked);
            // 
            // MixedWaveformRendererViewModeMenuItem
            // 
            this.MixedWaveformRendererViewModeMenuItem.Index = 1;
            this.MixedWaveformRendererViewModeMenuItem.Text = "チャンネルをミックスして表示";
            this.MixedWaveformRendererViewModeMenuItem.Click += new System.EventHandler(this.WaveformRendererViewModesMenuItem_Clicked);
            // 
            // FindMenu
            // 
            this.FindMenu.Index = 2;
            this.FindMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ShowFindDialogFindMenuItem,
            this.FindNextFindMenuItem});
            this.FindMenu.Text = "検索(&S)";
            this.FindMenu.Popup += new System.EventHandler(this.FindMenu_Popup);
            // 
            // ShowFindDialogFindMenuItem
            // 
            this.ShowFindDialogFindMenuItem.Index = 0;
            this.ShowFindDialogFindMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.ShowFindDialogFindMenuItem.Text = "トラックを検索";
            this.ShowFindDialogFindMenuItem.Click += new System.EventHandler(this.ShowFindDialogFindMenuItem_Click);
            // 
            // FindNextFindMenuItem
            // 
            this.FindNextFindMenuItem.Index = 1;
            this.FindNextFindMenuItem.Text = "次を検索";
            this.FindNextFindMenuItem.Click += new System.EventHandler(this.FindNextFindMenuItem_Click);
            // 
            // PlayMenu
            // 
            this.PlayMenu.Index = 3;
            this.PlayMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.PlayPausePlaybackMenuItem,
            this.StopPlaybackMenuItem,
            this.PreviousTrackPlaybackMenuItem,
            this.NextTrackPlaybackMenuItem});
            this.PlayMenu.Text = "再生(&P)";
            // 
            // PlayPausePlaybackMenuItem
            // 
            this.PlayPausePlaybackMenuItem.Index = 0;
            this.PlayPausePlaybackMenuItem.Text = "再生/一時停止";
            this.PlayPausePlaybackMenuItem.Click += new System.EventHandler(this.PlayPausePlaybackMenuItem_Click);
            // 
            // StopPlaybackMenuItem
            // 
            this.StopPlaybackMenuItem.Index = 1;
            this.StopPlaybackMenuItem.Text = "停止";
            this.StopPlaybackMenuItem.Click += new System.EventHandler(this.StopPlaybackMenuItem_Click);
            // 
            // PreviousTrackPlaybackMenuItem
            // 
            this.PreviousTrackPlaybackMenuItem.Index = 2;
            this.PreviousTrackPlaybackMenuItem.Text = "前のトラック";
            this.PreviousTrackPlaybackMenuItem.Click += new System.EventHandler(this.PreviousTrackPlaybackMenuItem_Click);
            // 
            // NextTrackPlaybackMenuItem
            // 
            this.NextTrackPlaybackMenuItem.Index = 3;
            this.NextTrackPlaybackMenuItem.Text = "次のトラック";
            this.NextTrackPlaybackMenuItem.Click += new System.EventHandler(this.NextTrackPlaybackMenuItem_Click);
            // 
            // OptionMenu
            // 
            this.OptionMenu.Index = 4;
            this.OptionMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.AvailableOutputDevicesPlaybackMenuItem,
            this.OutputLatencyPlaybackMenuItem,
            this.ShowWasapiAndMmcssOptionMenuItem});
            this.OptionMenu.Text = "設定(&O)";
            // 
            // AvailableOutputDevicesPlaybackMenuItem
            // 
            this.AvailableOutputDevicesPlaybackMenuItem.Index = 0;
            this.AvailableOutputDevicesPlaybackMenuItem.Text = "出力デバイス";
            this.AvailableOutputDevicesPlaybackMenuItem.Popup += new System.EventHandler(this.AvailableOutputDevicesPlaybackMenuItem_DropDownOpening);
            // 
            // OutputLatencyPlaybackMenuItem
            // 
            this.OutputLatencyPlaybackMenuItem.Index = 1;
            this.OutputLatencyPlaybackMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Latency16msLatencyMenuItem,
            this.Latency32msLatencyMenuItem,
            this.Latency64msLatencyMenuItem,
            this.Latency128msLatencyMenuItem,
            this.Latency256msLatencyMenuItem,
            this.Latency512msLatencyMenuItem});
            this.OutputLatencyPlaybackMenuItem.Text = "出力レイテンシ";
            // 
            // Latency16msLatencyMenuItem
            // 
            this.Latency16msLatencyMenuItem.Index = 0;
            this.Latency16msLatencyMenuItem.Tag = "16";
            this.Latency16msLatencyMenuItem.Text = "16ms";
            this.Latency16msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // Latency32msLatencyMenuItem
            // 
            this.Latency32msLatencyMenuItem.Index = 1;
            this.Latency32msLatencyMenuItem.Tag = "32";
            this.Latency32msLatencyMenuItem.Text = "32ms";
            this.Latency32msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // Latency64msLatencyMenuItem
            // 
            this.Latency64msLatencyMenuItem.Index = 2;
            this.Latency64msLatencyMenuItem.Tag = "64";
            this.Latency64msLatencyMenuItem.Text = "64ms";
            this.Latency64msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // Latency128msLatencyMenuItem
            // 
            this.Latency128msLatencyMenuItem.Index = 3;
            this.Latency128msLatencyMenuItem.Tag = "128";
            this.Latency128msLatencyMenuItem.Text = "128ms";
            this.Latency128msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // Latency256msLatencyMenuItem
            // 
            this.Latency256msLatencyMenuItem.Index = 4;
            this.Latency256msLatencyMenuItem.Tag = "256";
            this.Latency256msLatencyMenuItem.Text = "256ms";
            this.Latency256msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // Latency512msLatencyMenuItem
            // 
            this.Latency512msLatencyMenuItem.Index = 5;
            this.Latency512msLatencyMenuItem.Tag = "512";
            this.Latency512msLatencyMenuItem.Text = "512ms";
            this.Latency512msLatencyMenuItem.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // ShowWasapiAndMmcssOptionMenuItem
            // 
            this.ShowWasapiAndMmcssOptionMenuItem.Index = 2;
            this.ShowWasapiAndMmcssOptionMenuItem.Text = "WASAPI/MMCSS設定";
            this.ShowWasapiAndMmcssOptionMenuItem.Click += new System.EventHandler(this.ShowWasapiAndMmcssOptionMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.Index = 5;
            this.HelpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ShowReadMeHelpMenuItem,
            this.ShowHistoryHelpMenuItem,
            this.menuItem3,
            this.ShowVersionDialogHelpMenuItem});
            this.HelpMenu.Text = "ヘルプ(&H)";
            // 
            // ShowReadMeHelpMenuItem
            // 
            this.ShowReadMeHelpMenuItem.Index = 0;
            this.ShowReadMeHelpMenuItem.Text = "ReadMe.txt";
            this.ShowReadMeHelpMenuItem.Click += new System.EventHandler(this.ShowReadMeHelpMenuItem_Click);
            // 
            // ShowHistoryHelpMenuItem
            // 
            this.ShowHistoryHelpMenuItem.Index = 1;
            this.ShowHistoryHelpMenuItem.Text = "History.txt";
            this.ShowHistoryHelpMenuItem.Click += new System.EventHandler(this.ShowHistoryHelpMenuItem_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.Text = "-";
            // 
            // ShowVersionDialogHelpMenuItem
            // 
            this.ShowVersionDialogHelpMenuItem.Index = 3;
            this.ShowVersionDialogHelpMenuItem.Text = "バージョン情報";
            this.ShowVersionDialogHelpMenuItem.Click += new System.EventHandler(this.ShowVersionDialogHelpMenuItem_Click);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(183, 0);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.MainTabControl.SelectedIndex = -1;
            this.MainTabControl.SelectedTab = null;
            this.MainTabControl.Size = new System.Drawing.Size(801, 548);
            this.MainTabControl.TabIndex = 7;
            // 
            // ControlPanel
            // 
            this.ControlPanel.BackwardCommand = null;
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ControlPanel.ForwardCommand = null;
            this.ControlPanel.Location = new System.Drawing.Point(0, 548);
            this.ControlPanel.MoveToTrackStartCommand = null;
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.PauseCommand = null;
            this.ControlPanel.PlayCommand = null;
            this.ControlPanel.ResumeCommand = null;
            this.ControlPanel.Size = new System.Drawing.Size(984, 70);
            this.ControlPanel.StopCommand = null;
            this.ControlPanel.TabIndex = 3;
            this.ControlPanel.Seek += new System.EventHandler(this.ControlPanel_Seek);
            this.ControlPanel.VolumeChanged += new System.EventHandler(this.ControlPanel_VolumeChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(984, 640);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.StatusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "PulseTune";
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackPictureViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private LibPulseTune.UIControls.MediaControlPanel ControlPanel;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ToolStripStatusLabel PlaybackTimeStatusText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private LibPulseTune.UIControls.PlaylistExplorer PlaylistBrowser;
        private LibPulseTune.UIControls.ClosableTabControl MainTabControl;
        private LibPulseTune.UIControls.PictureViewer TrackPictureViewer;
        private System.Windows.Forms.MainMenu MainMenuRoot;
        private System.Windows.Forms.MenuItem FileMenu;
        private System.Windows.Forms.MenuItem CreateNewPlaylistFileMenuItem;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem OpenPlaylistFileMenuItem;
        private System.Windows.Forms.MenuItem OpenFolderFileMenuItem;
        private System.Windows.Forms.MenuItem ViewMenu;
        private System.Windows.Forms.MenuItem FindMenu;
        private System.Windows.Forms.MenuItem PlayMenu;
        private System.Windows.Forms.MenuItem HelpMenu;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem SavePlaylistFileMenuItem;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem AddFilesToPlaylistFileMenuItem;
        private System.Windows.Forms.MenuItem AddFolderToPlaylistFileMenuItem;
        private System.Windows.Forms.MenuItem AlwaysTopMostViewMenuItem;
        private System.Windows.Forms.MenuItem ShowFindDialogFindMenuItem;
        private System.Windows.Forms.MenuItem FindNextFindMenuItem;
        private System.Windows.Forms.MenuItem PlayPausePlaybackMenuItem;
        private System.Windows.Forms.MenuItem StopPlaybackMenuItem;
        private System.Windows.Forms.MenuItem PreviousTrackPlaybackMenuItem;
        private System.Windows.Forms.MenuItem NextTrackPlaybackMenuItem;
        private System.Windows.Forms.MenuItem ShowReadMeHelpMenuItem;
        private System.Windows.Forms.MenuItem ShowHistoryHelpMenuItem;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem ShowVersionDialogHelpMenuItem;
        private System.Windows.Forms.MenuItem OptionMenu;
        private System.Windows.Forms.MenuItem AvailableOutputDevicesPlaybackMenuItem;
        private System.Windows.Forms.MenuItem OutputLatencyPlaybackMenuItem;
        private System.Windows.Forms.MenuItem Latency16msLatencyMenuItem;
        private System.Windows.Forms.MenuItem Latency32msLatencyMenuItem;
        private System.Windows.Forms.MenuItem Latency64msLatencyMenuItem;
        private System.Windows.Forms.MenuItem Latency128msLatencyMenuItem;
        private System.Windows.Forms.MenuItem Latency256msLatencyMenuItem;
        private System.Windows.Forms.MenuItem Latency512msLatencyMenuItem;
        private LibPulseTune.UIControls.VolumeMeterControl LeftChannelVolumeMeter;
        private LibPulseTune.UIControls.VolumeMeterControl RightChannelVolumeMeter;
        private System.Windows.Forms.MenuItem ShowWasapiAndMmcssOptionMenuItem;
        private LibPulseTune.UIControls.WaveformRenderer WaveformRendererControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuItem ViewMenuSeparator1;
        private System.Windows.Forms.MenuItem WaveformRendererViewModesMenuItem;
        private System.Windows.Forms.MenuItem SeparateByChannelsWaveformRendererViewModeMenuItem;
        private System.Windows.Forms.MenuItem MixedWaveformRendererViewModeMenuItem;
    }
}
