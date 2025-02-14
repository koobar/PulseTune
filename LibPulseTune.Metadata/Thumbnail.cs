using System.Drawing;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using static LibPulseTune.Metadata.Utils.AsyncUtils;

namespace LibPulseTune.Metadata
{
    public class Thumbnail
    {
        // 非公開フィールド
        private readonly StorageFile file;

        // コンストラクタ
        public Thumbnail(StorageFile file)
        {
            this.file = file;
        }

        /// <summary>
        /// サムネイルをImageに変換する。
        /// </summary>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        private static Image ConvertThumbnailToImage(StorageItemThumbnail thumbnail)
        {
            if (thumbnail == null)
            {
                return null;
            }

            // IRandomAccessStreamからバイト配列を読み込む
            byte[] buffer = new byte[thumbnail.Size];
            using (var reader = new DataReader(thumbnail.GetInputStreamAt(0)))
            {
                var op = reader.LoadAsync((uint)thumbnail.Size);
                _ = op.GetResults();                                    // reader.LoadAsyncの待機

                reader.ReadBytes(buffer);
            }

            // バイト配列をMemoryStreamに変換し、Bitmapを生成
            using (var stream = new MemoryStream(buffer))
            {
                return new Bitmap(stream);
            }
        }

        public Image AsImage()
        {
            var musicView = CallAsyncMethod<StorageItemThumbnail, ThumbnailMode, uint, ThumbnailOptions>(
                this.file.GetThumbnailAsync,
                ThumbnailMode.MusicView,
                1600,
                ThumbnailOptions.UseCurrentScale);

            var picturesView = CallAsyncMethod<StorageItemThumbnail, ThumbnailMode, uint, ThumbnailOptions>(
                this.file.GetThumbnailAsync,
                ThumbnailMode.PicturesView,
                1600,
                ThumbnailOptions.UseCurrentScale);

            /* 取得されたサムネイルがPulseTuneのアイコンと同様であれば、
               タグに画像が埋め込まれていないものとみなす。
               そのために、MusicViewモードとPicturesViewモードでサムネイルを
               取得し、両者のサイズと種類、およびデータのサイズが一致すれば
               PulseTuneのアイコンとみなす。
               なお、オーディオファイルに対してPicturesViewモードで
               サムネイルの取得を行うと、通常、関連付けられたアプリケーションの
               アイコンが取得される。また、タグに画像が埋め込まれていない
               オーディオファイルに対してMusicViewモードでサムネイルの取得を
               行うと、通常、関連付けられたアプリケーションのアイコンが取得される。

               以上の挙動から、MusicViewモードとPicturesViewモードで取得した
               画像が異なる場合、MusicViewモードで取得したサムネイルが
               タグとして設定されたカバー画像などのデータであると推定する。*/
            if (musicView.OriginalWidth == picturesView.OriginalWidth &&
                musicView.OriginalHeight == picturesView.OriginalHeight &&
                musicView.ContentType == picturesView.ContentType &&
                musicView.Size == picturesView.Size)
            {
                // サムネイルを破棄
                musicView.Dispose();
                picturesView.Dispose();

                return null;
            }

            // PicturesViewモードのサムネイルを破棄
            picturesView.Dispose();

            return ConvertThumbnailToImage(musicView);
        }
    }
}
