using System.Windows;
using System.Windows.Controls;

using Com.ChinaSoft.Devinfo.ViewModels;

namespace Com.ChinaSoft.Devinfo.Views
{
    public partial class ChangeInfoWindow : Window
    {
        ChangeInfoViewModel vm;
        public ChangeInfoWindow(string id)
        {
            InitializeComponent();
            vm = new ChangeInfoViewModel(this, id);
            this.DataContext = vm;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.ShowForm = tab.SelectedIndex == 0;
        }

        public void ShowHistory()
        {
            tab.SelectedIndex = 1;
        }

        private void Submit_Touch(object sender, System.Windows.Input.TouchEventArgs e)
        {
            vm.SubmitCommand.Execute();
        }

        private void Cancel_Touch(object sender, System.Windows.Input.TouchEventArgs e)
        {
            vm.CancelCommand.Execute();
        }
    }
}
