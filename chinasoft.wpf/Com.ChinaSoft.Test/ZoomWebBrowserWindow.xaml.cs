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

namespace Com.ChinaSoft.Test
{
    /// <summary>
    /// Interaction logic for ZoomWebBrowserWindow.xaml
    /// </summary>
    public partial class ZoomWebBrowserWindow : Window
    {
        public ZoomWebBrowserWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OOK");
        }
    }
}
