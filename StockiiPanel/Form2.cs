using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StockiiPanel
{
    public partial class SNListDialog  : Form
    {
        public SNListDialog()
        {
            InitializeComponent();
            String strConn = "Server=127.0.0.1;User ID=root;Password=root;Database=stock;CharSet=utf8;";

            try
            {
                conn = new MySqlConnection(strConn);
                String sqlId = "select stock_id,stock_name from stock_basic_info order by stock_id";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlId, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds, "stock_basic_info");

                stockInfoList.DataSource = ds;
                stockInfoList.DataMember = "stock_basic_info";


                //改变DataGridView的表头
                stockInfoList.Columns[0].HeaderText = "代码";
                //设置该列宽度
                stockInfoList.Columns[0].Width = 50;

                stockInfoList.Columns[1].HeaderText = "名称";
                stockInfoList.Columns[1].Width = 128;

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

        private MySqlConnection conn;
        private void stockList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void stockInfoList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = stockInfoList.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                stockInfoList.Rows[i].HeaderCell.Value = j.ToString();
            } 
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            int k = stockInfoList.SelectedRows.Count;

            selectedList.BeginUpdate();
            for (int i = k -1; i >= 0; i--)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = stockInfoList.SelectedRows[i].Cells[0].Value.ToString();

                lvi.SubItems.Add(stockInfoList.SelectedRows[i].Cells[1].Value.ToString());

                lvi.Name = lvi.Text;
                if (!selectedList.Items.ContainsKey(lvi.Text))
                {
                    this.selectedList.Items.Add(lvi);
                }
                
            }
            selectedList.EndUpdate();
        }
    }
}
