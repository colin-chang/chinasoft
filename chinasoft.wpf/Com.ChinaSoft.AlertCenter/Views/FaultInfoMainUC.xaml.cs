using System.Windows.Controls;
using Com.ChinaSoft.UserControls.PageTransitions;

namespace Com.ChinaSoft.AlertCenter.Views
{
    /// <summary>
    ///     Interaction logic for FaultInfoMainUC.xaml
    /// </summary>
    internal partial class FaultInfoMainUC : UserControl
    {
        public FaultInfoMainUC()
        {
            InitializeComponent();

            // 显示"故障预警窗口"
            var faultInfoUC = new FaultInfoUC();
            pageTranstion_FaultInfo.TransitionType = PageTransitionType.None;
            pageTranstion_FaultInfo.ShowPage(faultInfoUC);
        }
    }
}