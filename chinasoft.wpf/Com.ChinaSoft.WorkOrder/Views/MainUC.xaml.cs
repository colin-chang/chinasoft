using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.WorkOrder.ViewModel;

namespace Com.ChinaSoft.WorkOrder.Views
{
    public partial class MainUC : UserControl
    {
        private MainViewModel _mainVm;

        public MainUC()
        {
            InitializeComponent();

            Loaded += MainUC_Loaded;
        }

        private void MainUC_Loaded(object sender, RoutedEventArgs e)
        {
            _mainVm = new MainViewModel();
            DataContext = _mainVm;

            ShowOrHideTabSeperator(-1);
        }

        /// <summary>
        ///     TabItem 选中事件
        /// </summary>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = TabWorkOrder.SelectedIndex;

            ShowOrHideTabSeperator(index);
        }

        /// <summary>
        ///     显示或隐藏TabItem分隔符
        /// </summary>
        private void ShowOrHideTabSeperator(int index)
        {
            if ((index == -1) || (index == 0))
            {
                ((TabItem) TabWorkOrder.Items[1]).Visibility = Visibility.Hidden;

                ((TabItem) TabWorkOrder.Items[3]).Visibility = Visibility.Visible;
            }
            else if (index == 2)
            {
                ((TabItem) TabWorkOrder.Items[1]).Visibility = Visibility.Hidden;
                ((TabItem) TabWorkOrder.Items[3]).Visibility = Visibility.Hidden;
            }
            else if (index == 4)
            {
                ((TabItem) TabWorkOrder.Items[3]).Visibility = Visibility.Hidden;
                ((TabItem) TabWorkOrder.Items[1]).Visibility = Visibility.Visible;
            }
        }
    }
}