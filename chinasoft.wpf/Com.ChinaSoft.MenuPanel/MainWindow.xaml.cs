using Com.ChinaSoft.Model.WpfSys;
using Com.ChinaSoft.Utility.Communication;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Com.ChinaSoft.MenuPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SchedulerWindow
    {
        /// <summary>
        /// 调用者窗口句柄
        /// </summary>
        public IntPtr Hwnd { get; private set; }

        private string appName;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            //设置源窗口句柄
            var parameters = this.GetStartParams();
            string handle = parameters?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(handle))
                return;
            this.Hwnd = new IntPtr(int.Parse(handle));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MenuIn();

            //向调用者发送消息
            MessageHelper.SendMessage(Hwnd, (PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource).Handle, null);
        }

        private void OpenChild(object sender, RoutedEventArgs e)
        {
            #region GetFileName

            Button btn = e.OriginalSource as Button;
            string app = btn?.Tag as string;
            if (string.IsNullOrWhiteSpace(app) || string.Equals(appName, app))
                return;
            this.appName = app;

            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, app + ".exe");
            if (!File.Exists(fileName))
            {
                MessageBox.Show(this, "程序文件丢失或不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            #endregion GetFileName

            MenuOut();

            StartChildApp(fileName);
        }

        private void MenuIn()
        {
            DoubleAnimation d = new DoubleAnimation(-130, 0, TimeSpan.FromSeconds(0.5));
            BeginAnimation(LeftProperty, d);
        }

        private void MenuOut()
        {
            DoubleAnimation d = new DoubleAnimation(0, -130, TimeSpan.FromSeconds(0.5));
            BeginAnimation(LeftProperty, d);
        }
    }
}