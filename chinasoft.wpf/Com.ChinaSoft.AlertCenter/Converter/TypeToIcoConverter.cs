using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.Utility;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    /// <summary>
    ///     "预警类别"到"图标"的转换
    /// </summary>
    internal class TypeToIcoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var warnItem = value as WarningItem;
            var attribute = EnumHelper.GetEnumAttr<StrategyTypeAttribute>(warnItem.StrategyType);

            var icopath = attribute.Icopath;
            var ext = Path.GetExtension(attribute.Icopath);

            icopath = string.Concat(icopath.Trim(ext.ToCharArray()),
                warnItem.Status == StrategyStatus.Handled ? "_handled" : "", ext);

            return icopath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}