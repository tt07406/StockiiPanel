using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupList.Show();
            //this.Controls.Add(this.groupList);
            //this.tabControl.Location = new System.Drawing.Point(157, 34);
        }

        private void boardButton_Click(object sender, EventArgs e)
        {
            groupList.Hide();
            //this.Controls.Remove(this.groupList);
            //tabControl.Location = new System.Drawing.Point(16, 34); ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void searchButton_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void endDatePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void 添加分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SNListDialog dialog = new SNListDialog();
            dialog.ShowDialog(this);
        }
    }

     /// 说明: 使用此Button时要设置ContextMenuStrip属性值  
　　      ///       单击些Button的Click事件要传入所在工做区的宽高  
　　       ///       如果没有所需的属性值,则如平时所使用的Button一至  
　　       /// 使用例子:  
　　      ///       DropButton.WorkSizeX = this.MdiParent.ClientRectangle.Width;  
　　       ///       DropButton.WorkSizeY = this.MdiParent.ClientRectangle.Height;  
　　       /// 应用:  
　　      /// 创建人: lrj  
　　    /// 创建日期:2008-05-22  
　　     /// 修改人:  
　　     /// 修改日期:  
　　     /// 
  
　　     public partial class DropButton : Button  
　　     {  
　　         private ContextMenuStrip contextMenuStrip;  
　　         private Point point;     //立标  
　　         private int x = 0;     //立标x  
　　         private int y = 0;     //立标y  
　　         private int workSize_x;//工做区x    
　　         private int workSize_y;//工做区y  

　　         public DropButton()  
　　        {  
　　             x = this.Size.Width ;  
　　             y = 0;  
　　         }  
　　         /// 
  
　　         /// 工做区的完  
　　         /// 
  
　　         public int WorkSizeX  
　　         {  
　　             get { return workSize_x; }  
　　             set { workSize_x = value; }  
　　         }  
　　         /// 
  
　　         /// 工做区的高  
　　         /// 
  
　　         public int WorkSizeY  
　　         {  
　　             get { return workSize_y; }  
　　             set { workSize_y = value - 55; }  
　　         }  
　　          ///
　　          
  
　　         /// ContextMenuStrip菜单  
　　         /// 
  
　　         public override ContextMenuStrip ContextMenuStrip  
　　         {  
　　             get { return contextMenuStrip; }  
　　             set   
　　             {  
　　                 if (contextMenuStrip == null)  
　　                 {  
　　                     contextMenuStrip = value;  
　　                 }  
　　             }  
　　        }   
　　         //  
　　         //重写的单击事件  
　　         //  
　　         protected override void OnClick(EventArgs e)  
　　         {  
　　             base.OnClick(e);  
　　             //菜单在工做区离边框的宽高  
　　             int _x = this.Parent.Location.X + this.Location.X  + contextMenuStrip.Size.Width;
                 int _y = this.Parent.Location.Y + this.Location.Y  + this.Size.Height + contextMenuStrip.Size.Height;  
　　             if
　　             (_x < WorkSizeX - 8)  
　　             {  
　　                 x = this.Size.Width;  
　　             }  
　　             else 
　　             {  
　　                 x = 0 - contextMenuStrip.Size.Width + this.Size.Width;  
　　             }  
　　             if 
　　             (_y < WorkSizeY)  
　　             {  
　　                 y = 0;  
　　             }  
　　             else 
　　             {  
　　                 y = 0 - contextMenuStrip.Size.Height ;  
　　             }  
　　              point =
　　            new Point(x, y);  
　　             contextMenuStrip.Show(this, point);  
　　         }  
　　         //  
　　         //使鼠标右键失效  
　　         //  
　　         protected override void OnMouseDown(MouseEventArgs mevent)  
　　        {  
　　            base.OnMouseDown(mevent);  
　　             if (mevent.Button.ToString() != "Right")  
　　             {  
　　            }  
　　         }  
　　     } 
}
