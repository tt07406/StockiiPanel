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
using DevComponents.DotNetBar;


namespace StockiiPanel
{
    public partial class Form1 : RibbonForm
    {
        private DataTable dt;
        private DataSet stockDs;//股票基本信息
        private DataSet ds;
        private int errorNo = -1;
        private Dictionary<String, int> totalPageList = new Dictionary<String, int>();
        private Dictionary<String, int> pageList = new Dictionary<String, int>();
        private Dictionary<String, String> sortColumnList = new Dictionary<String, String>();
        private Dictionary<String, bool> sortTypeList = new Dictionary<String, bool>();
        private int pagesize;//页大小
        private String curTabName = "";
        private String curGroupName = "";
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
            curTabName = tabControl.SelectedTab.Name;
            initTab(curTabName);
            
        }

        private void initTab(string curTabName)
        {
            if (curTabName != "")
            {
                if (!totalPageList.Keys.Contains(curTabName))
                    totalPageList[curTabName] = 0;
                if (!pageList.Keys.Contains(curTabName))
                    pageList[curTabName] = 0;
                if (!sortColumnList.Keys.Contains(curTabName))
                    sortColumnList[curTabName] = "";
                if (!sortTypeList.Keys.Contains(curTabName))
                    sortTypeList[curTabName] = true;
            }
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
            try
            {
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
            }
            catch (Exception)
            {
                
            }
            //反序列化载入分组列表
            


            

            areaItem.SubItems.Clear();
            industryItem.SubItems.Clear();
            //为版块菜单增加事件处理
            for (int i = 0; i < sectionToolStripMenuItem.DropDownItems.Count; ++i)
            {
                //sectionToolStripMenuItem.DropDownItems[i].Click += new EventHandler(sectionItem_Click);
                ButtonItem item = new ButtonItem();
                item.Name = sectionToolStripMenuItem.DropDownItems[i].Name;
                item.Text = sectionToolStripMenuItem.DropDownItems[i].Text;
                item.Click += new EventHandler(sectionItem_Click);
                areaItem.SubItems.Add(item);
            }

            for (int i = 0; i < industryToolStripMenuItem.DropDownItems.Count; ++i)
            {
                //industryToolStripMenuItem.DropDownItems[i].Click += new EventHandler(industryItem_Click);
                ButtonItem item = new ButtonItem();
                item.Name = industryToolStripMenuItem.DropDownItems[i].Name;
                item.Text = industryToolStripMenuItem.DropDownItems[i].Text;
                item.Click += new EventHandler(industryItem_Click);
                industryItem.SubItems.Add(item);
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
            initFormUI();

            // these code are only available after data initialization
            initGroupsButton();

        }

        private void initGroupsButton()
        {
            myGroups.SubItems.Clear();

            foreach (var dic in pList)
            {
                addNewGroupButton(dic.Key);
            }
        }

        private void initFormUI()
        {
            //nsumTab.Hide();
            //customCalTab.Hide();
            //crossSectionTab.Hide();
            tabControl.TabPages.Remove(nsumTab);
            tabControl.TabPages.Remove(customCalTab);
            tabControl.TabPages.Remove(crossSectionTab);
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
            ButtonItem item = (ButtonItem)sender;
            record.Clear();
            record[1] = item.Name;
            pagesize = 1000;
            pageList[curTabName] = 1;
            searchTab(curTabName);
        }

        private void industryItem_Click(object sender, EventArgs e)
        {
            ButtonItem item = (ButtonItem)sender;
            record.Clear();
            record[2] = item.Name;

            pagesize = 1000;
            pageList[curTabName] = 1;
            searchTab(curTabName);
        }

        private void upItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Name.Equals("customCalTab") || tabControl.SelectedTab.Name.Equals("crossSectionTab"))//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[3] = item.Name;

            pagesize = 1000;

            searchTab(curTabName);
        }

        private void downItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab.Name.Equals("customCalTab") || tabControl.SelectedTab.Name.Equals("crossSectionTab"))//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[4] = item.Name;

            pagesize = 1000;

            searchTab(curTabName);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            pagesize = 1000;
            pageList[curTabName] = 1;
            searchTab(curTabName);
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
            if (curGroupName == null || curGroupName.Equals(""))
            {
                MessageBox.Show("请选择一个分组");
                return;
            }
            string selectedName = curGroupName.ToString();
            SNListDialog dialog = new SNListDialog(pList, selectedName, stockDs);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = new ArrayList(dialog.SelectStocks);
            pList[name] = stocks;

            ButtonItem item = new ButtonItem(name);
            groupButtonItemClicked(item, e);
            saveGroup();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curGroupName == null || curGroupName.Equals(""))
            {
                MessageBox.Show("请选择一个分组");
                return;
            }
            string selectedName = curGroupName.ToString();
            pList.Remove(selectedName);
            groupList.Items.Clear();
            myGroups.SubItems.Remove(selectedName);
            saveGroup();
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
                stop = Commons.GetStockDayInfo(stocks, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                totalPageList[curTabName] = totalPages;
            }
            else //版块
            {

                stop = Commons.GetStockDayInfoBoard(record, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                totalPageList[curTabName] = totalPages;
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
                        MessageBox.Show("连接超时，请检查网络");
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
                        MessageBox.Show("连接网络错误");
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
            if (orgDs != null && orgDs.Tables["stock_day_info"] != null && pageList[curTabName] != 1)
            {
                orgDs.Tables["stock_day_info"].Merge(dt);
            }
            else 
            {
                rawDataGrid.DataSource = ds;
                rawDataGrid.DataMember = "stock_day_info";
            }
            pageLabel.Text = pageList[curTabName] + "/" + (int)totalPageList[curTabName];

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
                if (sortColumnList[curTabName] == rawDataGrid.Columns[k].DataPropertyName)
                {
                    sortIndex = k;
                }
                k++;
            }

            rawDataGrid.Columns[sortIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabName] ? SortOrder.Ascending : SortOrder.Descending;

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
            if ((int)totalPageList[curTabName] == pageList[curTabName])
            {
                MessageBox.Show("已是尾页");
                return;
            }
            pageList[curTabName]++;
            pagesize = 1000;

            searchTab(curTabName);
        }

        private void allButton_Click(object sender, EventArgs e)
        {
            pageList[curTabName] = 1;
            pagesize = 10000;
            totalPageList[curTabName] = 1;

            searchTab(curTabName);
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

                stop = Commons.GetNDaysSum(stocks, type, Convert.ToInt32(args[3]), args[4], args[5], sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
            }
            else //版块
            {
                stop = Commons.GetNDaysSumBoard(record, type, Convert.ToInt32(args[2]), args[3], args[4], sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
            }
            totalPageList[curTabName] = totalPages;

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
            //curTabName = tabControl.SelectedIndex;
            curTabName = tabControl.SelectedTab.Name;
            initTab(curTabName);
        }

        private void rawDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
            DataGridView gridView  = (DataGridView)sender;
            int curSortIndex = 0;
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (sortColumnList[curTabName] == gridView.Columns[i].DataPropertyName)
                {
                    curSortIndex = i;
                }
            }

            sortColumnList[curTabName] = gridView.Columns[e.ColumnIndex].DataPropertyName;
            if (curSortIndex == e.ColumnIndex)
            {
                if (gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    sortTypeList[curTabName] = false;
                }
                else
                {
                    sortTypeList[curTabName] = true;
                }
            }
            else
            {
                gridView.Columns[curSortIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabName] ? SortOrder.Ascending : SortOrder.Descending;
            Console.WriteLine("sort_name: {0}", gridView.Columns[e.ColumnIndex].DataPropertyName);
            pageList[curTabName] = 1;
            searchTab(curTabName);
            //gridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
        }
        private void buttonItem30_Click(object sender, EventArgs e)
        {
            SNListDialog dialog = new SNListDialog(pList, "", stockDs);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                return;
            }

            string name = dialog.GroupName;
            ArrayList stocks = new ArrayList(dialog.SelectStocks);
            pList.Add(name, stocks);

            addNewGroupButton(name);
            saveGroup();
            //groupList.Items.Add(name);
        }

        private void addNewGroupButton(string name)
        {
            ButtonItem tempButtonItem = new ButtonItem();
            tempButtonItem.Name = name;
            tempButtonItem.Text = name;
            tempButtonItem.Click += new EventHandler(groupButtonItemClicked);
            myGroups.SubItems.Add(tempButtonItem);
        }

        private void groupButtonItemClicked(object sender, EventArgs e)
        {
            groupList.Items.Clear();

            ButtonItem item = (ButtonItem)sender;
            ArrayList selectStocks = pList[item.Name];
            int k = selectStocks.Count;

            dt = Commons.classfiDt;
            for (int i = 0; i < k; ++i)
            {
                String id = selectStocks[i].ToString();

                DataRow[] drs = dt.Select("stockid = '" + id + "'");

                groupList.Items.Add(id + " : " + drs[0]["stockname"].ToString());
            }
            curGroupName = item.Name;
        }

        private void myGroups_Click(object sender, EventArgs e)
        {
            groupList.Show();
        }

        private void initNDaySumTab(int code)
        {
            switch (code)
            {
                case MONTH_SUM:
                    nsumTab.Text = "月和";
                    groupBox4.Text = "月和筛选";
                    groupBox5.Text = "月和数据";
                    daySumButton.Checked = false;
                    weekSumButton.Checked = false;
                    monthSumButton.Checked = true;
                    break;
                case WEEK_SUM:
                    nsumTab.Text = "周和";
                    groupBox4.Text = "周和筛选";
                    groupBox5.Text = "周和数据";
                    daySumButton.Checked = false;
                    weekSumButton.Checked = true;
                    monthSumButton.Checked = false;
                    break;
                case DAY_SUM:
                    nsumTab.Text = "日和";
                    groupBox4.Text = "日和筛选";
                    groupBox5.Text = "日和数据";
                    daySumButton.Checked = true;
                    weekSumButton.Checked = false;
                    monthSumButton.Checked = false;
                    break;
                default:
                    break;
            }
        }

        private void monthSum_Click(object sender, EventArgs e)
        {
            showThisTab(nsumTab);
            initNDaySumTab(MONTH_SUM);
        }

        private void weekSum_Click(object sender, EventArgs e)
        {
            showThisTab(nsumTab);
            initNDaySumTab(WEEK_SUM);
        }

        private void daySum_Click(object sender, EventArgs e)
        {
            showThisTab(nsumTab);
            initNDaySumTab(DAY_SUM);
        }

        private void nDaySumCal_Click(object sender, EventArgs e)
        {
            showThisTab(nsumTab);
        }

        private void customCal_Click(object sender, EventArgs e)
        {
            showThisTab(customCalTab);
        }

        private void crossSectionCal_Click(object sender, EventArgs e)
        {
            showThisTab(crossSectionTab);
        }


        public const int MONTH_SUM = 0001;
        public const int WEEK_SUM = 0002;
        public const int DAY_SUM = 0003;

        private void sectionResultGrid_Sorted(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = sectionResultGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                sectionResultGrid.Rows[i].HeaderCell.Value = j.ToString();

                if (sectionResultGrid.Rows[i].Cells["cross_type"].Value.ToString() == "positive")
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (sectionResultGrid.Rows[i].Cells["cross_type"].Value.ToString() == "negative")
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                }
                else
                {
                    sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }

            }
        }

    }    

}
