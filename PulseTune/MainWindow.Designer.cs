using LibPulseTune.Options;

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
            this.ShowWaveformRendererMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.SeparateByChannelsWaveformRendererViewModeMenuItem = new System.Windows.Forms.MenuItem();
            this.MixedWaveformRendererViewModeMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.HighestQualityWaveformRendererPrecisionMenuItem = new System.Windows.Forms.MenuItem();
            this.HighQualityWaveformRendererPrecisionMenuItem = new System.Windows.Forms.MenuItem();
            this.NormalQualityWaveformRendererPrecisionMenuItem = new System.Windows.Forms.MenuItem();
            this.LowQualityWaveformRendererPrecisionMenuItem = new System.Windows.Forms.MenuItem();
            this.LowestQualityWaveformRendererPrecisionMenuItem = new System.Windows.Forms.MenuItem();
            this.FindMenu = new System.Windows.Forms.MenuItem();
            this.ShowFindDialogFindMenuItem = new System.Windows.Forms.MenuItem();
            this.FindNextFindMenuItem = new System.Windows.Forms.MenuItem();
            this.PlayMenu = new System.Windows.Forms.MenuItem();
            this.PlayPausePlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.StopPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.PreviousTrackPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.NextTrackPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.AvailableOutputDevicesPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.OutputLatencyPlaybackMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.ShowWasapiAndMmcssOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpMenu = new System.Windows.Forms.MenuItem();
            this.ShowReadMeHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.ShowHistoryHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.ShowVersionDialogHelpMenuItem = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LevelMeterControl = new LibPulseTune.UIControls.LevelMeterPanel();
            this.WaveformRendererControl = new LibPulseTune.UIControls.WaveformPanel();
            this.AccessListControl = new LibPulseTune.UIControls.AccessList();
            this.TrackPictureViewer = new LibPulseTune.UIControls.PictureViewer();
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
            this.StatusText.BackColor = System.Drawing.SystemColors.Control;
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(118, 17);
            this.StatusText.Text = "PulseTune by koobar.";
            // 
            // PlaybackTimeStatusText
            // 
            this.PlaybackTimeStatusText.BackColor = System.Drawing.SystemColors.Control;
            this.PlaybackTimeStatusText.Name = "PlaybackTimeStatusText";
            this.PlaybackTimeStatusText.Size = new System.Drawing.Size(851, 17);
            this.PlaybackTimeStatusText.Spring = true;
            this.PlaybackTimeStatusText.Text = "00:00";
            this.PlaybackTimeStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.ShowWaveformRendererMenuItem,
            this.menuItem12,
            this.menuItem15,
            this.menuItem14});
            this.WaveformRendererViewModesMenuItem.Text = "波形表示モード";
            // 
            // ShowWaveformRendererMenuItem
            // 
            this.ShowWaveformRendererMenuItem.Index = 0;
            this.ShowWaveformRendererMenuItem.Text = "波形を表示する（高負荷）";
            this.ShowWaveformRendererMenuItem.Click += new System.EventHandler(this.ShowWaveformRendererMenuItem_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 1;
            this.menuItem12.Text = "-";
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 2;
            this.menuItem15.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SeparateByChannelsWaveformRendererViewModeMenuItem,
            this.MixedWaveformRendererViewModeMenuItem});
            this.menuItem15.Text = "ステレオ分離モード";
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
            // menuItem14
            // 
            this.menuItem14.Index = 3;
            this.menuItem14.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.HighestQualityWaveformRendererPrecisionMenuItem,
            this.HighQualityWaveformRendererPrecisionMenuItem,
            this.NormalQualityWaveformRendererPrecisionMenuItem,
            this.LowQualityWaveformRendererPrecisionMenuItem,
            this.LowestQualityWaveformRendererPrecisionMenuItem});
            this.menuItem14.Text = "描画の精度";
            // 
            // HighestQualityWaveformRendererPrecisionMenuItem
            // 
            this.HighestQualityWaveformRendererPrecisionMenuItem.Index = 0;
            this.HighestQualityWaveformRendererPrecisionMenuItem.Text = "最高品質";
            this.HighestQualityWaveformRendererPrecisionMenuItem.Click += new System.EventHandler(this.WaveformRendererRenderingPrecisionMenuItem_Click);
            // 
            // HighQualityWaveformRendererPrecisionMenuItem
            // 
            this.HighQualityWaveformRendererPrecisionMenuItem.Index = 1;
            this.HighQualityWaveformRendererPrecisionMenuItem.Text = "高品質";
            this.HighQualityWaveformRendererPrecisionMenuItem.Click += new System.EventHandler(this.WaveformRendererRenderingPrecisionMenuItem_Click);
            // 
            // NormalQualityWaveformRendererPrecisionMenuItem
            // 
            this.NormalQualityWaveformRendererPrecisionMenuItem.Index = 2;
            this.NormalQualityWaveformRendererPrecisionMenuItem.Text = "標準品質";
            this.NormalQualityWaveformRendererPrecisionMenuItem.Click += new System.EventHandler(this.WaveformRendererRenderingPrecisionMenuItem_Click);
            // 
            // LowQualityWaveformRendererPrecisionMenuItem
            // 
            this.LowQualityWaveformRendererPrecisionMenuItem.Index = 3;
            this.LowQualityWaveformRendererPrecisionMenuItem.Text = "低品質";
            this.LowQualityWaveformRendererPrecisionMenuItem.Click += new System.EventHandler(this.WaveformRendererRenderingPrecisionMenuItem_Click);
            // 
            // LowestQualityWaveformRendererPrecisionMenuItem
            // 
            this.LowestQualityWaveformRendererPrecisionMenuItem.Index = 4;
            this.LowestQualityWaveformRendererPrecisionMenuItem.Text = "最低品質";
            this.LowestQualityWaveformRendererPrecisionMenuItem.Click += new System.EventHandler(this.WaveformRendererRenderingPrecisionMenuItem_Click);
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
            this.NextTrackPlaybackMenuItem,
            this.menuItem1,
            this.AvailableOutputDevicesPlaybackMenuItem,
            this.OutputLatencyPlaybackMenuItem,
            this.menuItem2,
            this.ShowWasapiAndMmcssOptionMenuItem});
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
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "-";
            // 
            // AvailableOutputDevicesPlaybackMenuItem
            // 
            this.AvailableOutputDevicesPlaybackMenuItem.Index = 5;
            this.AvailableOutputDevicesPlaybackMenuItem.Text = "出力デバイス";
            // 
            // OutputLatencyPlaybackMenuItem
            // 
            this.OutputLatencyPlaybackMenuItem.Index = 6;
            this.OutputLatencyPlaybackMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem4,
            this.menuItem5,
            this.menuItem6,
            this.menuItem8,
            this.menuItem9,
            this.menuItem10});
            this.OutputLatencyPlaybackMenuItem.Text = "出力レイテンシ";
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 0;
            this.menuItem4.Tag = "16";
            this.menuItem4.Text = "16ms";
            this.menuItem4.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 1;
            this.menuItem5.Tag = "32";
            this.menuItem5.Text = "32ms";
            this.menuItem5.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 2;
            this.menuItem6.Tag = "64";
            this.menuItem6.Text = "64ms";
            this.menuItem6.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 3;
            this.menuItem8.Tag = "128";
            this.menuItem8.Text = "128ms";
            this.menuItem8.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 4;
            this.menuItem9.Tag = "256";
            this.menuItem9.Text = "256ms";
            this.menuItem9.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 5;
            this.menuItem10.Tag = "512";
            this.menuItem10.Text = "512ms";
            this.menuItem10.Click += new System.EventHandler(this.LatencyPlaybackMenuItem_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 7;
            this.menuItem2.Text = "-";
            // 
            // ShowWasapiAndMmcssOptionMenuItem
            // 
            this.ShowWasapiAndMmcssOptionMenuItem.Index = 8;
            this.ShowWasapiAndMmcssOptionMenuItem.Text = "WASAPI/MMCSS設定";
            this.ShowWasapiAndMmcssOptionMenuItem.Click += new System.EventHandler(this.ShowWasapiAndMmcssOptionMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.Index = 4;
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.LevelMeterControl);
            this.panel1.Controls.Add(this.WaveformRendererControl);
            this.panel1.Controls.Add(this.AccessListControl);
            this.panel1.Controls.Add(this.TrackPictureViewer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 548);
            this.panel1.TabIndex = 5;
            // 
            // LevelMeterControl
            // 
            this.LevelMeterControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LevelMeterControl.LeftMeterDecibels = -90F;
            this.LevelMeterControl.Location = new System.Drawing.Point(3, 315);
            this.LevelMeterControl.Name = "LevelMeterControl";
            this.LevelMeterControl.RightMeterDecibels = -90F;
            this.LevelMeterControl.Size = new System.Drawing.Size(174, 52);
            this.LevelMeterControl.TabIndex = 19;
            // 
            // WaveformRendererControl
            // 
            this.WaveformRendererControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WaveformRendererControl.EnableWaveformAntiAlias = true;
            this.WaveformRendererControl.Location = new System.Drawing.Point(3, 253);
            this.WaveformRendererControl.Name = "WaveformRendererControl";
            this.WaveformRendererControl.RenderingPrecision = LibPulseTune.Options.WaveformRendererRenderingPrecision.Normal;
            this.WaveformRendererControl.Size = new System.Drawing.Size(174, 60);
            this.WaveformRendererControl.StereoViewMode = LibPulseTune.Options.WaveformRendererStereoViewMode.Separated;
            this.WaveformRendererControl.TabIndex = 18;
            // 
            // AccessListControl
            // 
            this.AccessListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccessListControl.FullRowSelect = true;
            this.AccessListControl.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.AccessListControl.HideSelection = false;
            this.AccessListControl.ItemHeight = 0;
            this.AccessListControl.Location = new System.Drawing.Point(3, 3);
            this.AccessListControl.Name = "AccessListControl";
            this.AccessListControl.OwnerDraw = true;
            this.AccessListControl.SelectedLocation = null;
            this.AccessListControl.Size = new System.Drawing.Size(174, 249);
            this.AccessListControl.TabIndex = 17;
            this.AccessListControl.UseCompatibleStateImageBehavior = false;
            this.AccessListControl.View = System.Windows.Forms.View.Details;
            this.AccessListControl.DoubleClick += new System.EventHandler(this.AccessListControl_DoubleClick);
            // 
            // TrackPictureViewer
            // 
            this.TrackPictureViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackPictureViewer.BackColor = System.Drawing.SystemColors.Control;
            this.TrackPictureViewer.Location = new System.Drawing.Point(3, 371);
            this.TrackPictureViewer.Name = "TrackPictureViewer";
            this.TrackPictureViewer.Picture = null;
            this.TrackPictureViewer.Size = new System.Drawing.Size(174, 174);
            this.TrackPictureViewer.TabIndex = 9;
            this.TrackPictureViewer.TabStop = false;
            // 
            // MainTabControl
            // 
            this.MainTabControl.BackColor = System.Drawing.SystemColors.Control;
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
            this.ControlPanel.BackColor = System.Drawing.SystemColors.Control;
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
            this.BackColor = System.Drawing.SystemColors.Control;
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
            ((System.ComponentModel.ISupportInitialize)(this.TrackPictureViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private LibPulseTune.UIControls.MediaControlPanel ControlPanel;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ToolStripStatusLabel PlaybackTimeStatusText;
        private System.Windows.Forms.Splitter splitter1;
        private LibPulseTune.UIControls.ClosableTabControl MainTabControl;
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
        private System.Windows.Forms.MenuItem ViewMenuSeparator1;
        private System.Windows.Forms.MenuItem WaveformRendererViewModesMenuItem;
        private System.Windows.Forms.MenuItem SeparateByChannelsWaveformRendererViewModeMenuItem;
        private System.Windows.Forms.MenuItem MixedWaveformRendererViewModeMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem AvailableOutputDevicesPlaybackMenuItem;
        private System.Windows.Forms.MenuItem OutputLatencyPlaybackMenuItem;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem ShowWasapiAndMmcssOptionMenuItem;
        private System.Windows.Forms.MenuItem menuItem14;
        private System.Windows.Forms.MenuItem HighestQualityWaveformRendererPrecisionMenuItem;
        private System.Windows.Forms.MenuItem HighQualityWaveformRendererPrecisionMenuItem;
        private System.Windows.Forms.MenuItem NormalQualityWaveformRendererPrecisionMenuItem;
        private System.Windows.Forms.MenuItem LowQualityWaveformRendererPrecisionMenuItem;
        private System.Windows.Forms.MenuItem LowestQualityWaveformRendererPrecisionMenuItem;
        private LibPulseTune.UIControls.PictureViewer TrackPictureViewer;
        private LibPulseTune.UIControls.AccessList AccessListControl;
        private System.Windows.Forms.Panel panel1;
        private LibPulseTune.UIControls.WaveformPanel WaveformRendererControl;
        private LibPulseTune.UIControls.LevelMeterPanel LevelMeterControl;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem ShowWaveformRendererMenuItem;
        private System.Windows.Forms.MenuItem menuItem12;
    }
}
