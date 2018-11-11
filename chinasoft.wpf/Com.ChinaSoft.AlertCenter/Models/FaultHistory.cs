using System;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     故障预警历史记录
    /// </summary>
    public class FaultHistory
    {
        /// <summary>
        ///     预警唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     部位名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        ///     预警时间
        /// </summary>
        public string AlertTimeStr { get; set; }

        /// <summary>
        ///     预警公式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///     处理人
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     预警策略唯一标识
        /// </summary>
        public string StrategyId { get; set; }

        #region 其他属性

        /// <summary>
        ///     预警时间的“年-月-日”格式
        /// </summary>
        public string AlertDate
        {
            get
            {
                DateTime alertTime;
                if (DateTime.TryParse(AlertTimeStr, out alertTime))
                    return alertTime.ToString("yyyy-MM-dd");
                return AlertTimeStr;
            }
        }

        #endregion
    }
}