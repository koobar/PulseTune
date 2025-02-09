using System.Windows.Forms;

namespace PulseTune.Controls.BackendControls
{
    public class OptimizedListView : ListView
    {
        public OptimizedListView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }
    }
}
