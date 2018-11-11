using System.Windows;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.ViewModels;

namespace Com.ChinaSoft.AlertCenter
{
    /// <summary>
    ///     Interaction logic for LifetimeDetailWindow.xaml
    /// </summary>
    internal partial class LifetimeDetailWindow : Window
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="alertId">预警唯一标识</param>
        /// <param name="strategyId">预警策略的唯一标识</param>
        public LifetimeDetailWindow(string alertId, string strategyId)
        {
            InitializeComponent();

            Application.Current.Properties[PropertyKey.Key_LifetimeDetailWin] = this;
            Application.Current.Properties[PropertyKey.Key_LifetimeDetailPanel] = Panel;
            Application.Current.Properties[PropertyKey.Key_LifetimeDetailUIDispatcher] = Dispatcher;

            var vm = new LifetimeDetailViewModel(alertId, strategyId);
            DataContext = vm;
        }
    }
}