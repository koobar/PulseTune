using LibPulseTune;
using LibPulseTune.AudioSource;
using Microsoft.WindowsAPICodePack.Dialogs;
using PulseTune.Controls.BackendControls;
using PulseTune.Dialogs;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using PulseTune.Metadata.Track;
using PulseTune.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PulseTune.Controls
{
    internal partial class PlaylistViewer : UserControl, IMainTabControlPageElement
    {
        // 非公開定数
        internal const string COLUMN_HEADER_CAPTION_TITLE = @"タイトル";
        internal const string COLUMN_HEADER_CAPTION_ALBUM = @"アルバム";
        internal const string COLUMN_HEADER_CAPTION_ARTIST = @"アーティスト";
        internal const string COLUMN_HEADER_CAPTION_COMPOSERS = @"作曲者";
        internal const string COLUMN_HEADER_CAPTION_TRACKNUMBER = @"トラック番号";
        internal const string COLUMN_HEADER_CAPTION_BITRATE = @"ビットレート";
        internal const string COLUMN_HEADER_CAPTION_DURATION = @"演奏時間";
        internal const string COLUMN_HEADER_CAPTION_FORMAT = @"種類";

        // 非公開フィールド
        private readonly ColumnHeader TitleColumnHeader;
        private readonly ColumnHeader AlbumColumnHeader;
        private readonly ColumnHeader ArtistColumnHeader;
        private readonly ColumnHeader ComposersColumnHeader;
        private readonly ColumnHeader TrackNumberColumnHeader;
        private readonly ColumnHeader BitrateColumnHeader;
        private readonly ColumnHeader DurationColumnHeader;
        private readonly ColumnHeader FormatColumnHeader;
        private readonly Dictionary<int, ListViewColumnSorter> sorters;
        private Playlist playlist;
        private string latestFindPrompt;
        private bool latestFindIgnoreCase;
        private int latestFindResultIndex;
        private string displayName;

        private readonly ContextMenu contextMenu;
        private readonly MenuItem AddTrackToPlaylistMenuItem;
        private readonly MenuItem MoveUpSelectedTrackMenuItem;
        private readonly MenuItem MoveDownSelectedTrackMenuItem;
        private readonly MenuItem DeleteSelectedTrackFromPlaylistMenuItem;
        private readonly MenuItem OpenTrackLocationMenuItem;
        private readonly MenuItem ShowSelectedTrackDetailsMenuItem;

        // イベント
        public event EventHandler TrackDoubleClick;
        public event EventHandler DisplayNameChanged;

        // コンストラクタ
        public PlaylistViewer()
        {
            InitializeComponent();

            this.contextMenu = new ContextMenu();
            this.contextMenu.Popup += ContextMenu_Popup;
            this.ContextMenu = this.contextMenu;
            this.AddTrackToPlaylistMenuItem = new MenuItem();
            this.AddTrackToPlaylistMenuItem.Text = "トラックを追加(&A)...";
            this.AddTrackToPlaylistMenuItem.Click += AddTrackToPlaylistMenuItem_Click;
            this.MoveUpSelectedTrackMenuItem = new MenuItem();
            this.MoveUpSelectedTrackMenuItem.Text = "上に移動";
            this.MoveUpSelectedTrackMenuItem.Click += MoveUpSelectedTrackMenuItem_Click;
            this.MoveDownSelectedTrackMenuItem = new MenuItem();
            this.MoveDownSelectedTrackMenuItem.Text = "下に移動";
            this.MoveDownSelectedTrackMenuItem.Click += MoveDownSelectedTrackMenuItem_Click;
            this.DeleteSelectedTrackFromPlaylistMenuItem = new MenuItem();
            this.DeleteSelectedTrackFromPlaylistMenuItem.Text = "プレイリストから削除(&D)";
            this.DeleteSelectedTrackFromPlaylistMenuItem.Click += DeleteSelectedTrackFromPlaylistMenuItem_Click;
            this.OpenTrackLocationMenuItem = new MenuItem();
            this.OpenTrackLocationMenuItem.Text = "エクスプローラーで開く";
            this.OpenTrackLocationMenuItem.Click += OpenTrackLocationMenuItem_Click;
            this.ShowSelectedTrackDetailsMenuItem = new MenuItem();
            this.ShowSelectedTrackDetailsMenuItem.Text = "トラックのプロパティ";
            this.ShowSelectedTrackDetailsMenuItem.Click += ShowSelectedTrackDetailsMenuItem_Click;

            this.playlist = new Playlist();
            this.playlist.PlaylistChanged += OnPlaylistChanged;

            this.PlaylistItemViewer.AllowColumnReorder = false;     // 列ヘッダの並び替えを禁止
            this.PlaylistItemViewer.DragEnter += PlaylistItemViewer_DragEnter;
            this.PlaylistItemViewer.DragDrop += PlaylistItemViewer_DragDrop;

            this.TitleColumnHeader = new ColumnHeader();
            this.TitleColumnHeader.Text = COLUMN_HEADER_CAPTION_TITLE;
            this.AlbumColumnHeader = new ColumnHeader();
            this.AlbumColumnHeader.Text = COLUMN_HEADER_CAPTION_ALBUM;
            this.ArtistColumnHeader = new ColumnHeader();
            this.ArtistColumnHeader.Text = COLUMN_HEADER_CAPTION_ARTIST;
            this.ComposersColumnHeader = new ColumnHeader();
            this.ComposersColumnHeader.Text = COLUMN_HEADER_CAPTION_COMPOSERS;
            this.TrackNumberColumnHeader = new ColumnHeader();
            this.TrackNumberColumnHeader.Text = COLUMN_HEADER_CAPTION_TRACKNUMBER;
            this.BitrateColumnHeader = new ColumnHeader();
            this.BitrateColumnHeader.Text = COLUMN_HEADER_CAPTION_BITRATE;
            this.DurationColumnHeader = new ColumnHeader();
            this.DurationColumnHeader.Text = COLUMN_HEADER_CAPTION_DURATION;
            this.FormatColumnHeader = new ColumnHeader();
            this.FormatColumnHeader.Text = COLUMN_HEADER_CAPTION_FORMAT;

            // 列ヘッダを追加する。
            this.PlaylistItemViewer.Columns.Add(this.TitleColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.AlbumColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.ArtistColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.ComposersColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.TrackNumberColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.BitrateColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.DurationColumnHeader);
            this.PlaylistItemViewer.Columns.Add(this.FormatColumnHeader);

            // 各列ヘッダに対応するListViewColumnSorterを追加
            this.sorters = new Dictionary<int, ListViewColumnSorter>()
            {
                { this.TitleColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.TitleColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.AlbumColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.AlbumColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.ArtistColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.ArtistColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.ComposersColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.ComposersColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.TrackNumberColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.TrackNumberColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.BitrateColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.BitrateColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
                { this.DurationColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.DurationColumnHeader.DisplayIndex, Order = SortOrder.Ascending} },
                { this.FormatColumnHeader.DisplayIndex, new ListViewColumnSorter() { SortColumn = this.FormatColumnHeader.DisplayIndex, Order = SortOrder.Ascending } },
            };

            this.PlaylistItemViewer.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.Font = SystemFonts.CaptionFont;
        }

        /// <summary>
        /// 紐づけられたプレイリスト
        /// </summary>
        public Playlist Playlist
        {
            set
            {
                this.playlist = value;
            }
            get
            {
                return this.playlist;
            }
        }

        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName
        {
            set
            {
                this.displayName = value;
                this.DisplayNameChanged?.Invoke(this, EventArgs.Empty);
            }
            get
            {
                return this.displayName;
            }
        }

        #region UI処理の実装

        /// <summary>
        /// UI表示上で選択されているトラックを指定されたトラックに設定する。<br/>
        /// ※プレイリストのDB側の選択は変更しない。
        /// </summary>
        /// <param name="track"></param>
        public void SetUITrackSelectionTo(AudioTrackBase track)
        {
            this.PlaylistItemViewer.SelectedIndices.Clear();

            int index = -1;
            for (int i = 0; i < this.PlaylistItemViewer.Items.Count; ++i)
            {
                var item = (PlaylistViewerItem)this.PlaylistItemViewer.Items[i];

                if (item.Track == track)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return;
            }

            this.PlaylistItemViewer.SelectedIndices.Add(index);
        }

        /// <summary>
        /// UI表示上で選択されているトラックを取得する。
        /// </summary>
        /// <returns></returns>
        public AudioTrackBase GetUITrackSelection()
        {
            if (this.PlaylistItemViewer.SelectedItems.Count > 0)
            {
                return ((PlaylistViewerItem)this.PlaylistItemViewer.SelectedItems[0]).Track;
            }

            return null;
        }

        /// <summary>
        /// 開かれているプレイリストに未保存の変更があれば、それを保存するかどうかをユーザに確認し、指示に従って保存する。
        /// </summary>
        /// <param name="cancel"></param>
        public void AskSavePlaylist(out bool cancel)
        {
            cancel = false;

            if (this.Playlist.IsEdited)
            {
                var result = MessageBox.Show(
                    "プレイリストに加えられた未保存の変更があります。変更内容を保存しますか？",
                    "未保存の変更があります",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    ExportPlaylist();
                }
                else if (result == DialogResult.Cancel)
                {
                    cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// プレイリストの表示を更新する。
        /// </summary>
        public void UpdateView()
        {
            this.PlaylistItemViewer.Items.Clear();

            for (int i = 0; i < this.playlist.Count; ++i)
            {
                var track = this.playlist.GetTrack(i);
                var item = new PlaylistViewerItem(this.PlaylistItemViewer, track);

                this.PlaylistItemViewer.Items.Add(item);
            }

            AutoResizeColumns();
        }

        /// <summary>
        /// プレイリストのデータベースを、表示上のプレイリストと同期する。
        /// </summary>
        private void SyncPlaylist()
        {
            // 古い要素をすべて削除
            this.playlist.SuspendEvents();
            this.playlist.Clear();

            // 新しい並び順で追加
            foreach (PlaylistViewerItem item in this.PlaylistItemViewer.Items)
            {
                if (item != null && item.Track != null)
                {
                    this.playlist.Add(item.Track);
                }
            }

            this.playlist.ResumeEvents();
        }

        /// <summary>
        /// プレイリストビューの列の幅を自動調整する。
        /// </summary>
        private void AutoResizeColumns()
        {
            for (int i = 0; i < this.PlaylistItemViewer.Columns.Count; ++i)
            {
                this.PlaylistItemViewer.Columns[i].Width = -2;
            }

            // 幅の最大値を設定
            for (int i = 0; i < this.PlaylistItemViewer.Columns.Count; ++i)
            {
                int width = this.PlaylistItemViewer.Columns[i].Width;

                if (width >= 200)
                {
                    width = 200;
                }

                this.PlaylistItemViewer.Columns[i].Width = width;
            }
        }

        #endregion

        #region プレイリストの編集機能の実装

        public bool CanAddTrackToPlaylist()
        {
            return true;
        }

        public bool CanSelectAddTrack()
        {
            return true;
        }

        public bool CanSelectAddFolder()
        {
            return true;
        }

        public bool CanExportPlaylist()
        {
            return true;
        }

        /// <summary>
        /// 指定されたトラックをプレイリストに追加する。
        /// </summary>
        /// <param name="tracks"></param>
        public void AddTrackToPlaylist(params AudioTrackBase[] tracks)
        {
            this.Playlist.Add(tracks);

            int state = AudioPlayer.GetAudioPlayerState();
            if (state == AudioPlayer.AUDIOPLAYER_NOT_READY || state == AudioPlayer.AUDIOPLAYER_STATE_STOP)
            {
                UpdateView();
                return;
            }

            UpdateView();
        }

        /// <summary>
        /// 開くファイルを選択するダイアログを表示し、選択されたオーディオファイルをプレイリストのトラックとして追加する。
        /// </summary>
        public void SelectAddTrack()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Filter = AudioEngine.GetSupportedPlaybackFileFormatsDialogFilterString();

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var tracks = new List<AudioTrackBase>();

                    for (int i = 0; i < dialog.FileNames.Length; ++i)
                    {
                        if (AudioSourceProvider.IsPlaybackSupportedFileFormat(dialog.FileNames[i]))
                        {
                            var track = AudioTrackProvider.CreateFile(dialog.FileNames[i]);

                            if (track != null)
                            {
                                tracks.Add(track);
                            }
                        }
                    }

                    AddTrackToPlaylist(tracks.ToArray());
                }
            }
        }

        /// <summary>
        /// 開くフォルダを選択するダイアログを表示し、選択されたフォルダに含まれる
        /// ーディオファイルを、プレイリストのトラックとして追加する。
        /// </summary>
        public void SelectAddFolder()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Multiselect = true;
                
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var tracks = new List<AudioTrackBase>();

                    foreach (string fileName in dialog.FileNames)
                    {
                        foreach (string path in Directory.GetFiles(fileName))
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
                    }

                    AddTrackToPlaylist(tracks.ToArray());

                    // 後始末
                    PlaylistExplorerData.AddToRecent(dialog.FileName);
                    this.DisplayName = Path.GetFileName(dialog.FileName);
                }
            }
        }

        /// <summary>
        /// プレイリストをエクスポートする。
        /// </summary>
        public void ExportPlaylist()
        {
            string path = this.Playlist.Path;

            // プレイリストファイルのパスが空なら保存先を選択するダイアログを表示する。
            if (string.IsNullOrEmpty(path))
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Filter = "M3U プレイリスト(*.m3u;*.m3u8)|*.m3u;*.m3u8";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        path = dialog.FileName;
                    }
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("有効な保存先が選択されなかったため、操作は取り消されました。", "操作の取り消し", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ファイルに保存する。
            var writer = PlaylistWriterProvider.GetPlaylistReader(path);
            writer.OpenFile(path);
            writer.Write(this.Playlist);
        }

        /// <summary>
        /// 選択されているトラックを上に移動する。
        /// </summary>
        public void MoveUpSelectedTrack()
        {
            if (this.PlaylistItemViewer.SelectedIndices.Count > 0)
            {
                int selectedIndex = this.PlaylistItemViewer.SelectedIndices[0];

                if (selectedIndex != -1)
                {
                    int newIndex = selectedIndex - 1;
                    if (newIndex < 0)
                    {
                        newIndex = 0;
                    }

                    var item = this.PlaylistItemViewer.Items[selectedIndex];
                    this.PlaylistItemViewer.Items.RemoveAt(selectedIndex);
                    this.PlaylistItemViewer.Items.Insert(newIndex, item);
                }
            }

            SyncPlaylist();
        }

        /// <summary>
        /// 選択されているトラックを下に移動する。
        /// </summary>
        public void MoveDownSelectedTrack()
        {
            if (this.PlaylistItemViewer.SelectedIndices.Count > 0)
            {
                int selectedIndex = this.PlaylistItemViewer.SelectedIndices[0];

                if (selectedIndex != -1)
                {
                    int newIndex = selectedIndex + 1;
                    if (newIndex >= this.playlist.Count)
                    {
                        newIndex = this.playlist.Count - 1;
                    }

                    var item = this.PlaylistItemViewer.Items[selectedIndex];
                    this.PlaylistItemViewer.Items.RemoveAt(selectedIndex);
                    this.PlaylistItemViewer.Items.Insert(newIndex, item);
                }
            }

            SyncPlaylist();
        }

        /// <summary>
        /// 選択されているトラックを削除する。
        /// </summary>
        public void DeleteSelectedTrack()
        {
            if (this.PlaylistItemViewer.SelectedIndices.Count > 0)
            {
                this.PlaylistItemViewer.Items.RemoveAt(this.PlaylistItemViewer.SelectedIndices[0]);
                SyncPlaylist();
            }
        }

        /// <summary>
        /// 選択されているトラックの場所を開く。
        /// </summary>
        public void OpenSelectedTrackLocation()
        {
            var track = GetUITrackSelection();
            var path = string.Empty;

            if (track.IsAudioCDTrack)
            {
                path = $"{track.AudioCDDriveLetter}:\\Track{track.TrackNumber.ToString("00")}.cda";
            }
            else
            {
                path = track.Path;
            }

            ProcessUtils.OpenInExplorer(path);
        }

        /// <summary>
        /// 指定された列のコンテンツで並び変える。
        /// </summary>
        /// <param name="column"></param>
        private void Sort(int column)
        {
            var selectedTrack = this.playlist.SelectedTrack;

            // 並び替え順を取得
            var sorter = this.sorters[column];
            if (sorter.Order == SortOrder.Ascending)
            {
                sorter.Order = SortOrder.Descending;
            }
            else if (sorter.Order == SortOrder.Descending)
            {
                sorter.Order = SortOrder.Ascending;
            }

            // 並び替え処理
            this.PlaylistItemViewer.ListViewItemSorter = sorter;
            this.PlaylistItemViewer.Sort();

            // プレイリストが変更されても、PlaylistChangedイベントを発生させない。
            this.playlist.SuspendEvents();

            // 表示上のトラックリストとプレイリストを同期する。
            SyncPlaylist();

            // 並び替え前に選択されていたトラックがあればそれを再選択。
            if (selectedTrack != null)
            {
                this.playlist.SelectedTrack = selectedTrack;
            }

            // 後始末
            this.playlist.ResumeEvents();
        }

        #endregion

        #region プレイリスト内検索の実装

        /// <summary>
        /// 指定されたプロンプトでトラックを検索し、見つかったトラックを返す。見つからなければ-1を返す。
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public AudioTrackBase FindTrack(string prompt, bool ignoreCase)
        {
            var startIndex = this.latestFindResultIndex;
            AudioTrackBase result = null;

            if (prompt != this.latestFindPrompt)
            {
                this.latestFindPrompt = prompt;
                startIndex = 0;
            }

            this.latestFindIgnoreCase = ignoreCase;

            if (ignoreCase)
            {
                prompt = prompt.ToLower();
            }
            
            for (int i = startIndex; i < this.PlaylistItemViewer.Items.Count; ++i)
            {
                var item = (PlaylistViewerItem)this.PlaylistItemViewer.Items[i];
                var track = item.Track;
                
                // トラック情報を取得
                var trackInfo = $"{track.Path} {track.Path}";
                if (ignoreCase)
                {
                    trackInfo = trackInfo.ToLower();
                }

                // トラック情報の文字列内に、プロンプトに指定された文字列が含まれていれば、検索結果とする。
                if (trackInfo.Contains(prompt))
                {
                    result = track;
                }

                if (result != null)
                {
                    this.latestFindResultIndex = i;
                    break;
                }
            }

            return result;
        }

        public bool CanShowFindDialog()
        {
            return true;
        }

        /// <summary>
        /// 検索ダイアログを表示してトラックを検索し、見つかったトラックをハイライト表示する。
        /// </summary>
        public void ShowFindDialog()
        {
            using (var dialog = new FindDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var track = FindTrack(dialog.Prompt, dialog.IgnoreCase);

                    if (track != null)
                    {
                        SetUITrackSelectionTo(track);
                    }
                }
            }
        }

        /// <summary>
        /// 「次を検索」操作が実行可能な状態であるかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public bool CanFindNext()
        {
            return !string.IsNullOrEmpty(this.latestFindPrompt);
        }

        /// <summary>
        /// 前回の検索で使用された設定を用いて、前回のつづきからトラックを検索する。<br/>
        /// 初回検索ならShowFindDialogメソッドと同様の挙動をとる。
        /// </summary>
        public void FindNext()
        {
            if (!CanFindNext())
            {
                ShowFindDialog();
                return;
            }

            var track = FindTrack(this.latestFindPrompt, this.latestFindIgnoreCase);

            if (track != null)
            {
                SetUITrackSelectionTo(track);
            }
        }

        #endregion

        /// <summary>
        /// プレイリストが変更された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaylistChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        /// <summary>
        /// コントロールが読み込まれた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.AllowDrop = true;
            this.PlaylistItemViewer.AllowDrop = true;

            this.PlaylistItemViewer.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// ドラッグアンドドロップが行われた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistItemViewer_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            var list = new List<AudioTrackBase>();

            foreach (string path in fileName)
            {
                if (AudioSourceProvider.IsPlaybackSupportedFileFormat(path))
                {
                    list.Add(AudioTrackProvider.CreateFile(path));
                }
            }

            AddTrackToPlaylist(list.ToArray());
        }

        /// <summary>
        /// ドラッグアンドドロップを行うためのマウスカーソルがコントロールの領域に入った場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistItemViewer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// プレイリストビューがダブルクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistItemViewer_DoubleClick(object sender, EventArgs e)
        {
            var track = GetUITrackSelection();

            if (track != null)
            {
                this.playlist.SelectedTrack = track;
                this.TrackDoubleClick?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// プレイリストビューの列ヘッダがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistItemViewer_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (this.playlist.Count > 0)
            {
                Sort(e.Column);
            }
        }

        /// <summary>
        /// コンテキストメニューが開かれた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            bool trackSelected = GetUITrackSelection() != null;

            this.contextMenu.MenuItems.Clear();
            this.contextMenu.MenuItems.Add(this.AddTrackToPlaylistMenuItem);

            if (trackSelected)
            {
                this.contextMenu.MenuItems.Add(new MenuItem("-"));
                this.contextMenu.MenuItems.Add(this.MoveUpSelectedTrackMenuItem);
                this.contextMenu.MenuItems.Add(this.MoveDownSelectedTrackMenuItem);
                this.contextMenu.MenuItems.Add(new MenuItem("-"));
                this.contextMenu.MenuItems.Add(this.DeleteSelectedTrackFromPlaylistMenuItem);
                this.contextMenu.MenuItems.Add(new MenuItem("-"));
                this.contextMenu.MenuItems.Add(this.OpenTrackLocationMenuItem);
                this.contextMenu.MenuItems.Add(new MenuItem("-"));
                this.contextMenu.MenuItems.Add(this.ShowSelectedTrackDetailsMenuItem);
            }
        }

        /// <summary>
        /// トラックの詳細を表示するコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSelectedTrackDetailsMenuItem_Click(object sender, EventArgs e)
        {
            var track = GetUITrackSelection();

            if (track != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("パス：" + track.Path);
                sb.AppendLine("種類: " + track.FormatName);
                sb.AppendLine("タイトル: " + track.Title);
                sb.AppendLine("アルバム:" + track.Album);
                sb.AppendLine("アーティスト: " + track.Artist);
                sb.AppendLine("発行年: " + track.Year);
                sb.AppendLine("トラック番号: " + track.TrackNumber);

                MessageBox.Show(sb.ToString(), "プロパティ");
            }
        }

        /// <summary>
        /// トラックの場所を開くコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTrackLocationMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedTrackLocation();
        }

        /// <summary>
        /// プレイリストからトラックを削除するコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSelectedTrackFromPlaylistMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedTrack();
        }

        /// <summary>
        /// トラックを下に移動するコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveDownSelectedTrackMenuItem_Click(object sender, EventArgs e)
        {
            MoveDownSelectedTrack();
        }

        /// <summary>
        /// トラックを上に移動するコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveUpSelectedTrackMenuItem_Click(object sender, EventArgs e)
        {
            MoveUpSelectedTrack();
        }

        /// <summary>
        /// トラックを追加するコンテキストメニューがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTrackToPlaylistMenuItem_Click(object sender, EventArgs e)
        {
            SelectAddTrack();
        }
    }
}
