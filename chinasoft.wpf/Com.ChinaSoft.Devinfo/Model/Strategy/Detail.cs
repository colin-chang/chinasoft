namespace Com.ChinaSoft.Devinfo.Model
{
    public abstract class Detail
    {
        public string Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 处理标准
        /// </summary>
        public string BenchMark { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 机型唯一标识
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 部位唯一标识
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// 部位名称
        /// </summary>
        public string PartName { get; set; }

        public string QueryCode { get; set; }

        public string CycleUnitId { get; set; }

        /// <summary>
        /// 周期单位
        /// </summary>
        public string CycleUnitName { get; set; }

        /// <summary>
        /// 周期数值
        /// </summary>
        public float CycleCount { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public string DeviceStatus { get; set; }

        public string FashionId { get; set; }

        /// <summary>
        /// 处理方式
        /// </summary>
        public string FashionName { get; set; }

        /// <summary>
        /// 处理分类
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 处理内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 工时
        /// </summary>
        public float ManHour { get; set; }

        /// <summary>
        /// 处理专业名称
        /// </summary>
        public string MajorName { get; set; }
    }
}
