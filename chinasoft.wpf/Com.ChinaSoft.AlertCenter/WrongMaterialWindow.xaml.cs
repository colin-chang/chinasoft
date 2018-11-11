using Com.ChinaSoft.AlertCenter.ViewModels;
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

namespace Com.ChinaSoft.AlertCenter
{
    /// <summary>
    /// Interaction logic for WrongMaterialWindow.xaml
    /// </summary>
    public partial class WrongMaterialWindow : Window
    {
        private readonly WrongMaterialViewModel vm;
        public WrongMaterialWindow()
        {
            InitializeComponent();
            vm = new WrongMaterialViewModel();
            DataContext = vm;
        }
    }
}
