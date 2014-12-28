using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockiiPanel
{
    public partial class CustomDialog : Form
    {
        public CustomDialog()
        {
            InitializeComponent();

            stockCheckedListBox.Items.Add("日期");
            stockCheckedListBox.Items.Add("多头获利（元）");
            stockCheckedListBox.Items.Add("多空平衡（元）");
            stockCheckedListBox.Items.Add("卖价二（元）");
            stockCheckedListBox.Items.Add("现量");
            stockCheckedListBox.Items.Add("买量三");
            stockCheckedListBox.Items.Add("卖量三");
            stockCheckedListBox.Items.Add("空头回补");
            stockCheckedListBox.Items.Add("空头止损");
            stockCheckedListBox.Items.Add("现价（元）");
            stockCheckedListBox.Items.Add("换手（%）");
            stockCheckedListBox.Items.Add("买量二");
            stockCheckedListBox.Items.Add("流通股本（亿股）");
            stockCheckedListBox.Items.Add("内盘");
            stockCheckedListBox.Items.Add("更新日期");
            stockCheckedListBox.Items.Add("最低价流通市值（亿元）");
            stockCheckedListBox.Items.Add("强弱度");
            stockCheckedListBox.Items.Add("最低（元）");
            stockCheckedListBox.Items.Add("市盈率");
            stockCheckedListBox.Items.Add("委比");
            stockCheckedListBox.Items.Add("卖出价（元）");
            stockCheckedListBox.Items.Add("今开（元）");
            stockCheckedListBox.Items.Add("买入价（元）");
            stockCheckedListBox.Items.Add("笔涨跌");
            stockCheckedListBox.Items.Add("卖量一");
            stockCheckedListBox.Items.Add("流通市值（亿元）");
            stockCheckedListBox.Items.Add("日涨跌（元）");
            stockCheckedListBox.Items.Add("涨速（%）");
            stockCheckedListBox.Items.Add("最高（元）");
            stockCheckedListBox.Items.Add("买量一");
            stockCheckedListBox.Items.Add("量比");
            stockCheckedListBox.Items.Add("尾量差");
            stockCheckedListBox.Items.Add("外盘");
            stockCheckedListBox.Items.Add("卖量二");
            stockCheckedListBox.Items.Add("每笔均量");
            stockCheckedListBox.Items.Add("总市值（亿元）");
            stockCheckedListBox.Items.Add("最高价流通市值（亿元）");
            stockCheckedListBox.Items.Add("内外比");
            stockCheckedListBox.Items.Add("涨幅（%）");
            stockCheckedListBox.Items.Add("均价（元）");
            stockCheckedListBox.Items.Add("昨收（元）");
            stockCheckedListBox.Items.Add("总金额（亿元）");
            stockCheckedListBox.Items.Add("买价二（元）");
            stockCheckedListBox.Items.Add("振幅");
            stockCheckedListBox.Items.Add("总量");
            stockCheckedListBox.Items.Add("现价流通市值（亿元）");
            stockCheckedListBox.Items.Add("总股本（亿股）");
            stockCheckedListBox.Items.Add("均价流通市值（亿元）");
            stockCheckedListBox.Items.Add("多头止损（元）");
            stockCheckedListBox.Items.Add("卖价一（元）");
            stockCheckedListBox.Items.Add("每笔换手");
            stockCheckedListBox.Items.Add("活跃度");
            stockCheckedListBox.Items.Add("买价一（元）");
            stockCheckedListBox.Items.Add("买价三（元）");
            stockCheckedListBox.Items.Add("卖价三（元）");

            for (int i = 0; i < 57; ++i)
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
    }
}
