namespace Com.ChinaSoft.WorkOrder.Model.WorkOrder
{
    /// <summary>
    ///     工单辅料信息
    /// </summary>
    public class WorkOrderRecipe
    {
        /// <summary>
        ///     物料唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     物料名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     物料分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        ///     物料数量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        ///     是否替代件
        /// </summary>
        public string IsReplacePart { get; set; }

        /// <summary>
        ///     替代件优先级
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        ///     工单唯一标识
        /// </summary>
        public string WorkOrderId { get; set; }
    }
}