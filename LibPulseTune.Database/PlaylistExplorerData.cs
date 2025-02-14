using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibPulseTune.Database
{
    public static class PlaylistExplorerData
    {
        // 非公開フィールド
        private static readonly List<string> favoriteLocations = new List<string>();
        private static readonly List<string> recentLocations = new List<string>();
        private static readonly string favoriteLocationsDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PulseTune\\favorites.loc";
        private static readonly string recentLocationsDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PulseTune\\recent.loc";

        // 公開イベント
        public static event EventHandler FavoriteLocationsChanged;
        public static event EventHandler RecentLocationsChanged;

        /// <summary>
        /// 指定されたリストに、指定されたファイルから場所のリストを読み込む。
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="path"></param>
        private static void LoadLocation(List<string> locations, string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            using (var reader = new StreamReader(path))
            {
                while (reader.Peek() > -1)
                {
                    locations.Add(reader.ReadLine());
                }

                reader.Close();
            }
        }

        /// <summary>
        /// 指定されたリストに含まれる場所のリストを、指定されたファイルに保存する。
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="path"></param>
        private static void SaveLocation(List<string> locations, string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                foreach (string location in locations)
                {
                    writer.WriteLine(location);
                }

                writer.Close();
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public static void Init()
        {
            favoriteLocations.Clear();
            recentLocations.Clear();
        }

        /// <summary>
        /// 指定された場所をお気に入りに追加する。
        /// </summary>
        /// <param name="location"></param>
        public static void AddToFavorite(string location)
        {
            if (favoriteLocations.Contains(location))
            {
                return;
            }

            favoriteLocations.Add(location);
            FavoriteLocationsChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// 指定された場所を最近開いた場所に追加する。
        /// </summary>
        /// <param name="location"></param>
        public static void AddToRecent(string location)
        {
            if (recentLocations.Contains(location))
            {
                return;
            }

            recentLocations.Add(location);
            RecentLocationsChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// 指定された場所をお気に入りから削除する。
        /// </summary>
        /// <param name="location"></param>
        public static void RemoveFromFavorite(string location)
        {
            if (favoriteLocations.Contains(location))
            {
                favoriteLocations.Remove(location);
                FavoriteLocationsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 指定された場所を最近開いた場所から削除する。
        /// </summary>
        /// <param name="location"></param>
        public static void RemoveFromRecent(string location)
        {
            if (recentLocations.Contains(location))
            {
                recentLocations.Remove(location);
                RecentLocationsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 設定を読み込む。
        /// </summary>
        public static void Load()
        {
            Init();

            LoadLocation(favoriteLocations, favoriteLocationsDataPath);
            LoadLocation(recentLocations, recentLocationsDataPath);
        }

        /// <summary>
        /// 設定を保存する。
        /// </summary>
        public static void Save()
        {
            SaveLocation(favoriteLocations, favoriteLocationsDataPath);
            SaveLocation(recentLocations, recentLocationsDataPath);
        }

        /// <summary>
        /// お気に入りに追加された場所の個数を取得する。
        /// </summary>
        /// <returns></returns>
        public static int GetFavoriteLocationsCount()
        {
            return favoriteLocations.Count;
        }

        /// <summary>
        /// 最近開いた場所に追加された場所の個数を追加する。
        /// </summary>
        /// <returns></returns>
        public static int GetRecentLocationsCount()
        {
            return recentLocations.Count;
        }

        /// <summary>
        /// お気に入りに追加された場所のうち、指定されたインデックスに対応する場所を取得する。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetFavoriteLocation(int index)
        {
            return favoriteLocations[index];
        }

        /// <summary>
        /// 最近開いた場所に追加された場所のうち、指定されたインデックスに対応する場所を取得する。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetRecentLocation(int index)
        {
            return recentLocations[index];
        }
    }
}
