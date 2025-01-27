using PulseTune.Controls.BackendControls;

namespace PulseTune.Controls
{
    partial class PlaylistViewer
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
            this.PlaylistItemViewer = new PulseTune.Controls.BackendControls.OptimizedListView();
            this.SuspendLayout();
            // 
            // PlaylistItemViewer
            // 
            this.PlaylistItemViewer.AllowDrop = true;
            this.PlaylistItemViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlaylistItemViewer.FullRowSelect = true;
            this.PlaylistItemViewer.HideSelection = false;
            this.PlaylistItemViewer.Location = new System.Drawing.Point(0, 0);
            this.PlaylistItemViewer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PlaylistItemViewer.MultiSelect = false;
            this.PlaylistItemViewer.Name = "PlaylistItemViewer";
            this.PlaylistItemViewer.Size = new System.Drawing.Size(337, 234);
            this.PlaylistItemViewer.TabIndex = 0;
            this.PlaylistItemViewer.UseCompatibleStateImageBehavior = false;
            this.PlaylistItemViewer.View = System.Windows.Forms.View.Details;
            this.PlaylistItemViewer.DoubleClick += new System.EventHandler(this.PlaylistItemViewer_DoubleClick);
            // 
            // PlaylistViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PlaylistItemViewer);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PlaylistViewer";
            this.Size = new System.Drawing.Size(337, 234);
            this.ResumeLayout(false);

        }

        #endregion

        private OptimizedListView PlaylistItemViewer;
    }
}
