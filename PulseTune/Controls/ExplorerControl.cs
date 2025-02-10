using PulseTune.Controls.BackendControls;
using PulseTune.Dialogs;
using PulseTune.Metadata;
using PulseTune.Metadata.Playlist;
using PulseTune.Metadata.Track;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PulseTune.Controls
{
    internal class ExplorerControl : UserControl, IMainTabControlPageElement
    {
        // パネル
        private readonly Panel leftPanel;
        private readonly Panel mainPanel;

        // 左パネルに配置するもの
        private readonly ExplorerControlDetailViewer detailsViewer;

        // メインパネルに配置するもの
        private readonly Panel navigationPanel;
        private readonly VisualStyleIconButton backButton;
        private readonly VisualStyleIconButton forwardButton;
        private readonly VerticalTextBox currentLocationTextBox;
        private readonly FileSystemViewer viewer;

        // 非公開フィールド
        private readonly Playlist playlist;
        private string lastSearchPrompt;
        private bool lastSearchIgnoreCase;
        private int lastFindResultIndex;

        // イベント
        public event EventHandler ItemDoubleClick;
        public event EventHandler FileDoubleClick;
        public event EventHandler SelectedFileNamesChanged;
        public event EventHandler Navigated;

        // コンストラクタ
        public ExplorerControl()
        {
            this.playlist = new Playlist();
            
            // 各種パネルを作成
            this.leftPanel = new Panel();
            this.leftPanel.Dock = DockStyle.Left;
            this.leftPanel.Width = 150;
            this.mainPanel = new Panel();
            this.mainPanel.Dock = DockStyle.Fill;

            // 左パネルに配置するコントロールを作成
            this.detailsViewer = new ExplorerControlDetailViewer();
            this.detailsViewer.Dock = DockStyle.Fill;
            this.detailsViewer.LocationSelectionChanged += OnDetailsViewerLocationSelectionChanged;

            // メインパネルに配置するコントロールを作成
            this.navigationPanel = new Panel();
            this.navigationPanel.Dock = DockStyle.Top;
            this.navigationPanel.Height = 20;
            this.navigationPanel.BackColor = Color.White;
            this.backButton = new VisualStyleIconButton(
                VisualStyleElement.ScrollBar.ArrowButton.LeftHot,
                VisualStyleElement.ScrollBar.ArrowButton.LeftHot,
                VisualStyleElement.ScrollBar.ArrowButton.LeftPressed,
                VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled);
            this.backButton.Dock = DockStyle.Left;
            this.backButton.Width = this.navigationPanel.Height;
            this.backButton.MouseDown += OnBackButtonClick;
            this.forwardButton = new VisualStyleIconButton(
                VisualStyleElement.ScrollBar.ArrowButton.RightHot,
                VisualStyleElement.ScrollBar.ArrowButton.RightHot,
                VisualStyleElement.ScrollBar.ArrowButton.RightPressed,
                VisualStyleElement.ScrollBar.ArrowButton.RightDisabled);
            this.forwardButton.Dock = DockStyle.Left;
            this.forwardButton.Width = this.navigationPanel.Height;
            this.forwardButton.Enabled = false;
            this.forwardButton.MouseDown += OnForwardButtonClick;
            this.currentLocationTextBox = new VerticalTextBox();
            this.currentLocationTextBox.Dock = DockStyle.Fill;
            this.currentLocationTextBox.ReadOnly = true;
            this.currentLocationTextBox.TabStop = false;
            this.currentLocationTextBox.BackColor = SystemColors.Control;
            this.currentLocationTextBox.Height = this.navigationPanel.Height;
            this.viewer = new FileSystemViewer();
            this.viewer.Dock = DockStyle.Fill;
            this.viewer.Navigated += OnViewerNavigated;
            this.viewer.SelectedIndexChanged += OnViewerSelectedIndexChanged;
            this.viewer.DoubleClick += OnViewerDoubleClick;
            this.viewer.FileDoubleClick += OnViewerFileClick;

            // ナビゲーションパネルにコントロールを追加
            this.navigationPanel.Controls.Add(this.currentLocationTextBox);
            this.navigationPanel.Controls.Add(this.forwardButton);
            this.navigationPanel.Controls.Add(this.backButton);

            // 左パネルにコントロールを追加
            this.leftPanel.Controls.Add(this.detailsViewer);

            // メインパネルにコントロールを追加
            this.mainPanel.Controls.Add(this.viewer);
            this.mainPanel.Controls.Add(this.navigationPanel);
            
            // 各種パネルとセパレータを追加
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(new Splitter());
            this.Controls.Add(this.leftPanel);

            // デフォルトフォントを設定
            this.Font = SystemFonts.CaptionFont;
        }

        #region プロパティ

        /// <summary>
        /// コンテキストメニュー
        /// </summary>
        public new ContextMenu ContextMenu
        {
            set
            {
                this.viewer.ContextMenu = value;
            }
            get
            {
                return this.viewer.ContextMenu;
            }
        }

        /// <summary>
        /// 選択されたファイル名の一覧
        /// </summary>
        public string[] SelectedFileNames
        {
            get
            {
                return this.viewer.SelectedFileNames;
            }
        }

        /// <summary>
        /// 選択されたフォルダの一覧
        /// </summary>
        public string[] SelectedFolders
        {
            get
            {
                return this.viewer.SelectedFileNames;
            }
        }

        /// <summary>
        /// 選択されたファイル名
        /// </summary>
        public string SelectedFileName
        {
            get
            {
                var files = this.SelectedFileNames;

                if (files.Length == 0)
                {
                    return null;
                }

                return files[0];
            }
        }

        /// <summary>
        /// 選択されたフォルダ名
        /// </summary>
        public string SelectedFolder
        {
            get
            {
                var folders = this.SelectedFolders;

                if (folders.Length == 0)
                {
                    return null;
                }

                return folders[0];
            }
        }

        public Playlist Playlist
        {
            get
            {
                return this.playlist;
            }
        }

        #endregion

        #region 検索の実装

        /// <summary>
        /// 指定された検索設定でトラックを検索し、条件に一致するトラックのインデックスを返す。
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        private IList<int> FindTrack(int startIndex, string prompt, bool ignoreCase)
        {
            var result = new List<int>();

            if (string.IsNullOrEmpty(prompt))
            {
                return result;
            }

            if (ignoreCase)
            {
                prompt = prompt.ToLower();
            }

            for (int i = startIndex; i < this.viewer.Items.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.viewer.Items[i];
                var track = item.Path;
                if (ignoreCase)
                {
                    track = track.ToLower();
                }

                if (track.Contains(prompt))
                {
                    result.Add(i);
                    this.lastFindResultIndex = i;
                }
            }

            return result;
        }

        public bool CanShowFindDialog()
        {
            return true;
        }

        public bool CanFindNext()
        {
            return !string.IsNullOrEmpty(this.lastSearchPrompt);
        }

        /// <summary>
        /// 検索ダイアログを表示して最初から検索
        /// </summary>
        public void ShowFindDialog()
        {
            if (!CanShowFindDialog())
            {
                return;
            }

            using (var dialog = new FindDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var index = FindTrack(0, dialog.Prompt, dialog.IgnoreCase);

                    if (index.Count > 0)
                    {
                        this.viewer.SelectIndex(index);
                    }
                }
            }
        }

        /// <summary>
        /// 次を検索
        /// </summary>
        public void FindNext()
        {
            if (!CanFindNext())
            {
                ShowFindDialog();
            }

            var index = FindTrack(this.lastFindResultIndex, this.lastSearchPrompt, this.lastSearchIgnoreCase);

            if (index.Count > 0)
            {
                this.viewer.SelectIndex(index);
            }
        }

        #endregion

        public bool CanSelectAddTrack()
        {
            return false;
        }

        public bool CanSelectAddFolder()
        {
            return false;
        }

        public bool CanExportPlaylist()
        {
            return false;
        }

        public bool CanAddTrackToPlaylist()
        {
            return false;
        }

        public void AddTrackToPlaylist(params AudioTrackBase[] tracks)
        {
            throw new NotImplementedException();
        }

        public void SelectAddTrack()
        {
            throw new NotImplementedException();
        }

        public void SelectAddFolder()
        {
            throw new NotImplementedException();
        }

        public void ExportPlaylist()
        {
            throw new NotImplementedException();
        }

        public void UpdateView()
        {
            Reload();
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        public void Reload()
        {
            this.viewer.Reload();
        }

        /// <summary>
        /// 表示するファイルの拡張子の一覧を指定する。
        /// </summary>
        /// <param name="extensions"></param>
        public void SetFileFormatFilter(IEnumerable<string> extensions)
        {
            this.viewer.SetFileFilter(extensions);
        }

        /// <summary>
        /// 指定されたドライブを開く
        /// </summary>
        /// <param name="driveLetter"></param>
        public void SelectDrive(char driveLetter)
        {
            this.detailsViewer.SelectedLocation = $"{driveLetter}:\\";
        }

        /// <summary>
        /// 指定されたパスを開く。
        /// </summary>
        /// <param name="path"></param>
        public void Navigate(string path)
        {
            string fileName = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);

            // ドライブを選択する。
            SelectDrive(path[0]);

            // ディレクトリを開き、ファイルを選択する。
            this.viewer.Navigate(path);
            this.viewer.SelectFile(fileName);
        }

        private void OnDetailsViewerLocationSelectionChanged(object sender, EventArgs e)
        {
            string path = this.detailsViewer.SelectedLocation;

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            this.viewer.Navigate(path);
        }

        private void OnViewerSelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedFileNamesChanged?.Invoke(this, EventArgs.Empty);

            if (this.viewer.SelectedFileNames.Length >= 1 && this.viewer.SelectedFolders.Length == 0)
            {
                this.detailsViewer.ShowDetails(this.viewer.SelectedFileNames[0]);

                AudioTrackBase audioTrack = null;
                for (int i = 0; i < this.playlist.Count; ++i)
                {
                    var track = this.playlist.GetTrack(i);
                    if (!track.IsAudioCDTrack)
                    {
                        if (track.Path == this.SelectedFileNames[0])
                        {
                            audioTrack = track;
                            break;
                        }
                    }
                    else
                    {
                        string fileName = $"Track{track.AudioCDTrackNumber.ToString("00")}";
                        if (Path.GetFileNameWithoutExtension(this.SelectedFileNames[0]) == fileName)
                        {
                            audioTrack = track;
                            break;
                        }
                    }
                }

                if (audioTrack != null)
                {
                    this.playlist.SelectedTrack = audioTrack;
                }
            }
            else if (this.viewer.SelectedFileNames.Length == 0 && this.viewer.SelectedFolders.Length >= 1)
            {
                this.detailsViewer.ShowDetails(this.viewer.SelectedFolders[0]);
            }
        }

        private void OnViewerDoubleClick(object sender, EventArgs e)
        {
            this.ItemDoubleClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnViewerFileClick(object sender, EventArgs e)
        {
            this.FileDoubleClick?.Invoke(this, e);
        }

        private void OnViewerNavigated(object sender, EventArgs e)
        {
            this.backButton.Enabled = this.viewer.CanGoBack();
            this.forwardButton.Enabled = this.viewer.CanGoForward();

            this.detailsViewer.ShowDetails(this.viewer.CurrentPath);
            this.currentLocationTextBox.Text = this.viewer.CurrentPath;

            this.playlist.Clear();
            foreach (var fileName in this.viewer.FileNames)
            {
                this.playlist.Add(AudioTrackProvider.CreateFileFast(fileName));
            }

            this.Navigated?.Invoke(this, e);
        }

        private void OnForwardButtonClick(object sender, EventArgs e)
        {
            this.viewer.GoForward();
        }

        private void OnBackButtonClick(object sender, EventArgs e)
        {
            this.viewer.GoBack();
        }
    }
}
