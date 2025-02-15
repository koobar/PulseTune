using LibPulseTune.UIControls.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
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
            this.FullRowSelect = true;
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

            if (this.GetVScrollBarVisible())
            {
                Invalidate();
            }
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
        /// サブアイテムの描画処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            base.OnDrawSubItem(e);

            if (e.Item == null)
            {
                return;
            }

            var item = e.Item;
            int iconX = 0, iconY = 0, iconWidth = 0, iconHeight = 0;
            int textX = 0, textY = 0, textWidth = 0, textHeight = 0;
            Image icon = null;
            var text = string.Empty;
            var textAreaComputed = false;
            var hasIcon = false;

            if (e.ColumnIndex == 0)
            {
                if (item is ExplorerLikeListViewItem explorerLikeListViewItem)
                {
                    if (explorerLikeListViewItem.Icon != null)
                    {
                        iconX = e.SubItem.Bounds.X + SPACING;
                        iconY = e.SubItem.Bounds.Y + SPACING / 2;
                        iconWidth = explorerLikeListViewItem.Icon.Width;
                        iconHeight = explorerLikeListViewItem.Icon.Height;
                        textX = iconX + iconWidth + SPACING;
                        textY = e.SubItem.Bounds.Y;
                        textWidth = e.SubItem.Bounds.Width;
                        textHeight = e.SubItem.Bounds.Height;

                        if (e.ColumnIndex == 0)
                        {
                            icon = explorerLikeListViewItem.Icon;
                        }

                        text = e.SubItem.Text;
                        textAreaComputed = true;
                        hasIcon = true;
                    }
                }
            }

            if (!textAreaComputed)
            {
                textX = e.SubItem.Bounds.X + SPACING;
                textY = e.SubItem.Bounds.Y;
                textWidth = e.SubItem.Bounds.Width;
                textHeight = e.SubItem.Bounds.Height;
                text = e.SubItem.Text;
                textAreaComputed = true;
            }

            if (hasIcon)
            {
                // アイコンを描画
                e.Graphics.DrawImage(icon, iconX, iconY, iconWidth, iconHeight);
            }

            if (textAreaComputed)
            {
                // 文字列を描画
                TextRenderer.DrawText(e.Graphics, text, this.Font, new Rectangle(textX, textY, textWidth, textHeight), Color.Black, Color.Transparent, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
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
        }
    }
}
