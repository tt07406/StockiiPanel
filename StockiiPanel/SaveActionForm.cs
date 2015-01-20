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
        ArrayList combineArray = new ArrayList();//拼接的操作序列

        public SaveActionDialog(ArrayList list)
        {
            InitializeComponent();

            using (FileStream fileStream = new FileStream("actions.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                this.combineList = (SerializableDictionary<string, ArrayList>)xmlFormatter.Deserialize(fileStream);
            }

            if (combineList == null)
            {
                combineList = new SerializableDictionary<string, ArrayList>();
            }

            combineArray = list;
        }

        private void yes_Click(object sender, EventArgs e)
        {
            if (combineList.ContainsKey(name.Text.ToString()))
            {
                MessageBox.Show("操作名已存在！");
                return;
            }

            combineList[name.Text.ToString()] = combineArray;

            //保存到外部文件
            using (FileStream fileStream = new FileStream("actions.xml", FileMode.Create))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, ArrayList>));
                xmlFormatter.Serialize(fileStream, this.combineList);
            }
            this.Close();
        }

        private void no_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
