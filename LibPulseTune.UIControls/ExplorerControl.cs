using LibPulseTune.Engine.Playlists;
using LibPulseTune.Engine.Providers;
using LibPulseTune.Engine.Tracks;
using LibPulseTune.UIControls.BackendControls;
using LibPulseTune.UIControls.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LibPulseTune.UIControls
{
    public partial class ExplorerControl : UserControl, IMainTabControlPageElement
    {
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
            InitializeComponent();

            this.playlist = new Playlist();

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
                this.Viewer.ContextMenu = value;
            }
            get
            {
                return this.Viewer.ContextMenu;
            }
        }

        /// <summary>
        /// 選択されたファイル名の一覧
        /// </summary>
        public string[] SelectedFileNames
        {
            get
            {
                return this.Viewer.SelectedFileNames;
            }
        }

        /// <summary>
        /// 選択されたフォルダの一覧
        /// </summary>
        public string[] SelectedFolders
        {
            get
            {
                return this.Viewer.SelectedFileNames;
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

        /// <summary>
        /// 現在の場所
        /// </summary>
        public string CurrentPath
        {
            get
            {
                return this.Viewer.CurrentPath;
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

            for (int i = startIndex; i < this.Viewer.Items.Count; ++i)
            {
                var item = (FileSystemViewerItem)this.Viewer.Items[i];
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
                        this.Viewer.SelectIndex(index);
                    }

                    this.lastSearchPrompt = dialog.Prompt;
                    this.lastSearchIgnoreCase = dialog.IgnoreCase;
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
                this.Viewer.SelectIndex(index);
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
            this.Viewer.Reload();
        }

        /// <summary>
        /// 表示するファイルの拡張子の一覧を指定する。
        /// </summary>
        /// <param name="extensions"></param>
        public void SetFileFormatFilter(IEnumerable<string> extensions)
        {
            this.Viewer.SetFileFilter(extensions);
        }

        /// <summary>
        /// 指定されたドライブを開く
        /// </summary>
        /// <param name="driveLetter"></param>
        public void SelectDrive(char driveLetter)
        {
            this.Viewer.Navigate($"{driveLetter}:\\");
        }

        /// <summary>
        /// 指定されたパスを開く。
        /// </summary>
        /// <param name="path"></param>
        public void Navigate(string path)
        {
            var root = Directory.GetDirectoryRoot(path);

            if (root == path)
            {
                SelectDrive(path[0]);
            }
            else if (Directory.Exists(path))
            {
                // ドライブを選択する。
                SelectDrive(path[0]);

                // ディレクトリを開き、ファイルを選択する。
                this.Viewer.Navigate(path);
            }
            else
            {
                string fileName = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);

                // ドライブを選択する。
                SelectDrive(path[0]);

                // ディレクトリを開き、ファイルを選択する。
                this.Viewer.Navigate(path);
                this.Viewer.SelectFile(fileName);
            }
        }

        private void Viewer_Navigated(object sender, EventArgs e)
        {
            this.BackButton.Enabled = this.Viewer.CanGoBack();
            this.NextButton.Enabled = this.Viewer.CanGoForward();

            this.AddressTextBox.Text = this.Viewer.CurrentPath;

            this.playlist.Clear();
            foreach (var fileName in this.Viewer.FileNames)
            {
                this.playlist.Add(AudioTrackProvider.CreateFileFast(fileName));
            }

            this.Navigated?.Invoke(this, e);
        }

        private void Viewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedFileNamesChanged?.Invoke(this, EventArgs.Empty);

            if (this.Viewer.SelectedFileNames.Length >= 1 && this.Viewer.SelectedFolders.Length == 0)
            {
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
        }

        private void Viewer_DoubleClick(object sender, EventArgs e)
        {
            this.ItemDoubleClick?.Invoke(this, EventArgs.Empty);
        }

        private void Viewer_FileDoubleClick(object sender, EventArgs e)
        {
            this.FileDoubleClick?.Invoke(this, EventArgs.Empty);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Viewer.GoBack();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            this.Viewer.GoForward();
        }
    }
}
