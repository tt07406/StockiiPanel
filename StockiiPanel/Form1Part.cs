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

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        MyProgressBar myBar = new MyProgressBar();

        // 代理定义，可以在Invoke时传入相应的参数
        private delegate void funHandle(int nValue);
        private funHandle myHandle = null;

        /// <summary>
        /// 线程函数中调用的函数
        /// </summary>
        private void ShowProgressBar()
        {
            if (myBar == null || myBar.IsDisposed)
                myBar = new MyProgressBar();

            myHandle = new funHandle(myBar.SetProgressValue);
            //  myBar.MdiParent = this;
            myBar.StartPosition = FormStartPosition.CenterScreen;
            myBar.ShowDialog();
        }

        /// <summary>
        /// 线程函数，用于处理调用
        /// </summary>
        private void ThreadFun()
        {
            MethodInvoker mi = new MethodInvoker(ShowProgressBar);
            this.BeginInvoke(mi);

            System.Threading.Thread.Sleep(500); // sleep to show window

            int i = 0;
            while (!stop)
            {
                ++i;
                System.Threading.Thread.Sleep(50);
                // 这里直接调用代理
                this.Invoke(this.myHandle, new object[] { (i % 101) });
                if (i > 100)
                    i = 0;
            }
        }

        private bool stop = false;

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


        private void startDatePicker_ValueChanged(object sender, EventArgs e)
        {
            startDatePicker1.Value = startDatePicker.Value;
            startDatePicker2.Value = startDatePicker.Value;
            startDatePicker3.Value = startDatePicker.Value;
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


        private void endDatePicker2_ValueChanged(object sender, EventArgs e)
        {
            endDatePicker1.Value = endDatePicker2.Value;
            endDatePicker.Value = endDatePicker2.Value;
            endDatePicker3.Value = endDatePicker2.Value;
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


        private void sumWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            dt = (DataTable)ds.Tables["n_day_sum"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            pageLabel.Text = page + "/" + (int)totalPages;

            ndayGrid.DataSource = ds;
            ndayGrid.DataMember = "n_day_sum";

            ndayGrid.AllowUserToAddRows = false;//不显示最后空白行
            ndayGrid.EnableHeadersVisualStyles = false;

            //改变DataGridView的表头
            ndayGrid.Columns[0].HeaderText = "代码";
            ndayGrid.Columns[0].Width = 70;
            ndayGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            ndayGrid.Columns[0].Frozen = true;

            ndayGrid.Columns[1].HeaderText = "名称";
            ndayGrid.Columns[1].Width = 80;
            ndayGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            ndayGrid.Columns[1].Frozen = true;

            ndayGrid.Columns[2].HeaderText = "日期";
            ndayGrid.Columns[2].Width = 80;
            ndayGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["created"].ToString();

            if (daySumButton.Checked)
            {
                ndayGrid.Columns[3].HeaderText = intervalCombo.Text + "日和";
                ndayGrid.Columns[3].Width = 110;
                ndayGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["ndaysum"].ToString();
            }
            else if (weekSumButton.Checked)
            {
                ndayGrid.Columns[3].HeaderText = intervalCombo.Text + "周和";
                ndayGrid.Columns[3].Width = 110;
                ndayGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["ndaysum"].ToString();
            }
            else if (monthSumButton.Checked)
            {
                ndayGrid.Columns[3].HeaderText = intervalCombo.Text + "月和";
                ndayGrid.Columns[3].Width = 110;
                ndayGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["ndaysum"].ToString();
            }

            stop = true;
        }

        private void customWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            dt = (DataTable)ds.Tables["stock_day_diff"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            pageLabel.Text = page + "/" + (int)totalPages;

            calResultGrid.DataSource = ds;
            calResultGrid.DataMember = "stock_day_diff";

            calResultGrid.AllowUserToAddRows = false;//不显示最后空白行
            calResultGrid.EnableHeadersVisualStyles = false;

            //改变DataGridView的表头
            calResultGrid.Columns[0].HeaderText = "代码";
            calResultGrid.Columns[0].Width = 70;
            calResultGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            calResultGrid.Columns[0].Frozen = true;

            calResultGrid.Columns[1].HeaderText = "名称";
            calResultGrid.Columns[1].Width = 80;
            calResultGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            calResultGrid.Columns[1].Frozen = true;

            calResultGrid.Columns[2].HeaderText = "开始时间";
            calResultGrid.Columns[2].Width = 110;
            calResultGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["start_time"].ToString();

            calResultGrid.Columns[3].HeaderText = "结束时间";
            calResultGrid.Columns[3].Width = 110;
            calResultGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["end_time"].ToString();

            calResultGrid.Columns[4].HeaderText = "起始时间的值";
            calResultGrid.Columns[4].Width = 150;
            calResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["start_value"].ToString();

            calResultGrid.Columns[5].HeaderText = "结束时间的值";
            calResultGrid.Columns[5].Width = 150;
            calResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["end_value"].ToString();

            calResultGrid.Columns[6].HeaderText = compareIndexCombo.Text + "(" + compareCombo.Text + ")";
            calResultGrid.Columns[6].Width = 240;
            calResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["index_value"].ToString();

            stop = true;
        }

        private void crossWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            dt = (DataTable)ds.Tables["cross_info"];

            if (dt == null || dt.Rows.Count == 0)
            {
                stop = true;
                MessageBox.Show("无查询结果");
                return;
            }

            pageLabel.Text = page + "/" + (int)totalPages;

            sectionResultGrid.DataSource = ds;
            sectionResultGrid.DataMember = "cross_info";

            sectionResultGrid.AllowUserToAddRows = false;//不显示最后空白行
            sectionResultGrid.EnableHeadersVisualStyles = false;

            //改变DataGridView的表头
            sectionResultGrid.Columns[0].HeaderText = "代码";
            sectionResultGrid.Columns[0].Width = 70;
            sectionResultGrid.Columns[0].DataPropertyName = ds.Tables[0].Columns["stock_id"].ToString();
            sectionResultGrid.Columns[0].Frozen = true;

            sectionResultGrid.Columns[1].HeaderText = "名称";
            sectionResultGrid.Columns[1].Width = 80;
            sectionResultGrid.Columns[1].DataPropertyName = ds.Tables[0].Columns["stock_name"].ToString();
            sectionResultGrid.Columns[1].Frozen = true;

            sectionResultGrid.Columns[2].HeaderText = "开始日期";
            sectionResultGrid.Columns[2].Width = 110;
            sectionResultGrid.Columns[2].DataPropertyName = ds.Tables[0].Columns["start_date"].ToString();

            sectionResultGrid.Columns[3].HeaderText = "起始时间的值";
            sectionResultGrid.Columns[3].Width = 150;
            sectionResultGrid.Columns[3].DataPropertyName = ds.Tables[0].Columns["start_value"].ToString();

            sectionResultGrid.Columns[4].HeaderText = "结束日期";
            sectionResultGrid.Columns[4].Width = 110;
            sectionResultGrid.Columns[4].DataPropertyName = ds.Tables[0].Columns["end_date"].ToString();

            sectionResultGrid.Columns[5].HeaderText = "结束时间的值";
            sectionResultGrid.Columns[5].Width = 150;
            sectionResultGrid.Columns[5].DataPropertyName = ds.Tables[0].Columns["end_value"].ToString();

            sectionResultGrid.Columns[6].HeaderText = "开始－上市（天）";
            sectionResultGrid.Columns[6].Width = 150;
            sectionResultGrid.Columns[6].DataPropertyName = ds.Tables[0].Columns["start_list_date"].ToString();

            sectionResultGrid.Columns[7].HeaderText = "结束－上市（天）";
            sectionResultGrid.Columns[7].Width = 150;
            sectionResultGrid.Columns[7].DataPropertyName = ds.Tables[0].Columns["end_list_date"].ToString();

            sectionResultGrid.Columns[8].HeaderText = "跨区类型";
            sectionResultGrid.Columns[8].Width = 120;
            sectionResultGrid.Columns[8].DataPropertyName = ds.Tables[0].Columns["cross_type"].ToString();

            sectionResultGrid.Columns[9].HeaderText = "均值(" + indexCombox1.Text + ")";
            sectionResultGrid.Columns[9].Width = 180;
            sectionResultGrid.Columns[9].DataPropertyName = ds.Tables[0].Columns["avg"].ToString();

            sectionResultGrid.Columns[10].HeaderText = "差异(" + indexCombox1.Text + ")";
            sectionResultGrid.Columns[10].Width = 180;
            sectionResultGrid.Columns[10].DataPropertyName = ds.Tables[0].Columns["difference"].ToString();

            stop = true;
        }


        private void ndayGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = ndayGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                ndayGrid.Rows[i].HeaderCell.Value = j.ToString();

                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    ndayGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }

                //前两行显示绿色
                ndayGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                ndayGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
            }
        }

        private void calResultGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = calResultGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                calResultGrid.Rows[i].HeaderCell.Value = j.ToString();

                //隔行显示不同的颜色
                if (IsOdd(i))
                {
                    calResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }

                //前两行显示绿色
                calResultGrid.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                calResultGrid.Rows[i].Cells[1].Style.BackColor = Color.Lime;
            }
        }

        private void sectionResultGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewTextBoxColumn dgv_Text = new DataGridViewTextBoxColumn();
            //自动整理序列号
            int coun = sectionResultGrid.RowCount;
            for (int i = 0; i < coun; i++)
            {
                int j = i + 1;
                sectionResultGrid.Rows[i].HeaderCell.Value = j.ToString();

                sectionResultGrid.Rows[i].DefaultCellStyle.BackColor = Color.Red;

            }
        }

        /// <summary>
        /// 版块中设置时间
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="num">参数</param>
        /// <param name="year">年份</param>
        private void SetDate(String type, String num, String year)
        {
            int arg = Convert.ToInt32(num);
            int years = Convert.ToInt32(year);
            int month = 1;

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (type.Equals("season"))//季度
            {
                switch (arg)
                {
                    case 1:
                        month = 1;
                        startDate = new DateTime(years, month, 1);
                        month = 3;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 2:
                        month = 4;
                        startDate = new DateTime(years, month, 1);
                        month = 6;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 3:
                        month = 7;
                        startDate = new DateTime(years, month, 1);
                        month = 9;
                        endDate = new DateTime(years, month, 1);
                        break;
                    case 4:
                        month = 10;
                        startDate = new DateTime(years, month, 1);
                        month = 12;
                        endDate = new DateTime(years, month, 1);
                        break;
                    default:
                        break;
                }
            }
            else //月份
            {
                month = arg;
                startDate = new DateTime(years, month, 1);
                endDate = new DateTime(years, month, 1);
            }

            startDatePicker.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker1.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker2.Value = TimeControl.GetFirstDayOfMonth(startDate);
            startDatePicker3.Value = TimeControl.GetFirstDayOfMonth(startDate);

            endDatePicker.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker1.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker2.Value = TimeControl.GetLastDayOfMonth(endDate);
            endDatePicker3.Value = TimeControl.GetLastDayOfMonth(endDate);
        }
    }


}
