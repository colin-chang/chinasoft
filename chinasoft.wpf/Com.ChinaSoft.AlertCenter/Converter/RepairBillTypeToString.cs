﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Com.ChinaSoft.AlertCenter.Converter
{
    internal class RepairBillTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}