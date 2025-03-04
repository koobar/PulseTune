using LibPulseTune.UIControls.Utils;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static LibPulseTune.UIControls.WinApi;

namespace LibPulseTune.UIControls.BackendControls
{
    public class ExplorerLikeListView : OptimizedListView
    {
        // 非公開定数
        protected const int SPACING = 2;
        private const int GWL_STYLE = -16;
        private const int LVS_OWNERDRAWFIXED = 0x0400;
        private const int WM_MEASUREITEM = 0x002C + 0x2000;

        // 色とブラシの定義
        private static readonly Color SelectedItemBackColor = Color.FromArgb(205, 232, 255);
        private static readonly Color SelectedItemBorderColor = Color.FromArgb(153, 209, 255);
        private static readonly Color HotItemBackColor = Color.FromArgb(229, 243, 255);
        private static readonly Brush SelectedItemBrush = new SolidBrush(SelectedItemBackColor);
        private static readonly Pen SelectedItemBorderPen = new Pen(SelectedItemBorderColor);
        private static readonly Brush HotItemBrush = new SolidBrush(HotItemBackColor);
        private int rowHeight;

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
        /// アイテムの高さ
        /// </summary>
        public int ItemHeight
        {
            set
            {
                this.rowHeight = value;

                var size = this.Size;
                var style = GetWindowLong(this.Handle, GWL_STYLE);
                style |= LVS_OWNERDRAWFIXED;
                SetWindowLong(this.Handle, GWL_STYLE, style);

                this.Size = new Size(size.Width, size.Height + 1);
                style ^= LVS_OWNERDRAWFIXED;
                SetWindowLong(this.Handle, GWL_STYLE, style);

                this.Size = size;
            }
            get
            {
                return this.rowHeight;
            }
        }

        /// <summary>
        /// コントロール上でマウスカーソルの移動が発生した場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.mousePoint = e.Location;

            // 再描画すべきアイテムのみを検出して再描画
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is ExplorerLikeListViewItem item)
                {
                    bool contains = item.GetBounds(ItemBoundsPortion.ItemOnly).Contains(this.mousePoint);

                    if (contains != item.MouseHover)
                    {
                        RedrawItems(item.Index, item.Index, true);
                    }

                    item.MouseHover = contains;
                }
            }
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
            float textX = 0, textY = 0, textWidth = 0, textHeight = 0;
            Icon icon = null;
            var text = string.Empty;
            var textAreaComputed = false;
            var hasIcon = false;

            if (e.ColumnIndex == 0)
            {
                if (item is ExplorerLikeListViewItem explorerLikeListViewItem)
                {
                    if (explorerLikeListViewItem.Icon != null)
                    {
                        if (e.ColumnIndex == 0)
                        {
                            icon = explorerLikeListViewItem.Icon;
                            hasIcon = true;
                        }

                        textX = (SPACING + icon.Width) + SPACING;
                        textY = e.SubItem.Bounds.Y;
                        textWidth = e.SubItem.Bounds.Width;
                        textHeight = e.SubItem.Bounds.Height;
                        text = e.SubItem.Text;
                        textAreaComputed = true;
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
                e.Graphics.DrawIcon(icon, new Rectangle(SPACING, (e.Bounds.Y + (e.Bounds.Height - icon.Height) / 2), icon.Width, icon.Height));
            }

            if (textAreaComputed)
            {
                // 文字列を描画
                TextRenderer.DrawText(
                    e.Graphics, 
                    text, 
                    this.Font, 
                    new Rectangle((int)textX, (int)textY, (int)textWidth, (int)textHeight), 
                    Color.Black, 
                    Color.Transparent, 
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
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

        /// <summary>
        /// ウィンドウプロシージャ
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_MEASUREITEM == m.Msg)
            {
                var measure = Marshal.PtrToStructure<MEASUREITEMSTRUCT>(m.LParam);
                measure.itemHeight = (int)(this.rowHeight * Math.Max(1.0f, this.DeviceDpi / 100.0f));
                Marshal.StructureToPtr(measure, m.LParam, false);

                m.Result = (IntPtr)1;
            }
        }
    }
}
