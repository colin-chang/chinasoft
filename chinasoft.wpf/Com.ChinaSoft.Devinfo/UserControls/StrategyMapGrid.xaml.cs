using Com.ChinaSoft.Devinfo.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.UserControls
{
    /// <summary>
    /// StrategyMapGrid.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyMapGrid : UserControl
    {
        public StrategyMapGrid()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private ObservableCollection<StrategyMap> maps = new ObservableCollection<StrategyMap>();
        public ObservableCollection<StrategyMap> Maps { get { return maps; } }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (SelectedCellsChanged == null)
                return;

            SelectedCellsChanged(sender, e);
        }

        public event SelectedCellsChangedEventHandler SelectedCellsChanged;
    }
}
