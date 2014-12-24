using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;//在C#中使用ArrayList必须引用Collections类
using MySql.Data.MySqlClient;

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        private MySqlConnection conn;
        private DataTable dt;
        private DataSet ds;

        public Form1()
        {
            InitializeComponent();
            pList = new Dictionary<string, ArrayList>();
            InitialCombo();
        }

        private void InitialCombo()
        {
            indexCombo.Items.Add("均价");
            indexCombo.Items.Add("振幅");
            indexCombo.Items.Add("换手");
            indexCombo.Items.Add("量比");
            indexCombo.Items.Add("总金额");
            indexCombo.Items.Add("涨幅");

            indexCombo.SelectedIndex = 0;

            for (int i = 3; i <= 30; ++i)
            {
                intervalCombo.Items.Add(i + "");
            }

            intervalCombo.SelectedIndex = 0;

            typeCombo.Items.Add("负和");
            typeCombo.Items.Add("所有和");
            typeCombo.Items.Add("正和");

            typeCombo.SelectedIndex = 0;

            compareCombo.Items.Add("指定两天减");
            compareCombo.Items.Add("指定时间段内的和");
            compareCombo.Items.Add("指定时间段内最小值");
            compareCombo.Items.Add("指定两天加");
            compareCombo.Items.Add("指定时间段内最大值");
            compareCombo.Items.Add("两个时间段时涨幅依据分段");
            compareCombo.Items.Add("指定两天比值");

            compareCombo.SelectedIndex = 0;

            compareIndexCombo.Items.Add("振幅");
            compareIndexCombo.Items.Add("流通股本");
            compareIndexCombo.Items.Add("均价流通值");
            compareIndexCombo.Items.Add("总股本");
            compareIndexCombo.Items.Add("均价");
            compareIndexCombo.Items.Add("现价");
            compareIndexCombo.Items.Add("总市值");
            compareIndexCombo.Items.Add("换手");
            compareIndexCombo.Items.Add("量比");
            compareIndexCombo.Items.Add("总金额");
            compareIndexCombo.Items.Add("涨幅");

            compareIndexCombo.SelectedIndex = 0;

            indexCombox1.Items.Add("均价");
            indexCombox1.Items.Add("总市值");
            indexCombox1.Items.Add("昨收");
            indexCombox1.Items.Add("流通股本");
            indexCombox1.Items.Add("均价流通市值");
            indexCombox1.Items.Add("总股本");

            indexCombox1.SelectedIndex = 0;
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
            String strConn = "Server=127.0.0.1;User ID=root;Password=root;Database=stock;CharSet=utf8;";

            //初始化版块菜单
            try
            {
                conn = new MySqlConnection(strConn);
                String sqlId = "select * from area_info";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlId, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds, "area_info");

                DataView dvMenuOptions = new DataView(ds.Tables["area_info"]);

                foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
                {
                    ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                    tsItemParent.Text = rvMain["area_name"].ToString();
                    tsItemParent.Name = rvMain["area_id"].ToString();
                    sectionToolStripMenuItem.DropDownItems.Add(tsItemParent);
                }

                sqlId = "select * from industry_info";
                cmd = new MySqlCommand(sqlId, conn);
                da = new MySqlDataAdapter(cmd);

                da.Fill(ds, "industry_info");

                dvMenuOptions = new DataView(ds.Tables["industry_info"]);

                foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
                {
                    ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                    tsItemParent.Text = rvMain["industry_name"].ToString();
                    tsItemParent.Name = rvMain["industry_id"].ToString();
                    industryToolStripMenuItem.DropDownItems.Add(tsItemParent);
                }

            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("不能连接到数据库");
                        break;
                    case 1045:
                        MessageBox.Show("无效的用户名密码");
                        break;
                    case 1049:
                        MessageBox.Show("数据库不存在");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            finally
            {
                conn.Close();
            }
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
            SNListDialog dialog = new SNListDialog(pList,"");
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = dialog.SelectStocks;
            pList.Add(name, stocks);
            groupList.Items.Add(name);
        }

        private Dictionary<String, ArrayList> pList;

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (groupList.SelectedItems.Count == 0)
            {
                editToolStripMenuItem.Visible = false;
                deleteToolStripMenuItem.Visible = false;
            }
            else
            {
                editToolStripMenuItem.Visible = true;
                deleteToolStripMenuItem.Visible = true;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectedName = groupList.SelectedItem.ToString();
            SNListDialog dialog = new SNListDialog(pList, selectedName);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = dialog.SelectStocks;
            pList[name] = stocks;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectedName = groupList.SelectedItem.ToString();
            pList.Remove(selectedName);
            groupList.Items.Remove(selectedName);
        }

        private void compareCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (compareCombo.SelectedIndex == 5)
            {
                compareIndexCombo.Enabled = false;
            }
            else
            {
                compareIndexCombo.Enabled = true;
            }
        }

        /// <summary>

        /// 判断一个数是否是奇数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsOdd(int n)
        {
            if (n % 2 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
