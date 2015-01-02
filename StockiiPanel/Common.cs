using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;
using System.Collections;//在C#中使用ArrayList必须引用Collections类
using System.IO;

namespace StockiiPanel
{
    /// <summary>
    /// 公共接口类
    /// </summary>
    class Commons
    {
        private static MySqlConnection conn;
        public static int colNum = 57;

        //版块分类
        enum Board
        {
            Section = 1,
            Industry = 2,
            Up = 3,
            Down = 4
        }

        /// <summary>
        /// 查询股票所有分组信息，包括地区和行业两种
        /// </summary>
        /// <param name="sectionToolStripMenuItem">地区菜单</param>
        /// <param name="industryToolStripMenuItem">行业菜单</param>
        public static void GetStockClassification(ToolStripMenuItem sectionToolStripMenuItem, ToolStripMenuItem industryToolStripMenuItem)
        {
            /*
            String strConn = "Server=127.0.0.1;User ID=root;Password=root;Database=stock;CharSet=utf8;";

            //初始化版块菜单
            try
            {
                conn = new MySqlConnection(strConn);
                String sqlId = "select * from area_info";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlId, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds, "area_info");

                DataView dvMenuOptions = new DataView(ds.Tables["area_info"]);

                foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
                {
                    ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                    tsItemParent.Text = rvMain["area_name"].ToString();
                    tsItemParent.Name = rvMain["area_id"].ToString();
                    sectionToolStripMenuItem.DropDownItems.Add(tsItemParent);
                }

                sqlId = "select * from industry_info";
                cmd = new MySqlCommand(sqlId, conn);
                da = new MySqlDataAdapter(cmd);

                da.Fill(ds, "industry_info");

                dvMenuOptions = new DataView(ds.Tables["industry_info"]);

                foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
                {
                    ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                    tsItemParent.Text = rvMain["industry_name"].ToString();
                    tsItemParent.Name = rvMain["industry_id"].ToString();
                    industryToolStripMenuItem.DropDownItems.Add(tsItemParent);
                }
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
                    case 24:
                        MessageBox.Show("内存不足");
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
             */

            DataSet ds = JSONHandler.GetClassfication();
            DataTable dt = ds.Tables["stockclassification"];

            DataView dvMenuOptions = new DataView(dt.DefaultView.ToTable(true,new string[]{"areaname"}));//distinct

            foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
            {
                ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                tsItemParent.Text = rvMain["areaname"].ToString();
                tsItemParent.Name = rvMain["areaname"].ToString();
                sectionToolStripMenuItem.DropDownItems.Add(tsItemParent);
            }
            dvMenuOptions = new DataView(dt.DefaultView.ToTable(true, new string[] { "industryname" }));

            foreach (DataRowView rvMain in dvMenuOptions)//循环得到主菜单
            {
                ToolStripMenuItem tsItemParent = new ToolStripMenuItem();

                tsItemParent.Text = rvMain["industryname"].ToString();
                tsItemParent.Name = rvMain["industryname"].ToString();
                industryToolStripMenuItem.DropDownItems.Add(tsItemParent);
            }
        }

        /// <summary>
        /// 查询股票所有基本信息，包括股票id、股票名称以及上市日期
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockBasicInfo()
        {
            /*
            String strConn = "Server=127.0.0.1;User ID=root;Password=root;Database=stock;CharSet=utf8;";

            DataSet ds = new DataSet();
            
            try
            {
                conn = new MySqlConnection(strConn);
                String sqlId = "select stock_id,stock_name from stock_basic_info order by stock_id";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlId, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                da.Fill(ds, "stock_basic_info");
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
                    case 24:
                        MessageBox.Show("内存不足");
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
            */

            DataSet ds = JSONHandler.GetStockBasicInfo();
            return ds;
        }

        /// <summary>
        /// 查询股票所有详细信息，包括股票每天的各种指标值
        /// </summary>
        /// <param name="stockid">股票市场中股票的id号</param>
        /// <param name="sortname">排序字段</param>
        /// <param name="asc">升序或者降序排序</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="page">分页查询中的第几页</param>
        /// <param name="pagesize">分页查询中，每页查询的数量</param>
        /// <param name="totalpage">总页数</param>
        /// <returns></returns>
        public static bool GetStockDayInfo(ArrayList stockid, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, object errorNo, DataSet ds, object totalpage)
        {
            bool stop = false;

            int line = (page - 1) * pagesize;

            String sqlId = "select * from stock_basic_info as A,stock_day_info as B where A.stock_id=B.stock_id and created between '" + startDate + "' and '" + endDate + "'";

            sqlId += " and ( B.stock_id ='" + stockid[0].ToString() + "'";
            stockid.RemoveAt(0);
            foreach (string stockId in stockid)
            {
                sqlId += " or B.stock_id ='" + stockId + "'";
            }
            sqlId += ") and seq_no >= ( select seq_no from stock_day_info order by seq_no limit "+ line + ",1 ) limit " + pagesize;

            String strConn = "Server=127.0.0.1;User ID=root;Password=root;Database=stock;CharSet=utf8;";

            //BackgroundWorker worker = (BackgroundWorker)sender;
            try
            {
                conn = new MySqlConnection(strConn);

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sqlId, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                

                da.Fill(ds, "stock_day_info");

            }
            catch (MySqlException ex)
            {
                errorNo = ex.Number;
                stop = true;
            }
            catch (Exception)
            {
                errorNo = 24;
                stop = true;
            }
            finally
            {
                conn.Close();
            }

            return stop;
        }

        /// <summary>
        /// 导出列表中数据到CSV中
        /// </summary>
        /// <param name="dt">要导出的DataTable</param>
        public static void ExportDataGridToCSV(DataTable dt)
        {
             SaveFileDialog saveFileDialog =  new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.csv";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "csv files|*.csv";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.FileName = "";

            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != null) //打开保存文件对话框
            {
                string fileName = saveFileDialog.FileName;//文件名字
                using (StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.Default))
                {
                    //Tabel header
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        streamWriter.Write(dt.Columns[i].ColumnName);
                        streamWriter.Write(",");
                    }
                    streamWriter.WriteLine("");
                    //Table body
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            streamWriter.Write(dt.Rows[i][j]);
                            streamWriter.Write(",");
                        }
                        streamWriter.WriteLine("");
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        /// <summary>
        /// Delete special symbol
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DelQuota(string str)
        {
            string result = str;
            string[] strQuota = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "`", ";", "'", ",", ".", "/", ":", "/,", "<", ">", "?" };
            for (int i = 0; i < strQuota.Length; i++)
            {
                if (result.IndexOf(strQuota[i]) > -1)
                    result = result.Replace(strQuota[i], "");
            }
            return result;
        }

        /// <summary>
        /// 从dataGridView中的选中行或所有行并放到一个新表中
        /// </summary>
        /// <param name="isSelect">是否选中行</param>
        /// <returns>datatable</returns>
        public static DataTable StructrueDataTable(DataGridView dataGridView, bool isSelect)
        {
            #region 从dataGridView中选取行并放到一个新表中，然后再绑定到dataGridView中
            DataTable dataTable = new DataTable();

            int length = 0;

            switch (dataGridView.Name)
            {
                case "rawDataGrid":
                    length = colNum;
                    break;
                case "ndayGrid":
                    length = 5;
                    break;
                case "calResultGrid":
                    length = 7;
                    break;
                case "sectionResultGrid":
                    length = 11;
                    break;
                default:
                    break;
            }

            //添加表头
            for (int col = 0; col < length; col++)
            {
                string columnName = dataGridView.Columns[col].HeaderText;
                dataTable.Columns.Add(columnName,dataGridView.Columns[col].ValueType);
            }

            if (isSelect)
            {

                for (int r = dataGridView.SelectedRows.Count - 1; r >= 0; r--)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int c = 0; c < length; c++)
                    {
                        dataRow[c] = dataGridView.SelectedRows[r].Cells[c].Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            else
            {

                for (int r = 0; r < dataGridView.Rows.Count; r++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int c = 0; c < length; c++)
                    {
                        dataRow[c] = dataGridView.Rows[r].Cells[c].Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
            #endregion
        }

        /// <summary>
        /// 判断是不是交易日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool isTradeDay(String date)
        {          
            return true;
        }

        /// <summary>
        /// 查询股票所有详细信息，区分版块
        /// </summary>
        /// <param name="record">版块记录</param>
        /// <param name="sortname">排序字段</param>
        /// <param name="asc">升序或者降序排序</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="page">分页查询中的第几页</param>
        /// <param name="pagesize">分页查询中，每页查询的数量</param>
        /// <param name="totalpage">总页数</param>
        /// <returns></returns>
        public static bool GetStockDayInfoBoard(Dictionary<int,string> record, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, object errorNo, DataSet ds, object totalpage)
        {
            bool stop = false;
            return stop;
        }

        /// <summary>
        /// 查询股票N日和
        /// </summary>
        /// <param name="stockid">股票市场中股票的id号</param>
        /// <param name="type">类型，1日和，2周和，3月和</param>
        /// <param name="num">参数</param>
        /// <param name="sumname">求和的指标名称</param>
        /// <param name="sumtype">求和类型</param>
        /// <param name="sortname">排序字段</param>
        /// <param name="asc">升序或者降序排序</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="page">分页查询中的第几页</param>
        /// <param name="pagesize">分页查询中，每页查询的数量</param>
        /// <param name="errorNo">错误码</param>
        /// <param name="ds">结果集</param>
        /// <param name="totalpage">总页数</param>
        /// <returns></returns>
        public static bool GetNDaysSum(ArrayList stockid, int type, int num, String sumname, String sumtype, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, object errorNo, DataSet ds, object totalpage)
        {
            bool stop = false;

            return stop;
        }

        /// <summary>
        /// 查询股票N日和,区分版块
        /// </summary>
        /// <param name="record">版块记录</param>
        /// <param name="type">类型，1日和，2周和，3月和</param>
        /// <param name="num">参数</param>
        /// <param name="sumname">求和的指标名称</param>
        /// <param name="sumtype">求和类型</param>
        /// <param name="sortname">排序字段</param>
        /// <param name="asc">升序或者降序排序</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="page">分页查询中的第几页</param>
        /// <param name="pagesize">分页查询中，每页查询的数量</param>
        /// <param name="errorNo">错误码</param>
        /// <param name="ds">结果集</param>
        /// <param name="totalpage">总页数</param>
        /// <returns></returns>
        public static bool GetNDaysSumBoard(Dictionary<int, string> record, int type, int num, String sumname, String sumtype, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, object errorNo, DataSet ds, object totalpage)
        {
            bool stop = false;

            return stop;
        }

        /// <summary>
        /// 查询股票的自定义计算结果
        /// </summary>
        /// <param name="stockid">股票市场中股票的id号</param>
        /// <param name="min">筛选最小值</param>
        /// <param name="max">筛选最大值</param>
        /// <param name="optname">自定义计算的指标名</param>
        /// <param name="opt">自定义计算方式</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="errorNo">错误码</param>
        /// <param name="ds">结果集</param>
        /// <returns></returns>
        public static bool GetStockDaysDiff(ArrayList stockid, double min, double max, String optname, String opt, String startDate, String endDate, object errorNo, DataSet ds)
        {
            bool stop = false;

            return stop;
        }

        /// <summary>
        /// 查询股票的自定义计算结果，区分版块
        /// </summary>
        /// <param name="record">版块记录</param>
        /// <param name="min">筛选最小值</param>
        /// <param name="max">筛选最大值</param>
        /// <param name="optname">自定义计算的指标名</param>
        /// <param name="opt">自定义计算方式</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="errorNo">错误码</param>
        /// <param name="ds">结果集</param>
        /// <returns></returns>
        public static bool GetStockDaysDiffBoard(Dictionary<int, string> record, double min, double max, String optname, String opt, String startDate, String endDate, object errorNo, DataSet ds)
        {
            bool stop = false;

            return stop;
        }

        /// <summary>
        /// 查询股票跨区的信息
        /// </summary>
        /// <param name="weight">跨区的权重值</param>
        /// <param name="optname">自定义计算的指标名</param>
        /// <param name="opt">自定义计算方式</param>
        /// <param name="startDate">查询起始时间点</param>
        /// <param name="endDate">查询结束时间点</param>
        /// <param name="errorNo">错误码</param>
        /// <param name="ds">结果集</param>
        /// <returns></returns>
        public static bool GetCrossInfoCmd(double weight, String optname, String startDate, String endDate, object errorNo, DataSet ds)
        {
            bool stop = false;

            return stop;
        }

    }
}
