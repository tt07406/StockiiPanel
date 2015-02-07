using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Drawing;
using System.IO;
using System.Data;
using System.Collections;//在C#中使用ArrayList必须引用Collections类
using DevComponents.DotNetBar;
using System.Diagnostics;

namespace StockiiPanel
{
    public partial class Form1 : RibbonForm
    {
        ArrayList combineArray = new ArrayList();//拼接的操作序列
        ArrayList bufferArray = new ArrayList();//用于保留前一个拼接序列
        String actionName = "";
        bool isCustom = true;//是否是自选
        ArrayList boardStocks = new ArrayList();//板块选中的股票

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.TabPages.Remove(tabControl.SelectedTab);
        }


        private void upItem_MouseDown(object sender, EventArgs e)
        {
            //groupList.Show();     
            //boardMenuStrip.Show(this, boardBar.Location.X - upContainer.Size.Width - downContainer.Size.Width, upContainer.Size.Height + ribbonPanel3.Location.Y);
            //sectionToolStripMenuItem.Visible = false;
            //industryToolStripMenuItem.Visible = false;
            //upToolStripMenuItem.Visible = false;
            //downToolStripMenuItem.Visible = false;
            
            //upToolStripMenuItem.ShowDropDown();
            upMenuStrip.Show(this, boardBar.Location.X - upContainer.Size.Width - downContainer.Size.Width, upContainer.Size.Height + ribbonPanel3.Location.Y);

        }

        private void downItem_MouseDown(object sender, EventArgs e)
        {
            //groupList.Show();
            //boardMenuStrip.Show(this, boardBar.Location.X - downContainer.Size.Width, downContainer.Size.Height + ribbonPanel3.Location.Y);
            //sectionToolStripMenuItem.Visible = false;
            //industryToolStripMenuItem.Visible = false;
            //upToolStripMenuItem.Visible = false;
            //downToolStripMenuItem.Visible = false;
            
            //downToolStripMenuItem.ShowDropDown();
            downMenuStrip.Show(this, boardBar.Location.X - downContainer.Size.Width, downContainer.Size.Height + ribbonPanel3.Location.Y);
        }


        private void showThisTab(TabPage tab)
        {
            if (!tabControl.Controls.Contains(tab))
            {
                tabControl.Controls.Add(tab);
                tabControl.SelectTab(tab);
                if (tabControl.TabCount == 1)
                {
                    curTabName = tabControl.SelectedTab.Name;
                    initTab(curTabName);
                    if (curTabName.Equals("rawDataTab") || curTabName.Equals("nsumTab"))//向上版块和向下版块只针对原始数据和n日和有效
                        boardBar.Visible = true;
                    else
                        boardBar.Visible = false;
                }
            }
            else
            {
                tabControl.SelectTab(tab);
            }

        }

        private void stocksInfo_Click(object sender, EventArgs e)
        {
            showThisTab(rawDataTab);
        }

        private void maketInfo_Click(object sender, EventArgs e)
        {
            //showThisTab(marketTab);
        }

        private void dumpCurPage_Click(object sender, EventArgs e)
        {
            dt = Commons.StructrueDataTable(combineResult, false);
            Commons.ExportDataGridToCSV(dt);
        }
        private void dumpCurSelected_Click(object sender, EventArgs e)
        {
            dt = Commons.StructrueDataTable(combineResult, true);
            Commons.ExportDataGridToCSV(dt);
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\doc\\help.chm");
        }

        private void SaveActions_Click(object sender, EventArgs e)
        {
            if (combineArray == null || combineArray.Count == 0)
            {
                MessageBox.Show("空操作");
                return;
            }

            ArrayList headers = new ArrayList();//表头
            for (int i = 0; i < combineResult.ColumnCount; i++)
                headers.Add(combineResult.Columns[i].HeaderText);

            SaveActionDialog dialog = new SaveActionDialog(combineArray, headers);
            dialog.ShowDialog(this);
        }


        private void autoCombineItem_Click(object sender, EventArgs e)
        {
            ActionDialog dialog = new ActionDialog();
            dialog.ShowDialog(this);

            if (dialog.CombineArray.Count == 0)
            {
                MessageBox.Show("空操作");
                return;
            }

            actionName = dialog.ActionName;
            this.splitContainer1.SplitterDistance = 25;
            foreach (String item in dialog.CombineArray)
            {
                if (item.StartsWith("customCal"))//如果包含自定义查询，则要选择分组或版块
                {
                    if (isCustom == true && groupList.Items.Count == 0)
                    {
                        MessageBox.Show("请选择一个分组");
                        return;
                    }
                    else if (isCustom == false && record.Count == 0)
                    {
                        MessageBox.Show("请选择一个版块");
                        return;
                    }
                }
            }

            // 启动Loading线程
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadFun));
            thread.Start();

            //启动后台线程
            stop = false;

            for (int i = 0; i < dialog.CombineArray.Count; i++)
            {
                if (isCustom && dialog.CombineArray[i].ToString().StartsWith("customCal"))
                    dialog.CombineArray[i] += "," + curGroupName.ToString();
            }

            combineWorker.RunWorkerAsync(dialog.CombineArray);
        }


        private void combineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ArrayList queryList = (ArrayList)e.Argument;
            dt = new DataTable();

            foreach (String query in queryList)
            {
                String[] args = query.Split(',');//解析参数

                ds = new DataSet();

                if (args[0].Equals("customCal"))//自定义
                {
                    String startDate = args[5];
                    String endDate = args[6];


                    if (args.Length == 8)//自选
                    {
                        String name = args[7];//取选中的分组
                        ArrayList stocks = new ArrayList(pList[name]);

                        stop = Commons.GetStockDaysDiff(stocks, Convert.ToDouble(args[1]), Convert.ToDouble(args[2]), args[3], args[4], startDate, endDate, out errorNo, out ds);
                    }
                    else //版块
                    {
                        stop = Commons.GetStockDaysDiffBoard(record, Convert.ToDouble(args[1]), Convert.ToDouble(args[2]), args[3], args[4], startDate, endDate, out errorNo, out ds);
                    }
                }
                else
                {
                    String startDate = args[3];
                    String endDate = args[4];

                    stop = Commons.GetCrossInfoCmd(Convert.ToDouble(args[1]), args[2], startDate, endDate, out errorNo, out ds);
                }

                if (stop)//查询出错
                {
                    return;
                }

                dt = Commons.CombineDt(ds, dt);
            }
        }

        private void combineWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            buffResult = (DataTable)combineResult.DataSource;//保存上一个
            headText.Clear();
            for (int i = 0; i < combineResult.ColumnCount; i++)
                headText.Add(combineResult.Columns[i].HeaderText);
            bufferArray = (ArrayList)combineArray.Clone();

            combineResult.DataSource = dt;

            //设置表头
            ArrayList combineHeaders = new ArrayList();//拼接的操作序列表头
            try
            {
                using (FileStream fileStream = new FileStream("headers.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                    SerializableDictionary<string, ArrayList> combineHeaderList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
                    if (combineHeaderList != null)
                    {
                        combineHeaders = (ArrayList)combineHeaderList[actionName].Clone();
                        Console.WriteLine(combineHeaders.Count + ":" + combineResult.ColumnCount);
                        for (int i = 0; i < combineResult.ColumnCount; i++)
                            combineResult.Columns[i].HeaderText = combineHeaders[i].ToString();
                    }
                }



                //保存查询参数
                using (FileStream fileStream = new FileStream("actions.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                    SerializableDictionary<string, ArrayList> combineList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
                    if (combineList != null)
                    {
                        combineArray = (ArrayList)combineList[actionName].Clone();
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
            stop = true;
        }


        private void newGroupItem_Click(object sender, EventArgs e)
        {
            if (calResultGrid.SelectedRows.Count == 0 && sectionResultGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择股票");
                return;
            }
            Random ro = new Random();
            string newName = curGroupName.ToString()+ ro.Next();

            //名字不重复
            while (pList.ContainsKey(newName))
            {
                newName = curGroupName.ToString() + ro.Next();
            }

            ArrayList stocks = new ArrayList();
            if (tabControl.SelectedTab.Name.Equals("customCalTab"))
            {
                for (int i = 0; i < calResultGrid.SelectedRows.Count; ++i)
                {
                    stocks.Add(calResultGrid.SelectedRows[i].Cells["stock_id"].Value.ToString());
                }
            }
            else
            {
                for (int i = 0; i < sectionResultGrid.SelectedRows.Count; ++i)
                {
                    stocks.Add(sectionResultGrid.SelectedRows[i].Cells["stock_id"].Value.ToString());
                }
            }
            pList.Add(newName, stocks);

            SNListDialog dialog = new SNListDialog(pList, newName, stockDs);
            dialog.ShowDialog(this);

            pList.Remove(newName);
            if (!dialog.IsSuccess)
            {              
                return;
            }

            string name = dialog.GroupName;
            stocks = new ArrayList(dialog.SelectStocks);
            addNewGroupButton(name);
            pList[name] = stocks;

            isCustom = true;
            groupLabel.Text = "自选股票：";
            ButtonItem item = new ButtonItem(name);
            groupButtonItemClicked(item, e);
            saveGroup();
        }


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

        private void manageGroup_Click(object sender, EventArgs e)
        {
            //groupList.Show();
            

            GroupDialog dialog = new GroupDialog(pList, stockDs);
            dialog.ShowDialog(this);

            if (!dialog.IsSuccess)
            {
                using (FileStream fileStream = new FileStream("group.xml", FileMode.Open))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                    this.pList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
                }
                return;
            }
            isCustom = true;
            groupLabel.Text = "自选股票：";
            string name = dialog.GroupName;
            ArrayList stocks = new ArrayList(dialog.SelectStocks);
            pList[name] = stocks;

            //更新界面
            initGroupsButton();

            ButtonItem item = new ButtonItem(name);
            groupButtonItemClicked(item, e);
            saveGroup();
        }

        private void raisingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String argStr = e.Argument.ToString();
            String[] args = argStr.Split(',');
            String type = args[0];
            String startDate = args[1];
            String endDate = args[2];

            ds = new DataSet();
            int totalPages = 0;

            switch (type)
	        {
                case "raisingLimitTab":
                    stop = Commons.GetRaisingLimitDay(startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                    totalPageList[curTabName] = totalPages;
                    return;
                case "growthBoardTab":
                    stop = Commons.GetGrowthBoard(startDate, endDate, Convert.ToInt32(args[3]), out errorNo, out ds, out totalPages);
                    totalPageList[curTabName] = totalPages;
                    return;
		        default:
                    break;
	        }

            if (args.Length == 4)//自选
            {
                ArrayList stocks;
                String name = args[3];//取选中的分组
                stocks = new ArrayList(pList[name]);
                switch (type)
                {
                    case "raisingLimitInfoTab":
                        stop = Commons.GetRaisingLimitInfo(stocks, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    case "raisingLimitIntervalTab":
                        stop = Commons.GetRaisingLimitInterval(stocks, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    case "stockStopTab":
                        stop = Commons.GetStockStop(stocks, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    default:
                        break;
                }
            }
            else //版块
            {
                switch (type)
                {
                    case "raisingLimitInfoTab":
                        stop = Commons.GetRaisingLimitInfoBoard(record, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    case "raisingLimitIntervalTab":
                        stop = Commons.GetRaisingLimitIntervalBoard(record, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    case "stockStopTab":
                        stop = Commons.GetStockStopBoard(record, sortColumnList[curTabName], sortTypeList[curTabName], startDate, endDate, pageList[curTabName], pagesize, out errorNo, out ds, out totalPages);
                        break;
                    default:
                        break;
                }
                
            }
            totalPageList[curTabName] = totalPages;
        }

        private void raisingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            String tableName = "";
            String translateFile = "";
            DataGridView thisView = null;
            bool customSort = true;
            switch (curTabName)
            {
                case "raisingLimitInfoTab":
                    tableName = "raising_limit_info";
                    translateFile = "raisingLimitInfo.xml";
                    thisView = raisingLimitInfoGrid;
                    break;
                case "raisingLimitTab":
                    tableName = "raising_limit_info_day";
                    translateFile = "raisingLimitInfoday.xml";
                    thisView = raisingLimitGrid;
                    customSort = false;
                    break;
                case "raisingLimitIntervalTab":
                    tableName = "raising_limit_info_interval";
                    translateFile = "raisingLimitInfoInterval.xml";
                    thisView = raisingLimitIntervalGrid;
                    customSort = false;
                    break;
                case "stockStopTab":
                    tableName = "stock_stop";
                    translateFile = "stockStop.xml";
                    thisView = stockStopGrid;
                    break;
                case "growthBoardTab":
                    tableName = "growth_board";
                    translateFile = "growthboard.xml";
                    thisView = growthBoardGrid;
                    break;
                default:
                    stop = true;
                    MessageBox.Show("无查询结果");
                    return;
            }

            dt = (DataTable)ds.Tables[tableName];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }
            DataSet orgDs = (DataSet)thisView.DataSource;
            if (orgDs != null && orgDs.Tables[tableName] != null && pageList[curTabName] != 1)
            {
                orgDs.Tables[tableName].Merge(dt);
            }
            else
            {
                thisView.DataSource = ds;
                thisView.DataMember = tableName;
            }
            pageLabel.Text = pageList[curTabName] + "/" + (int)totalPageList[curTabName];

            thisView.AllowUserToAddRows = false;//不显示最后空白行
            thisView.EnableHeadersVisualStyles = false;
            int k = 0;
            int sortIndex = 0;
            SerializableDictionary<string, string> raisingLimitInfoDict = new SerializableDictionary<string, string>();
            using (FileStream fileStream = new FileStream(translateFile, FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                raisingLimitInfoDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
            }
            foreach (var item in raisingLimitInfoDict)
            {
                thisView.Columns[k].HeaderText = item.Value;
                thisView.Columns[k].Width = 100;// 60 + item.Value.Length * 10;
                thisView.Columns[k].DataPropertyName = ds.Tables[0].Columns[item.Key].ToString();
                switch (item.Key)
                {
                    case "stock_id":
                    case "stock_name":
                        thisView.Columns[k].Frozen = true;
                        break;
                    default:
                        break;
                }
                if (customSort)
                {
                    thisView.Columns[k].SortMode = DataGridViewColumnSortMode.Programmatic;
                }
                
                if (sortColumnList[curTabName] == thisView.Columns[k].DataPropertyName)
                {
                    sortIndex = k;
                }
                k++;
            }
            if (customSort)
            {
                thisView.Columns[sortIndex].HeaderCell.SortGlyphDirection = sortTypeList[curTabName] ? SortOrder.Ascending : SortOrder.Descending;
            }

            stop = true;
        }
        private void dataGridViewX1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号

            DataGridView gridView = (DataGridView)sender;
            int coun = gridView.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                gridView.Rows[i].HeaderCell.Value = j.ToString();

                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    gridView.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;

                }
                switch (curTabName)
                {
                    case "raisingLimitTab":
                    case "growthBoardTab":
                        break;
                    default:
                        gridView.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                        gridView.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        break;
                }
                
            }
        }


        //private void raisingLimitGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        //{
        //    DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
        //    //自动整理序列号

        //    int coun = raisingLimitGrid.RowCount;
        //    for (int i = 0; i < coun; i++)
        //    {
        //        int j = i + 1;
        //        raisingLimitGrid.Rows[i].HeaderCell.Value = j.ToString();

        //        //隔行显示不同的颜色
        //        if (IsOdd(i))
        //        {
        //            raisingLimitGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;

        //        }
        //        raisingLimitGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
        //        raisingLimitGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
        //    }
        //}
        //private void raisingLimitIntervalGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        //{
        //    DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
        //    //自动整理序列号

        //    int coun = raisingLimitIntervalGrid.RowCount;
        //    for (int i = 0; i < coun; i++)
        //    {
        //        int j = i + 1;
        //        raisingLimitIntervalGrid.Rows[i].HeaderCell.Value = j.ToString();

        //        //隔行显示不同的颜色
        //        if (IsOdd(i))
        //        {
        //            raisingLimitIntervalGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;

        //        }
        //        raisingLimitIntervalGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
        //        raisingLimitIntervalGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
        //    }
        //}

        //private void stockStopGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        //{
        //    DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
        //    //自动整理序列号

        //    int coun = stockStopGrid.RowCount;
        //    for (int i = 0; i < coun; i++)
        //    {
        //        int j = i + 1;
        //        stockStopGrid.Rows[i].HeaderCell.Value = j.ToString();

        //        //隔行显示不同的颜色
        //        if (IsOdd(i))
        //        {
        //            stockStopGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;

        //        }
        //        stockStopGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
        //        stockStopGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;

        //    }
        //}
        const int CLOSE_SIZE = 15;
        //tabPage标签图片 
        //Bitmap image = new Bitmap("E:\\ico_close.gif");

        //绘制“Ｘ”号即关闭按钮 
        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.tabControl.GetTabRect(e.Index);

                //先添加TabPage属性    
                e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);                             

                //画关闭符号 
                if (e.State == DrawItemState.Selected)
                {
                    //再画一个矩形框 
                    using (Pen p = new Pen(Color.White))
                    {
                        myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                        myTabRect.Width = CLOSE_SIZE;
                        myTabRect.Height = CLOSE_SIZE;
                        e.Graphics.DrawRectangle(p, myTabRect);
                    }

                    //填充矩形框 
                    Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.LightGray;

                    using (Brush b = new SolidBrush(recColor))
                    {
                        e.Graphics.FillRectangle(b, myTabRect);
                    }
                    using (Pen objpen = new Pen(Color.Black))
                    {
                        //============================================= 
                        //自己画X 
                        //"\"线 
                        Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                        Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                        e.Graphics.DrawLine(objpen, p1, p2);
                        //"/"线 
                        Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                        Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                        e.Graphics.DrawLine(objpen, p3, p4);

                        ////============================================= 
                        //使用图片 
                        //Bitmap bt = new Bitmap(image);
                        //Point p5 = new Point(myTabRect.X, 4);
                        //e.Graphics.DrawImage(bt, p5);
                        //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, objpen.Brush, p5); 
                    }
                }
                e.Graphics.Dispose();
            }
            catch (Exception)
            { }
        }

        //关闭按钮功能 
        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;
                //计算关闭区域    
                Rectangle myTabRect = this.tabControl.GetTabRect(this.tabControl.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡    
                bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;
                if (isClose == true)
                {
                    this.tabControl.TabPages.Remove(this.tabControl.SelectedTab);
                }
            }
        }


        private void timeCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (timeCombox.SelectedItem.ToString().Equals("最近7天"))
            {
                startDateTimePicker.Value = DateTime.Now.AddDays(-7);
                endDateTimePicker.Value = DateTime.Now;
            }
            else if (timeCombox.SelectedItem.ToString().Equals("最近3天"))
            {
                startDateTimePicker.Value = DateTime.Now.AddDays(-3);
                endDateTimePicker.Value = DateTime.Now;
            }
        }
    }
}
