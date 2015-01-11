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

namespace StockiiPanel
{
    public partial class Form1 : RibbonForm
    {
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl.TabPages.Remove(tabControl.SelectedTab);
        }


        private void upItem_MouseDown(object sender, MouseEventArgs e)
        {
            boardMenuStrip.Show(this, ribbonBar4.Location.X + upContainer.Size.Width, boardMenuStrip.Height + upContainer.Size.Height + ribbonPanel3.Location.Y);
            sectionToolStripMenuItem.Visible = false;
            industryToolStripMenuItem.Visible = false;
            upToolStripMenuItem.Visible = false;
            downToolStripMenuItem.Visible = false;
            upToolStripMenuItem.ShowDropDown();
        }

        private void downItem_MouseDown(object sender, MouseEventArgs e)
        {

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
    }
}
