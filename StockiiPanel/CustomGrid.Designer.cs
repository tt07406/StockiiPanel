namespace StockiiPanel
{
    partial class CustomGrid
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.pageLabel = new System.Windows.Forms.Label();
            this.allButton = new System.Windows.Forms.Button();
            this.moreButton = new System.Windows.Forms.Button();
            this.rawContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combinePageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.combineSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.rawContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(681, 377);
            this.dataGridView.TabIndex = 0;
            // 
            // pageLabel
            // 
            this.pageLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pageLabel.AutoSize = true;
            this.pageLabel.Location = new System.Drawing.Point(325, 392);
            this.pageLabel.Name = "pageLabel";
            this.pageLabel.Size = new System.Drawing.Size(23, 12);
            this.pageLabel.TabIndex = 6;
            this.pageLabel.Text = "0/0";
            // 
            // allButton
            // 
            this.allButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.allButton.Location = new System.Drawing.Point(365, 386);
            this.allButton.Name = "allButton";
            this.allButton.Size = new System.Drawing.Size(62, 25);
            this.allButton.TabIndex = 5;
            this.allButton.Text = "显示全部";
            this.allButton.UseVisualStyleBackColor = true;
            this.allButton.Click += new System.EventHandler(this.allButton_Click);
            // 
            // moreButton
            // 
            this.moreButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.moreButton.Location = new System.Drawing.Point(248, 386);
            this.moreButton.Name = "moreButton";
            this.moreButton.Size = new System.Drawing.Size(61, 25);
            this.moreButton.TabIndex = 4;
            this.moreButton.Text = "显示更多";
            this.moreButton.UseVisualStyleBackColor = true;
            this.moreButton.Click += new System.EventHandler(this.moreButton_Click);
            // 
            // rawContextMenuStrip
            // 
            this.rawContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveTableToolStripMenuItem,
            this.saveSelectToolStripMenuItem,
            this.combinePageToolStripMenuItem,
            this.combineSelectToolStripMenuItem});
            this.rawContextMenuStrip.Name = "rawContextMenuStrip";
            this.rawContextMenuStrip.Size = new System.Drawing.Size(149, 92);
            this.rawContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.rawContextMenuStrip_Opening);
            // 
            // saveTableToolStripMenuItem
            // 
            this.saveTableToolStripMenuItem.Name = "saveTableToolStripMenuItem";
            this.saveTableToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveTableToolStripMenuItem.Text = "导出本页";
            // 
            // saveSelectToolStripMenuItem
            // 
            this.saveSelectToolStripMenuItem.Name = "saveSelectToolStripMenuItem";
            this.saveSelectToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveSelectToolStripMenuItem.Text = "导出选中内容";
            // 
            // combinePageToolStripMenuItem
            // 
            this.combinePageToolStripMenuItem.Name = "combinePageToolStripMenuItem";
            this.combinePageToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.combinePageToolStripMenuItem.Text = "拼接本页";
            // 
            // combineSelectToolStripMenuItem
            // 
            this.combineSelectToolStripMenuItem.Name = "combineSelectToolStripMenuItem";
            this.combineSelectToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.combineSelectToolStripMenuItem.Text = "拼接所选";
            // 
            // CustomGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pageLabel);
            this.Controls.Add(this.allButton);
            this.Controls.Add(this.moreButton);
            this.Controls.Add(this.dataGridView);
            this.Name = "CustomGrid";
            this.Size = new System.Drawing.Size(687, 416);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.rawContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label pageLabel;
        private System.Windows.Forms.Button allButton;
        private System.Windows.Forms.Button moreButton;
        private System.Windows.Forms.ContextMenuStrip rawContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem saveTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem combinePageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem combineSelectToolStripMenuItem;
    }
}
