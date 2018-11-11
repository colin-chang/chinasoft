using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Com.ChinaSoft.Resource.Converter
{
    /// <summary>
    /// 按钮组Active样式转换器
    /// </summary>
    public class ButtonGroupStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var panel = Application.Current.Properties["Panel"] as Panel;
            var uc = panel?.Children[0];
            var currentUC = uc as FrameworkElement;
            string basic = parameter?.ToString();
            Style defaultStyle = currentUC?.Resources[basic] as Style;

            if (values == null || values.Length < 2 || values[0] == null || values[1] == null)
                return defaultStyle;
            int status = (int)values[0], current;
            if (!int.TryParse(values[1].ToString(), out current))
                return defaultStyle;

            string key = status == current ? basic + "-active" : basic;
            return currentUC?.Resources[key] as Style;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }

    /// <summary>
    /// 可见性翻转转换器
    /// </summary>
    public class VisibilityReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    /// <summary>
    /// Bool-可见性转换器
    /// </summary>
    public class Bool2VisiblityReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((bool)value) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((Visibility)value) == Visibility.Visible ? false : true;
    }

    /// <summary>
    /// 按钮组激活状态转换器
    /// </summary>
    public class ButtonGroupActiveStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var panel = Application.Current.Properties["Panel"] as Panel;
            if (panel == null)
                return null;
            var currentUC = panel.Children[0] as FrameworkElement;
            if (currentUC == null)
                return null;
            if (parameter == null)
                return null;

            var paras = parameter as ButtonGroupActiveConverterParameter;
            string key = string.Equals(paras.Status, value?.ToString()) ? $"{paras.StyleKey}-active" : paras.StyleKey;
            return currentUC.Resources[key] as Style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
