using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Converter;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.ViewModels;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.AlertCenter.Views
{
    /// <summary>
    ///     MainUC.xaml 的交互逻辑
    /// </summary>
    internal partial class MainUC : UserControl
    {
        private readonly MainViewModel _vm;

        public MainUC()
        {
            _vm = new MainViewModel();
            DataContext = _vm;

            InitializeComponent();
        }

        /// <summary>
        ///     预警状态更改时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SelectedStrategyType_Changed(object sender, SelectionChangedEventArgs e)
        {
            var cbx = sender as ComboBox;

            if (cbx != null)
            {
                var selStatus = (StrategyStatus) cbx.SelectedItem;

                _vm.ResetPager();
                _vm.SearchStatus = selStatus;
            }

            ring?.Show();

            await _vm.LoadWarningsAsync();

            ring?.Hide();

            e.Handled = true;
        }

        /// <summary>
        ///     ScrollViewer滚动改变时的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;

            if ((sv == null) || (sv.ScrollableHeight <= 0)) return;
            if (sv.VerticalOffset < sv.ScrollableHeight) return;
            if (_vm.Pager >= _vm.TotalPagers) return;

            ring?.Show();

            _vm.Pager++;
            await _vm.LoadWarningsAsync(false);

            ring?.Hide();

            e.Handled = true;
        }

        /// <summary>
        ///     加载预警类别时的事件处理程序
        /// </summary>
        private void LoadStrategyTypeHandler(object sender, RoutedEventArgs e)
        {
            var panel = sender as StackPanel;
            var btns = panel?.Children.OfType<Button>();

            var countTo = btns.Count();
            for (var idx = 0; idx < countTo; idx++)
            {
                var btn = btns.ElementAt(idx);
                btn.Tag = 0;

                var curType = (StrategyType) (idx - 1); // 由于枚举值从-1开始，这里将索引减1后再转换为枚举值
                var desc = EnumHelper.GetEnumAttr<StrategyTypeAttribute>(curType)?.Description;
                btn.Content = desc;
                btn.Command = _vm.SelectStrategyTypeCommand;
                btn.CommandParameter = curType;

                var styleKey = idx == 0
                    ? "button-group-first"
                    : idx == countTo - 1
                        ? "button-group-last"
                        : "button-group-item";

                var styleBinding = new Binding("SelectedType")
                {
                    Converter = new SelectedTypeToStyleConverter(curType, styleKey)
                };
                btn.SetBinding(StyleProperty, styleBinding);
            }
        }
    }
}