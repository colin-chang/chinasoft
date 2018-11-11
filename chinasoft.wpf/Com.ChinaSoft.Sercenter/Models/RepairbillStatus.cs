using System.ComponentModel;

namespace Com.ChinaSoft.Sercenter.Models
{
    /// <summary>
    ///     问题状态
    /// </summary>
    internal enum RepairbillStatus
    {
        /// <summary>
        ///     全部
        /// </summary>
        [Description("全部")] All = 0,

        /// <summary>
        ///     未处理（已提交）
        /// </summary>
        [Description("未处理")] Unhandled = 1,

        /// <summary>
        ///     已分配
        /// </summary>
        [Description("已分配")] Assigned = 2,

        /// <summary>
        ///     已处理
        /// </summary>
        [Description("已处理")] Handled = 3,

        /// <summary>
        ///     已评分
        /// </summary>
        [Description("已评价")] Scored = 4
    }
}