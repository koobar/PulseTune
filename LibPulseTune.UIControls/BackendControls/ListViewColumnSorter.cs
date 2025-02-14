using System.Collections;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    internal class ListViewColumnSorter : IComparer
    {
        // コンストラクタ
        public ListViewColumnSorter()
        {
            this.SortColumn = 0;
            this.Order = SortOrder.Ascending;
        }

        /// <summary>
        /// ソートする列のインデックス
        /// </summary>
        public int SortColumn { get; set; }

        /// <summary>
        /// ソート順
        /// </summary>
        public SortOrder Order { get; set; }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            ListViewItem item1 = (ListViewItem)x;
            ListViewItem item2 = (ListViewItem)y;

            string text1 = item1.SubItems[this.SortColumn].Text;
            string text2 = item2.SubItems[this.SortColumn].Text;

            // 文字列として比較
            int result = string.Compare(text1, text2);

            // 降順の場合は結果を反転
            if (this.Order == SortOrder.Descending)
            {
                result = -result;
            }

            return result;
        }
    }
}
