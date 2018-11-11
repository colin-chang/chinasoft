using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.ChinaSoft.Sercenter.Converter
{
    /// <summary>
    ///     替换字符串中的换行符
    /// </summary>
    internal class ReplaceContentNewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var content = value as string;

            content = content?.Replace(Environment.NewLine, "  ");

            if (content?.Length > 55)
                content = content.Substring(0, 55) + "......";

            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}