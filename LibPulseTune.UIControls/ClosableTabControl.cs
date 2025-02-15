using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LibPulseTune.UIControls
{
    public class ClosableTabControl : UserControl
    {
        #region タブページのコンテンツを描画する領域を実装したコントロール

        class TabPagePanel : Panel
        {
            public TabPagePanel()
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                TabRenderer.DrawTabPage(e.Graphics, e.ClipRectangle);
                base.OnPaint(e);
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                Refresh();
                base.OnSizeChanged(e);
            }
        }

        #endregion

        #region つまみを示すコントロールの実装

        internal class Thumb : UserControl
        {
            // 非公開定数
            private const int CLOSE_BUTTON_WIDTH = 15;
            private const int CLOSE_BUTTON_HEIGHT = 15;
            private const int CLOSE_BUTTON_LEFT_MARGIN = 1;
            private const int CLOSE_BUTTON_RIGHT_MARGIN = 3;

            // イベント
            public event EventHandler<TabPageEventArgs> CloseButtonClick;
            public event EventHandler<TabPageEventArgs> TabItemClick;

            // 非公開フィールド
            private readonly bool isClosable;
            private bool flagMouseEnter;

            // コンストラクタ
            public Thumb(ClosableTabPage tabPage)
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

                this.isClosable = tabPage.IsClosable;

                this.AutoSize = false;
                this.AutoScaleMode = AutoScaleMode.Dpi;
                this.TabPage = tabPage;
            }

            #region プロパティ

            /// <summary>
            /// テキスト
            /// </summary>
            public new string Text
            {
                set
                {
                    base.Text = value;

                    UpdateWidth();
                    Invalidate();
                }
                get
                {
                    return base.Text;
                }
            }

            /// <summary>
            /// 対応するタブページ
            /// </summary>
            public ClosableTabPage TabPage { private set; get; }

            /// <summary>
            /// 選択された状態として描画するかどうか
            /// </summary>
            public bool DrawAsSelected { set; get; }

            #endregion

            /// <summary>
            /// コントロールの幅を更新する。
            /// </summary>
            private void UpdateWidth()
            {
                using (var g = CreateGraphics())
                {
                    float width = TextRenderer.MeasureText(this.Text, this.Font).Width;

                    if (this.isClosable)
                    {
                        width += CLOSE_BUTTON_LEFT_MARGIN + CLOSE_BUTTON_WIDTH + CLOSE_BUTTON_RIGHT_MARGIN;
                    }

                    this.ClientSize = new Size((int)Math.Round(width, MidpointRounding.AwayFromZero), this.ClientSize.Height);
                }
            }

            /// <summary>
            /// 閉じるボタンの矩形を取得する。
            /// </summary>
            /// <returns></returns>
            public Rectangle GetCloseButtonRect()
            {
                return new Rectangle(
                    this.ClientRectangle.Right - CLOSE_BUTTON_WIDTH - CLOSE_BUTTON_RIGHT_MARGIN, 
                    this.Height / 2 - CLOSE_BUTTON_HEIGHT / 2,
                    CLOSE_BUTTON_WIDTH, 
                    CLOSE_BUTTON_HEIGHT);
            }

            /// <summary>
            /// 描画処理
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                // タブのつまみの状態を取得する。
                var tabItemState = TabItemState.Normal;
                if (this.DrawAsSelected)
                {
                    tabItemState = TabItemState.Selected;
                }
                else
                {
                    if (this.flagMouseEnter)
                    {
                        if (e.ClipRectangle.Contains(PointToClient(Cursor.Position)))
                        {
                            tabItemState = TabItemState.Hot;
                        }
                    }
                }

                // タブを描画する。
                TabRenderer.DrawTabItem(e.Graphics, e.ClipRectangle, tabItemState);
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, SystemColors.ControlText, TextFormatFlags.VerticalCenter);

                if (this.isClosable)
                {
                    // 閉じるボタンを取得する。
                    var closeButtonRect = GetCloseButtonRect();
                    VisualStyleElement closeButton = VisualStyleElement.Window.CloseButton.Normal;
                    if (this.flagMouseEnter)
                    {
                        if (closeButtonRect.Contains(PointToClient(Cursor.Position)))
                        {
                            closeButton = VisualStyleElement.Window.CloseButton.Hot;
                        }
                    }

                    // 閉じるボタンを描画する。
                    var closeButtonRenderer = new VisualStyleRenderer(closeButton);
                    closeButtonRenderer.DrawBackground(e.Graphics, closeButtonRect);
                }
            }

            /// <summary>
            /// マウスがクリックされた場合の処理
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseClick(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this.isClosable && GetCloseButtonRect().Contains(PointToClient(Cursor.Position)))
                    {
                        this.CloseButtonClick?.Invoke(this, new TabPageEventArgs(this.TabPage));
                    }
                    else
                    {
                        this.TabItemClick?.Invoke(this, new TabPageEventArgs(this.TabPage));
                    }
                }

                base.OnMouseClick(e);
            }

            /// <summary>
            /// マウスカーソルがコントロールの領域に入った場合の処理
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseEnter(EventArgs e)
            {
                this.flagMouseEnter = true;
                Invalidate();

                base.OnMouseEnter(e);
            }

            /// <summary>
            /// マウスカーソルがコントロールの領域内で移動した場合処理
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseMove(MouseEventArgs e)
            {
                Invalidate();

                base.OnMouseMove(e);
            }

            /// <summary>
            /// マウスカーソルがコントロールの領域から出た場合の処理
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseLeave(EventArgs e)
            {
                this.flagMouseEnter = false;
                Invalidate();

                base.OnMouseLeave(e);
            }
        }

        #endregion

        #region ヘッダ領域を示すコントロールの実装

        class HeaderPanel : ScrollableControl
        {
            // 非公開フィールド
            private readonly List<Thumb> thumbs;
            private readonly int horizontalScrollBarHeight;
            private ClosableTabPage selectedTabPage;
            private bool isHorizontalScrollBarVisible;

            // イベント
            public event EventHandler<TabPageEventArgs> TabClosed;
            public event EventHandler AnyTabItemClick;

            // コンストラクタ
            public HeaderPanel()
            {
                this.thumbs = new List<Thumb>();
                this.AutoSize = false;
                this.AutoScroll = true;
                this.HorizontalScroll.Visible = true;

                // 水平スクロールバーの高さを無理やり求める。
                // Win10では15くらいだが、OSやDPIの違いで異なる場合があるので、厳密に求めておく。
                using (var tmp = new HScrollBar())
                {
                    this.horizontalScrollBarHeight = tmp.Height;
                }
            }

            #region プロパティ

            /// <summary>
            /// 選択されたタブのコンテンツ
            /// </summary>
            public ClosableTabPage SelectedTabPage
            {
                set
                {
                    this.selectedTabPage = value;
                    InvalidateAllTabItems();
                }
                get
                {
                    return this.selectedTabPage;
                }
            }

            /// <summary>
            /// タブページ数
            /// </summary>
            public int TabCount
            {
                get
                {
                    return this.thumbs.Count;
                }
            }

            #endregion

            #region 公開メソッド

            /// <summary>
            /// タブページを追加する。
            /// </summary>
            /// <param name="tabPage"></param>
            public void AddTabPage(ClosableTabPage tabPage)
            {
                var tabItem = new Thumb(tabPage);
                tabItem.Text = tabPage.Text;
                tabItem.Dock = DockStyle.Left;
                tabItem.TabItemClick += OnTabItemClick;
                tabItem.CloseButtonClick += OnTabClosed;

                this.thumbs.Add(tabItem);

                this.Controls.Clear();
                for (int i = this.thumbs.Count - 1; i >= 0; i--)
                {
                    this.Controls.Add(this.thumbs[i]);
                }
            }

            /// <summary>
            /// 指定されたタブページを削除する。
            /// </summary>
            /// <param name="tabPage"></param>
            public void RemoveTabPage(ClosableTabPage tabPage)
            {
                int index = -1;

                for (int i = 0; i < this.thumbs.Count; i++)
                {
                    if (this.thumbs[i].TabPage == tabPage)
                    {
                        index = i;
                        break;
                    }
                }

                RemoveTabPageAt(index);
            }

            /// <summary>
            /// 指定されたインデックスのタブページを削除する。
            /// </summary>
            /// <param name="index"></param>
            public void RemoveTabPageAt(int index)
            {
                if (index == -1)
                {
                    return;
                }

                var tabItem = this.thumbs[index];
                var tabPage = tabItem.TabPage;

                if (tabPage != null)
                {
                    if (tabPage.Control != null)
                    {
                        tabPage.Control.Visible = false;
                    }

                    tabPage.Dispose();
                }

                this.Controls.Remove(tabItem);
                this.thumbs.RemoveAt(index);
            }

            /// <summary>
            /// 指定されたインデックスのタブページを取得する。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ClosableTabPage GetTabPage(int index)
            {
                return this.thumbs[index].TabPage;
            }

            #endregion

            /// <summary>
            /// 指定されたインデックスのつまみのテキストを設定する。
            /// </summary>
            /// <param name="index"></param>
            /// <param name="text"></param>
            public void SetText(int index, string text)
            {
                this.thumbs[index].Text = text;
                this.thumbs[index].Invalidate();
            }

            /// <summary>
            /// 指定されたインデックスのタブのつまみの矩形を取得する。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            protected Rectangle GetTabItemRect(int index)
            {
                return this.thumbs[index].ClientRectangle;
            }

            /// <summary>
            /// 指定されたインデックスのタブの閉じるボタンの矩形を取得する。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            protected Rectangle GetCloseButtonRect(int index)
            {
                return this.thumbs[index].GetCloseButtonRect();
            }

            /// <summary>
            /// すべてのタブアイテムを再描画する。
            /// </summary>
            private void InvalidateAllTabItems()
            {
                for (int i = 0; i < this.thumbs.Count; ++i)
                {
                    this.thumbs[i].DrawAsSelected = this.thumbs[i].TabPage == this.selectedTabPage;
                    this.thumbs[i].Invalidate();
                }
            }

            /// <summary>
            /// タブが選択された場合の処理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnTabItemClick(object sender, TabPageEventArgs e)
            {
                this.SelectedTabPage = e.TabPage;
                this.AnyTabItemClick?.Invoke(this, EventArgs.Empty);
            }

            /// <summary>
            /// タブの閉じるボタンがクリックされた場合の処理
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnTabClosed(object sender, TabPageEventArgs e)
            {
                RemoveTabPage(e.TabPage);

                this.TabClosed?.Invoke(sender, e);
            }

            protected override void OnScroll(ScrollEventArgs se)
            {
                base.OnScroll(se);

                // スクロールされた場合は全つまみを再描画する。
                foreach (var thumb in this.thumbs)
                {
                    thumb.Invalidate();
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                bool horizontalVisible = this.HorizontalScroll.Visible;

                // 水平スクロールバーの表示状態が変わった場合、高さを調整
                if (horizontalVisible != this.isHorizontalScrollBarVisible)
                {
                    // 必ずリサイズ前にこの処理を行うこと！
                    // リサイズ後だと何度もOnResizeが呼び出されることになる。
                    this.isHorizontalScrollBarVisible = horizontalVisible;
                    
                    if (horizontalVisible)
                    {
                        this.Height += this.horizontalScrollBarHeight;
                    }
                    else
                    {
                        this.Height -= this.horizontalScrollBarHeight;
                    }
                }
            }
        }

        #endregion

        // 非公開フィールド
        private readonly HeaderPanel TabHeaderSpace;
        private readonly TabPagePanel TabPageArea;
        private ClosableTabPage selectedTab;

        // コンストラクタ
        public ClosableTabControl()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.Padding = new Padding(0, 3, 0, 0);

            this.TabPageArea = new TabPagePanel();
            this.TabHeaderSpace = new HeaderPanel();
            this.TabPageArea.BackColor = SystemColors.Control;
            this.TabPageArea.Dock = DockStyle.Fill;
            this.TabPageArea.Location = new Point(0, 20);
            this.TabHeaderSpace.Dock = DockStyle.Top;
            this.TabHeaderSpace.Location = new Point(0, 0);
            this.TabHeaderSpace.SelectedTabPage = null;
            this.TabHeaderSpace.Height = 23;
            this.TabHeaderSpace.AnyTabItemClick += OnAnyTabItemClick;
            this.TabHeaderSpace.TabClosed += OnTabClosed;

            this.Controls.Add(this.TabPageArea);
            this.Controls.Add(this.TabHeaderSpace);
        }

        #region プロパティ

        /// <summary>
        /// 選択されているタブ
        /// </summary>
        public ClosableTabPage SelectedTab
        {
            set
            {
                OnSelectedTabPageChanging(value);
            }
            get
            {
                return this.selectedTab;
            }
        }

        /// <summary>
        /// 選択されているタブのインデックス
        /// </summary>
        public int SelectedIndex
        {
            set
            {
                if (value >= 0 && this.TabHeaderSpace.TabCount - 1 >= value)
                {
                    this.SelectedTab = this.TabHeaderSpace.GetTabPage(value);
                }
            }
            get
            {
                for (int i = 0; i < this.TabHeaderSpace.TabCount; i++)
                {
                    if (this.TabHeaderSpace.GetTabPage(i) == this.selectedTab)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// タブページ数
        /// </summary>
        public int TabCount
        {
            get
            {
                return this.TabHeaderSpace.TabCount;
            }
        }

        #endregion

        /// <summary>
        /// タブページを追加する。
        /// </summary>
        /// <param name="tabPage"></param>
        public void AddTabPage(ClosableTabPage tabPage)
        {
            tabPage.TextChanged += OnTabPageTextChanged;
            this.TabHeaderSpace.AddTabPage(tabPage);

            if (this.TabHeaderSpace.TabCount == 1)
            {
                this.SelectedIndex = 0;
            }

            this.TabHeaderSpace.Invalidate();
        }

        /// <summary>
        /// タブページを削除する。
        /// </summary>
        /// <param name="tabPage"></param>
        public void RemoveTabPage(ClosableTabPage tabPage)
        {
            tabPage.TextChanged -= OnTabPageTextChanged;
            this.TabHeaderSpace.RemoveTabPage(tabPage);
            this.TabPageArea.Controls.Clear();

            // 再描画
            this.TabHeaderSpace.Invalidate();
            this.TabPageArea.Invalidate();
        }

        /// <summary>
        /// 指定されたインデックスのタブページを削除する。
        /// </summary>
        /// <param name="index"></param>
        public void RemoveTabPageAt(int index)
        {
            this.TabHeaderSpace.RemoveTabPageAt(index);
        }

        /// <summary>
        /// 指定されたインデックスのタブページを追加する。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ClosableTabPage GetTabPage(int index)
        {
            return this.TabHeaderSpace.GetTabPage(index);
        }

        /// <summary>
        /// 指定されたタブページを表示する。
        /// </summary>
        /// <param name="tabPage"></param>
        protected void ShowTabPage(ClosableTabPage tabPage)
        {
            if (tabPage == null)
            {
                return;
            }

            // 変更前に選択されているタブページがあれば非表示化する。
            if (this.selectedTab != null && this.selectedTab.Control != null)
            {
                this.selectedTab.Control.Visible = false;
            }

            // 選択されているタブページを設定
            this.selectedTab = tabPage;
            this.TabPageArea.BackColor = this.selectedTab.BackColor;

            // タブページに設定された子コントロールを表示する。
            this.selectedTab.Control.Parent = this.TabPageArea;
            this.selectedTab.Control.Left = 2;
            this.selectedTab.Control.Top = 2;
            this.selectedTab.Control.Width = this.TabPageArea.Width - 6;
            this.selectedTab.Control.Height = this.TabPageArea.Height - 6;
            this.selectedTab.Control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.selectedTab.Control.Visible = true;
        }

        /// <summary>
        /// いずれかのタブのつまみがクリックされた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnAnyTabItemClick(object sender, EventArgs e)
        {
            ShowTabPage(this.TabHeaderSpace.SelectedTabPage);
        }

        /// <summary>
        /// 選択されたタブページが変更された場合の処理
        /// </summary>
        /// <param name="tabPage"></param>
        protected void OnSelectedTabPageChanging(ClosableTabPage tabPage)
        {
            ShowTabPage(tabPage);

            // タブページの選択状態をヘッダ部分にも反映
            this.TabHeaderSpace.SelectedTabPage = tabPage;
        }

        /// <summary>
        /// タブが閉じられた場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnTabClosed(object sender, TabPageEventArgs e)
        {
            // 閉じるボタンが押されたタブページを削除する。
            RemoveTabPage(e.TabPage);

            // 1つ以上のタブページが存在するなら、一番最後のタブページを選択する。
            if (this.TabCount > 0)
            {
                var page = GetTabPage(this.TabCount - 1);

                this.SelectedTab = page;
            }
        }

        /// <summary>
        /// タブページのテキストが変更された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnTabPageTextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                this.TabHeaderSpace.SetText(i, GetTabPage(i).Text);
            }

            this.TabHeaderSpace.Invalidate();
            this.TabPageArea.Invalidate();
        }
    }
}
