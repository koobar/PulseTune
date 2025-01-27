using LibPulseTune.Plugin.Sdk.Metadata.Track;
using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class PlaylistViewerItem : ListViewItem
    {
        // 非公開フィールド
        private readonly ListView parentListView;
        private AudioTrackBase track;
        private bool isHighlight;

        #region コンストラクタ

        public PlaylistViewerItem(ListView viewer)
        {
            this.parentListView = viewer;
        }

        public PlaylistViewerItem(ListView viewer, AudioTrackBase track) : this(viewer)
        {
            this.Track = track;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// このアイテムをハイライト表示するかどうかを示す。
        /// </summary>
        public bool IsHighlight
        {
            set
            {
                if (value)
                {
                    this.BackColor = Color.LightSteelBlue;
                }
                else
                {
                    this.BackColor = this.ListView.BackColor;
                }

                this.isHighlight = value;
            }
            get
            {
                return this.isHighlight;
            }
        }

        /// <summary>
        /// このアイテムに対応するトラック
        /// </summary>
        public AudioTrackBase Track
        {
            set
            {
                UpdateTrack(value);
                this.track = value;
            }
            get
            {
                return this.track;
            }
        }

        #endregion

        /// <summary>
        /// 指定されたトラックを表示するよう更新する。
        /// </summary>
        /// <param name="track"></param>
        protected virtual void UpdateTrack(AudioTrackBase track)
        {
            var items = new string[this.parentListView.Columns.Count];

            for (int i = 0; i < this.parentListView.Columns.Count; ++i)
            {
                var column = this.parentListView.Columns[i].Text;

                if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_TITLE)
                {
                    items[i] = track.Title;
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_ALBUM)
                {
                    items[i] = track.Album;
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_ARTIST)
                {
                    items[i] = track.Artist;
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_COMPOSERS)
                {
                    items[i] = track.Composers;
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_TRACKNUMBER)
                {
                    items[i] = $"{track.TrackNumber}";
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_BITRATE)
                {
                    items[i] = $"{track.Bitrate}kbps";
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_DURATION)
                {
                    items[i] = $"{track.Duration.Minutes}分{track.Duration.Seconds}秒";
                }
                else if (column == PlaylistViewer.COLUMN_HEADER_CAPTION_FORMAT)
                {
                    items[i] = track.FormatName;
                }
            }

            // 古いサブアイテムを初期化
            this.SubItems.Clear();

            if (items.Length > 0)
            {
                this.Text = items[0];
            }

            // サブアイテムを追加
            for (int i = 1; i < items.Length; ++i)
            {
                this.SubItems.Add(items[i]);
            }
        }
    }
}
