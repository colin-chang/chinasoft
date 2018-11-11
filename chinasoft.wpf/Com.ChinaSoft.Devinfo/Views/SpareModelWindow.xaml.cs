using Com.ChinaSoft.Devinfo.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.Views
{
    public partial class SpareModelWindow : Window
    {
        private SpareModelViewModel vm;

        public SpareModelWindow(string id)
        {
            InitializeComponent();
            vm = new SpareModelViewModel(this, id);
            DataContext = vm;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopupBrowser.IsOpen = ((TabControl)sender).SelectedIndex == 0;
        }
    }
}