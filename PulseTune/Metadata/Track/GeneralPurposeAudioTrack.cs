using LibPulseTune.Plugin.Sdk.Metadata.Track;
using System;
using System.ComponentModel;
using System.Drawing;
using Windows.Storage;
using Windows.Storage.FileProperties;
using static PulseTune.Utils.AsyncUtils;

namespace PulseTune.Metadata.Track
{
    /// <summary>
    /// WinRT APIを使用してタグが読み込まれたトラック
    /// </summary>
    public class GeneralPurposeAudioTrack : AudioTrackBase
    {
        // 非公開フィールド
        private StorageFile file;
        private MusicProperties musicProperties;
        private Thumbnail thumbnail;
        private bool isTagLoaded;

        #region コンストラクタ

        /// <summary>
        /// 指定されたパスのファイルを示すよう、Trackのインスタンスを初期化する。
        /// </summary>
        /// <param name="path"></param>
        public GeneralPurposeAudioTrack(string path, bool fastMode) : base(path)
        {
            if (fastMode)
            {
                var worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    this.file = CallAsyncMethod(StorageFile.GetFileFromPathAsync, path);

                    if (this.file == null)
                    {
                        return;
                    }

                    this.musicProperties = CallAsyncMethod(this.file.Properties.GetMusicPropertiesAsync);
                    this.thumbnail = new Thumbnail(this.file);
                };
                worker.RunWorkerCompleted += delegate
                {
                    this.isTagLoaded = true;
                    worker.Dispose();
                };
            }
            else
            {
                this.file = CallAsyncMethod(StorageFile.GetFileFromPathAsync, path);

                if (this.file == null)
                {
                    return;
                }

                this.musicProperties = CallAsyncMethod(this.file.Properties.GetMusicPropertiesAsync);
                this.thumbnail = new Thumbnail(this.file);
                this.isTagLoaded = true;
            }
        }

        /// <summary>
        /// 指定されたパスのファイルを示すよう、Trackのインスタンスを初期化する。
        /// </summary>
        /// <param name="path"></param>
        public GeneralPurposeAudioTrack(string path) : this(path, false) { }

        #endregion

        #region プロパティ

        /// <summary>
        /// このトラックがオーディオCDのトラックであるかどうかを示す。
        /// </summary>
        public override bool IsAudioCDTrack
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// フォーマット名
        /// </summary>
        public override string FormatName
        {
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// このトラックが含まれるオーディオCDが挿入されたドライブのドライブレターを示す。
        /// </summary>
        public override char AudioCDDriveLetter
        {
            get
            {
                return '\0';
            }
        }

        /// <summary>
        /// タイトル
        /// </summary>
        public override string Title
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Title = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null || string.IsNullOrEmpty(this.musicProperties.Title))
                {
                    return System.IO.Path.GetFileName(this.Path);
                }

                return this.musicProperties.Title;
            }
        }

        /// <summary>
        /// サブタイトル
        /// </summary>
        public override string Subtitle
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Subtitle = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (!this.isTagLoaded || string.IsNullOrEmpty(this.musicProperties.Subtitle))
                {
                    return "不明";
                }

                return this.musicProperties.Subtitle;
            }
        }

        /// <summary>
        /// アルバム名
        /// </summary>
        public override string Album
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Album = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (string.IsNullOrEmpty(this.musicProperties.Album))
                {
                    return "不明";
                }

                return this.musicProperties.Album;
            }
        }

        /// <summary>
        /// アーティスト名
        /// </summary>
        public override string Artist
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Artist = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (string.IsNullOrEmpty(this.musicProperties.Artist))
                {
                    return "不明";
                }

                return this.musicProperties.Artist;
            }
        }

        /// <summary>
        /// 発行者
        /// </summary>
        public override string Publisher
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Publisher = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                return this.musicProperties.Publisher;
            }
        }

        /// <summary>
        /// 作詞者
        /// </summary>
        public override string Songwriters
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Writers.Clear();
                foreach (string writer in value.Split(','))
                {
                    this.musicProperties.Writers.Add(writer);
                }
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (this.musicProperties.Writers.Count > 0)
                {
                    return string.Join(", ", this.musicProperties.Writers);
                }

                return "不明";
            }
        }

        /// <summary>
        /// 作曲者
        /// </summary>
        public override string Composers
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Composers.Clear();
                foreach (string composer in value.Split(','))
                {
                    this.musicProperties.Composers.Add(composer);
                }
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (this.musicProperties.Composers.Count > 0)
                {
                    return string.Join(", ", this.musicProperties.Composers);
                }

                return "不明";
            }
        }

        /// <summary>
        /// 指揮者
        /// </summary>
        public override string Conductors
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }
                this.musicProperties.Conductors.Clear();
                foreach (string conductor in value.Split(','))
                {
                    this.musicProperties.Conductors.Add(conductor);
                }
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return "不明";
                }

                if (this.musicProperties.Conductors.Count > 0)
                {
                    return string.Join(", ", this.musicProperties.Composers);
                }

                return "不明";
            }
        }

        /// <summary>
        /// トラック番号
        /// </summary>
        public override uint TrackNumber
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.TrackNumber = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return 0;
                }

                return this.musicProperties.TrackNumber;
            }
        }

        /// <summary>
        /// 年
        /// </summary>
        public override uint Year
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Year = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return 0;
                }

                return this.musicProperties.Year;
            }
        }

        /// <summary>
        /// ビットレート
        /// </summary>
        public override uint Bitrate
        {
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return 0;
                }

                return this.musicProperties.Bitrate / 1000;
            }
        }

        /// <summary>
        /// レーティング
        /// </summary>
        public override uint Rating
        {
            set
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    throw new NullReferenceException();
                }

                this.musicProperties.Rating = value;
            }
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return 0;
                }

                return this.musicProperties.Rating;
            }
        }

        /// <summary>
        /// 演奏時間
        /// </summary>
        public override TimeSpan Duration
        {
            get
            {
                if (!this.isTagLoaded || this.musicProperties == null)
                {
                    return TimeSpan.FromSeconds(0);
                }

                return this.musicProperties.Duration;
            }
        }

        /// <summary>
        /// トラックの画像
        /// </summary>
        public override Image Picture
        {
            get
            {
                if (!this.isTagLoaded)
                {
                    return null;
                }

                return this.thumbnail.AsImage();
            }
        }

        #endregion
    }
}
