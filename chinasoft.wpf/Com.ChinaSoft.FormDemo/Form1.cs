using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.ChinaSoft.FormDemo
{
    public partial class Form1 : Form
    {
        //private readonly string xulrunnerPath = Application.StartupPath + "/xulrunner";
        //private const string testUrl = "https://www.alipay.com/";
        //private GeckoWebBrowser Browser;
        public Form1()
        {
            InitializeComponent();
            //Xpcom.Initialize(xulrunnerPath);
        }

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    Browser = new GeckoWebBrowser();
        //    Browser.Parent = this;
        //    Browser.Dock = DockStyle.Fill;
        //    Browser.Navigate(testUrl);
        //}
    }
}
