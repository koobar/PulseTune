using LibPulseTune.AudioSource.Vorbis;
using System;
using System.ComponentModel;

namespace PulseTune.Metadata.Track
{
    internal class VorbisAudioTrack : GeneralPurposeAudioTrack
    {
        // 非公開フィールド
        private TimeSpan duration;
        private uint bitRate;

        #region コンストラクタ

        /// <summary>
        /// 指定されたパスのファイルを示すよう、Trackのインスタンスを初期化する。
        /// </summary>
        /// <param name="path"></param>
        public VorbisAudioTrack(string path, bool fastMode) : base(path, fastMode)
        {
            if (fastMode)
            {
                TimeSpan duration = TimeSpan.FromSeconds(0);
                uint bitRate = 0;

                var worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    var vorbis = new VorbisAudioSource(path);
                    duration = vorbis.GetDuration();
                    bitRate = vorbis.NominalBitrate / 1000;
                    vorbis.Dispose();
                };
                worker.RunWorkerCompleted += delegate
                {
                    this.duration = duration;
                    this.bitRate = bitRate;
                    worker.Dispose();
                };

                worker.RunWorkerAsync();
            }
            else
            {
                var vorbis = new VorbisAudioSource(path);
                this.duration = vorbis.GetDuration();
                this.bitRate = vorbis.NominalBitrate / 1000;
                vorbis.Dispose();
            }
        }

        public VorbisAudioTrack(string path) : this(path, false)
        {

        }

        #endregion

        #region プロパティ

        /// <summary>
        /// フォーマット名
        /// </summary>
        public override string FormatName
        {
            get
            {
                return "Vorbis";
            }
        }

        /// <summary>
        /// 演奏時間
        /// </summary>
        public override TimeSpan Duration
        {
            get
            {
                return this.duration;
            }
        }

        /// <summary>
        /// ビットレート
        /// </summary>
        public override uint Bitrate
        {
            get
            {
                return this.bitRate;
            }
        }

        #endregion
    }
}
