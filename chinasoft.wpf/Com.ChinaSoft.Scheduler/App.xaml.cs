using System.Windows;
using System.Windows.Threading;

namespace Com.ChinaSoft.Scheduler
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("抱歉，程序出现错误！\r\n" + "错误消息：" + e.Exception.Message, "程序错误", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show("Fuck");
            e.Handled = true;
        }
    }
}
