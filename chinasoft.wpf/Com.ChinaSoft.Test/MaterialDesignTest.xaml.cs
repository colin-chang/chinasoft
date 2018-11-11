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
    /// Interaction logic for MaterialDesignTest.xaml
    /// </summary>
    public partial class MaterialDesignTest : Window
    {
        public MaterialDesignTest()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ThisCalendar.SelectedDate?.ToString());
        }

        private void ThisCalendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
        }
    }
}
