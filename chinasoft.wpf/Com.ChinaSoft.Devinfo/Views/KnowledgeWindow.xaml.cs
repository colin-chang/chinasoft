using Com.ChinaSoft.Devinfo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// KnowledgeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class KnowledgeWindow : Window
    {
        KnowledgeNodeViewModel vm;
        public KnowledgeWindow()
        {
            InitializeComponent();
        }

        public KnowledgeWindow(string text):this()
        {
            vm = new KnowledgeNodeViewModel(text);
            this.DataContext = vm;
        }
    }
}
