using System.Windows;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Sercenter.ViewModels;

namespace Com.ChinaSoft.Sercenter
{
    /// <summary>
    ///     Interaction logic for QuestionDetailWindow.xaml
    /// </summary>
    internal partial class QuestionDetailWindow : Window
    {
        private readonly Repairbill _item;

        public QuestionDetailWindow()
        {
            InitializeComponent();
        }

        public QuestionDetailWindow(Repairbill item) : this()
        {
            _item = item;

            //缓存全局变量
            Application.Current.Properties[PropertyKey.KeyQuestionDetailWin] = this; // 保存当前窗口的引用，关闭操作时需要使用
            Application.Current.Properties[PropertyKey.KeyQuestionDetailPanel] = Panel;
            Application.Current.Properties[PropertyKey.KeyQuestionDetailWinUiDispatcher] = Dispatcher;

            if (item.Status == RepairbillStatus.Unhandled)
            {
                Height -= 120;
                Width -= 100;
            }

            var viewModel = new QuestionViewModel(item.Id);
            DataContext = viewModel;

            Loaded += QuestionDetailWindow_Loaded;
        }

        private void QuestionDetailWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DetailUc.RepairbillStatus = _item.Status;
        }
    }
}