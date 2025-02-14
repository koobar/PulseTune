using System.Drawing;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
{
    public class ExplorerLikeListViewItem : ListViewItem
    {
        /// <summary>
        /// アイコン
        /// </summary>
        public Bitmap Icon { set; get; }

        /// <summary>
        /// アイテムの領域にマウスカーソルが重なっているかどうかを示すフラグ
        /// </summary>
        public bool MouseHover { set; get; }
    }
}
