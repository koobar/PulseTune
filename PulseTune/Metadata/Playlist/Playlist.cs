using PulseTune.Metadata.Track;
using System;
using System.Collections.Generic;

namespace PulseTune.Metadata.Playlist
{
    internal class Playlist
    {
        // 非公開フィールド
        private readonly List<AudioTrackBase> tracks;
        private int currentIndex;
        private bool flgSuspendEvent;

        // イベント
        public event EventHandler PlaylistChanged;

        // コンストラクタ
        public Playlist()
        {
            this.tracks = new List<AudioTrackBase>();
            this.IsEdited = false;
            this.Path = null;
            this.currentIndex = -1;

            Init();
        }

        #region プロパティ

        /// <summary>
        /// プレイリストの場所
        /// </summary>
        public string Path { set; get; }

        /// <summary>
        /// 選択されたトラック
        /// </summary>
        public AudioTrackBase SelectedTrack
        {
            set
            {
                if (value == null)
                {
                    this.currentIndex = -1;
                    return;
                }

                int idx = -1;

                for (int i = 0; i < this.tracks.Count; ++i)
                {
                    var track = GetTrack(i);

                    if (track == value || track.Path == value.Path)
                    {
                        idx = i;
                        break;
                    }
                }

                if (idx != -1)
                {
                    this.currentIndex = idx;
                }
            }
            get
            {
                if (this.currentIndex == -1)
                {
                    return null;
                }

                return this.tracks[this.currentIndex];
            }
        }

        /// <summary>
        /// プレイリストに含まれるトラック数
        /// </summary>
        public int Count
        {
            get
            {
                return this.tracks.Count;
            }
        }

        /// <summary>
        /// 編集済みであるかどうかを示す。
        /// </summary>
        public bool IsEdited { set; get; }

        #endregion

        /// <summary>
        /// プレイリストが変更された場合のイベントを強制的に発生させる。
        /// </summary>
        public void ForceInvokeChangedEvent()
        {
            this.PlaylistChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// プレイリストが変更された場合のイベントを発生させる。
        /// </summary>
        private void InvokeChangedEvent()
        {
            if (this.flgSuspendEvent)
            {
                return;
            }

            ForceInvokeChangedEvent();
        }

        /// <summary>
        /// プレイリストを初期化する。
        /// </summary>
        public void Init()
        {
            this.tracks.Clear();
            this.Path = null;
            this.IsEdited = false;
        }

        public void SuspendEvents()
        {
            this.flgSuspendEvent = true;
        }

        public void ResumeEvents()
        {
            this.flgSuspendEvent = false;
            InvokeChangedEvent();
        }

        /// <summary>
        /// トラックをクリアする
        /// </summary>
        public void Clear()
        {
            this.tracks.Clear();

            // 後始末
            this.IsEdited = true;
            InvokeChangedEvent();
        }

        /// <summary>
        /// 指定されたトラックがこのプレイリストに含まれているかを判定する。
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool Contains(AudioTrackBase track)
        {
            bool result = this.tracks.Contains(track);

            if (!result)
            {
                foreach (var cmp in this.tracks)
                {
                    if (cmp.IsAudioCDTrack == track.IsAudioCDTrack)
                    {
                        if (track.IsAudioCDTrack)
                        {
                            // 同じオーディオCDのトラックを指し示すトラックであるかどうかを判定する。
                            if (track.AudioCDDriveLetter == cmp.AudioCDDriveLetter && track.TrackNumber == cmp.TrackNumber)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            // 同じファイルを示すトラックであるかどうかを判定する。
                            if (cmp.Path.ToLower() == track.Path.ToLower())
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// トラックを追加する。
        /// </summary>
        /// <param name="tracks"></param>
        public void Add(params AudioTrackBase[] tracks)
        {
            this.tracks.AddRange(tracks);

            // 後始末
            this.IsEdited = true;
            InvokeChangedEvent();
        }

        /// <summary>
        /// 指定されたインデックスのトラックを取得する。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AudioTrackBase GetTrack(int index)
        {
            return this.tracks[index];
        }

        /// <summary>
        /// 次のトラックを選択する。
        /// </summary>
        public void SelectNextTrack()
        {
            this.currentIndex += 1;
            if (this.currentIndex >= this.tracks.Count)
            {
                this.currentIndex = 0;
            }
        }

        /// <summary>
        /// 前のトラックを選択する。
        /// </summary>
        public void SelectPreviousTrack()
        {
            this.currentIndex -= 1;
            if (this.currentIndex == -1)
            {
                this.currentIndex = this.tracks.Count - 1;
            }
        }
    }
}
