using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Com.ChinaSoft.UserControls.RoundCorner
{
    /// <summary>
    /// Interaction logic for RoundCornerWindow.xaml
    /// </summary>
    [ContentProperty("InnerContent")]
    public partial class RoundCornerWindow : System.Windows.Controls.UserControl
    {
        public RoundCornerWindow()
        {
            InitializeComponent();
        }

        public object InnerContent
        {
            get { return GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(RoundCornerWindow));

        private void CloseWindow_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive)?.Close();
        }
    }
}