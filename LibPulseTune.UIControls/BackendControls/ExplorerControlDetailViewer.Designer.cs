
namespace LibPulseTune.UIControls.BackendControls
{
    partial class ExplorerControlDetailViewer
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
            this.label2 = new System.Windows.Forms.Label();
            this.DetailsTextBox = new System.Windows.Forms.TextBox();
            this.accessList1 = new LibPulseTune.UIControls.AccessList();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "表示する場所：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(3, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "詳細情報";
            // 
            // DetailsTextBox
            // 
            this.DetailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DetailsTextBox.Location = new System.Drawing.Point(3, 199);
            this.DetailsTextBox.Multiline = true;
            this.DetailsTextBox.Name = "DetailsTextBox";
            this.DetailsTextBox.ReadOnly = true;
            this.DetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DetailsTextBox.Size = new System.Drawing.Size(194, 278);
            this.DetailsTextBox.TabIndex = 3;
            this.DetailsTextBox.WordWrap = false;
            // 
            // accessList1
            // 
            this.accessList1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accessList1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.accessList1.HideSelection = false;
            this.accessList1.Location = new System.Drawing.Point(3, 18);
            this.accessList1.Name = "accessList1";
            this.accessList1.OwnerDraw = true;
            this.accessList1.SelectedLocation = null;
            this.accessList1.Size = new System.Drawing.Size(194, 160);
            this.accessList1.TabIndex = 4;
            this.accessList1.UseCompatibleStateImageBehavior = false;
            this.accessList1.View = System.Windows.Forms.View.Details;
            this.accessList1.LocationSelectionChanged += new System.EventHandler(this.accessList1_LocationSelectionChanged);
            // 
            // ExplorerControlDetailViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.accessList1);
            this.Controls.Add(this.DetailsTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.Name = "ExplorerControlDetailViewer";
            this.Size = new System.Drawing.Size(200, 480);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DetailsTextBox;
        private AccessList accessList1;
    }
}
