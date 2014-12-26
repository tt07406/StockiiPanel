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
    public partial class SNListDialog  : Form
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="list">已有的分组</param>
        /// <param name="name">空表示添加分组，否则为要编辑分组的名字</param>
        /// <param name="ds">股票信息</param>
        public SNListDialog(Dictionary<String, System.Collections.ArrayList> list,string name,DataSet ds)
        {
            pList = list;
            InitializeComponent();

            stockInfoList.DataSource = ds;
            stockInfoList.DataMember = "stock_basic_info";

            dt = (DataTable)ds.Tables["stock_basic_info"];

            //改变DataGridView的表头
            stockInfoList.Columns[0].HeaderText = "代码";
            //设置该列宽度
            stockInfoList.Columns[0].Width = 50;

            stockInfoList.Columns[1].HeaderText = "名称";
            stockInfoList.Columns[1].Width = 128;


            if (name.Equals(""))
            {
                selectStocks = new System.Collections.ArrayList();
                groupName = "";
            }
            else
            {
                groupNameBox.Text = name;
                groupName = name;
                selectStocks = new System.Collections.ArrayList(pList[name]);//传引用
                int k = selectStocks.Count;

                selectedList.BeginUpdate();
                for (int i = 0; i < k; ++i)
                {
                    ListViewItem lvi = new ListViewItem();

                    lvi.Text = selectStocks[i].ToString();

                    DataRow[] drs = dt.Select("stock_id = '" + lvi.Text + "'");

                    lvi.SubItems.Add(drs[0]["stock_name"].ToString());

                    lvi.Name = lvi.Text;
                    if (!selectedList.Items.ContainsKey(lvi.Text))
                    {
                        this.selectedList.Items.Add(lvi);
                    }

                }
                selectedList.EndUpdate();

                clearButton.Enabled = true;
            }
        }

        private DataTable dt;

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

            clearButton.Enabled = true;
        }

        private void allAddButton_Click(object sender, EventArgs e)
        {
            selectedList.Items.Clear();
            selectedList.BeginUpdate();

            int coun = stockInfoList.RowCount;
            for (int i = 0; i < coun; i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = stockInfoList.Rows[i].Cells[0].Value.ToString();

                lvi.SubItems.Add(stockInfoList.Rows[i].Cells[1].Value.ToString());

                lvi.Name = lvi.Text;

                this.selectedList.Items.Add(lvi);
            } 

            selectedList.EndUpdate();

            clearButton.Enabled = true;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            selectedList.Items.Clear();
            if (selectedList.Items.Count == 0)
            {
                clearButton.Enabled = false;
            }
        }

        private void selectedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedList.SelectedItems.Count == 0)
            {
                deleteButton.Enabled = false;
            }
            else
            {
                deleteButton.Enabled = true;
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int k = selectedList.SelectedItems.Count;

            selectedList.BeginUpdate();
            for (int i = k -1; i >= 0; i--)
            {
                selectedList.Items.Remove(selectedList.SelectedItems[i]);
            }

            selectedList.EndUpdate();
            deleteButton.Enabled = false;
            if (selectedList.Items.Count == 0)
            {
                clearButton.Enabled = false;
            }
        }

        private void selectedList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            e.Item.Selected = e.Item.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {


            if (groupName.Equals("") && pList.ContainsKey(groupNameBox.Text))
            {
                MessageBox.Show(groupNameBox.Text + "已存在");
                return;
            }

            int count = selectedList.Items.Count;

            if (count == 0)
            {
                MessageBox.Show("已选列表为空");
                return;
            }

            selectStocks.Clear();

            for (int i = 0; i < count; ++i)
            {
                selectStocks.Add(selectedList.Items[i].Name);
            }
            groupName = groupNameBox.Text;
            isSuccess = true;
            
            this.Close();
        }

        private Dictionary<String, System.Collections.ArrayList> pList;
        private bool isSuccess = false;

        public bool IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; }
        }
    }
}
