using LibPulseTune.Plugin.Sdk.Metadata.Playlist;
using LibPulseTune.Plugin.Sdk.Metadata.Track;

namespace PulseTune.Controls
{
    internal interface IMainTabControlPageElement
    {
        /// <summary>
        /// このタブ要素におけるプレイリストを取得する。
        /// </summary>
        Playlist Playlist { get; }

        /// <summary>
        /// プレイリストにトラックを追加する操作が可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanAddTrackToPlaylist();

        /// <summary>
        /// ファイルを選択してプレイリストにトラックを追加する操作が可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanSelectAddTrack();

        /// <summary>
        /// フォルダを選択してプレイリストにトラックを追加する操作が可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanSelectAddFolder();

        /// <summary>
        /// プレイリストのエクスポートが可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanExportPlaylist();

        /// <summary>
        /// 検索ダイアログを表示して検索する操作が可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanShowFindDialog();

        /// <summary>
        /// 次を検索する操作が可能であるかどうかを取得する。
        /// </summary>
        /// <returns></returns>
        bool CanFindNext();

        /// <summary>
        /// 指定されたトラックをプレイリストに追加する。
        /// </summary>
        /// <param name="tracks"></param>
        void AddTrackToPlaylist(params AudioTrackBase[] tracks);

        /// <summary>
        /// ファイルを選択してプレイリストにトラックを追加する。
        /// </summary>
        void SelectAddTrack();

        /// <summary>
        /// フォルダを選択してプレイリストにトラックを追加する。
        /// </summary>
        void SelectAddFolder();

        /// <summary>
        /// このタブ要素におけるプレイリストをエクスポートする。
        /// </summary>
        void ExportPlaylist();

        /// <summary>
        /// 検索ダイアログを表示して検索する。
        /// </summary>
        void ShowFindDialog();
        
        /// <summary>
        /// 次を検索する。
        /// </summary>
        void FindNext();

        /// <summary>
        /// 表示を更新する。
        /// </summary>
        void UpdateView();
    }
}
