using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Com.ChinaSoft.UserControls
{
    /// <summary>
    /// AlertView.xaml 的交互逻辑
    /// </summary>
    public partial class AlertView : Window
    {
        AlerViewModel vm;
        public AlertView()
        {
            InitializeComponent();

            vm = new AlerViewModel();
            this.DataContext = vm;
        }

        public AlertView(DelegateCommand<IAlertViewItem> cmd, Window owner) : this()
        {
            this.Owner = owner;
            this.Command = cmd;
        }

        public DelegateCommand<IAlertViewItem> Command { set { vm.Command = value; } }

        public ObservableCollection<IAlertViewItem> Items { get { return vm.Items; } }

        public void AppendItem(IAlertViewItem item)
        {
            vm.Items.Add(item);
            box.Height = Items.Count * 71;
            this.Height = box.Height;
        }

        public void ClearItems()
        {
            vm.Items.Clear();
            this.Height = 0;
            box.Height = 0;
        }

        private void box_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!typeof(CancelItem).IsInstanceOfType((sender as ListBox).SelectedItem))
                vm.Command.Execute(box.SelectedItem as IAlertViewItem);

            this.DialogResult = true;
        }
    }

    public class AlerViewModel : BindableBase
    {
        public ObservableCollection<IAlertViewItem> Items { get; } = new ObservableCollection<IAlertViewItem>();

        public DelegateCommand<IAlertViewItem> Command
        {
            get;
            set;
        }
    }
}
