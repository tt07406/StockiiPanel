using System.Collections;//在C#中使用ArrayList必须引用Collections类

namespace StockiiPanel
{
    partial class SNListDialog 
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
            this.groupNameBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stockInfoList = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.selectedList = new System.Windows.Forms.ListView();
            this.stockID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.stockName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addButton = new System.Windows.Forms.Button();
            this.allAddButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stockInfoList)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "组名：";
            // 
            // groupNameBox
            // 
            this.groupNameBox.Location = new System.Drawing.Point(60, 9);
            this.groupNameBox.Name = "groupNameBox";
            this.groupNameBox.Size = new System.Drawing.Size(569, 21);
            this.groupNameBox.TabIndex = 1;
            this.groupNameBox.Text = "新建分组1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stockInfoList);
            this.groupBox1.Location = new System.Drawing.Point(12, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 387);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "可选列表";
            // 
            // stockInfoList
            // 
            this.stockInfoList.AllowUserToAddRows = false;
            this.stockInfoList.AllowUserToDeleteRows = false;
            this.stockInfoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stockInfoList.Location = new System.Drawing.Point(6, 21);
            this.stockInfoList.Name = "stockInfoList";
            this.stockInfoList.ReadOnly = true;
            this.stockInfoList.RowHeadersWidth = 62;
            this.stockInfoList.RowTemplate.Height = 23;
            this.stockInfoList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.stockInfoList.Size = new System.Drawing.Size(272, 358);
            this.stockInfoList.TabIndex = 0;
            this.stockInfoList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.stockInfoList_DataBindingComplete);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.selectedList);
            this.groupBox2.Location = new System.Drawing.Point(397, 47);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(289, 387);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "已选列表";
            // 
            // selectedList
            // 
            this.selectedList.CheckBoxes = true;
            this.selectedList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.stockID,
            this.stockName});
            this.selectedList.FullRowSelect = true;
            this.selectedList.GridLines = true;
            this.selectedList.Location = new System.Drawing.Point(6, 22);
            this.selectedList.Name = "selectedList";
            this.selectedList.Size = new System.Drawing.Size(272, 359);
            this.selectedList.TabIndex = 0;
            this.selectedList.UseCompatibleStateImageBehavior = false;
            this.selectedList.View = System.Windows.Forms.View.Details;
            this.selectedList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.selectedList_ItemChecked);
            this.selectedList.SelectedIndexChanged += new System.EventHandler(this.selectedList_SelectedIndexChanged);
            // 
            // stockID
            // 
            this.stockID.Text = "代码";
            this.stockID.Width = 65;
            // 
            // stockName
            // 
            this.stockName.Text = "名称";
            this.stockName.Width = 150;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(302, 135);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(80, 24);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "添加->";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // allAddButton
            // 
            this.allAddButton.Location = new System.Drawing.Point(302, 185);
            this.allAddButton.Name = "allAddButton";
            this.allAddButton.Size = new System.Drawing.Size(80, 24);
            this.allAddButton.TabIndex = 5;
            this.allAddButton.Text = "全部添加->";
            this.allAddButton.UseVisualStyleBackColor = true;
            this.allAddButton.Click += new System.EventHandler(this.allAddButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(302, 255);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(80, 24);
            this.deleteButton.TabIndex = 6;
            this.deleteButton.Text = "删除";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Enabled = false;
            this.clearButton.Location = new System.Drawing.Point(302, 305);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(80, 24);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "清空";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(216, 462);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(80, 24);
            this.confirmButton.TabIndex = 8;
            this.confirmButton.Text = "确定";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(397, 462);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(80, 24);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // SNListDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 514);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.allAddButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupNameBox);
            this.Controls.Add(this.label1);
            this.Name = "SNListDialog";
            this.Text = "新建分组";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stockInfoList)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox groupNameBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button allAddButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListView selectedList;
        private System.Windows.Forms.ColumnHeader stockID;
        private System.Windows.Forms.ColumnHeader stockName;
        private System.Windows.Forms.DataGridView stockInfoList;
        private ArrayList selectStocks;

        public ArrayList SelectStocks
        {
            get { return selectStocks; }
            set { selectStocks = value; }
        }
        private string groupName;

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

    }
}