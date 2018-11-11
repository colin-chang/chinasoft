using System;
using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.UserControls
{
    /// <summary>
    /// BindingWebBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class BindingWebBrowser : UserControl
    {
        public BindingWebBrowser()
        {
            InitializeComponent();
        }

        public WebBrowser Browser { get { return this.Wb; } }

        public void Refresh()
        {
            this.Wb.Refresh();
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(BindingWebBrowser), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as BindingWebBrowser).FindName("Wb") as WebBrowser).Source = e.NewValue == null ? null : new Uri(e.NewValue.ToString());
                }));
    }
}
