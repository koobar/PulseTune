using LibPulseTune.Database;
using LibPulseTune.UIControls.BackendControls;
using LibPulseTune.UIControls.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class AccessList : ExplorerLikeListView
    {
        // 非公開定数
        private const string SHELL_NAMESPACE_QUICKACCESS = @"shell:::{679F85CB-0220-4080-B29B-5540CC05AAB6}";
        private const int WM_DEVICECHANGE = 0x219;              // デバイス変化のWindowsイベントの値
        private const int DBT_DEVICEARRIVAL = 0x8000;           // USBの挿入
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;    // USBの取り外し
        private const int DBT_DEVTYP_VOLUME = 0x00000002;       // デバイスの種類がボリューム

        // 非公開フィールド
        private readonly List<DriveInfo> connectedDrives;

        // イベント
        public event EventHandler LocationSelectionChanged;

        // コンストラクタ
        public AccessList()
        {
            this.connectedDrives = new List<DriveInfo>();

            this.View = View.Details;
            this.HeaderStyle = ColumnHeaderStyle.None;
            this.Columns.Add(new ColumnHeader() { Text = "場所" });
            this.SelectedIndexChanged += OnSelectedIndexChanged;

            MainWindowProcMgr.RegisterAction(WM_DEVICECHANGE, OnDeviceChanged);
        }

        // デストラクタ
        ~AccessList()
        {
            MainWindowProcMgr.UnregisterAction(WM_DEVICECHANGE, OnDeviceChanged);
        }

        /// <summary>
        /// 選択された場所
        /// </summary>
        public string SelectedLocation
        {
            set
            {
                if (this.IsDesignMode())
                {
                    return;
                }

                UpdateAvailableLocations();

                foreach (ListViewGroup group in this.Groups)
                {
                    foreach (ListViewItem item in group.Items)
                    {
                        item.Selected = item.Tag.ToString() == value;
                    }
                }
            }
            get
            {
                ListViewItem selection = null;
                foreach (ListViewGroup group in this.Groups)
                {
                    foreach (ListViewItem item in group.Items)
                    {
                        if (item.Selected)
                        {
                            selection = item;
                            break;
                        }
                    }
                }

                if (selection != null)
                {
                    return selection.Tag.ToString();
                }

                return null;
            }
        }

        /// <summary>
        /// ドライブを読み込む。
        /// </summary>
        private void LoadDrives()
        {
            var driveGroup = new ListViewGroup();
            driveGroup.Header = "ドライブ";
            foreach (var info in this.connectedDrives)
            {
                if (info.IsReady)
                {
                    var item = new ExplorerLikeListViewItem();
                    item.Tag = info.RootDirectory;
                    item.Icon = WinApi.ExtractIconFromPath(info.RootDirectory.FullName, WinApi.ExtractIconSize.Small);
                    item.Text = $"{info.RootDirectory.FullName} ({info.VolumeLabel})";

                    driveGroup.Items.Add(item);
                    this.Items.Add(item);
                }
            }
            this.Groups.Add(driveGroup);
        }

        /// <summary>
        /// クイックアクセスを読み込む。
        /// </summary>
        private void LoadQuickAccess()
        {
            // Windows 10以降か？
            if (Environment.OSVersion.Version.Major >= 10)
            {
                var shell = new Shell32.Shell();
                var folder = shell.NameSpace(SHELL_NAMESPACE_QUICKACCESS);
                var quickAccessGroup = new ListViewGroup();
                quickAccessGroup.Header = "クイックアクセス";

                foreach (Shell32.FolderItem folderItem in folder.Items())
                {
                    var item = new ExplorerLikeListViewItem();
                    item.Tag = folderItem.Path;
                    item.Icon = WinApi.ExtractIconFromPath(folderItem.Path, WinApi.ExtractIconSize.Small);
                    item.Text = Path.GetFileName(folderItem.Path);

                    quickAccessGroup.Items.Add(item);
                    this.Items.Add(item);
                }

                this.Groups.Insert(0, quickAccessGroup);
            }
        }

        /// <summary>
        /// お気に入りを読み込む。
        /// </summary>
        private void LoadFavorites()
        {
            var favoriteGroup = new ListViewGroup();
            favoriteGroup.Header = "お気に入り";
            
            for (int i = 0; i < PlaylistExplorerData.GetFavoriteLocationsCount(); i++)
            {
                var location = PlaylistExplorerData.GetFavoriteLocation(i);
                var item = new ExplorerLikeListViewItem();
                item.Tag = location;
                item.Icon = WinApi.ExtractIconFromPath(location, WinApi.ExtractIconSize.Small);
                item.Text = Path.GetFileName(location);

                favoriteGroup.Items.Add(item);
                this.Items.Add(item);
            }

            this.Groups.Insert(0, favoriteGroup);
        }

        /// <summary>
        /// 利用可能なドライブのリストを更新する。
        /// </summary>
        public void UpdateAvailableLocations()
        {
            // 古いアイテムとグループを初期化
            this.Items.Clear();
            this.Groups.Clear();

            LoadDrives();
            LoadQuickAccess();
            LoadFavorites();

            this.Columns[0].Width = -2;
        }

        private void UpdateConnectedDrives()
        {
            var currentDrives = DriveInfo.GetDrives();
            var flgChanged = false;

            if (currentDrives.Length != this.connectedDrives.Count)
            {
                flgChanged = true;
            }
            else
            {
                for (int i = 0; i < this.connectedDrives.Count; i++)
                {
                    if (!this.connectedDrives[i].Equals(currentDrives[i]))
                    {
                        flgChanged = true;
                        break;
                    }
                }
            }

            if (!flgChanged)
            {
                return;
            }

            this.connectedDrives.Clear();
            this.connectedDrives.AddRange(currentDrives);
            UpdateAvailableLocations();
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.LocationSelectionChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// コントロールのハンドルが作成された場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.IsDesignMode())
            {
                return;
            }

            UpdateConnectedDrives();
            UpdateAvailableLocations();
        }

        /// <summary>
        /// コントロールのハンドルが破棄された場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (this.IsDesignMode())
            {
                return;
            }
        }

        /// <summary>
        /// コンピュータに接続されたドライブの状態が変化した場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDriveStateChanged(object sender, EventArgs e)
        {
            if (this.IsDesignMode())
            {
                return;
            }

            var location = this.SelectedLocation;

            UpdateAvailableLocations();
            this.SelectedLocation = location;
        }
        
        /// <summary>
        /// コンピュータに接続された外部デバイスが変更された場合の処理
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        protected virtual void OnDeviceChanged(IntPtr wParam, IntPtr lParam)
        {
            if (wParam == IntPtr.Zero || lParam == IntPtr.Zero)
            {
                return;
            }

            long wp = wParam.ToInt64();
            int lp = Marshal.ReadInt32(lParam, 4);

            if (lp == DBT_DEVTYP_VOLUME)
            {
                switch (wp)
                {
                    case DBT_DEVICEARRIVAL:
                        UpdateConnectedDrives();
                        break;
                    case DBT_DEVICEREMOVECOMPLETE:
                        UpdateConnectedDrives();
                        break;
                }
            }
        }
    }
}
