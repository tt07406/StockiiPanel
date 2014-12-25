using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace StockiiPanel
{
    public partial class Form1 : Form
    {
        MyProgressBar myBar = new MyProgressBar();

        // 代理定义，可以在Invoke时传入相应的参数
        private delegate void funHandle(int nValue);
        private funHandle myHandle = null;

        /// <summary>
        /// 线程函数中调用的函数
        /// </summary>
        private void ShowProgressBar()
        {
            myHandle = new funHandle(myBar.SetProgressValue);
            myBar.ShowDialog();
        }

        /// <summary>
        /// 线程函数，用于处理调用
        /// </summary>
        private void ThreadFun()
        {
            MethodInvoker mi = new MethodInvoker(ShowProgressBar);
            this.BeginInvoke(mi);

            System.Threading.Thread.Sleep(1000); // sleep to show window

            int i = 0;
            while(!stop)
            {
                ++i;
                System.Threading.Thread.Sleep(100);
                // 这里直接调用代理
                this.Invoke(this.myHandle, new object[] { (i % 100) });
                if (i > 100)
                    i = 0; 
            }

            stop = false;

            myBar.Close();
        }

        private bool stop = false;
    }
}
