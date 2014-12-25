using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StockiiPanel
{
    public class CircularProgressBar : Control
    {
        private Color mainColor = Color.Blue;

        public Color MainColor
        {
            get { return mainColor; }
            set
            {
                mainColor = value;
                this.Invalidate();
            }
        }
        private int value = 100;

        public int Value
        {
            get { return this.value; }
            set
            {
                this.value = value;
                this.Invalidate();
            }
        }

        private int lastWidth = 0;
        private int lastHeight = 0;
        private Point lastLocation = Point.Empty;
        public CircularProgressBar()
        {
            //指定控件的样式和行为
            //this.SetStyle(ControlStyles.UserPaint, true); //用户自行重绘
            //this.SetStyle(ControlStyles.ResizeRedraw, true); //调整大小时重绘
            //this.SetStyle(ControlStyles.DoubleBuffer, true);// 双缓冲
            //this.SetStyle(ControlStyles.Opaque, false);
            //this.BackColor = Color.White;
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
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            this.lastWidth = this.Width;
            this.lastHeight = this.Height;
            this.lastLocation = this.Location;
            base.OnHandleCreated(e);
        }
        private bool isSizeChangeAble = true; //是否允许OnSizeChanged执行
        protected override void OnSizeChanged(EventArgs e)
        {
            if (isSizeChangeAble)
            {
                isSizeChangeAble = false;
                if (this.Width < this.lastWidth || this.Height < this.lastHeight)
                {
                    this.Width = Math.Min(this.Width, this.Height);
                    this.Height = Math.Min(this.Width, this.Height);
                    this.lastWidth = this.Width;
                    this.lastHeight = this.Height;
                    base.OnSizeChanged(e);
                    isSizeChangeAble = true;
                    return;
                }
                if (this.Width > this.lastWidth || this.Height > this.lastHeight)
                {
                    this.Width = Math.Max(this.Width, this.Height);
                    this.Height = Math.Max(this.Width, this.Height);
                    this.lastWidth = this.Width;
                    this.lastHeight = this.Height;
                    base.OnSizeChanged(e);
                    isSizeChangeAble = true;
                    return;
                }
                this.lastWidth = this.Width;
                this.lastHeight = this.Height;
                base.OnSizeChanged(e);
                isSizeChangeAble = true;
                return;
            }
        }
        private int circularWidth = 16;
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                if (this.value == 100)
                {
                    e.Graphics.FillEllipse(new SolidBrush(this.mainColor), new Rectangle(new Point(e.ClipRectangle.X + circularWidth / 2, e.ClipRectangle.Y + circularWidth / 2), new Size(e.ClipRectangle.Width - 1 - circularWidth, e.ClipRectangle.Height - 1 - circularWidth)));
                }
                using (Pen p = new Pen(Brushes.LightGray, circularWidth))
                {
                    //设置起止点线帽  
                    //p.StartCap = LineCap.Round;
                    //p.EndCap = LineCap.Round;
                    //设置连续两段的联接样式  
                    p.LineJoin = LineJoin.Round;
                    e.Graphics.DrawEllipse(p, new Rectangle(new Point(e.ClipRectangle.X + circularWidth / 2, e.ClipRectangle.Y + circularWidth / 2), new Size(e.ClipRectangle.Width - 1 - circularWidth, e.ClipRectangle.Height - 1 - circularWidth)));
                }
                if (this.value < 100)
                {
                    using (Pen p = new Pen(new SolidBrush(this.mainColor), circularWidth))
                    {
                        //设置起止点线帽  
                        //p.StartCap = LineCap.Round;
                        //p.EndCap = LineCap.Round;
                        //设置连续两段的联接样式  
                        p.LineJoin = LineJoin.Round;
                        e.Graphics.DrawArc(p, new Rectangle(new Point(e.ClipRectangle.X + circularWidth / 2, e.ClipRectangle.Y + circularWidth / 2), new Size(e.ClipRectangle.Width - 1 - circularWidth, e.ClipRectangle.Height - 1 - circularWidth)), 45, (float)((float)this.value * 3.6));
                    }
                }
                if (this.value == 100)
                {
                    SizeF size = e.Graphics.MeasureString(this.value.ToString(), new Font("黑体", 15F, System.Drawing.FontStyle.Bold));
                    //e.Graphics.DrawString(this.value.ToString(), new Font("黑体", 15F, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), new Point(this.Width / 2 - (int)size.Width / 2 - 1, this.Height / 2 - (int)size.Height / 2 + 2));
                    e.Graphics.DrawString("Loading", new Font("华文新魏", 12F, System.Drawing.FontStyle.Bold), new SolidBrush(Color.White), new Point(this.Width / 2 - (int)size.Width - 1, this.Height / 2 - (int)size.Height / 2 + 2));
                }
                else
                {
                    SizeF size = e.Graphics.MeasureString(this.value.ToString(), new Font("黑体", 15F, System.Drawing.FontStyle.Bold));
                    //e.Graphics.DrawString(this.value.ToString(), new Font("黑体", 15F, System.Drawing.FontStyle.Bold), new SolidBrush(Color.DimGray), new Point(this.Width / 2 - (int)size.Width / 2 - 1, this.Height / 2 - (int)size.Height / 2 + 2));
                    e.Graphics.DrawString("Loading", new Font("华文新魏", 12F, System.Drawing.FontStyle.Bold), new SolidBrush(Color.DimGray), new Point(this.Width / 2 - (int)size.Width  - 1, this.Height / 2 - (int)size.Height / 2 + 2));
                }
            }
            catch { }
        }
    }
}
