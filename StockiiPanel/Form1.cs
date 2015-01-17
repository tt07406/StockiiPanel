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
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        private DataTable dt;
        private DataSet stockDs;//股票基本信息
        private DataSet ds;
        private int errorNo = -1;
        private List<int> totalPageList = new List<int>();
        private List<int> pageList= new List<int>();
        private List<String> sortColumnList = new List<String>();
        private List<bool> sortTypeList = new List<bool>();
        private int pagesize;//页大小
        private int curTabIndex = 0;

        private SerializableDictionary<String, ArrayList> pList;
        private Dictionary<int, string> record;//上次记录

        private CustomDialog customDialog;

        public Form1()
        {
            InitializeComponent();
            pList = new SerializableDictionary<string, ArrayList>();
            InitialCombo();
            customDialog = new CustomDialog();
            customDialog.StartPosition = FormStartPosition.CenterScreen;
            record = new Dictionary<int, string>();
            for (int i = 0; i < tabControl.TabCount; i++)
            {
                pageList.Add(1);
                totalPageList.Add(1);
                sortColumnList.Add("");
                sortTypeList.Add(true);
            }
            curTabIndex = tabControl.SelectedIndex;
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

        public void initBeforeShow()
        {
            Commons.GetTradeDate();

            Commons.GetStockClassification(sectionToolStripMenuItem, industryToolStripMenuItem);
            stockDs = Commons.GetStockBasicInfo();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //反序列化载入分组列表
            using (FileStream fileStream = new FileStream("group.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                this.pList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
            }
            using (FileStream fileStream = new FileStream("raw.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                this.rawDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
            }
            
            using (FileStream fileStream = new FileStream("cross.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                this.crossDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
            }

            foreach (var dic in pList)
            {
                groupList.Items.Add(dic.Key);
            }
            

            //为版块菜单增加事件处理
            for (int i = 0; i < sectionToolStripMenuItem.DropDownItems.Count; ++i)
            {
                sectionToolStripMenuItem.DropDownItems[i].Click += new EventHandler(sectionItem_Click);
            }

            for (int i = 0; i < industryToolStripMenuItem.DropDownItems.Count; ++i)
            {
                industryToolStripMenuItem.DropDownItems[i].Click += new EventHandler(industryItem_Click);
            }

            foreach (ToolStripMenuItem c in upToolStripMenuItem.DropDownItems)
            {
                if (!c.HasDropDownItems)
                    c.Click += new EventHandler(upItem_Click);

                foreach (ToolStripMenuItem dc in c.DropDownItems)
                {
                    if (!dc.HasDropDownItems)
                        dc.Click += new EventHandler(upItem_Click);

                    foreach (ToolStripMenuItem sdc in dc.DropDownItems)
                    {
                        foreach (ToolStripMenuItem ssdc in sdc.DropDownItems)
                        {
                            //ssdc.Click += new EventHandler(upItem_Click);
                            ssdc.Click += new EventHandler(timeItem_Click);
                        }
                    }
                }
            }

            foreach (ToolStripMenuItem c in downToolStripMenuItem.DropDownItems)
            {
                if (!c.HasDropDownItems)
                    c.Click += new EventHandler(downItem_Click);

                foreach (ToolStripMenuItem dc in c.DropDownItems)
                {
                    if (!dc.HasDropDownItems)
                        dc.Click += new EventHandler(downItem_Click);

                    foreach (ToolStripMenuItem sdc in dc.DropDownItems)
                    {
                        foreach (ToolStripMenuItem ssdc in sdc.DropDownItems)
                        {
                            //ssdc.Click += new EventHandler(downItem_Click);
                            ssdc.Click += new EventHandler(timeItem_Click);
                        }
                    }
                }
            }
        }

        private void timeItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            String name = item.Name;

            int headEnd = name.IndexOf("Tool");
            int tailStart = name.IndexOf("Item")+4;
            int length = 1;
            String arg = null;
            String year = null;

            if (name.StartsWith("season"))
            {
                length = headEnd - 6;
                arg = name.Substring(6, length);
                year = name.Substring(tailStart, 4);
                SetDate("season", arg, year);
            }
            else
            {
                length = headEnd - 5;
                arg = name.Substring(5, length);
                year = name.Substring(tailStart, 4);
                SetDate("month", arg, year);
            }

            if (name.Contains("Down"))
            {
                downItem_Click(sender, e);
            }
            else
            {
                upItem_Click(sender, e);
            }
        }

        private void sectionItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[1] = item.Name;
            pagesize = 1000;
            pageList[curTabIndex] = 1;
            searchTab(curTabIndex);
        }

        private void industryItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[2] = item.Name;

            pagesize = 1000;
            pageList[curTabIndex] = 1;
            searchTab(curTabIndex);
        }

        private void upItem_Click(object sender, EventArgs e)
        {
            if (curTabIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[3] = item.Name;

            pagesize = 1000;

            searchTab(curTabIndex);
        }

        private void downItem_Click(object sender, EventArgs e)
        {
            if (curTabIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[4] = item.Name;

            pagesize = 1000;

            searchTab(curTabIndex);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            pagesize = 1000;
            pageList[curTabIndex] = 1;
            searchTab(curTabIndex);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void 添加分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SNListDialog dialog = new SNListDialog(pList,"",stockDs);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = new ArrayList(dialog.SelectStocks);
            pList.Add(name, stocks);
            groupList.Items.Add(name);
        }


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
            SNListDialog dialog = new SNListDialog(pList, selectedName, stockDs);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = new ArrayList(dialog.SelectStocks);
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

        private void rawDataGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            
            int coun = rawDataGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                rawDataGrid.Rows[i].HeaderCell.Value = j.ToString();
                
                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    rawDataGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }
            }
        }

        private void bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();
            int totalPages;
            if (args.Length == 3)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);
                stop = Commons.GetStockDayInfo(stocks, sortColumnList[curTabIndex], sortTypeList[curTabIndex], startDate, endDate, pageList[curTabIndex], pagesize, out errorNo, out ds, out totalPages);
                totalPageList[curTabIndex] = totalPages;
            }
            else //版块
            {

                stop = Commons.GetStockDayInfoBoard(record, sortColumnList[curTabIndex], sortTypeList[curTabIndex], startDate, endDate, pageList[curTabIndex], pagesize, out errorNo, out ds, out totalPages);
                totalPageList[curTabIndex] = totalPages;
            }
        }

        private void bkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            myBar.Close();
            if (stop)//因异常停止
            {
                switch (errorNo)
                {
                    case 0:
                        MessageBox.Show("超时，不能连接到数据库");
                        break;
                    case 24:
                        MessageBox.Show("内存溢出");
                        break;
                    case 1045:
                        MessageBox.Show("无效的用户名密码");
                        break;
                    case 1049:
                        MessageBox.Show("数据库不存在");
                        break;
                    default:
                        MessageBox.Show("连接数据库错误");
                        break;
                }
                return;
            }

            dt = (DataTable)ds.Tables["stock_day_info"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");               
                return;
            }
            DataSet orgDs = (DataSet)rawDataGrid.DataSource;
            if (orgDs != null && orgDs.Tables["stock_day_info"] != null && pageList[curTabIndex] != 1)
            {
                orgDs.Tables["stock_day_info"].Merge(dt);
            }
            else 
            {
                rawDataGrid.DataSource = ds;
                rawDataGrid.DataMember = "stock_day_info";
            }
            pageLabel.Text = pageList[curTabIndex] + "/" + (int)totalPageList[curTabIndex];

            rawDataGrid.AllowUserToAddRows = false;//不显示最后空白行
            rawDataGrid.EnableHeadersVisualStyles = false;
            int k = 0;
            int sortIndex = 0;
            foreach (var item in rawDict)
            {
                rawDataGrid.Columns[k].HeaderText = item.Value;
                rawDataGrid.Columns[k].Width = 60 + item.Value.Length * 10;
                rawDataGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns[item.Key].ToString();
                rawDataGrid.Columns[k].SortMode = DataGridViewColumnSortMode.Programmatic;
                switch (item.Key)
                {
                    case "stock_id":
                    case "created":
                    case "turnover_ratio":
                    case "volume_ratio":
                    case "growth_ratio":
                    case "avg_price":
                    case "amplitude_ratio":
                    case "total_money":
                        rawDataGrid.Columns[k].HeaderCell.Style.ForeColor = Color.Red;
                        break;
                    default:
                        break;
                }
                if (sortColumnList[curTabIndex] == rawDataGrid.Columns[k].DataPropertyName)
                {
                    sortIndex = k;
                }
                k++;
            }

            rawDataGrid.Columns[sortIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabIndex] ? SortOrder.Ascending : SortOrder.Descending;

            for (int i = 2; i < rawDataGrid.Columns.Count; ++i)
            {
                rawDataGrid.Columns[i].Visible = false;
            }
            rawDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            rawDataGrid.AutoResizeColumns();
            
            Commons.colNum = rawDict.Count;

            SetColumns();
            stop = true;
        }

        private void bkWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void setColToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rawDataGrid.RowCount > 0)
            {
                customDialog.ShowDialog(this);

                SetColumns();
            }
            else
            {
                MessageBox.Show("查询结果为空，请先查询");
            }
               
        }
        
        /// <summary>
        /// 设置列
        /// </summary>
        private void SetColumns()
        {
            string[] cols = customDialog.StrCollected.Split(',');

            //选中的列可见
            for (int i = 2; i < Commons.colNum ; ++i)
            {
                rawDataGrid.Columns[i].Visible = false;
            }

            foreach (string col in cols)
            {
                if (col == string.Empty)
                    break;

                int i = Convert.ToInt32(col);

                rawDataGrid.Columns[i + 2].Visible = true;
            }

            //冻结列
            rawDataGrid.AutoGenerateColumns = false;
            for (int i = 0, j = 0; i < rawDataGrid.Columns.Count; ++i)
            {
                if (rawDataGrid.Columns[i].Visible)
                {
                    j++;
                }
                else
                {
                    rawDataGrid.Columns[i].Frozen = false;
                    continue;
                }

                if (j <= customDialog.FrozenNum)
                {
                    rawDataGrid.Columns[i].Frozen = true;
                }
                else
                {
                    rawDataGrid.Columns[i].Frozen = false;
                }
            }

            int coun = rawDataGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                //被冻结的行绿色
                for (int j = 0; j < rawDataGrid.Columns.Count; ++j)
                {
                    if (rawDataGrid.Columns[j].Frozen)
                        rawDataGrid.Rows[i].Cells[j].Style.BackColor = Color.Lime;
                    else
                        rawDataGrid.Rows[i].Cells[j].Style.BackColor = rawDataGrid.Rows[i].DefaultCellStyle.BackColor;
                }
            } 
        }

       

        private void moreButton_Click(object sender, EventArgs e)
        {
            if ((int)totalPageList[curTabIndex] == pageList[curTabIndex])
            {
                MessageBox.Show("已是尾页");
                return;
            }
            pageList[curTabIndex]++;
            pagesize = 1000;

            searchTab(curTabIndex);
        }

        private void allButton_Click(object sender, EventArgs e)
        {
            pageList[curTabIndex] = 1;
            pagesize = 10000;
            totalPageList[curTabIndex] = 1;

            searchTab(curTabIndex);
        }

        private void sumWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();

            int type = 1;
            if (daySumButton.Checked)
            {
                type = 1;
            }
            else if (weekSumButton.Checked)
            {
                type = 2;
            }
            else if (monthSumButton.Checked)
            {
                type = 3;
            }
            int totalPages;
            if (args.Length == 6 )//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);

                stop = Commons.GetNDaysSum(stocks, type, Convert.ToInt32(args[3]), args[4], args[5], sortColumnList[curTabIndex], sortTypeList[curTabIndex], startDate, endDate, pageList[curTabIndex], pagesize, out errorNo, out ds, out totalPages);
            }
            else //版块
            {
                stop = Commons.GetNDaysSumBoard(record, type, Convert.ToInt32(args[2]), args[3], args[4], sortColumnList[curTabIndex], sortTypeList[curTabIndex], startDate, endDate, pageList[curTabIndex], pagesize, out errorNo, out ds, out totalPages);
            }
            totalPageList[curTabIndex] = totalPages;

        }

        private void customWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();
            
            if (args.Length == 7)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);

                stop = Commons.GetStockDaysDiff(stocks, Convert.ToDouble(args[3]), Convert.ToDouble(args[4]), args[5], args[6], startDate, endDate, out errorNo, out ds);
            }
            else //版块
            {
                stop = Commons.GetStockDaysDiffBoard(record, Convert.ToDouble(args[2]), Convert.ToDouble(args[3]), args[4], args[5], startDate, endDate, out errorNo, out ds);
            }
        }


        private void crossWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();

            stop = Commons.GetCrossInfoCmd(Convert.ToDouble(args[2]), args[3], startDate, endDate, out errorNo, out ds);

        }

        private void daySumButton_CheckedChanged(object sender, EventArgs e)
        {
            intervalCombo.Items.Clear();
            for (int i = 3; i <= 31; i++)
                intervalCombo.Items.Add(i+"");
            intervalCombo.SelectedIndex = 0;
        }

        private void weekSumButton_CheckedChanged(object sender, EventArgs e)
        {
            intervalCombo.Items.Clear();
            for (int i = 1; i <= 6; i++)
                intervalCombo.Items.Add(i + "");
            intervalCombo.SelectedIndex = 0;
        }

        private void monthSumButton_CheckedChanged(object sender, EventArgs e)
        {
            intervalCombo.Items.Clear();
            for (int i = 1; i <= 12; i++)
                intervalCombo.Items.Add(i + "");
            intervalCombo.SelectedIndex = 0;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            curTabIndex = tabControl.SelectedIndex;
        }

        private void rawDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            DataGridView gridView  = (DataGridView)sender;
            int curSortIndex = 0;
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (sortColumnList[curTabIndex] == gridView.Columns[i].DataPropertyName)
                {
                    curSortIndex = i;
                }
            }

            sortColumnList[curTabIndex] = gridView.Columns[e.ColumnIndex].DataPropertyName;
            if (curSortIndex == e.ColumnIndex)
            {
                if (gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    sortTypeList[curTabIndex] = false;
                }
                else
                {
                    sortTypeList[curTabIndex] = true;
                }
            }
            else
            {
                gridView.Columns[curSortIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabIndex] ? SortOrder.Ascending : SortOrder.Descending;
            Console.WriteLine("sort_name: {0}", gridView.Columns[e.ColumnIndex].DataPropertyName);
            pageList[curTabIndex] = 1;
            searchTab(curTabIndex);
            //gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
        }

    }    

}
