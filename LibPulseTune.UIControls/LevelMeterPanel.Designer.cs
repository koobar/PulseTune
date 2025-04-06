namespace LibPulseTune.UIControls
{
    partial class LevelMeterPanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.RightVolumeMeter = new LibPulseTune.UIControls.BackendControls.VolumeMeterControl();
            this.LeftVolumeMeter = new LibPulseTune.UIControls.BackendControls.VolumeMeterControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(201, 15);
            this.panel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Right;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(183, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 15);
            this.label4.TabIndex = 19;
            this.label4.Text = "0db";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "-90db";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(35, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "LEVEL METER";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RightVolumeMeter);
            this.panel2.Controls.Add(this.LeftVolumeMeter);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(201, 38);
            this.panel2.TabIndex = 1;
            // 
            // RightVolumeMeter
            // 
            this.RightVolumeMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RightVolumeMeter.Decibels = -90F;
            this.RightVolumeMeter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RightVolumeMeter.Location = new System.Drawing.Point(0, 21);
            this.RightVolumeMeter.MinimumDecibels = -30F;
            this.RightVolumeMeter.Name = "RightVolumeMeter";
            this.RightVolumeMeter.ScaleSpacing = 1F;
            this.RightVolumeMeter.Size = new System.Drawing.Size(201, 17);
            this.RightVolumeMeter.TabIndex = 3;
            // 
            // LeftVolumeMeter
            // 
            this.LeftVolumeMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LeftVolumeMeter.Decibels = -90F;
            this.LeftVolumeMeter.Dock = System.Windows.Forms.DockStyle.Top;
            this.LeftVolumeMeter.Location = new System.Drawing.Point(0, 0);
            this.LeftVolumeMeter.MinimumDecibels = -30F;
            this.LeftVolumeMeter.Name = "LeftVolumeMeter";
            this.LeftVolumeMeter.ScaleSpacing = 1F;
            this.LeftVolumeMeter.Size = new System.Drawing.Size(201, 17);
            this.LeftVolumeMeter.TabIndex = 2;
            // 
            // LevelMeterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "LevelMeterPanel";
            this.Size = new System.Drawing.Size(201, 53);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private LibPulseTune.UIControls.BackendControls.VolumeMeterControl RightVolumeMeter;
        private LibPulseTune.UIControls.BackendControls.VolumeMeterControl LeftVolumeMeter;
    }
}
