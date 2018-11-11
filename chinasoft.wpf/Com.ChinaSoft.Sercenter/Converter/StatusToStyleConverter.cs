using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.Sercenter.Models;

namespace Com.ChinaSoft.Sercenter.Converter
{
    /// <summary>
    ///     状态和响应样式的转换器
    /// </summary>
    internal class StatusToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var panel = Application.Current.Properties[PropertyKey.KeyParentWindowPanel] as Panel;

            var currentUc = panel?.Children[0] as FrameworkElement;
            if (currentUc == null) return null;

            var status = (RepairbillStatus) value;

            var resource = currentUc.Resources[$"ItemStatus.{status}"];

            return resource as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}