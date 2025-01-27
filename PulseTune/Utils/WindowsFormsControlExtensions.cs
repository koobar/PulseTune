using System.ComponentModel;
using System.Windows.Forms;

namespace PulseTune.Utils
{
    public static class WindowsFormsControlExtensions
    {
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
