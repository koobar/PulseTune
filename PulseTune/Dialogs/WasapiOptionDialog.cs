using LibPulseTune.Wasapi;
using PulseTune.Utils;
using System;
using System.Windows.Forms;

namespace PulseTune.Dialogs
{
    public partial class WasapiOptionDialog : Form
    {
        // コンストラクタ
        public WasapiOptionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームが読み込まれた場合の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.IsDesignMode())
            {
                return;
            }

            if (OptionManager.EnableMMCSS)
            {
                switch (OptionManager.MmThreadCharacteristics)
                {
                    case MmThreadCharacteristics.Audio:
                        this.AvailableMMCSSThreadModesComboBox.SelectedIndex = this.AvailableMMCSSThreadModesComboBox.Items.IndexOf("Audio");
                        break;
                    case MmThreadCharacteristics.Playback:
                        this.AvailableMMCSSThreadModesComboBox.SelectedIndex = this.AvailableMMCSSThreadModesComboBox.Items.IndexOf("PlayBack");
                        break;
                    case MmThreadCharacteristics.ProAudio:
                        this.AvailableMMCSSThreadModesComboBox.SelectedIndex = this.AvailableMMCSSThreadModesComboBox.Items.IndexOf("Pro Audio");
                        break;
                }
            }
            else
            {
                this.AvailableMMCSSThreadModesComboBox.SelectedIndex = this.AvailableMMCSSThreadModesComboBox.Items.IndexOf("無効");
            }

            this.MMCSSThreadPriorityTrackBar.Value = (int)OptionManager.PlaybackThreadPriority;
        }

        private void MMCSSThreadPriorityTrackBar_ValueChanged(object sender, EventArgs e)
        {
            this.MMCSSThreadPriorityLabel.Text = ((AvThreadPriority)this.MMCSSThreadPriorityTrackBar.Value).ToString();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            OptionManager.EnableMMCSS = this.AvailableMMCSSThreadModesComboBox.Text != "無効";
            OptionManager.PlaybackThreadPriority = (AvThreadPriority)this.MMCSSThreadPriorityTrackBar.Value;

            switch (this.AvailableMMCSSThreadModesComboBox.Text)
            {
                case "Audio":
                    OptionManager.MmThreadCharacteristics = MmThreadCharacteristics.Audio;
                    break;
                case "PlayBack":
                    OptionManager.MmThreadCharacteristics = MmThreadCharacteristics.Playback;
                    break;
                case "Pro Audio":
                    OptionManager.MmThreadCharacteristics = MmThreadCharacteristics.ProAudio;
                    break;
                default:
                    throw new Exception($"不明なMMCSSのスレッドモード '{this.AvailableMMCSSThreadModesComboBox.Text}' が選択されました。");
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
