using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.UserControls.ConfirmBox
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        private readonly MessageBoxButton _msgBoxButton;
        private readonly MessageBoxImage _msgBoxImage;

        public ConfirmWindow()
        {
            InitializeComponent();
        }

        public ConfirmWindow(string content, string title, MessageBoxButton msgBoxButton, MessageBoxImage msgBoxImage)
            : this()
        {
            _msgBoxButton = msgBoxButton;
            _msgBoxImage = msgBoxImage;
            TbTitle.Text = title;
            TbContent.Text = content;

            Loaded += ConfirmWindow_Loaded;
        }

        private void ConfirmWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowConfirmBoxImage(_msgBoxImage);
            ShowButton(_msgBoxButton);
        }

        /// <summary>
        /// 确定显示的图标样式
        /// </summary>
        /// <param name="msgBoxImage"><see cref="MessageBoxImage"/>枚举值</param>
        private void ShowConfirmBoxImage(MessageBoxImage msgBoxImage)
        {
            string styleKey = $"ConfirmBoxImage.{msgBoxImage}";
            ConfirmBoxImage.Style = FindResource(styleKey) as Style;
        }

        /// <summary>
        /// 确定要显示的按钮数量和类型
        /// </summary>
        /// <param name="msgBoxButton"><see cref="MessageBoxButton"/>枚举值</param>
        private void ShowButton(MessageBoxButton msgBoxButton)
        {
            var btnOk = new Button() { Content = "确认" };
            btnOk.TouchDown += OKEventHandler;
            btnOk.Click += OKEventHandler;
            btnOk.Style = this.Resources["Form-SubmitButton"] as Style;
            ButtonContainer.Children.Add(btnOk);

            if (msgBoxButton == MessageBoxButton.OKCancel)
            {
                var btnCancel = new Button() { Content = "取消" };
                btnCancel.TouchDown += CancelEventHandler;
                btnCancel.Click += CancelEventHandler;
                btnCancel.Style = this.Resources["Form-SubmitButton"] as Style;
                ButtonContainer.Children.Add(btnCancel);
            }
        }

        private void OKEventHandler(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            e.Handled = true;
        }

        private void CancelEventHandler(object sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void CloseWindow_TouchDown(object sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }
    }
}