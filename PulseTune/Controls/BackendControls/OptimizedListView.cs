using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    internal class OptimizedListView : ListView
    {
        // コンストラクタ
        public OptimizedListView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
