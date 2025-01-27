
namespace PulseTune.Dialogs
{
    partial class WasapiOptionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.AvailableMMCSSThreadModesComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MMCSSThreadPriorityTrackBar = new System.Windows.Forms.TrackBar();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.MMCSSThreadPriorityLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MMCSSThreadPriorityTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "MMCSSスレッドモード";
            // 
            // AvailableMMCSSThreadModesComboBox
            // 
            this.AvailableMMCSSThreadModesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AvailableMMCSSThreadModesComboBox.FormattingEnabled = true;
            this.AvailableMMCSSThreadModesComboBox.Items.AddRange(new object[] {
            "無効",
            "Audio",
            "PlayBack",
            "Pro Audio"});
            this.AvailableMMCSSThreadModesComboBox.Location = new System.Drawing.Point(12, 27);
            this.AvailableMMCSSThreadModesComboBox.Name = "AvailableMMCSSThreadModesComboBox";
            this.AvailableMMCSSThreadModesComboBox.Size = new System.Drawing.Size(121, 23);
            this.AvailableMMCSSThreadModesComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "MMCSSスレッド優先度";
            // 
            // MMCSSThreadPriorityTrackBar
            // 
            this.MMCSSThreadPriorityTrackBar.AutoSize = false;
            this.MMCSSThreadPriorityTrackBar.Location = new System.Drawing.Point(12, 76);
            this.MMCSSThreadPriorityTrackBar.Maximum = 2;
            this.MMCSSThreadPriorityTrackBar.Minimum = -1;
            this.MMCSSThreadPriorityTrackBar.Name = "MMCSSThreadPriorityTrackBar";
            this.MMCSSThreadPriorityTrackBar.Size = new System.Drawing.Size(360, 22);
            this.MMCSSThreadPriorityTrackBar.TabIndex = 3;
            this.MMCSSThreadPriorityTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.MMCSSThreadPriorityTrackBar.ValueChanged += new System.EventHandler(this.MMCSSThreadPriorityTrackBar_ValueChanged);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BtnCancel.Location = new System.Drawing.Point(297, 124);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 25);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BtnOK.Location = new System.Drawing.Point(216, 124);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 25);
            this.BtnOK.TabIndex = 5;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // MMCSSThreadPriorityLabel
            // 
            this.MMCSSThreadPriorityLabel.AutoSize = true;
            this.MMCSSThreadPriorityLabel.Location = new System.Drawing.Point(136, 58);
            this.MMCSSThreadPriorityLabel.Name = "MMCSSThreadPriorityLabel";
            this.MMCSSThreadPriorityLabel.Size = new System.Drawing.Size(46, 15);
            this.MMCSSThreadPriorityLabel.TabIndex = 6;
            this.MMCSSThreadPriorityLabel.Text = "Normal";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "設定は次回再生時から反映されます。";
            // 
            // WasapiOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MMCSSThreadPriorityLabel);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.MMCSSThreadPriorityTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AvailableMMCSSThreadModesComboBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WasapiOptionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WASAPI/MMCSSスレッド設定";
            ((System.ComponentModel.ISupportInitialize)(this.MMCSSThreadPriorityTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AvailableMMCSSThreadModesComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar MMCSSThreadPriorityTrackBar;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Label MMCSSThreadPriorityLabel;
        private System.Windows.Forms.Label label3;
    }
}