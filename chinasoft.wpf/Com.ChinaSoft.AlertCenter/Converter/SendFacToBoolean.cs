using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     报修单是否发送给厂家
    /// </summary>
    internal class SendFacToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}