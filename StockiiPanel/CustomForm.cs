using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace StockiiPanel
{
    public partial class CustomDialog : Form
    {
        private SerializableDictionary<string, string> rawDict = new SerializableDictionary<string, string>();

        public CustomDialog()
        {
            InitializeComponent();

            using (FileStream fileStream = new FileStream("raw.xml", FileMode.Open))
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                this.rawDict = (SerializableDictionary<string, string>)xmlFormatter.Deserialize(fileStream);
            }

            int k = 0;
            foreach (var item in rawDict)
            {
                if (k>1)
                    stockCheckedListBox.Items.Add(item.Value);
                k++;
            }
            

            for (int i = 0; i < k ; ++i)
            {
                comboBox1.Items.Add(i + "");
            }

            comboBox1.SelectedIndex = frozenNum;

            for (int j = 0; j < stockCheckedListBox.Items.Count; j++)
            {
                if (strCollected == string.Empty)
                {
                    strCollected = j + "";
                }
                else
                {
                    strCollected = strCollected + "," + j;
                }
                stockCheckedListBox.SetItemChecked(j, true);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                for (int j = 0; j < stockCheckedListBox.Items.Count; j++)
                    stockCheckedListBox.SetItemChecked(j, true);
            }
            else
            {
                for (int j = 0; j < stockCheckedListBox.Items.Count; j++)
                    stockCheckedListBox.SetItemChecked(j, false);
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            frozenNum = comboBox1.SelectedIndex;
            strCollected = string.Empty;
            for (int i = 0; i < stockCheckedListBox.Items.Count; i++)
            {
                if (stockCheckedListBox.GetItemChecked(i))
                {
                    if (strCollected == string.Empty)
                    {
                        strCollected = i + "";
                    }
                    else
                    {
                        strCollected = strCollected + "," + i;
                    }
                }
            }

            this.Close();
        }

        private string strCollected = string.Empty;
        private int frozenNum = 2;

        public int FrozenNum
        {
            get { return frozenNum; }
            set { frozenNum = value; }
        }

        public string StrCollected
        {
            get { return strCollected; }
            set { strCollected = value; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
