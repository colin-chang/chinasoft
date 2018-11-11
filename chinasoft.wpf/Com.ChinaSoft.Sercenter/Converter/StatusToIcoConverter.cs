using System;
using System.Globalization;
using System.Windows.Data;
using Com.ChinaSoft.Sercenter.Models;

namespace Com.ChinaSoft.Sercenter.Converter
{
    /// <summary>
    ///     报修状态和相应图标的转换器
    /// </summary>
    internal class StatusToIcoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (RepairbillStatus) value;

            var imgPath = $"/Images/ico/{status}.png";

            return imgPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}