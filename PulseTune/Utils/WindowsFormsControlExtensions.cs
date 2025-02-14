using System.ComponentModel;
using System.Windows.Forms;

namespace PulseTune.Utils
{
    public static class WindowsFormsControlExtensions
    {
        /// <summary>
        /// こメニューアイテムの親が持つメニューアイテムのうち、こメニューだけを選択状態にする。
        /// </summary>
        /// <param name="menuItem"></param>
        public static void CheckOnlyThisMenuItem(this MenuItem menuItem)
        {
            var parent = menuItem.Parent;

            for (int i = 0; i < parent.MenuItems.Count; i++)
            {
                var item = parent.MenuItems[i];
                item.Checked = item == menuItem;
            }
        }

        /// <summary>
        /// フォームがVisual Studioのデザイナで表示されているかどうかを判定する。
        /// </summary>
        /// <returns></returns>
        public static bool IsDesignMode(this Control ctrl)
        {
            // 自身のデザインモードを取得
            bool isDesignMode = false;// form.DesignMode;

            // 親コントロールを再帰取得
            Control parent = ctrl.Parent;
            while (parent != null)
            {
                ISite site = parent.Site;
                if (site != null)
                {
                    isDesignMode |= site.DesignMode;
                }

                parent = parent.Parent;
            }

            return isDesignMode;
        }
    }
}
