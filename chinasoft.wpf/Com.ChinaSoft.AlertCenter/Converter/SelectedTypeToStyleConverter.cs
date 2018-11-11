using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Models;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     转换"预警类别"筛选时的样式
    /// </summary>
    internal class SelectedTypeToStyleConverter : IValueConverter
    {
        private readonly string _styleKey; // 样式key
        private readonly StrategyType _type; // 预警类别

        public SelectedTypeToStyleConverter(StrategyType type, string styleKey)
        {
            _type = type;
            _styleKey = styleKey;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mainPanel = Application.Current.Properties[PropertyKey.Key_ParentWindowPanel] as Panel;

            var currentUc = mainPanel?.Children[0] as FrameworkElement;
            if (currentUc == null) return null;

            var vmSelectedType = (StrategyType) value;

            var style = vmSelectedType == _type
                ? currentUc.Resources[$"{_styleKey}-active"]
                : currentUc.Resources[_styleKey];

            return style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}