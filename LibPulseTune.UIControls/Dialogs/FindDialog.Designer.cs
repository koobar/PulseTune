namespace LibPulseTune.UIControls.Dialogs
{
    partial class FindDialog
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
            BtnCancel = new System.Windows.Forms.Button();
            BtnFind = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            PromptTextBox = new System.Windows.Forms.TextBox();
            IgnoreCaseCheckBox = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // BtnCancel
            // 
            BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            BtnCancel.Location = new System.Drawing.Point(197, 104);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new System.Drawing.Size(75, 25);
            BtnCancel.TabIndex = 0;
            BtnCancel.Text = "キャンセル(&F)";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // BtnFind
            // 
            BtnFind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            BtnFind.Location = new System.Drawing.Point(116, 104);
            BtnFind.Name = "BtnFind";
            BtnFind.Size = new System.Drawing.Size(75, 25);
            BtnFind.TabIndex = 1;
            BtnFind.Text = "検索(&S)";
            BtnFind.UseVisualStyleBackColor = true;
            BtnFind.Click += BtnFind_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(151, 15);
            label1.TabIndex = 2;
            label1.Text = "検索文字列を入力してください";
            // 
            // PromptTextBox
            // 
            PromptTextBox.Location = new System.Drawing.Point(12, 27);
            PromptTextBox.Name = "PromptTextBox";
            PromptTextBox.Size = new System.Drawing.Size(260, 23);
            PromptTextBox.TabIndex = 3;
            // 
            // IgnoreCaseCheckBox
            // 
            IgnoreCaseCheckBox.AutoSize = true;
            IgnoreCaseCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            IgnoreCaseCheckBox.Location = new System.Drawing.Point(12, 56);
            IgnoreCaseCheckBox.Name = "IgnoreCaseCheckBox";
            IgnoreCaseCheckBox.Size = new System.Drawing.Size(190, 20);
            IgnoreCaseCheckBox.TabIndex = 4;
            IgnoreCaseCheckBox.Text = "大文字と小文字を区別せず検索";
            IgnoreCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // FindDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 141);
            Controls.Add(IgnoreCaseCheckBox);
            Controls.Add(PromptTextBox);
            Controls.Add(label1);
            Controls.Add(BtnFind);
            Controls.Add(BtnCancel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FindDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "トラックの検索";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PromptTextBox;
        private System.Windows.Forms.CheckBox IgnoreCaseCheckBox;
    }
}