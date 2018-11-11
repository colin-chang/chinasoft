namespace Com.ChinaSoft.WorkOrder.Model.WorkOrder
{
    /// <summary>
    ///     工艺标准
    /// </summary>
    public class WorkOrderTechStandards
    {
        /// <summary>
        ///     工艺标准唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     参数分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     参数值
        /// </summary>
        public string ParmValue { get; set; }

        /// <summary>
        ///     参数值的单位
        /// </summary>
        public string ParmUnit { get; set; }

        /// <summary>
        ///     参数范围
        /// </summary>
        public string Range { get; set; }

        /// <summary>
        ///     工单唯一标识
        /// </summary>
        public string WorkOrderId { get; set; }
    }
}