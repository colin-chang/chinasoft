using System.Windows.Controls;

using Com.ChinaSoft.Devinfo.ViewModels;
using Com.ChinaSoft.Devinfo.Model;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Prism.Commands;
using System.Windows;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// StrategyUC.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyUC : UserControl
    {
        StrategyViewModel vm;
        public StrategyUC()
        {
            this.InitializeComponent();
            vm = new StrategyViewModel(mg.Maps);
            this.DataContext = vm;

            mg.SelectedCellsChanged += DataGrid_SelectedCellsChanged;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            var cells = dg?.SelectedCells;
            if (cells == null || cells.Count <= 0)
                return;
            var cell = cells[0];
            string path = cell.Column.SortMemberPath;
            var map = cell.Item as StrategyMap;
            if (map == null)
                return;
            //点击部位
            if (string.Equals(path, "BomName"))
                vm.SelecteBomCommand.Execute(map);
            //点击数字
            else
            {
                if (!path.Contains("Impr"))
                    (vm.GetType().GetProperty(path.Replace("Count", "Command"))?.GetValue(vm) as DelegateCommand<StrategyMap>)?.Execute(map);
                else
                    vm.TechImprCommand.Execute(new { Map = map, ImprType = path.StartsWith("ProImpr") ? DetailTypeEnum.ProImpr : DetailTypeEnum.BigImpr });
                this.row_detail.Height = new GridLength(1,GridUnitType.Star);
            }
        }

        private void PathsUpdated(object sender, DataTransferEventArgs e)
        {
            e.Handled = true;

            var txt = e.OriginalSource as TextBlock;
            var paths = txt?.DataContext as ObservableCollection<StrategyMap>;
            if (paths == null)
                return;

            txt.Inlines.Clear();
            for (int i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                bool isLast = i == paths.Count - 1;
                Hyperlink lk = new Hyperlink(new Run(isLast ? path.BomName : $"{path.BomName} > ")) { Command = vm.PathCommand, CommandParameter = path, IsEnabled = !isLast };

                txt.Inlines.Add(lk);
            }
        }

        private void Close_Detail(object sender, System.Windows.RoutedEventArgs e)
        {
            this.row_detail.Height = new GridLength(0);
        }
    }
}
