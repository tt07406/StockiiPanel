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

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.TabPages.Remove(tabControl.SelectedTab);
        }


        private void upItem_MouseDown(object sender, EventArgs e)
        {
            //groupList.Show();
            boardMenuStrip.Show(this, ribbonBar4.Location.X + upContainer.Size.Width, boardMenuStrip.Height + upContainer.Size.Height + ribbonPanel3.Location.Y);
            sectionToolStripMenuItem.Visible = false;
            industryToolStripMenuItem.Visible = false;
            upToolStripMenuItem.Visible = false;
            downToolStripMenuItem.Visible = false;
            upToolStripMenuItem.ShowDropDown();
        }

        private void downItem_MouseDown(object sender, EventArgs e)
        {
            //groupList.Show();
            boardMenuStrip.Show(this, ribbonBar4.Location.X+ upContainer.Size.Width+ downContainer.Size.Width, boardMenuStrip.Height + downContainer.Size.Height + ribbonPanel3.Location.Y);
            sectionToolStripMenuItem.Visible = false;
            industryToolStripMenuItem.Visible = false;
            upToolStripMenuItem.Visible = false;
            downToolStripMenuItem.Visible = false;
            downToolStripMenuItem.ShowDropDown();
        }


        private void showThisTab(TabPage tab)
        {
            if (!tabControl.Controls.Contains(tab))
            {
                tabControl.Controls.Add(tab);
                tabControl.SelectTab(tab);
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
            showThisTab(marketTab);
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

            foreach (String item in dialog.CombineArray)
            {
                if (item.StartsWith("customCal"))//如果包含自定义查询，则要选择分组或版块
                {
                    if (groupList.Visible == true && groupList.Items.Count == 0)
                    {
                        MessageBox.Show("请选择一个分组");
                        return;
                    }
                    else if (groupList.Visible == false && record.Count == 0)
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
                if (groupList.Visible && dialog.CombineArray[i].ToString().StartsWith("customCal"))
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

    }
}
