using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PulseTune.Utils
{
    internal class DriveStateWatcher
    {
        // 非公開フィールド
        private readonly Dictionary<char, bool> driveReadyStateDictionary;
        private readonly Timer driveWatcherTimer;

        // イベント
        public event EventHandler DriveStateChanged;

        // コンストラクタ
        public DriveStateWatcher()
        {
            this.driveReadyStateDictionary = new Dictionary<char, bool>();
            this.driveWatcherTimer = new Timer();
            this.driveWatcherTimer.Interval = 3000;
            this.driveWatcherTimer.Tick += OnTimerTick;

            CheckDrives();
        }

        /// <summary>
        /// 開始
        /// </summary>
        public void Start()
        {
            this.driveWatcherTimer.Start();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.driveWatcherTimer.Stop();
        }

        /// <summary>
        /// ドライブリストの更新
        /// </summary>
        private void Update()
        {
            var drives = DriveInfo.GetDrives();
            this.driveReadyStateDictionary.Clear();

            foreach (var drive in drives)
            {
                this.driveReadyStateDictionary.Add(drive.RootDirectory.FullName[0], drive.IsReady);
            }
        }

        /// <summary>
        /// 使用可能ドライブの一覧を確認し、変化があればイベントを発生させる。
        /// </summary>
        protected virtual void CheckDrives()
        {
            var isStateChanged = false;
            var drives = DriveInfo.GetDrives();

            if (drives.Length == this.driveReadyStateDictionary.Count)
            {
                int cnt = 0;
                foreach (char driveLetter in this.driveReadyStateDictionary.Keys)
                {
                    if (driveLetter == drives[cnt].RootDirectory.FullName[0])
                    {
                        if (drives[cnt].IsReady != this.driveReadyStateDictionary[driveLetter])
                        {
                            isStateChanged = true;
                            break;
                        }
                    }
                    else
                    {
                        isStateChanged = true;
                        break;
                    }
                    cnt++;
                }
            }
            else
            {
                isStateChanged = true;
            }

            if (isStateChanged)
            {
                Update();
                this.DriveStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            CheckDrives();
        }
    }
}
