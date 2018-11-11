using Com.ChinaSoft.UserControls.ConfirmBox;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Com.ChinaSoft.Model.WpfSys
{
    public class AppBase : Application
    {
        public AppBase()
        {
            if (StartupUri == null)
                StartupUri = StartupFrom;

            this.DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        public virtual Uri StartupFrom => new Uri("MainWindow.xaml", UriKind.Relative);

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ConfirmBox.Show("抱歉，程序出现错误！\r\n" + "错误消息：" + e.Exception.Message);

            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //进程启动相关参数
            if (e.Args.Length > 0)
                Properties["StartupArgs"] = StartupArgs.Parse(e.Args[0]);

            base.OnStartup(e);
        }
    }
}
