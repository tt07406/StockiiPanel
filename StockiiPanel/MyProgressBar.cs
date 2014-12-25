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
    public partial class MyProgressBar : Form
    {
        public MyProgressBar()
        {
            InitializeComponent();
        }

        public void SetProgressValue(int nValue)
        {
            circularProgressBarEx1.Value = nValue;
            if (nValue == 99)
                circularProgressBarEx1.Value = 1;  
        }
    }
}
