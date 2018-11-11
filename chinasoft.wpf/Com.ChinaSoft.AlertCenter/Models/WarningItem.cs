using System;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     预警记录
    /// </summary>
    [Serializable]
    public class WarningItem
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     预警内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     预警时间
        /// </summary>
        public string AlertTimeStr { get; set; }

        /// <summary>
        ///     处理时间
        /// </summary>
        public string DisposeTimeStr { get; set; }

        /// <summary>
        ///     处理状态(0.未处理 1.已处理)
        /// </summary>
        public StrategyStatus Status { get; set; }

        /// <summary>
        ///     设备唯一标识
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        ///     预警类型(0.故障预警 1.寿命预警 2.错料预警)
        /// </summary>
        public StrategyType StrategyType { get; set; }

        /// <summary>
        ///     预警策略的唯一标识
        /// </summary>
        public string StrategyId { get; set; }

        #region 其他属性

        /// <summary>
        ///     预警时间的年份
        /// </summary>
        public string AlertYear
        {
            get
            {
                DateTime alertTime;
                if (DateTime.TryParse(AlertTimeStr, out alertTime))
                    return alertTime.Year.ToString();
                return AlertTimeStr;
            }
        }

        /// <summary>
        ///     预警时间的“月-日”格式
        /// </summary>
        public string AlertDay
        {
            get
            {
                DateTime alertTime;
                if (DateTime.TryParse(AlertTimeStr, out alertTime))
                    return alertTime.ToString("MM-dd");
                return AlertTimeStr;
            }
        }

        #endregion 其他属性
    }
}