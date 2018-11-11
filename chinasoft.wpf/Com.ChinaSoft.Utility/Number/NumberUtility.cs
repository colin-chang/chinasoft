using System;

namespace Com.ChinaSoft.Utility.Number
{
    /// <summary>
    /// 数值工具类
    /// </summary>
    public static class NumberUtility
    {
        /// <summary>
        /// 将Double类型数值转换为保留2位小数位
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double? FormatDoubleToFractionalDigits(double? val)
        {
            return val.HasValue ? Math.Round(val.Value, 2) : val;
        }
    }
}