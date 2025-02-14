namespace LibPulseTune.UIControls
{
    partial class MediaControlPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.PlayPauseButton = new System.Windows.Forms.Button();
            this.BackwardButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.ForwardButton = new System.Windows.Forms.Button();
            this.SeekBarControl = new LibPulseTune.UIControls.SeekBar();
            this.RepeatModeCheckButton = new System.Windows.Forms.CheckBox();
            this.ShuffleCheckButton = new System.Windows.Forms.CheckBox();
            this.VolumeTrackBar = new System.Windows.Forms.TrackBar();
            this.VolumeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // PlayPauseButton
            // 
            this.PlayPauseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayPauseButton.Image = global::LibPulseTune.UIControls.Properties.Resources.play;
            this.PlayPauseButton.Location = new System.Drawing.Point(3, 33);
            this.PlayPauseButton.Name = "PlayPauseButton";
            this.PlayPauseButton.Size = new System.Drawing.Size(39, 39);
            this.PlayPauseButton.TabIndex = 0;
            this.PlayPauseButton.UseVisualStyleBackColor = true;
            this.PlayPauseButton.Click += new System.EventHandler(this.PlayPauseButton_Click);
            // 
            // BackwardButton
            // 
            this.BackwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackwardButton.Image = global::LibPulseTune.UIControls.Properties.Resources.backward;
            this.BackwardButton.Location = new System.Drawing.Point(48, 40);
            this.BackwardButton.Name = "BackwardButton";
            this.BackwardButton.Size = new System.Drawing.Size(32, 32);
            this.BackwardButton.TabIndex = 1;
            this.BackwardButton.UseVisualStyleBackColor = true;
            this.BackwardButton.Click += new System.EventHandler(this.BackwardButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopButton.Image = global::LibPulseTune.UIControls.Properties.Resources.stop;
            this.StopButton.Location = new System.Drawing.Point(81, 40);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(32, 32);
            this.StopButton.TabIndex = 2;
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // ForwardButton
            // 
            this.ForwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ForwardButton.Image = global::LibPulseTune.UIControls.Properties.Resources.forward;
            this.ForwardButton.Location = new System.Drawing.Point(114, 40);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(32, 32);
            this.ForwardButton.TabIndex = 3;
            this.ForwardButton.UseVisualStyleBackColor = true;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // SeekBarControl
            // 
            this.SeekBarControl.AllowMouseWheelValueChange = false;
            this.SeekBarControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SeekBarControl.DecrementStep = 3;
            this.SeekBarControl.IncrementStep = 3;
            this.SeekBarControl.Location = new System.Drawing.Point(3, 3);
            this.SeekBarControl.Maximum = 100;
            this.SeekBarControl.Minimum = 0;
            this.SeekBarControl.Name = "SeekBarControl";
            this.SeekBarControl.Size = new System.Drawing.Size(414, 20);
            this.SeekBarControl.TabIndex = 4;
            this.SeekBarControl.Value = 0;
            this.SeekBarControl.Seek += new System.EventHandler(this.SeekBarControl_Seek);
            this.SeekBarControl.Click += new System.EventHandler(this.SeekBarControl_Seek);
            // 
            // RepeatModeCheckButton
            // 
            this.RepeatModeCheckButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RepeatModeCheckButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.RepeatModeCheckButton.AutoCheck = false;
            this.RepeatModeCheckButton.Image = global::LibPulseTune.UIControls.Properties.Resources.repeat_off;
            this.RepeatModeCheckButton.Location = new System.Drawing.Point(152, 40);
            this.RepeatModeCheckButton.Name = "RepeatModeCheckButton";
            this.RepeatModeCheckButton.Size = new System.Drawing.Size(32, 32);
            this.RepeatModeCheckButton.TabIndex = 6;
            this.RepeatModeCheckButton.ThreeState = true;
            this.RepeatModeCheckButton.UseVisualStyleBackColor = true;
            this.RepeatModeCheckButton.Click += new System.EventHandler(this.RepeatModeCheckButton_Click);
            // 
            // ShuffleCheckButton
            // 
            this.ShuffleCheckButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShuffleCheckButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.ShuffleCheckButton.Image = global::LibPulseTune.UIControls.Properties.Resources.shuffle;
            this.ShuffleCheckButton.Location = new System.Drawing.Point(185, 40);
            this.ShuffleCheckButton.Name = "ShuffleCheckButton";
            this.ShuffleCheckButton.Size = new System.Drawing.Size(32, 32);
            this.ShuffleCheckButton.TabIndex = 7;
            this.ShuffleCheckButton.UseVisualStyleBackColor = true;
            this.ShuffleCheckButton.Click += new System.EventHandler(this.ShuffleCheckButton_CheckedChanged);
            // 
            // VolumeTrackBar
            // 
            this.VolumeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.VolumeTrackBar.AutoSize = false;
            this.VolumeTrackBar.Location = new System.Drawing.Point(279, 52);
            this.VolumeTrackBar.Maximum = 120;
            this.VolumeTrackBar.Name = "VolumeTrackBar";
            this.VolumeTrackBar.Size = new System.Drawing.Size(138, 20);
            this.VolumeTrackBar.TabIndex = 9;
            this.VolumeTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.VolumeTrackBar.Value = 90;
            this.VolumeTrackBar.ValueChanged += new System.EventHandler(this.VolumeTrackBar_ValueChanged);
            // 
            // VolumeLabel
            // 
            this.VolumeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.VolumeLabel.AutoSize = true;
            this.VolumeLabel.Location = new System.Drawing.Point(279, 34);
            this.VolumeLabel.Name = "VolumeLabel";
            this.VolumeLabel.Size = new System.Drawing.Size(53, 12);
            this.VolumeLabel.TabIndex = 10;
            this.VolumeLabel.Text = "音量：n％";
            // 
            // MediaControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.VolumeLabel);
            this.Controls.Add(this.VolumeTrackBar);
            this.Controls.Add(this.ShuffleCheckButton);
            this.Controls.Add(this.RepeatModeCheckButton);
            this.Controls.Add(this.SeekBarControl);
            this.Controls.Add(this.ForwardButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.BackwardButton);
            this.Controls.Add(this.PlayPauseButton);
            this.Name = "MediaControlPanel";
            this.Size = new System.Drawing.Size(420, 74);
            ((System.ComponentModel.ISupportInitialize)(this.VolumeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PlayPauseButton;
        private System.Windows.Forms.Button BackwardButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button ForwardButton;
        private SeekBar SeekBarControl;
        private System.Windows.Forms.CheckBox RepeatModeCheckButton;
        private System.Windows.Forms.CheckBox ShuffleCheckButton;
        private System.Windows.Forms.TrackBar VolumeTrackBar;
        private System.Windows.Forms.Label VolumeLabel;
    }
}
