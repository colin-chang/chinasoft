using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    internal class StatusToDescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (StrategyStatus) value;

            var attribute = EnumHelper.GetEnumAttr<DescriptionAttribute>(status);

            return attribute.Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}