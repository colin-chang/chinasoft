using Com.ChinaSoft.Model.WpfSys;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Com.ChinaSoft.Scheduler
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : SchedulerWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string appName = string.Empty;

        private void OpenChild(object sender, RoutedEventArgs e)
        {
            #region GetFileName

            Button btn = e.OriginalSource as Button;
            string app = btn?.Tag as string;
            if (string.IsNullOrWhiteSpace(app))
                return;
            this.appName = app;

            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, app + ".exe");
            if (!File.Exists(fileName))
            {
                MessageBox.Show(this, "程序文件丢失或不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            #endregion GetFileName

            this.StartChildApp(fileName);

            e.Handled = true;
        }

        private void OpenMenu(object sender, RoutedEventArgs e)
        {
            #region Obsolete

            //Storyboard storyboard = Resources["MenuIn"] as Storyboard;
            //storyboard.Begin(PanelMenu);

            //e.Handled = true;

            #endregion Obsolete
        }
    }
}