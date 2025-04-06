namespace LibPulseTune.UIControls
{
    partial class WaveformPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.waveformRenderer1 = new LibPulseTune.UIControls.BackendControls.WaveformRenderer();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "WAVEFORM";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waveformRenderer1
            // 
            this.waveformRenderer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveformRenderer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waveformRenderer1.EnableWaveformAntiAlias = true;
            this.waveformRenderer1.Location = new System.Drawing.Point(0, 15);
            this.waveformRenderer1.Name = "waveformRenderer1";
            this.waveformRenderer1.RenderingPrecision = LibPulseTune.Options.WaveformRendererRenderingPrecision.Normal;
            this.waveformRenderer1.Size = new System.Drawing.Size(150, 45);
            this.waveformRenderer1.StereoViewMode = LibPulseTune.Options.WaveformRendererStereoViewMode.Separated;
            this.waveformRenderer1.TabIndex = 1;
            // 
            // WaveformPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.waveformRenderer1);
            this.Controls.Add(this.label1);
            this.Name = "WaveformPanel";
            this.Size = new System.Drawing.Size(150, 60);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private LibPulseTune.UIControls.BackendControls.WaveformRenderer waveformRenderer1;
    }
}
