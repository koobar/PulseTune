using LibPulseTune.Codecs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace LibPulseTune.UIControls.Utils
{
    public static class WindowsFormsControlExtensions
    {
        /// <summary>
        /// 指定された拡張子のフィルタ文字列を生成する。
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private static string MakeFilterExtensionString(IList<string> extensions)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < extensions.Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(";");
                }

                sb.Append("*");
                sb.Append(extensions[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 再生がサポートされているファイルをファイル選択ダイアログで選択するためのフィルタ文字列を取得する。
        /// </summary>
        /// <returns></returns>
        public static string GetSupportedPlaybackFileFormatsDialogFilterString()
        {
            var sb = new StringBuilder();
            var allTypes = MakeFilterExtensionString(AudioSourceProvider.GetAllRegisteredFormatExtensions());

            sb.Append("対応するすべての形式(");
            sb.Append(allTypes);
            sb.Append(")|");
            sb.Append(allTypes);

            foreach (var formatName in AudioSourceProvider.GetRegisteredFormatNames())
            {
                var filter = MakeFilterExtensionString(AudioSourceProvider.GetRegisteredExtensions(formatName));

                sb.Append("|");
                sb.Append(formatName);
                sb.Append("(");
                sb.Append(filter);
                sb.Append(")|");
                sb.Append(filter);
            }

            sb.Append("|");
            sb.Append("すべてのファイル(*.*)|*.*");

            return sb.ToString();
        }

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
