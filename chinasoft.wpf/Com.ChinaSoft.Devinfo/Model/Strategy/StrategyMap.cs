namespace Com.ChinaSoft.Devinfo.Model
{
    public class StrategyMap
    {
        public string Id { get; set; }
        /// <summary>
        /// bom节点名称
        /// </summary>
        public string BomName { get; set; }
        /// <summary>
        /// 机型唯一标识
        /// </summary>
        public string ModeId { get; set; }
        public string QueryCode { get; set; }
        /// <summary>
        /// 部位唯一标识
        /// </summary>
        public string PartId { get; set; }
        /// <summary>
        /// 点检数量
        /// </summary>
        public int CheckCount { get; set; }
        /// <summary>
        /// 维修数量
        /// </summary>
        public int RepairCount { get; set; }
        /// <summary>
        /// 保养数量
        /// </summary>
        public int MaintainCount { get; set; }
        /// <summary>
        /// 润滑数量
        /// </summary>
        public int LubricationCount { get; set; }
        /// <summary>
        /// 项修数量
        /// </summary>
        public int ProImprCount { get; set; }
        /// <summary>
        /// 大修数量
        /// </summary>
        public int BigImprCount { get; set; }
        
        public int DetectPoinCount { get; set; }
        /// <summary>
        /// 监测点数量
        /// </summary>
        public int DetectPointCount { get { return this.DetectPoinCount; } }
        /// <summary>
        /// 换件数量
        /// </summary>
        public int ChangePartCount { get; set; }
    }
}
