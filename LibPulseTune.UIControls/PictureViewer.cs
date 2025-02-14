using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public class PictureViewer : PictureBox
    {
        // 非公開フィールド
        private Image picture;

        // コンストラクタ
        public PictureViewer()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// 描画するイメージ
        /// </summary>
        public Image Picture
        {
            set
            {
                this.picture = value;

                // 再描画
                Invalidate();
            }
            get
            {
                return this.picture;
            }
        }

        /// <summary>
        /// 指定された画像を、アスペクト比を維持しつつ指定されたサイズにリサイズする。
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Image ResizeKeepAspect(Image image, float width, float height)
        {
            if (image == null)
            {
                return null;
            }

            // 変更倍率を取得する
            float scale = Math.Min(width / image.Width, height / image.Height);
            int widthToScale = (int)(image.Width * scale);
            int heightToScale = (int)(image.Height * scale);

            Bitmap bitmap = new Bitmap(widthToScale, widthToScale);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 背景色を塗る
                SolidBrush solidBrush = new SolidBrush(Color.Black);
                graphics.FillRectangle(solidBrush, new RectangleF(0, 0, width, height));

                // サイズ変更した画像に、左上を起点に変更する画像を描画する
                graphics.DrawImage(image, 0, 0, widthToScale, heightToScale);

                return bitmap;
            }
        }

        /// <summary>
        /// コントロールの描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // コントロールの領域を背景色でクリア
            e.Graphics.Clear(this.BackColor);

            if (this.picture == null)
            {
                return;
            }

            Image img = this.picture;

            // 描画する画像がコントロールのサイズより大きければ、アスペクトを維持しつつリサイズ
            if (this.picture.Width > this.DisplayRectangle.Width || this.picture.Height > this.DisplayRectangle.Height)
            {
                img = ResizeKeepAspect(this.picture, this.Width, this.Height);
            }

            // 画像を描画する
            int x = (this.Width / 2) - (img.Width / 2);
            int y = (this.Height / 2) - (img.Height / 2);
            e.Graphics.DrawImage(img, x, y, img.Width, img.Height);

            // コントロールの境界線を描画する
            var color = Color.FromArgb(130, 135, 144);
            using (var pen = new Pen(color))
            {
                e.Graphics.DrawRectangle(pen, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
            }
        }
    }
}
