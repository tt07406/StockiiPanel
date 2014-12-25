using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace StockiiPanel
{
    public class CircularProgressBarEx : Control
    {
        private CircularProgressBar circularProgressBar = new CircularProgressBar();
        private string completeText = "";
        private string doingText = "";

        public int Value
        {
            get { return this.circularProgressBar.Value; }
            set
            {
                this.circularProgressBar.Value = value;
                this.Invalidate();
            }
        }

        public Color Color
        {
            get { return this.circularProgressBar.MainColor; }
            set
            {
                this.circularProgressBar.MainColor = value;
                this.Invalidate();
            }
        }

        [LocalizableAttribute(true)]
        public string CompleteText
        {
            get { return completeText; }
            set
            {
                completeText = value;
                this.Invalidate();
            }
        }

        [LocalizableAttribute(true)]
        public string DoingText
        {
            get { return doingText; }
            set
            {
                doingText = value;
                this.Invalidate();
            }
        }

        public CircularProgressBarEx()
        {
            //指定控件的样式和行为
            this.SetStyle(
               ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.ResizeRedraw |
               ControlStyles.Selectable |
               ControlStyles.ContainerControl |
               ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Opaque, false);
            this.UpdateStyles();
            this.BackColor = Color.White;
            circularProgressBar.Dock = DockStyle.Top;
            this.Controls.Add(circularProgressBar);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Value == 100)
            {
                SizeF size = e.Graphics.MeasureString(this.completeText, new Font("微软雅黑", 12F));
                e.Graphics.DrawString(this.completeText, new Font("微软雅黑", 12F), new SolidBrush(Color.Gray), new Point(this.Width / 2 - (int)size.Width / 2 - 1, this.Height - (int)size.Height - 5));
            }
            else
            {
                SizeF size = e.Graphics.MeasureString(this.doingText, new Font("微软雅黑", 12F));
                e.Graphics.DrawString(this.doingText, new Font("微软雅黑", 12F), new SolidBrush(Color.Gray), new Point(this.Width / 2 - (int)size.Width / 2 - 1, this.Height - (int)size.Height - 5));
            }

        }
    }
}
