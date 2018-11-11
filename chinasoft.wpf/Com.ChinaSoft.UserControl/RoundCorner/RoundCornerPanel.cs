using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Com.ChinaSoft.UserControls.RoundCorner
{
    [ContentProperty("InnerContent")]
    public class RoundCornerPanel : UserControl
    {
        public RoundCornerPanel()
        {
            Initialize();
        }

        private void Initialize()
        {
            var resource = new ResourceDictionary
            {
                Source = new Uri("/Com.ChinaSoft.UserControls;component/RoundCorner/RoundCornerDict.xaml",
                    UriKind.RelativeOrAbsolute)
            };
            Resources = resource;
        }


        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register("InnerContent", typeof(object), typeof(RoundCornerPanel));

        public object InnerContent
        {
            get { return GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RoundCornerPanel));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
