namespace LibPulseTune.Plugin.Sdk
{
    /// <summary>
    /// プラグイン側からPulseTune本体に実装された機能を呼び出すための機能「システムコール」を定義したクラスです。
    /// </summary>
    public static class SystemCalls
    {
        public static class Application
        {
            /// <summary>
            /// PulseTuneのバージョンを取得するシステムコールです。
            /// </summary>
            public static SystemCall GetPulseTuneVersion { set; get; }

            /// <summary>
            /// カスタムオプションに指定されたキーの値が存在するかどうか判定します。
            /// </summary>
            public static SystemCall ContainsCustomOptionValue { set; get; }

            public static SystemCall SetCustomOptionValue { set; get; }

            public static SystemCall GetCustomOptionValue { set; get; }
        }

        public static class AudioSource
        {
            /// <summary>
            /// 指定されたファイルからオーディオソースを開くシステムコールです。<br/>
            /// このシステムコールを用いてオーディオソースを開くと、別のプラグインで実装された
            /// デコーダを使用することもできます。
            /// </summary>
            public static SystemCall CreateAudioSource { set; get; }
        }

        public static class AudioTrack
        {
            /// <summary>
            /// 指定されたファイルを開いてAudioTrackBaseを継承する型の値として返すシステムコールです
            /// </summary>
            public static SystemCall CreateFile { set; get; }

            /// <summary>
            /// カスタムコンストラクタを使用してオーディオトラックを開き、AudioTrackBaseを継承する型の値として返すシステムコールです
            /// </summary>
            public static SystemCall CreateUseCustomConstructor { set; get; }

            /// <summary>
            /// 指定されたパスのファイルが、再生をサポートされているフォーマットであるかどうかを判定するシステムコールです
            /// </summary>
            public static SystemCall IsPlaybackSupportedFileFormat { set; get; }
        }

        /// <summary>
        /// メインウィンドウを操作するためのシステムコールを定義したクラスです。
        /// </summary>
        public static class MainWindow
        {
            /// <summary>
            /// メインウィンドウのインスタンスを取得するシステムコールです
            /// </summary>
            public static SystemCall GetMainWindow { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、ファイルメニューを取得するシステムコールです
            /// </summary>
            public static SystemCall GetFileMenu { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、表示メニューを取得するシステムコールです
            /// </summary>
            public static SystemCall GetViewMenu { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、検索メニューを取得するシステムコールです
            /// </summary>
            public static SystemCall GetFindMenu { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、再生メニューを取得するシステムコールです
            /// </summary>
            public static SystemCall GetPlaybackMenu { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、設定メニューを取得するシステムコールです。
            /// </summary>
            public static SystemCall GetOptionMenu { set; get; }

            /// <summary>
            /// メインメニュー項目のうち、ヘルプメニューを取得するシステムコールです
            /// </summary>
            public static SystemCall GetHelpMenu { set; get; }
            
            /// <summary>
            /// メインタブコントロールにタブページを追加するシステムコールです。
            /// </summary>
            public static SystemCall AddMainTabPage { set; get; }

            /// <summary>
            /// メインタブコントロールの選択されたページのインデックスを設定するシステムコールです。
            /// </summary>
            public static SystemCall SetSelectedMainTabIndex { set; get; }

            /// <summary>
            /// メインタブコントロールの選択されたページのインデックスを取得するシステムコールです。
            /// </summary>
            public static SystemCall GetSelectedMainTabIndex { set; get; }

            /// <summary>
            /// メインタブコントロールの選択されたページのインスタンスを設定するシステムコールです。
            /// </summary>
            public static SystemCall SetSelectedMainTabPage { set; get; }

            /// <summary>
            /// メインタブコントロールの選択されたページのインスタンスを取得するシステムコールです。
            /// </summary>
            public static SystemCall GetSelectedMainTabPage { set; get; }

            /// <summary>
            /// 選択されているプレイリストにトラックを追加するシステムコールです。
            /// </summary>
            public static SystemCall AddTracksToCurrentPlaylist { set; get; }
        }

        /// <summary>
        /// 閉じるボタン付きタブコントロールと、それに表示するタブページを操作するためのシステムコールを定義したクラスです。
        /// </summary>
        public static class ClosableTabControl
        {
            /// <summary>
            /// 閉じるボタン付きタブコントロールに表示するための、新しいタブページのインスタンスを作成するシステムコールです。
            /// </summary>
            public static SystemCall CreateNewTabPage { set; get; }

            /// <summary>
            /// 閉じるボタン付きタブコントロールに表示するための、新しいプレイリストタブページのインスタンスを作成するシステムコールです。
            /// </summary>
            public static SystemCall CreateNewPlaylistTabPage { set; get; }

            /// <summary>
            /// 閉じるボタン付きタブコントロールに表示するタブページに表示するコンテンツを設定するシステムコールです。
            /// </summary>
            public static SystemCall SetTabPageContent { set; get; }

            /// <summary>
            /// 閉じるボタン付きタブコントロールに表示するタブページに表示されているコンテンツを取得するシステムコールです。
            /// </summary>
            public static SystemCall GetTabPageContent { set; get; }
        }
    }
}
