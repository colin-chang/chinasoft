using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Models;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     "报警状态"的样式转换
    /// </summary>
    internal class StatusToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var panel = Application.Current.Properties["Panel"] as Panel;

            var currentUC = panel?.Children[0] as FrameworkElement;
            if (currentUC == null) return null;


            var status = (StrategyStatus) value;

            var resource = status == StrategyStatus.Unhandled
                ? currentUC.Resources["ItemStatus.UnHandled"]
                : currentUC.Resources["ItemStatus.Handled"];

            return resource as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}