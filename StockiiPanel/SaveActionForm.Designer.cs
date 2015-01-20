namespace StockiiPanel
{
    partial class SaveActionDialog
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
            this.name = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.yes = new DevComponents.DotNetBar.ButtonX();
            this.no = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.name.Border.Class = "TextBoxBorder";
            this.name.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.name.ButtonCustom.Tooltip = "";
            this.name.ButtonCustom2.Tooltip = "";
            this.name.DisabledBackColor = System.Drawing.Color.White;
            this.name.ForeColor = System.Drawing.Color.Black;
            this.name.Location = new System.Drawing.Point(121, 39);
            this.name.Name = "name";
            this.name.PreventEnterBeep = true;
            this.name.Size = new System.Drawing.Size(100, 21);
            this.name.TabIndex = 0;
            // 
            // yes
            // 
            this.yes.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.yes.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.yes.Location = new System.Drawing.Point(30, 102);
            this.yes.Name = "yes";
            this.yes.Size = new System.Drawing.Size(75, 23);
            this.yes.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.yes.TabIndex = 1;
            this.yes.Text = "保存";
            this.yes.Click += new System.EventHandler(this.yes_Click);
            // 
            // no
            // 
            this.no.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.no.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.no.Location = new System.Drawing.Point(157, 102);
            this.no.Name = "no";
            this.no.Size = new System.Drawing.Size(75, 23);
            this.no.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.no.TabIndex = 2;
            this.no.Text = "取消";
            this.no.Click += new System.EventHandler(this.no_Click);
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(30, 39);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "操作名称：";
            // 
            // SaveActionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 140);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.no);
            this.Controls.Add(this.yes);
            this.Controls.Add(this.name);
            this.Name = "SaveActionForm";
            this.Text = "保存操作";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX name;
        private DevComponents.DotNetBar.ButtonX yes;
        private DevComponents.DotNetBar.ButtonX no;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}