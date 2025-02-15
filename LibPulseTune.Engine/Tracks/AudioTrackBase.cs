using System;
using System.Drawing;

namespace LibPulseTune.Engine.Tracks
{
    public class AudioTrackBase
    {
        #region コンストラクタ

        protected AudioTrackBase(string path)
        {
            this.Path = path;
        }

        protected AudioTrackBase() { }

        #endregion

        #region プロパティ

        /// <summary>
        /// パス
        /// </summary>
        public string Path { private set; get; }

        /// <summary>
        /// このトラックがオーディオCDのトラックであるかどうかを示す。
        /// </summary>
        public virtual bool IsAudioCDTrack
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// このトラックが含まれるオーディオCDが挿入されたドライブのドライブレターを示す。
        /// </summary>
        public virtual char AudioCDDriveLetter
        {
            get
            {
                return '_';
            }
        }

        /// <summary>
        /// このトラックのトラック番号を示す。
        /// </summary>
        public virtual int AudioCDTrackNumber
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return -1;
            }
        }

        /// <summary>
        /// フォーマット名
        /// </summary>
        public virtual string FormatName
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// タイトル
        /// </summary>
        public virtual string Title
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// サブタイトル
        /// </summary>
        public virtual string Subtitle
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// アルバム名
        /// </summary>
        public virtual string Album
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// アーティスト名
        /// </summary>
        public virtual string Artist
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// 発行者
        /// </summary>
        public virtual string Publisher
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// 作詞者
        /// </summary>
        public virtual string Songwriters
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// 作曲者
        /// </summary>
        public virtual string Composers
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// 指揮者
        /// </summary>
        public virtual string Conductors
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return "不明";
            }
        }

        /// <summary>
        /// トラック番号
        /// </summary>
        public virtual uint TrackNumber
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 年
        /// </summary>
        public virtual uint Year
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// ビットレート
        /// </summary>
        public virtual uint Bitrate
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// レーティング
        /// </summary>
        public virtual uint Rating
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 演奏時間
        /// </summary>
        public virtual TimeSpan Duration
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return TimeSpan.FromSeconds(0);
            }
        }

        /// <summary>
        /// トラックの画像
        /// </summary>
        public virtual Image Picture
        {
            set
            {
                throw new NotImplementedException();
            }
            get
            {
                return null;
            }
        }

        #endregion
    }
}
