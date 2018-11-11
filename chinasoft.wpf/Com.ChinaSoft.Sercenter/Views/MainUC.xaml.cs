using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.Sercenter.Converter;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Sercenter.ViewModels;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.Sercenter.Views
{
    /// <summary>
    ///     Interaction logic for MainUC.xaml
    /// </summary>
    public partial class MainUC : UserControl
    {
        private readonly MainViewModel _viewModel;

        public MainUC()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void LoadRepairbillStatusHandler(object sender, RoutedEventArgs e)
        {
            var panel = sender as StackPanel;
            var btns = panel?.Children.OfType<Button>();

            var countTo = btns.Count();
            for (var idx = 0; idx < countTo; idx++)
            {
                var btn = btns.ElementAt(idx);
                btn.Tag = 0;

                var curType = (RepairbillStatus) idx; // 由于枚举值从-1开始，这里将索引减1后再转换为枚举值
                var desc = EnumHelper.GetEnumAttr<DescriptionAttribute>(curType)?.Description;
                btn.Content = desc;
                btn.Command = _viewModel.SelectReparibillStatusCommand;
                btn.CommandParameter = curType;

                var styleKey = idx == 0
                    ? "button-group-first"
                    : idx == countTo - 1
                        ? "button-group-last"
                        : "button-group-item";

                var styleBinding = new Binding(nameof(_viewModel.SelectedStatus))
                {
                    Converter = new SelectedTypeToStyleConverter(curType, styleKey)
                };
                btn.SetBinding(StyleProperty, styleBinding);
            }
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if ((scrollViewer == null) || (scrollViewer.ScrollableHeight < 0)
                || (scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight)
                || (_viewModel.Pager >= _viewModel.TotalPages))
                return;

            ring.Show();

            _viewModel.Pager++;
            await _viewModel.BindRepairBillList(false);

            ring.Hide();
        }
    }
}