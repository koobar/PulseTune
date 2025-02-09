using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    public class ExplorerLikeListView : OptimizedListView
    {
        // 非公開定数
        private const int SPACING = 3;

        // 色とブラシの定義
        private static readonly Color SelectedItemBackColor = Color.FromArgb(205, 232, 255);
        private static readonly Color SelectedItemBorderColor = Color.FromArgb(153, 209, 255);
        private static readonly Color HotItemBackColor = Color.FromArgb(229, 243, 255);
        private static readonly Brush SelectedItemBrush = new SolidBrush(SelectedItemBackColor);
        private static readonly Pen SelectedItemBorderPen = new Pen(SelectedItemBorderColor);
        private static readonly Brush HotItemBrush = new SolidBrush(HotItemBackColor);

        // 非公開フィールド
        private Point mousePoint;

        // コンストラクタ
        public ExplorerLikeListView()
        {
            this.View = View.Details;
            this.OwnerDraw = true;
        }

        /// <summary>
        /// コントロール上でマウスカーソルの移動が発生した場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mousePoint = e.Location;

            // 再描画すべきアイテムを取得
            var items = new List<ExplorerLikeListViewItem>();
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is ExplorerLikeListViewItem)
                {
                    ExplorerLikeListViewItem item = (ExplorerLikeListViewItem)this.Items[i];
                    bool contains = item.GetBounds(ItemBoundsPortion.ItemOnly).Contains(this.mousePoint);

                    if (contains != item.MouseHover)
                    {
                        items.Add(item);
                    }

                    item.MouseHover = contains;
                }
            }

            // 再描画対象のアイテムをすべて再描画
            foreach (var item in items)
            {
                RedrawItems(item.Index, item.Index, true);
            }

            // 再描画対象をクリア
            items.Clear();
            items.TrimExcess();
        }

        /// <summary>
        /// コントロールの領域からマウスカーソルが脱出した場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.mousePoint = Point.Empty;

            Invalidate();
        }

        /// <summary>
        /// マウスのホイールが回された場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Invalidate();
        }

        /// <summary>
        /// カラムヘッダの描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        /// <summary>
        /// カラムの幅が変更される場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            base.OnColumnWidthChanging(e);
        }

        /// <summary>
        /// アイテムの描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }

            var item = e.Item;

            if (item.Selected)
            {
                e.Graphics.DrawRectangle(SelectedItemBorderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                e.Graphics.FillRectangle(SelectedItemBrush, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            }
            else if (this.mousePoint != Point.Empty && item.GetBounds(ItemBoundsPortion.ItemOnly).Contains(this.mousePoint))
            {
                e.Graphics.FillRectangle(HotItemBrush, e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            int iconX = 0, iconY = 0, iconWidth = 0, iconHeight = 0;
            int textX = 0, textY = 0, textWidth = 0, textHeight = 0;
            Image icon = null;
            string text = string.Empty;

            if (item is ExplorerLikeListViewItem explorerLikeListViewItem)
            {
                if (explorerLikeListViewItem.Icon == null)
                {
                    textX = SPACING;
                    textY = e.Bounds.Y;
                    textWidth = e.Bounds.Width;
                    textHeight = e.Bounds.Height;
                    text = explorerLikeListViewItem.Text;
                }
                else
                {
                    iconX = e.Bounds.X + SPACING;
                    iconY = e.Bounds.Y + SPACING / 2;
                    iconWidth = explorerLikeListViewItem.Icon.Width;
                    iconHeight = explorerLikeListViewItem.Icon.Height;
                    textX = iconX + iconWidth + SPACING;
                    textY = e.Bounds.Y;
                    textWidth = e.Bounds.Width;
                    textHeight = e.Bounds.Height;

                    icon = explorerLikeListViewItem.Icon;
                    text = explorerLikeListViewItem.Text;
                }
            }
            else
            {
                textX = SPACING;
                textY = e.Bounds.Y;
                textWidth = e.Bounds.Width;
                textHeight = e.Bounds.Height;
                text = item.Text;
            }

            if (icon != null)
            {
                // アイコンを描画
                e.Graphics.DrawImage(icon, iconX, iconY, iconWidth, iconHeight);
            }

            // 文字列を描画
            using (var sf = new StringFormat())
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;

                e.Graphics.DrawString(text, this.Font, Brushes.Black, new Rectangle(textX, textY, textWidth, textHeight), sf);
            }
        }
    }
}
