using System.Windows.Controls;
using Com.ChinaSoft.AlertCenter.ViewModels;

namespace Com.ChinaSoft.AlertCenter.Views
{
    /// <summary>
    ///     Interaction logic for KnowledgeBaseUC.xaml
    /// </summary>
    internal partial class KnowledgeBaseUC : UserControl
    {
        public KnowledgeBaseUC()
        {
            InitializeComponent();

            DataContext = new KnowledgeBaseViewModel();
        }
    }
}