using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public partial class LevelMeterPanel : UserControl
    {
        public LevelMeterPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 左チャンネルのデシベル
        /// </summary>
        public float LeftMeterDecibels
        {
            set
            {
                this.LeftVolumeMeter.Decibels = value;
            }
            get
            {
                return this.LeftVolumeMeter.Decibels;
            }
        }

        /// <summary>
        /// 右チャンネルのデシベル
        /// </summary>
        public float RightMeterDecibels
        {
            set
            {
                this.RightVolumeMeter.Decibels = value;
            }
            get
            {
                return this.RightVolumeMeter.Decibels;
            }
        }

        /// <summary>
        /// メーターをリセットする。
        /// </summary>
        public void Reset()
        {
            this.LeftVolumeMeter.Reset();
            this.RightVolumeMeter.Reset();
        }
    }
}
