using Com.ChinaSoft.Devinfo.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// MainUC.xaml 的交互逻辑
    /// </summary>
    public partial class MainUC : UserControl
    {
        public MainUC()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void Navigate2(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var link = e.OriginalSource as Hyperlink;
            Type ucType = link?.Tag as Type;
            if (ucType?.IsSubclassOf(typeof(UserControl))!=true)
                return;

            this.Navigate2(Activator.CreateInstance(ucType) as UserControl);
        }
    }
}
