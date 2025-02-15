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
        private static readonly string favoriteLocationsDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\PulseTune\\favorites.loc";

        // 公開イベント
        public static event EventHandler FavoriteLocationsChanged;

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
        /// 指定された場所がお気に入りに登録されているかどうかを判定する。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool ContainsFavorite(string location)
        {
            return favoriteLocations.Contains(location);
        }

        /// <summary>
        /// 設定を読み込む。
        /// </summary>
        public static void Load()
        {
            Init();

            LoadLocation(favoriteLocations, favoriteLocationsDataPath);
        }

        /// <summary>
        /// 設定を保存する。
        /// </summary>
        public static void Save()
        {
            SaveLocation(favoriteLocations, favoriteLocationsDataPath);
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
        /// お気に入りに追加された場所のうち、指定されたインデックスに対応する場所を取得する。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetFavoriteLocation(int index)
        {
            return favoriteLocations[index];
        }
    }
}
