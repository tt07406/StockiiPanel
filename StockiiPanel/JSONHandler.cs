using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Net;
using System.Collections;//在C#中使用ArrayList必须引用Collections类

namespace StockiiPanel
{
    /// <summary>
    /// JSON处理类
    /// </summary>
    static class JSONHandler
    {
        public static string commonURL = "http://stockii-gf.oicp.net/client/api";
        //public static string localURL = "http://192.168.1.220:8080/client/api";
        public static string localURL = "http://www.stockii.com:8090/client/api"; 

        /// <summary>
        /// 将JSON解析成DataSet只限标准的JSON数据
        /// 例如：Json＝{t1:[{name:'数据name',type:'数据type'}]} 或 Json＝{t1:[{name:'数据name',type:'数据type'}],t2:[{id:'数据id',gx:'数据gx',val:'数据val'}]}
        /// </summary>
        /// <param name="Json">Json字符串</param>
        /// <returns>DataSet</returns>
        public static DataSet JsonToDataSet(string Json)
        {
            try
            {
                DataSet ds = new DataSet();
                JavaScriptSerializer JSS = new JavaScriptSerializer();

                JSS.MaxJsonLength = Int32.MaxValue; //取得最大数值,代码在对象过大时会报错
                object obj = JSS.DeserializeObject(Json);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;


                foreach (var item in datajson)
                {
                    DataTable dt = new DataTable(item.Key);

                    object[] rows = (object[])item.Value;
                    foreach (var row in rows)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)row;
                        DataRow dr = dt.NewRow();
                        foreach (KeyValuePair<string, object> sss in val)
                        {
                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key.ToString());
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 将DataSet转化成JSON数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToJson(DataSet ds)
        {
            string json = string.Empty;
            try
            {
                if (ds.Tables.Count == 0)
                    throw new Exception("DataSet中Tables为0");
                json = "{";
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    json += "T" + (i + 1) + ":[";
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {
                        json += "{";
                        for (int k = 0; k < ds.Tables[i].Columns.Count; k++)
                        {
                            json += ds.Tables[i].Columns[k].ColumnName + ":'" + ds.Tables[i].Rows[j][k].ToString() + "'";
                            if (k != ds.Tables[i].Columns.Count - 1)
                                json += ",";
                        }
                        json += "}";
                        if (j != ds.Tables[i].Rows.Count - 1)
                            json += ",";
                    }
                    json += "]";
                    if (i != ds.Tables.Count - 1)
                        json += ",";


                }
                json += "}";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return json;
        }

        #region DataTable 转换为Json 字符串
        /// <summary>
        /// DataTable 对象 转换为Json 字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dt)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
            ArrayList arrayList = new ArrayList();
            foreach (DataRow dataRow in dt.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();  //实例化一个参数集合
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, ToStr(dataRow[dataColumn.ColumnName]));
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }

            return javaScriptSerializer.Serialize(arrayList);  //返回一个json字符串
        }
        #endregion

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }
        #endregion

        #region 转换为string字符串类型
        /// <summary>
        ///  转换为string字符串类型
        /// </summary>
        /// <param name="s">获取需要转换的值</param>
        /// <param name="format">需要格式化的位数</param>
        /// <returns>返回一个新的字符串</returns>
        public static string ToStr(this object s, string format = "")
        {
            string result = "";
            try
            {
                if (format == "")
                {
                    result = s.ToString();
                }
                else
                {
                    result = string.Format("{0:" + format + "}", s);
                }
            }
            catch
            {
            }
            return result;
        }
        #endregion

        public static int test()
        {
            int count = 0;
            string jsonText = "";

            try
            {
                FileStream aFile = new FileStream("test.txt", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                jsonText = sr.ReadToEnd();
              
                sr.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return count;
            }

            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
            string jsonarray = jo["liststockclassficationresponse"]["stockclassification"].ToString();
            string zone_en = jo["liststockclassficationresponse"]["count"].ToString();

            count = Convert.ToInt32(zone_en);
            Console.WriteLine(jsonarray);
            DataSet jsDs = JsonToDataSet("{'stockclassification' :"+ jsonarray+"}");

            jsDs.WriteXml("test.xml");

            return count;
        }
        
        /// <summary>
        /// 获取所有的交易日列表
        /// </summary>
        /// <returns></returns>
        public static DataSet GetTradeDate()
        {
            string jsonText = "";

            try
            {
                //FileStream aFile = new FileStream("tradedates.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();

                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "listtradedate";
                args["response"] = "json";
                jsonText = WebService.Post(url, args);

            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return null;
            }

            string jsonarray = jo.First.First.Last.ToString();

            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");
            //jsDs.WriteXml("classfication.xml");

            return jsDs;
        }

        /// <summary>
        /// 获取股票所有分组信息，包括地区和行业两种
        /// </summary>
        /// <returns></returns>
        public static DataSet GetClassfication()
        {
            string jsonText = "";

            try
            {
                //FileStream aFile = new FileStream("classification.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "liststockclassification";
                args["response"] = "json";
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return null;
            }

            string jsonarray = jo.First.First.Last.ToString();
            
            DataSet jsDs = JsonToDataSet( "{"+ jsonarray +"}");
            //jsDs.WriteXml("classfication.xml");

            return jsDs;
        }

        /// <summary>
        /// 查询股票所有基本信息，包括股票id、股票名称以及上市日期
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockBasicInfo()
        {
            string jsonText = "";

            try
            {
                //FileStream aFile = new FileStream("stockbasicinfo.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "liststockbasicinfo";
                args["response"] = "json";
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return null;
            }

            string jsonarray = jo.First.First.Last.ToString();

            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");

            jsDs.Tables["stockbasicinfo"].Columns["stockid"].ColumnName = "stock_id";
            jsDs.Tables["stockbasicinfo"].Columns["stockname"].ColumnName = "stock_name";
            jsDs.Tables["stockbasicinfo"].TableName = "stock_basic_info";

            return jsDs;
        }

        /// <summary>
        /// 获取股票所有详细信息，包括股票每天的各种指标值
        /// </summary>
        /// <returns></returns>
        public static DataSet GetStockDayInfo(ArrayList stockid, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, String filter, out int totalpage,out int errorNo)
        {
            string jsonText = "";
            totalpage = 1;
            errorNo = 0;

            try
            {
                //FileStream aFile = new FileStream("stockdayinfo.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "liststockdayinfo";
                args["response"] = "json";
                if (stockid.Count != 0)
                {
                    args["stockid"] = String.Join(",", stockid.ToArray());
                }
                
                args["page"] = page+"";
                args["pagesize"] = pagesize + "";
                args["asc"] = asc + "";
                if (!filter.Equals(""))
                    args["filter"] = filter;
                if (!sortname.Equals(""))
                    args["sortname"] = sortname;
                if (!startDate.Equals(""))
                    args["starttime"] = startDate;
                if (!endDate.Equals(""))
                    args["endtime"] = endDate;
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                errorNo = 1;
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                errorNo = 0;
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            string num = jo.First.First.First.First.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");
            totalpage = Convert.ToInt32(num) / pagesize + 1;

            return jsDs;
        }

        public static DataSet GetNDaysSum(ArrayList stockid, int type, int num, String sumname, String sumtype, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, String filter, out int totalpage, out int errorNo)
        {
            string jsonText = "";
            totalpage = 1;
            errorNo = 0;
            try
            {
                //string name = "";
                //switch (type)
                //{
                //    case 1:
                //        name = "stockdaysum.txt";
                //        break;
                //    case 2:
                //        name = "stockweeksum.txt";
                //        break;
                //    default:
                //        name = "stockmonthsum.txt";
                //        break;
                //}
                //FileStream aFile = new FileStream(name, FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                switch (type)
                {
                    case 1:
                        args["command"] = "listdaysum";
                        args["days"] = num.ToString();
                        break;
                    case 2:
                        args["command"] = "listweeksum";
                        args["weeks"] = num.ToString();
                        break;
                    default:
                        args["command"] = "listmonthsum";
                        args["months"] = num.ToString();
                        break;
                }
                args["response"] = "json";
                if (stockid.Count != 0)
                {
                    args["stockid"] = String.Join(",", stockid.ToArray());
                }
                args["starttime"] = startDate;
                args["endtime"] = endDate;
                args["sumname"] = sumname;
                args["sumtype"] = sumtype;
                if (sortname.Trim().Length != 0)
                {
                    args["sortname"] = sortname;
                    args["asc"] = asc.ToString();
                }
                if (filter.Trim().Length != 0)
                {
                    args["filter"] = filter;
                }
                args["page"] = page.ToString();
                args["pagesize"] = pagesize.ToString();
                jsonText = WebService.Post(url, args);
                
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                errorNo = 0;
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            string count = jo.First.First.First.First.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");
            totalpage = Convert.ToInt32(count) / pagesize + 1;

            return jsDs;
        }
        public static DataSet GetStockDaysDiff(ArrayList stockid, double min, double max, String optname, String opt, String startDate, String endDate, out int errorNo)
        {
            string jsonText = "";
            errorNo = 0;
            try
            {
                //FileStream aFile = new FileStream("stockdaysdiff.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                switch (opt)
                {
                    case "seperate":
                        args["command"] = "listgrowthampdis";
                        break;
                    case "sum":
                        args["command"] = "listndayssum";
                        args["sumname"] = optname;
                        break;
                    default:
                        args["command"] = "liststockdaysdiff";
                        args["optname"] = optname;
                        args["opt"] = opt;
                        break;
                }
                args["response"] = "json";
                args["stockid"] = String.Join(",", stockid.ToArray());
                args["starttime"] = startDate;
                args["endtime"] = endDate;
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                errorNo = 0;
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");

            return jsDs;
        }

        public static DataSet GetCrossInfo(double weight, String optName, String startDate, String endDate)
        {
            string jsonText = "";

            try
            {
                //FileStream aFile = new FileStream("crossinfo.txt", FileMode.OpenOrCreate);
                //StreamReader sr = new StreamReader(aFile, UnicodeEncoding.GetEncoding("GB2312"));
                //jsonText = sr.ReadToEnd();

                //sr.Close();
                
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "listcrossinfo";
                args["response"] = "json";
                args["starttime"] = startDate;
                args["endtime"] = endDate;
                args["optname"] = optName;
                args["weight"] = weight.ToString();
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");

            return jsDs;
        }
        public static DataSet GetRaisingLimitInfo(ArrayList stockid, String sortname, bool asc, String startDate, String endDate, int page, int pagesize, out int totalpage, out int errorNo)
        {
            string jsonText = "";
            totalpage = 1;
            errorNo = 0;

            try
            {
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "listraisinglimitinfo";
                args["response"] = "json";
                if (stockid.Count != 0)
                {
                    args["stockid"] = String.Join(",", stockid.ToArray());
                }
                args["starttime"] = startDate;
                args["endtime"] = endDate;
                if (sortname.Trim().Length != 0)
                {
                    args["sortname"] = sortname;
                    args["asc"] = asc.ToString();
                }
                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            string num = jo.First.First.First.First.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");
            totalpage = Convert.ToInt32(num) / pagesize + 1;

            return jsDs;
        }
        public static DataSet GetRaisingLimitDay(String startDate, String endDate, int page, int pagesize, out int totalpage, out int errorNo)
        {
            string jsonText = "";
            totalpage = 1;
            errorNo = 0;

            try
            {
                string url = localURL;
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["command"] = "listraisinglimitinfoday";
                args["response"] = "json";
                args["starttime"] = startDate;
                args["endtime"] = endDate;

                jsonText = WebService.Post(url, args);
            }
            catch (IOException ex)
            {
                Console.WriteLine("An IOException has been thrown!");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine("An WebException has been thrown!");
                Console.WriteLine(ex.ToString());
                return null;
            }

            JObject jo = JObject.Parse(jsonText);

            if (jo.First.First.Last == null)
            {
                return new DataSet();
            }

            string jsonarray = jo.First.First.Last.ToString();
            string num = jo.First.First.First.First.ToString();
            DataSet jsDs = JsonToDataSet("{" + jsonarray + "}");
            totalpage = Convert.ToInt32(num) / pagesize + 1;

            return jsDs;
        }
    }
}
