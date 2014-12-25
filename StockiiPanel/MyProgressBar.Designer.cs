namespace StockiiPanel
{
    partial class MyProgressBar
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
            this.circularProgressBarEx1 = new StockiiPanel.CircularProgressBar();
            this.SuspendLayout();
            // 
            // circularProgressBarEx1
            // 
            this.circularProgressBarEx1.BackColor = System.Drawing.Color.White;
            this.circularProgressBarEx1.Location = new System.Drawing.Point(-1, -1);
            this.circularProgressBarEx1.MainColor = System.Drawing.Color.LimeGreen;
            this.circularProgressBarEx1.Name = "circularProgressBarEx1";
            this.circularProgressBarEx1.Size = new System.Drawing.Size(163, 163);
            this.circularProgressBarEx1.TabIndex = 0;
            this.circularProgressBarEx1.Text = "circularProgressBarEx1";
            this.circularProgressBarEx1.Value = 1;
            // 
            // MyProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 159);
            this.Controls.Add(this.circularProgressBarEx1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MyProgressBar";
            this.Text = "MyProgressBar";
            this.TransparencyKey = System.Drawing.Color.White;
            this.ResumeLayout(false);

        }

        #endregion

        private CircularProgressBar circularProgressBarEx1;

    }
}