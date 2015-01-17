using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockiiPanel
{
    public partial class CustomGrid : UserControl
    {
        private bool _customSort = false;

        public bool CustomSort
        {
            get { return _customSort; }
            set { _customSort = value; }
        }
        private bool _allowPaging = true;

        public bool AllowPaging
        {
            get { return _allowPaging; }
            set { _allowPaging = value; }
        }
        private bool _allowCombine = false;

        public bool AllowCombine
        {
            get { return _allowCombine; }
            set { _allowCombine = value; }
        }
        //private bool _allowDump = true;

        //public bool AllowDump
        //{
        //    get { return _allowDump; }
        //    set { _allowDump = value; }
        //}
        private DataTable _dataTable = null;

        public DataTable DataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }
        public CustomGrid()
        {
            InitializeComponent();
            if (!_allowPaging)
            {
                moreButton.Visible = false;
                pageLabel.Visible = false;
                allButton.Visible = false;
            }
        }
        public void bind(DataTable dt)
        {
            if (dt != null)
            {
                dataGridView.DataSource = dt;
                _dataTable = dt;
            }
        }

        private void moreButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("parent: {0}", this.ParentForm.ToString());
        }

        private void allButton_Click(object sender, EventArgs e)
        {

        }

        private void rawContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView.RowCount > 0)
            {
                for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                    rawContextMenuStrip.Items[i].Visible = true;
            }
            else
            {
                for (int i = 0; i < rawContextMenuStrip.Items.Count; ++i)
                    rawContextMenuStrip.Items[i].Visible = false;
            }
            if (!_allowCombine)
            {
                combinePageToolStripMenuItem.Visible = false;
                combineSelectToolStripMenuItem.Visible = false;
            }
            
        }
    }
}
