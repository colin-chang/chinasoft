using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     日期的字符串类型转换为“年-月-日”格式
    /// </summary>
    internal class DateTimeStringToFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                DateTime dt;
                if (DateTime.TryParse(value.ToString(), out dt))
                    return dt.ToString("yyyy-MM-dd");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}