namespace Com.ChinaSoft.WorkOrder.Model.WorkOrder
{
    /// <summary>
    ///     工单状态
    /// </summary>
    public enum WorkOrderStatus
    {
        /// <summary>
        ///     未执行
        /// </summary>
        NotStarted = 0,

        /// <summary>
        ///     进行中
        /// </summary>
        InProcess = 1,

        /// <summary>
        ///     已完成
        /// </summary>
        Done = 2
    }
}