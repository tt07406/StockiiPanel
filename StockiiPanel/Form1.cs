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
        private object totalPages;//总页数
        private int page;//第几页
        private int pagesize;//页大小

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
            compareCombo.Items.Add("指定时间段内最大值比最小值");
            compareCombo.Items.Add("指定两天加");
            compareCombo.Items.Add("指定时间段内最大值减最小值");
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
            //反序列化载入分组列表
            using (FileStream fileStream = new FileStream("group.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                this.pList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
            }

            foreach (var dic in pList)
            {
                groupList.Items.Add(dic.Key);
            }
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
                            ssdc.Click += new EventHandler(upItem_Click);
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
                            ssdc.Click += new EventHandler(downItem_Click);
                        }
                    }
                }
            }
        }

        private void sectionItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[1] = item.Name;

            searchTab(tabControl.SelectedIndex);
        }

        private void industryItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[2] = item.Name;

            searchTab(tabControl.SelectedIndex);
        }

        private void upItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[3] = item.Name;

            searchTab(tabControl.SelectedIndex);
        }

        private void downItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex > 1)//向上版块和向下版块只针对原始数据和n日和有效
                return;

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            record.Clear();
            record[4] = item.Name;

            searchTab(tabControl.SelectedIndex);
        }

        private void searchTab(int type)
        {
            if (!Commons.isTradeDay(startDatePicker.Value.ToString("yyyy-MM-dd")) || !Commons.isTradeDay(endDatePicker.Value.ToString("yyyy-MM-dd")))
            {
                return;
            }

            if (groupList.Visible == true && groupList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择一个分组");
                return;
            }
            else if (groupList.Visible == false && record.Count == 0)
            {
                MessageBox.Show("请选择一个版块");
                return;
            }

            // 启动Loading线程
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFun));
            thread.Start();

            //启动后台线程
            stop = false;
            pageLabel.Text = "0/0";

            page = 1;
            pagesize = 10000;
            totalPages = 1;
            String args;
            if (groupList.Visible)//自选
            {
                args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + "," + groupList.SelectedItem.ToString();
            }
            else
            {
                args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + ",";
            }

            switch (type)
            {
                case 0:
                    bkWorker.RunWorkerAsync(args);
                    break;
                case 1:
                    sumWorker.RunWorkerAsync(args);
                    break;
                case 2:
                    customWorker.RunWorkerAsync(args);
                    break;
                case 3:
                    crossWorker.RunWorkerAsync(args);
                    break;
                default:
                    break;
            }
            
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            searchTab(0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void endDatePicker2_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker2.Value;
            endDatePicker.Value = endDatePicker2.Value;
            endDatePicker3.Value = endDatePicker2.Value;
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

        private SerializableDictionary<String, ArrayList> pList;

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
            object error = errorNo;//装箱

            if (args[2] != string.Empty)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);
                stop = Commons.GetStockDayInfo(stocks, "", true, startDate, endDate, page, pagesize, error, ds, totalPages);
            }
            else //版块
            {
                stop = Commons.GetStockDayInfoBoard(record, "", true, startDate, endDate, page, pagesize, error, ds, totalPages);
            }

            errorNo = (int)error;//拆箱
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

            dt.Columns.Add("cir_of_cap_stock_tmp", typeof(decimal));
            dt.Columns.Add("total_money_tmp", typeof(decimal));
            foreach (DataRow Row in dt.Rows)
            {
                Row["cir_of_cap_stock_tmp"] = Math.Round(Convert.ToDouble(Row["cir_of_cap_stock"].ToString()) / 10000, 4);
                Row["total_money_tmp"] = Math.Round(Convert.ToDouble(Row["total_money"].ToString()) / 100000000, 4);
                Row["total_stock"] = Math.Round(Convert.ToDouble(Row["total_stock"].ToString()) / 10000, 4);
            }

            pageLabel.Text = page + "/" + (int)totalPages;

            rawDataGrid.DataSource = ds;
            rawDataGrid.DataMember = "stock_day_info";

            rawDataGrid.AllowUserToAddRows = false;//不显示最后空白行
            rawDataGrid.EnableHeadersVisualStyles = false;

            //改变DataGridView的表头
            rawDataGrid.Columns[0].HeaderText = "代码";
            rawDataGrid.Columns[0].HeaderCell.Style.ForeColor = Color.Red;
            //设置该列宽度
            rawDataGrid.Columns[0].Width = 70;
            rawDataGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            //rawDataGrid.Columns[0].Frozen = true;

            rawDataGrid.Columns[1].HeaderText = "名称";
            rawDataGrid.Columns[1].Width = 80;
            rawDataGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            //rawDataGrid.Columns[1].Frozen = true;

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

            for (int i = Commons.colNum; i < rawDataGrid.Columns.Count; ++i)
            {
                rawDataGrid.Columns[i].Visible = false;
            }

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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //序列化保存
            using (FileStream fileStream = new FileStream("group.xml", FileMode.Create))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                xmlFormatter.Serialize(fileStream, this.pList);
            }
        }

        private void saveTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    dt = Commons.StructrueDataTable(rawDataGrid, false);
                    break;
                case 1:
                    dt = Commons.StructrueDataTable(ndayGrid, false);
                    break;
                case 2:
                    dt = Commons.StructrueDataTable(calResultGrid, false);
                    break;
                case 3:
                    dt = Commons.StructrueDataTable(sectionResultGrid, false);
                    break;
                default:
                    break;
            }
            
            Commons.ExportDataGridToCSV(dt);
        }

        private void saveSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    dt = Commons.StructrueDataTable(rawDataGrid, true);
                    break;
                case 1:
                    dt = Commons.StructrueDataTable(ndayGrid, true);
                    break;
                case 2:
                    dt = Commons.StructrueDataTable(calResultGrid, true);
                    break;
                case 3:
                    dt = Commons.StructrueDataTable(sectionResultGrid, true);
                    break;
                default:
                    break;
            }

            Commons.ExportDataGridToCSV(dt);
        }

        private void moreButton_Click(object sender, EventArgs e)
        {
            if (!Commons.isTradeDay(startDatePicker.Value.ToString("yyyy-MM-dd")) || !Commons.isTradeDay(endDatePicker.Value.ToString("yyyy-MM-dd")))
            {
                return;
            }

            if ((int)totalPages == page)
            {
                MessageBox.Show("已是尾页");
                return;
            }

            if (groupList.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择一个分组");
                return;
            }

            // 启动Loading线程
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFun));
            thread.Start();

            //启动后台线程
            stop = false;
            pageLabel.Text = "0/0";

            page = 2;
            pagesize = 10000;
            totalPages = 2;
            String args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + "," + groupList.SelectedItem.ToString();
            bkWorker.RunWorkerAsync(args);
        }

        private void allButton_Click(object sender, EventArgs e)
        {
            if (groupList.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择一个分组");
                return;
            }

            // 启动Loading线程
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFun));
            thread.Start();

            //启动后台线程
            stop = false;
            pageLabel.Text = "0/0";

            page = 1;
            pagesize = 100000;
            totalPages = 1;
            String args = startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd") + "," + groupList.SelectedItem.ToString();
            bkWorker.RunWorkerAsync(args);
        }

        private void startDatePicker_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker.Value;
            startDatePicker2.Value = startDatePicker.Value;
            startDatePicker3.Value = startDatePicker.Value;
        }

        private void rawContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    if (rawDataGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    combinePageToolStripMenuItem.Visible = false;
                    combineSelectToolStripMenuItem.Visible = false;
                    break;
                case 1:
                    if (ndayGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    combinePageToolStripMenuItem.Visible = false;
                    combineSelectToolStripMenuItem.Visible = false;
                    break;
                case 2:
                    if (calResultGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    break;
                case 3:
                    if (sectionResultGrid.RowCount > 0)
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = true;
                    }
                    else
                    {
                        for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                            rawContextMenuStrip.Items[i].Visible = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void endDatePicker_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker.Value;
            endDatePicker2.Value = endDatePicker.Value;
            endDatePicker3.Value = endDatePicker.Value;
        }

        private void startDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker.Value = startDatePicker1.Value;
            startDatePicker2.Value = startDatePicker1.Value;
            startDatePicker3.Value = startDatePicker1.Value;
        }

        private void endDatePicker1_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker.Value = endDatePicker1.Value;
            endDatePicker2.Value = endDatePicker1.Value;
            endDatePicker3.Value = endDatePicker1.Value;
        }

        private void startDatePicker2_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker2.Value;
            startDatePicker.Value = startDatePicker2.Value;
            startDatePicker3.Value = startDatePicker2.Value;
        }

        private void startDatePicker3_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker3.Value;
            startDatePicker2.Value = startDatePicker3.Value;
            startDatePicker.Value = startDatePicker3.Value;
        }

        private void endDatePicker3_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker3.Value;
            endDatePicker2.Value = endDatePicker3.Value;
            endDatePicker.Value = endDatePicker3.Value;
        }

        private void searchButton1_Click(object sender, EventArgs e)
        {
            searchTab(1);
        }

        private void sumWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();
            object error = errorNo;//装箱
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

            if (args[2] != string.Empty)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);
                
                stop = Commons.GetNDaysSum(stocks, type, Convert.ToInt32(intervalCombo.Text), indexCombo.Text, typeCombo.Text, "", true, startDate, endDate, page, pagesize, error, ds, totalPages);
            }
            else //版块
            {
                stop = Commons.GetNDaysSumBoard(record, type, Convert.ToInt32(intervalCombo.Text), indexCombo.Text, typeCombo.Text, "", true, startDate, endDate, page, pagesize, error, ds, totalPages);
            }

            errorNo = (int)error;//拆箱
        }

        private void customWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();
            object error = errorNo;//装箱

            if (args[2] != string.Empty)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);

                stop = Commons.GetStockDaysDiff(stocks, Convert.ToDouble(smallBox.Text), Convert.ToDouble(bigBox.Text), compareIndexCombo.Text, compareCombo.Text, startDate, endDate, error, ds);
            }
            else //版块
            {
                stop = Commons.GetStockDaysDiffBoard(record, Convert.ToDouble(smallBox.Text), Convert.ToDouble(bigBox.Text), compareIndexCombo.Text, compareCombo.Text, startDate, endDate, error, ds);
            }

            errorNo = (int)error;//拆箱
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            searchTab(2);
        }

        private void crossWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String startDate = args[0];
            String endDate = args[1];

            ds = new DataSet();
            object error = errorNo;//装箱

            if (args[2] != string.Empty)//自选
            {
                String name = args[2];//取选中的分组
                ArrayList stocks = new ArrayList(pList[name]);

                stop = Commons.GetCrossInfoCmd(Convert.ToDouble(weighBox.Text), indexCombox1.Text, startDate, endDate, error, ds);
            }
            else //版块
            {
                stop = Commons.GetCrossInfoCmdBoard(record,Convert.ToDouble(weighBox.Text), indexCombox1.Text, startDate, endDate, error, ds);
            }

            errorNo = (int)error;//拆箱
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
