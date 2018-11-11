namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     寿命预警-报修单详情
    /// </summary>
    public class RepairBillDetail
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     输入时间
        /// </summary>
        public string InputTime { get; set; }

        /// <summary>
        ///     报警唯一标识
        /// </summary>
        public string AlertId { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     报修单内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     是否发送给厂家(0.不发送 1.发送)
        /// </summary>
        public int IsSendFac { get; set; }

        /// <summary>
        ///     问题类型
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        ///     反馈途径(0.设备反馈 1.企业反馈 2.服务中心反馈)
        /// </summary>
        public int FbWay { get; set; }

        /// <summary>
        ///     状态（0.保存,1.提交）。保存后还可以修改，提交后只能查看不能再修改
        /// </summary>
        public RepairBillStatus Status { get; set; }

        /// <summary>
        ///     设备唯一标识
        /// </summary>
        public string EquipmentId { get; set; }
    }
}