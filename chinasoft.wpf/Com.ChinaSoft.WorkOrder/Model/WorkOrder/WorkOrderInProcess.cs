using Com.ChinaSoft.Utility.Number;

namespace Com.ChinaSoft.WorkOrder.Model.WorkOrder
{
    public class WorkOrderInProcess
    {
        private double? _bobbinQlt = 0;
        private double? _cullQlt = 0;
        private double? _filtertipQlt = 0;
        private double? _output = 0;
        private double? _scrapQlt = 0;

        /// <summary>
        ///     实际生产量
        /// </summary>
        public double? Output
        {
            get { return NumberUtility.FormatDoubleToFractionalDigits(_output); }
            set { _output = value; }
        }

        /// <summary>
        ///     烟丝消耗
        /// </summary>
        public double? ScrapQlt
        {
            get { return NumberUtility.FormatDoubleToFractionalDigits(_scrapQlt); }
            set { _scrapQlt = value; }
        }

        /// <summary>
        ///     嘴棒消耗
        /// </summary>
        public double? FiltertipQlt
        {
            get { return NumberUtility.FormatDoubleToFractionalDigits(_filtertipQlt); }
            set { _filtertipQlt = value; }
        }

        /// <summary>
        ///     盘纸消耗
        /// </summary>
        public double? BobbinQlt
        {
            get { return NumberUtility.FormatDoubleToFractionalDigits(_bobbinQlt); }
            set { _bobbinQlt = value; }
        }

        /// <summary>
        ///     剔除量
        /// </summary>
        public double? CullQlt
        {
            get { return NumberUtility.FormatDoubleToFractionalDigits(_cullQlt); }
            set { _cullQlt = value; }
        }
    }
}