using LibPulseTune.Options;
using System.Windows.Forms;

namespace LibPulseTune.UIControls
{
    public partial class WaveformPanel : UserControl
    {
        public WaveformPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 波形の描画精度
        /// </summary>
        public WaveformRendererRenderingPrecision RenderingPrecision
        {
            set
            {
                this.waveformRenderer1.RenderingPrecision = value;
            }
            get
            {
                return this.waveformRenderer1.RenderingPrecision;
            }
        }

        /// <summary>
        /// ステレオ音声の表示モード
        /// </summary>
        public WaveformRendererStereoViewMode StereoViewMode
        {
            set
            {
                this.waveformRenderer1.StereoViewMode = value;
            }
            get
            {
                return this.waveformRenderer1.StereoViewMode;
            }
        }

        /// <summary>
        /// 波形をアンチエイリアスして描画するかどうか
        /// </summary>
        public bool EnableWaveformAntiAlias
        {
            set
            {
                this.waveformRenderer1.EnableWaveformAntiAlias = value;
            }
            get
            {
                return this.waveformRenderer1.EnableWaveformAntiAlias;
            }
        }

        /// <summary>
        /// 指定された波形を描画する。
        /// </summary>
        /// <param name="waveform"></param>
        public void PaintWaveform(float[] waveform)
        {
            this.waveformRenderer1.PaintWaveform(waveform);
        }

        /// <summary>
        /// 指定された2チャンネル分の波形を描画する。
        /// </summary>
        /// <param name="lch"></param>
        /// <param name="rch"></param>
        public void PaintWaveform(float[] lch, float[] rch)
        {
            this.waveformRenderer1.PaintWaveform(lch, rch);
        }
    }
}
