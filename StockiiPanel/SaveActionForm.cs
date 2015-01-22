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
    
    public partial class SaveActionDialog : Form
    {
        private SerializableDictionary<String, ArrayList> combineList = new SerializableDictionary<string,ArrayList>();//所有的操作
        private SerializableDictionary<String, ArrayList> combineHeaderList = new SerializableDictionary<string, ArrayList>();//所有的表头
        ArrayList combineArray = new ArrayList();//拼接的操作序列
        ArrayList combineHeaders = new ArrayList();//拼接的操作序列表头

        public SaveActionDialog(ArrayList list,ArrayList headers)
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

            combineArray = list;
            combineHeaders = headers;
        }

        private void yes_Click(object sender, EventArgs e)
        {
            if (combineList.ContainsKey(name.Text.ToString()))
            {
                MessageBox.Show("操作名已存在！");
                return;
            }

            combineList[name.Text.ToString()] = combineArray;
            combineHeaderList[name.Text.ToString()] = combineHeaders;

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

            MessageBox.Show("保存成功！");
            this.Close();
        }

        private void no_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
