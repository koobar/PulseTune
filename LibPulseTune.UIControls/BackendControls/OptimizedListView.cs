using System.Windows.Forms;

namespace LibPulseTune.UIControls.BackendControls
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
