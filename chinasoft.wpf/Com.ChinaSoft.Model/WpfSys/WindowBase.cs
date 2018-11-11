using Com.ChinaSoft.Utility.Communication;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Com.ChinaSoft.Model.WpfSys
{
    public abstract class WindowBase : Window
    {
        /// <summary>
        /// 调用者窗口句柄
        /// </summary>
        public IntPtr Hwnd { get; private set; }
        public WindowBase()
        {
            //添加Panle
            Panel panel = new Grid();
            this.AddChild(panel);

            //设置样式
            this.Height = 748;
            this.Width = 1280;

            //this.Left = 0;
            //this.Top = 250;
            //this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.WindowStyle = System.Windows.WindowStyle.None;
            //this.ShowInTaskbar = false;
            this.Topmost = true;
#if DEBUG
            this.Topmost = false;
#endif
            this.Background = new SolidColorBrush(Color.FromRgb(36, 38, 50));

            //缓存全局变量
            Application.Current.Properties["Panel"] = panel;
            Application.Current.Properties["UIDispatcher"] = this.Dispatcher;

            //显示指定页面
            this.Navigate2(this.GetStartUC() ?? this.MainUC);
            this.Title = this.WindowTitle;

            //设置源窗口句柄
            var parameters = this.GetStartParams();
            string handle = parameters?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(handle))
                return;
            this.Hwnd = new IntPtr(int.Parse(handle));

            //向调用者发送消息
            this.Loaded += (sender, e) => MessageHelper.SendMessage(Hwnd, (PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource).Handle, null);
        }

        public virtual UserControl MainUC
        {
            get
            {
                var ass = Assembly.GetEntryAssembly();
                var type = ass.GetType(ass.FullName.Split(',').FirstOrDefault() + ".Views.MainUC");
                if (type == null)
                    return null;
                return Activator.CreateInstance(type) as UserControl;
            }
        }

        public virtual string WindowTitle { get { return string.Empty; } }
    }
}
