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
        private int totalPages;//总页数
        private int page;//第几页
        private int pagesize;//页大小

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
            Commons.GetTradeDate();

            Commons.GetStockClassification(sectionToolStripMenuItem, industryToolStripMenuItem);
            stockDs = Commons.GetStockBasicInfo();

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
            page = 1;
            pagesize = 10000;
            totalPages = 1;

            searchTab(tabControl.SelectedIndex);
        }

        private void industryItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[2] = item.Name;

            page = 1;
            pagesize = 10000;
            totalPages = 1;

            searchTab(tabControl.SelectedIndex);
        }

        private void upItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[3] = item.Name;

            page = 1;
            pagesize = 10000;
            totalPages = 1;

            searchTab(tabControl.SelectedIndex);
        }

        private void downItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[4] = item.Name;

            page = 1;
            pagesize = 10000;
            totalPages = 1;

            searchTab(tabControl.SelectedIndex);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 10000;
            totalPages = 1;
            searchTab(0);
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

            if (args.Length == 3)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);
                stop = Commons.GetStockDayInfo(stocks, "", true, startDate, endDate, page, pagesize,out errorNo, out ds,out totalPages);
            }
            else //版块
            {
                stop = Commons.GetStockDayInfoBoard(record, "", true, startDate, endDate, page, pagesize, out errorNo, out ds, out totalPages);
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

           
            pageLabel.Text = page + "/" + (int)totalPages;

            rawDataGrid.DataSource = ds;
            rawDataGrid.DataMember = "stock_day_info";

            rawDataGrid.AllowUserToAddRows = false;//不显示最后空白行
            rawDataGrid.EnableHeadersVisualStyles = false;

            int k = 0;
            foreach (var item in rawDict)
            {
                rawDataGrid.Columns[k].HeaderText = item.Value;
                rawDataGrid.Columns[k].Width = 60 + item.Value.Length * 10;
                rawDataGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns[item.Key].ToString();

                switch (item.Key)
                {
                    case "stock_id":
                    case "created":
                    case "turnover_ratio":
                    case "volume_ratio":
                    case "growth_ratio":
                    case "avg_price":
                    case "amplitude_ratio":
                        rawDataGrid.Columns[k].HeaderCell.Style.ForeColor = Color.Red;
                        break;
                    case "total_money":
                        rawDataGrid.Columns[k].HeaderCell.Style.ForeColor = Color.Red;
                        rawDataGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns["total_money_tmp"].ToString();
                        break;
                    case "cir_of_cap_stock":
                        rawDataGrid.Columns[k].HeaderCell.Style.ForeColor = Color.Red;
                        rawDataGrid.Columns[k].DataPropertyName = ds.Tables[0].Columns["cir_of_cap_stock_tmp"].ToString();
                        break;
                    default:
                        break;
                }
                k++;
            }
            /*
            //改变DataGridView的表头
            rawDataGrid.Columns[0].HeaderText = "代码";
            rawDataGrid.Columns[0].HeaderCell.Style.ForeColor = Color.Red;
            //设置该列宽度
            rawDataGrid.Columns[0].Width = 70;
            rawDataGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();

            rawDataGrid.Columns[1].HeaderText = "名称";
            rawDataGrid.Columns[1].Width = 80;
            rawDataGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();

            rawDataGrid.Columns[2].HeaderText = "日期";
            rawDataGrid.Columns[2].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[2].Width = 80;
            rawDataGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["created"].ToString();

            rawDataGrid.Columns[3].HeaderText = "多头获利（元）";
            rawDataGrid.Columns[3].Width = 120;
            rawDataGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["bull_profit"].ToString();

            rawDataGrid.Columns[4].HeaderText = "多空平衡（元）";
            rawDataGrid.Columns[4].Width = 120;
            rawDataGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["bbi_balance"].ToString();

            rawDataGrid.Columns[5].HeaderText = "卖价二（元）";
            rawDataGrid.Columns[5].Width = 110;
            rawDataGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["num2_sell_price"].ToString();

            rawDataGrid.Columns[6].HeaderText = "现量";
            rawDataGrid.Columns[6].Width = 80;
            rawDataGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["last_deal_amount"].ToString();

            rawDataGrid.Columns[7].HeaderText = "买量三";
            rawDataGrid.Columns[7].Width = 90;
            rawDataGrid.Columns[7].DataPropertyName = ds.Tables[0].Columns["num3_buy"].ToString();

            rawDataGrid.Columns[8].HeaderText = "卖量三";
            rawDataGrid.Columns[8].Width = 90;
            rawDataGrid.Columns[8].DataPropertyName = ds.Tables[0].Columns["num3_sell"].ToString();

            rawDataGrid.Columns[9].HeaderText = "空头回补";
            rawDataGrid.Columns[9].Width = 100;
            rawDataGrid.Columns[9].DataPropertyName = ds.Tables[0].Columns["short_covering"].ToString();

            rawDataGrid.Columns[10].HeaderText = "空头止损";
            rawDataGrid.Columns[10].Width = 100;
            rawDataGrid.Columns[10].DataPropertyName = ds.Tables[0].Columns["bear_stop_losses"].ToString();

            rawDataGrid.Columns[11].HeaderText = "现价（元）";
            rawDataGrid.Columns[11].Width = 90;
            rawDataGrid.Columns[11].DataPropertyName = ds.Tables[0].Columns["current_price"].ToString();

            rawDataGrid.Columns[12].HeaderText = "换手（%）";
            rawDataGrid.Columns[12].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[12].Width = 90;
            rawDataGrid.Columns[12].DataPropertyName = ds.Tables[0].Columns["turnover_ratio"].ToString();

            rawDataGrid.Columns[13].HeaderText = "买量二";
            rawDataGrid.Columns[13].Width = 90;
            rawDataGrid.Columns[13].DataPropertyName = ds.Tables[0].Columns["num2_buy"].ToString();

            rawDataGrid.Columns[14].HeaderText = "流通股本（亿股）";
            rawDataGrid.Columns[14].Width = 130;
            rawDataGrid.Columns[14].DataPropertyName = ds.Tables[0].Columns["cir_of_cap_stock_tmp"].ToString();


            rawDataGrid.Columns[15].HeaderText = "内盘";
            rawDataGrid.Columns[15].Width = 80;
            rawDataGrid.Columns[15].DataPropertyName = ds.Tables[0].Columns["sell"].ToString();

            rawDataGrid.Columns[16].HeaderText = "更新日期";
            rawDataGrid.Columns[16].Width = 120;
            rawDataGrid.Columns[16].DataPropertyName = ds.Tables[0].Columns["update_date"].ToString();

            rawDataGrid.Columns[17].HeaderText = "最低价流通市值（亿元）";
            rawDataGrid.Columns[17].Width = 200;
            rawDataGrid.Columns[17].DataPropertyName = ds.Tables[0].Columns["min_circulation_value"].ToString();

            rawDataGrid.Columns[18].HeaderText = "强弱度";
            rawDataGrid.Columns[18].Width = 90;
            rawDataGrid.Columns[18].DataPropertyName = ds.Tables[0].Columns["relative_strength_index"].ToString();

            rawDataGrid.Columns[19].HeaderText = "最低（元）";
            rawDataGrid.Columns[19].Width = 90;
            rawDataGrid.Columns[19].DataPropertyName = ds.Tables[0].Columns["min"].ToString();

            rawDataGrid.Columns[20].HeaderText = "市盈率";
            rawDataGrid.Columns[20].Width = 90;
            rawDataGrid.Columns[20].DataPropertyName = ds.Tables[0].Columns["pe_ratio"].ToString();

            rawDataGrid.Columns[21].HeaderText = "委比";
            rawDataGrid.Columns[21].Width = 80;
            rawDataGrid.Columns[21].DataPropertyName = ds.Tables[0].Columns["DaPanWeiBi"].ToString();

            rawDataGrid.Columns[22].HeaderText = "卖出价（元）";
            rawDataGrid.Columns[22].Width = 110;
            rawDataGrid.Columns[22].DataPropertyName = ds.Tables[0].Columns["sold_price"].ToString();

            rawDataGrid.Columns[23].HeaderText = "今开（元）";
            rawDataGrid.Columns[23].Width = 100;
            rawDataGrid.Columns[23].DataPropertyName = ds.Tables[0].Columns["today_begin_price"].ToString();

            rawDataGrid.Columns[24].HeaderText = "买入价（元）";
            rawDataGrid.Columns[24].Width = 110;
            rawDataGrid.Columns[24].DataPropertyName = ds.Tables[0].Columns["bought_price"].ToString();

            rawDataGrid.Columns[25].HeaderText = "笔涨跌";
            rawDataGrid.Columns[25].Width = 90;
            rawDataGrid.Columns[25].DataPropertyName = ds.Tables[0].Columns["upordown_per_deal"].ToString();

            rawDataGrid.Columns[26].HeaderText = "卖量一";
            rawDataGrid.Columns[26].Width = 90;
            rawDataGrid.Columns[26].DataPropertyName = ds.Tables[0].Columns["num1_sell"].ToString();

            rawDataGrid.Columns[27].HeaderText = "流通市值（亿元）";
            rawDataGrid.Columns[27].Width = 160;
            rawDataGrid.Columns[27].DataPropertyName = ds.Tables[0].Columns["circulation_value"].ToString();

            rawDataGrid.Columns[28].HeaderText = "日涨跌（元）";
            rawDataGrid.Columns[28].Width = 110;
            rawDataGrid.Columns[28].DataPropertyName = ds.Tables[0].Columns["daily_up_down"].ToString();

            rawDataGrid.Columns[29].HeaderText = "涨速（%）";
            rawDataGrid.Columns[29].Width = 100;
            rawDataGrid.Columns[29].DataPropertyName = ds.Tables[0].Columns["growth_speed"].ToString();

            rawDataGrid.Columns[30].HeaderText = "最高（元）";
            rawDataGrid.Columns[30].Width = 100;
            rawDataGrid.Columns[30].DataPropertyName = ds.Tables[0].Columns["max"].ToString();

            rawDataGrid.Columns[31].HeaderText = "买量一";
            rawDataGrid.Columns[31].Width = 90;
            rawDataGrid.Columns[31].DataPropertyName = ds.Tables[0].Columns["num1_buy"].ToString();

            rawDataGrid.Columns[32].HeaderText = "量比";
            rawDataGrid.Columns[32].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[32].Width = 80;
            rawDataGrid.Columns[32].DataPropertyName = ds.Tables[0].Columns["volume_ratio"].ToString();

            rawDataGrid.Columns[33].HeaderText = "尾量差";
            rawDataGrid.Columns[33].Width = 90;
            rawDataGrid.Columns[33].DataPropertyName = ds.Tables[0].Columns["DaPanWeiCha"].ToString();

            rawDataGrid.Columns[34].HeaderText = "外盘";
            rawDataGrid.Columns[34].Width = 80;
            rawDataGrid.Columns[34].DataPropertyName = ds.Tables[0].Columns["buy"].ToString();

            rawDataGrid.Columns[35].HeaderText = "卖量二";
            rawDataGrid.Columns[35].Width = 90;
            rawDataGrid.Columns[35].DataPropertyName = ds.Tables[0].Columns["num2_sell"].ToString();

            rawDataGrid.Columns[36].HeaderText = "每笔均量";
            rawDataGrid.Columns[36].Width = 100;
            rawDataGrid.Columns[36].DataPropertyName = ds.Tables[0].Columns["num_per_deal"].ToString();

            rawDataGrid.Columns[37].HeaderText = "总市值（亿元）";
            rawDataGrid.Columns[37].Width = 120;
            rawDataGrid.Columns[37].DataPropertyName = ds.Tables[0].Columns["total_value"].ToString();

            rawDataGrid.Columns[38].HeaderText = "最高价流通市值（亿元）";
            rawDataGrid.Columns[38].Width = 200;
            rawDataGrid.Columns[38].DataPropertyName = ds.Tables[0].Columns["max_circulation_value"].ToString();

            rawDataGrid.Columns[39].HeaderText = "内外比";
            rawDataGrid.Columns[39].Width = 90;
            rawDataGrid.Columns[39].DataPropertyName = ds.Tables[0].Columns["sb_ratio"].ToString();

            rawDataGrid.Columns[40].HeaderText = "涨幅（%）";
            rawDataGrid.Columns[40].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[40].Width = 100;
            rawDataGrid.Columns[40].DataPropertyName = ds.Tables[0].Columns["growth_ratio"].ToString();

            rawDataGrid.Columns[41].HeaderText = "均价（元）";
            rawDataGrid.Columns[41].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[41].Width = 100;
            rawDataGrid.Columns[41].DataPropertyName = ds.Tables[0].Columns["avg_price"].ToString();

            rawDataGrid.Columns[42].HeaderText = "昨收（元）";
            rawDataGrid.Columns[42].Width = 100;
            rawDataGrid.Columns[42].DataPropertyName = ds.Tables[0].Columns["ytd_end_price"].ToString();

            rawDataGrid.Columns[43].HeaderText = "总金额（亿元）";
            rawDataGrid.Columns[43].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[43].Width = 120;
            rawDataGrid.Columns[43].DataPropertyName = ds.Tables[0].Columns["total_money_tmp"].ToString();

            rawDataGrid.Columns[44].HeaderText = "买价二（元）";
            rawDataGrid.Columns[44].Width = 110;
            rawDataGrid.Columns[44].DataPropertyName = ds.Tables[0].Columns["num2_buy_price"].ToString();

            rawDataGrid.Columns[45].HeaderText = "振幅";
            rawDataGrid.Columns[45].HeaderCell.Style.ForeColor = Color.Red;
            rawDataGrid.Columns[45].Width = 80;
            rawDataGrid.Columns[45].DataPropertyName = ds.Tables[0].Columns["amplitude_ratio"].ToString();

            rawDataGrid.Columns[46].HeaderText = "总量";
            rawDataGrid.Columns[46].Width = 80;
            rawDataGrid.Columns[46].DataPropertyName = ds.Tables[0].Columns["total_deal_amount"].ToString();

            rawDataGrid.Columns[47].HeaderText = "现价流通市值（亿元）";
            rawDataGrid.Columns[47].Width = 200;
            rawDataGrid.Columns[47].DataPropertyName = ds.Tables[0].Columns["current_circulation_value"].ToString();

            rawDataGrid.Columns[48].HeaderText = "总股本（亿股）";
            rawDataGrid.Columns[48].Width = 120;
            rawDataGrid.Columns[48].DataPropertyName = ds.Tables[0].Columns["total_stock"].ToString();

            rawDataGrid.Columns[49].HeaderText = "均价流通市值（亿元）";
            rawDataGrid.Columns[49].Width = 200;
            rawDataGrid.Columns[49].DataPropertyName = ds.Tables[0].Columns["avg_circulation_value"].ToString();

            rawDataGrid.Columns[50].HeaderText = "多头止损（元）";
            rawDataGrid.Columns[50].Width = 120;
            rawDataGrid.Columns[50].DataPropertyName = ds.Tables[0].Columns["bull_stop_losses"].ToString();

            rawDataGrid.Columns[51].HeaderText = "卖价一（元）";
            rawDataGrid.Columns[51].Width = 110;
            rawDataGrid.Columns[51].DataPropertyName = ds.Tables[0].Columns["num1_sell_price"].ToString();

            rawDataGrid.Columns[52].HeaderText = "每笔换手";
            rawDataGrid.Columns[52].Width = 100;
            rawDataGrid.Columns[52].DataPropertyName = ds.Tables[0].Columns["turn_per_deal"].ToString();

            rawDataGrid.Columns[53].HeaderText = "活跃度";
            rawDataGrid.Columns[53].Width = 90;
            rawDataGrid.Columns[53].DataPropertyName = ds.Tables[0].Columns["activity"].ToString();

            rawDataGrid.Columns[54].HeaderText = "买价一（元）";
            rawDataGrid.Columns[54].Width = 110;
            rawDataGrid.Columns[54].DataPropertyName = ds.Tables[0].Columns["num1_buy_price"].ToString();

            rawDataGrid.Columns[55].HeaderText = "买价三（元）";
            rawDataGrid.Columns[55].Width = 110;
            rawDataGrid.Columns[55].DataPropertyName = ds.Tables[0].Columns["num3_buy_price"].ToString();

            rawDataGrid.Columns[56].HeaderText = "卖价三（元）";
            rawDataGrid.Columns[56].Width = 110;
            rawDataGrid.Columns[56].DataPropertyName = ds.Tables[0].Columns["num3_sell_price"].ToString();
            */
            for (int i = rawDict.Count; i < rawDataGrid.Columns.Count; ++i)
            {
                rawDataGrid.Columns[i].Visible = false;
            }
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
            if ((int)totalPages == page)
            {
                MessageBox.Show("已是尾页");
                return;
            }

            page = 2;
            pagesize = 10000;
            totalPages = 2;

            searchTab(0);
        }

        private void allButton_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 100000;
            totalPages = 1;

            searchTab(0);
        }

 

        private void searchButton1_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 10000;
            totalPages = 1;

            searchTab(1);
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

            if (args.Length == 6 )//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);

                stop = Commons.GetNDaysSum(stocks, type, Convert.ToInt32(args[3]), args[4], args[5], "", true, startDate, endDate, page, pagesize, out errorNo, out ds, out totalPages);
            }
            else //版块
            {
                stop = Commons.GetNDaysSumBoard(record, type, Convert.ToInt32(args[2]), args[3], args[4], "", true, startDate, endDate, page, pagesize, out errorNo, out ds, out totalPages);
            }

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

        private void calculateButton_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 10000;
            totalPages = 1;
            searchTab(2);
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

        private void moreButton1_Click(object sender, EventArgs e)
        {
            if ((int)totalPages == page)
            {
                MessageBox.Show("已是尾页");
                return;
            }

            page = 2;
            pagesize = 10000;
            totalPages = 2;

            searchTab(1);
        }

        private void allButton1_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 100000;
            totalPages = 1;

            searchTab(1);
        }

        private void moreButton3_Click(object sender, EventArgs e)
        {
            if ((int)totalPages == page)
            {
                MessageBox.Show("已是尾页");
                return;
            }

            page = 2;
            pagesize = 10000;
            totalPages = 2;

            searchTab(3);
        }

        private void allButton3_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 100000;
            totalPages = 1;

            searchTab(3);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            page = 1;
            pagesize = 10000;
            totalPages = 1;
            searchTab(3);
        }

    }    

}
