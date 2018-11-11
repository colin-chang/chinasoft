using System;
using System.Globalization;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     "预警类别"到"描述"的转换
    /// </summary>
    internal class TypeToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (StrategyType) value;
            var attribute = EnumHelper.GetEnumAttr<StrategyTypeAttribute>(type);
            var rtn = attribute.Description;

            return rtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}