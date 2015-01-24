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
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// 启动画面本身
        /// </summary>
        static SplashScreen instance;

        /// <summary>
        /// 显示的图片
        /// </summary>
        Bitmap bitmap;

        public static SplashScreen Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public SplashScreen()
        {
            InitializeComponent();

            // 设置窗体的类型
            const string showInfo = "正在初始化股票信息，请稍后...";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            bitmap = new Bitmap(Properties.Resources.lauching);
            ClientSize = bitmap.Size;

            using (Font font = new Font("Consoles", 8))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawString(showInfo, font, Brushes.Gray, 320, 241);
                }
            }

            BackgroundImage = bitmap;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static void ShowSplashScreen()
        {
            instance = new SplashScreen();
            instance.Show();
        }
    }
}
