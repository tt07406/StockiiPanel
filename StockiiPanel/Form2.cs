using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

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
        public SNListDialog(Dictionary<String, System.Collections.ArrayList> list,string name,DataSet ds, bool isEdited = true)
        {
            pList = list;
            InitializeComponent();

            //stockInfoList.DataSource = ds;
            //stockInfoList.DataMember = "stock_basic_info";

            dt = (DataTable)ds.Tables["stock_basic_info"];
            dv = new DataView(dt);

            stockInfoList.DataSource = dv;

            //改变DataGridView的表头
            stockInfoList.Columns[0].HeaderText = "代码";
            //设置该列宽度
            stockInfoList.Columns[0].Width = 60;
            stockInfoList.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();

            stockInfoList.Columns[1].HeaderText = "名称";
            stockInfoList.Columns[1].Width = 130;
            stockInfoList.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();

            for (int i = 2; i < stockInfoList.Columns.Count; ++i)
                stockInfoList.Columns[i].Visible = false;


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

            groupNameBox.Enabled = isEdited;
            dt = ds.Tables[1];
            DataView dvMenuOptions = new DataView(dt.DefaultView.ToTable(true, new string[] { "areaname" }));//distinct

            foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
            {
                if (rvMain["areaname"].ToString().Equals(""))
                    continue;

                areaCombo.Items.Add(rvMain["areaname"].ToString());
            }
            dvMenuOptions = new DataView(dt.DefaultView.ToTable(true, new string[] { "industryname" }));

            foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
            {
                if (rvMain["industryname"].ToString().Equals(""))
                    continue;

                industryCombox.Items.Add(rvMain["industryname"].ToString());
            }
        }

        private DataTable dt;
        private DataView dv;

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
            //selectedList.Items.Clear();
            selectedList.BeginUpdate();

            int coun = stockInfoList.RowCount;
            for (int i = 0; i < coun; i++)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = stockInfoList.Rows[i].Cells[0].Value.ToString();

                lvi.SubItems.Add(stockInfoList.Rows[i].Cells[1].Value.ToString());

                lvi.Name = lvi.Text;

                if (!selectedList.Items.ContainsKey(lvi.Text))
                {
                    this.selectedList.Items.Add(lvi);
                }
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

        private void KeyWord_TextChanged(object sender, EventArgs e)
        {
            if (this.KeyWord.Text.Equals(""))
            {
                return;
            }

            dv.RowFilter = "stock_id like '%" + this.KeyWord.Text + "%' or stock_name like '%" + this.KeyWord.Text + "%'";
        }

        private void areaCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sql = new StringBuilder("stock_id in (");
            DataRow[] rows = dt.Select("areaname = '" + areaCombo.Text + "'");

            foreach (DataRow row in rows)
            {
                sql.Append("'" + row["stockid"] + "',");
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            dv.RowFilter = sql.ToString();
        }

        private void industryCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sql = new StringBuilder("stock_id in (");
            DataRow[] rows = dt.Select("industryname = '" + industryCombox.Text + "'");

            foreach (DataRow row in rows)
            {
                sql.Append("'" + row["stockid"] + "',");
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            dv.RowFilter = sql.ToString();
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            dv.RowFilter = "";
        }
    }
}
