using System.Windows;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.ViewModels;

namespace Com.ChinaSoft.AlertCenter
{
    /// <summary>
    ///     Interaction logic for AlertDetailWinow.xaml
    /// </summary>
    internal partial class FaultDetailWinow : Window
    {
        private readonly FaultDetailViewModel vm;

        public FaultDetailWinow(string alertId, string strategyId)
        {
            InitializeComponent();

            //缓存全局变量
            Application.Current.Properties[PropertyKey.Key_FaultDetailWin] = this; // 保存当前窗口的引用，关闭操作时需要使用
            Application.Current.Properties[PropertyKey.Key_FaultDetailWinPanel] = Panel;
            Application.Current.Properties[PropertyKey.Key_FaultDetailWinUIDispatcher] = Dispatcher;

            vm = new FaultDetailViewModel(alertId, strategyId);
            DataContext = vm;
        }
    }
}