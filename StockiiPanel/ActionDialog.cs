using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Collections;//在C#中使用ArrayList必须引用Collections类

namespace StockiiPanel
{
    public partial class ActionDialog : Form
    {
        private SerializableDictionary<String, ArrayList> combineList = new SerializableDictionary<string, ArrayList>();//所有的操作
        private SerializableDictionary<String, ArrayList> combineHeaderList = new SerializableDictionary<string, ArrayList>();//所有的表头
        ArrayList combineArray = new ArrayList();//拼接的操作序列
        String actionName = "";
        private String configDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Stockii";

        public String ActionName
        {
            get { return actionName; }
            set { actionName = value; }
        }

        public ArrayList CombineArray
        {
            get { return combineArray; }
            set { combineArray = value; }
        }

        public ActionDialog()
        {
            InitializeComponent();

            using (FileStream fileStream = new FileStream("actions.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                this.combineList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
            }

            using (FileStream fileStream = new FileStream("headers.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                this.combineHeaderList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
            }

            if (combineList == null)
            {
                combineList = new SerializableDictionary<string, ArrayList>();
            }

            foreach (var item in combineList)
            {
                actionList.Items.Add(item.Key);
            }
        }

        private void actionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (actionList.SelectedItem == null)
                return;

            actionName = actionList.SelectedItem.ToString();
            ArrayList list = combineList[actionName];//拼接序列
            int number = list.Count;// 操作里拼接的个数

            numLabel.Text = "拼接个数：" + number;
            actions.Items.Clear();

            foreach (var item in list)
            {
                String[] args = item.ToString().Split(',');//解析参数

                if (args[0].Equals("crossSection"))
                {
                    actions.Items.Add("跨区：权重（%）－>" + args[1] + "||指标－>" + args[2]);
                }
                else
                {
                    actions.Items.Add("自定义计算：最小值－>" + args[1] + "||最大值－>" + args[2] + "||比较指标－>" + args[3] + "||比较方法－>" + args[4]);
                }
            }
            
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem == null || actionList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择一项");
                return;
            }

            DialogResult dr = MessageBox.Show("确认删除？", "删除操作", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);

            if (dr == DialogResult.Yes)
            {
                String key = actionList.SelectedItem.ToString();
                combineList.Remove(key);
                combineHeaderList.Remove(key);

                //保存到外部文件
                using (FileStream fileStream = new FileStream("actions.xml", FileMode.Create))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                    xmlFormatter.Serialize(fileStream, this.combineList);
                }

                //保存表头到外部文件
                using (FileStream fileStream = new FileStream("headers.xml", FileMode.Create))
                {
                    XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                    xmlFormatter.Serialize(fileStream, this.combineHeaderList);
                }

                actionList.Items.RemoveAt(actionList.SelectedIndex);
            }
            else
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (actionList.SelectedItem == null||actionList.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择一项");
                return;
            }

            if (startDatePicker.Value.CompareTo(endDatePicker.Value) > 0)
            {
                MessageBox.Show("开始时间大于结束时间");
                return;
            }

            if (!Commons.isTradeDay(startDatePicker.Value) || !Commons.isTradeDay(endDatePicker.Value))
            {
                MessageBox.Show("非交易日");
                return;
            }
            actionName = actionList.SelectedItem.ToString();
            ArrayList list = combineList[actionName];//拼接序列

            foreach (String item in list)
            {
                combineArray.Add(item + "," + startDatePicker.Value.ToString("yyyy-MM-dd") + "," + endDatePicker.Value.ToString("yyyy-MM-dd"));//增加时间 
            }

            this.Close();
        }
    }
}
