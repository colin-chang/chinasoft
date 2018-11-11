namespace Com.ChinaSoft.AlertCenter.Models
{
    public class BomChangeInfo
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     bom结构唯一标识
        /// </summary>
        public string BomId { get; set; }

        /// <summary>
        ///     换件人
        /// </summary>
        public string InputUser { get; set; }

        /// <summary>
        ///     换件数量
        /// </summary>
        public string ChangeNum { get; set; }

        /// <summary>
        ///     换件原因
        /// </summary>
        public string ChangeReason { get; set; }

        /// <summary>
        ///     部位名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        ///     预警时间
        /// </summary>
        public string AlertTimeStr { get; set; }

        /// <summary>
        ///     换件日期
        /// </summary>
        public string ChangeDateStr { get; set; }
    }
}