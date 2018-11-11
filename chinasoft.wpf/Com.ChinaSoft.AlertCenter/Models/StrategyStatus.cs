using System.ComponentModel;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     预警处理状态
    /// </summary>
    public enum StrategyStatus
    {
        /// <summary>
        ///     全部
        /// </summary>
        [Description("全部")] All = -1,

        /// <summary>
        ///     未处理
        /// </summary>
        [Description("未处理")] Unhandled = 0,

        /// <summary>
        ///     已处理
        /// </summary>
        [Description("已处理")] Handled = 1
    }
}