using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.Sercenter.Models;

namespace Com.ChinaSoft.Sercenter.Converter
{
    /// <summary>
    ///     标题栏选中的状态和样式的转换器
    /// </summary>
    internal class SelectedTypeToStyleConverter : IValueConverter
    {
        private readonly string _styleKey; // 样式key
        private readonly RepairbillStatus _type; // 预警类别

        public SelectedTypeToStyleConverter(RepairbillStatus type, string styleKey)
        {
            _type = type;
            _styleKey = styleKey;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mainPanel = Application.Current.Properties[PropertyKey.KeyParentWindowPanel] as Panel;

            var currentUc = mainPanel?.Children[0] as FrameworkElement;
            if (currentUc == null) return null;

            var vmSelectedType = (RepairbillStatus) value;

            var style = vmSelectedType == _type
                ? currentUc.Resources[$"{_styleKey}-active"]
                : currentUc.Resources[_styleKey];

            return style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}