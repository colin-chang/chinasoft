using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.ChinaSoft.Sercenter.Converter
{
    internal class DateTimeToFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = value as DateTime?;
            return dateTime?.ToString("yyyy-MM-dd HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}