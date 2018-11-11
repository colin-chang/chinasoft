using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.Sercenter.Converter
{
    /// <summary>
    ///     报修状态和相应文字描述的转换器
    /// </summary>
    internal class StatusToDescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (RepairbillStatus) value;
            var attribute = EnumHelper.GetEnumAttr<DescriptionAttribute>(type);
            var rtn = attribute.Description;

            return rtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}