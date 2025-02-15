namespace LibPulseTune.UIControls
{
    partial class ExplorerControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.NextButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.Viewer = new LibPulseTune.UIControls.BackendControls.FileSystemViewer();
            this.AddressTextBox = new LibPulseTune.UIControls.BackendControls.VerticalTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.NextButton);
            this.panel1.Controls.Add(this.BackButton);
            this.panel1.Controls.Add(this.AddressTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 30);
            this.panel1.TabIndex = 0;
            // 
            // NextButton
            // 
            this.NextButton.Image = ((System.Drawing.Image)(resources.GetObject("NextButton.Image")));
            this.NextButton.Location = new System.Drawing.Point(29, 3);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(24, 24);
            this.NextButton.TabIndex = 3;
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Image = ((System.Drawing.Image)(resources.GetObject("BackButton.Image")));
            this.BackButton.Location = new System.Drawing.Point(4, 3);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(24, 24);
            this.BackButton.TabIndex = 2;
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // Viewer
            // 
            this.Viewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Viewer.AutoArrange = false;
            this.Viewer.FullRowSelect = true;
            this.Viewer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Viewer.HideSelection = false;
            this.Viewer.Location = new System.Drawing.Point(3, 33);
            this.Viewer.Name = "Viewer";
            this.Viewer.OwnerDraw = true;
            this.Viewer.Size = new System.Drawing.Size(506, 408);
            this.Viewer.TabIndex = 1;
            this.Viewer.UseCompatibleStateImageBehavior = false;
            this.Viewer.View = System.Windows.Forms.View.Details;
            this.Viewer.Navigated += new System.EventHandler(this.Viewer_Navigated);
            this.Viewer.FileDoubleClick += new System.EventHandler(this.Viewer_FileDoubleClick);
            this.Viewer.SelectedIndexChanged += new System.EventHandler(this.Viewer_SelectedIndexChanged);
            this.Viewer.DoubleClick += new System.EventHandler(this.Viewer_DoubleClick);
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AddressTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.AddressTextBox.LeftRightPadding = ((uint)(5u));
            this.AddressTextBox.Location = new System.Drawing.Point(59, 3);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.ReadOnly = false;
            this.AddressTextBox.Size = new System.Drawing.Size(450, 24);
            this.AddressTextBox.TabIndex = 1;
            this.AddressTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // ExplorerControlEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.Controls.Add(this.Viewer);
            this.Controls.Add(this.panel1);
            this.Name = "ExplorerControlEx";
            this.Size = new System.Drawing.Size(512, 444);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private BackendControls.VerticalTextBox AddressTextBox;
        private BackendControls.FileSystemViewer Viewer;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button BackButton;
    }
}
