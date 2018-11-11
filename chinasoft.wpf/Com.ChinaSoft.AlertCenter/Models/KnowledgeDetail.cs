namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     专家经验详情
    /// </summary>
    public class KnowledgeDetail
    {
        /// <summary>
        ///     专家经验唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     部位唯一标识
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        ///     部位名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        ///     故障编码
        /// </summary>
        public string FaultCode { get; set; }

        /// <summary>
        ///     现象
        /// </summary>
        public string FailureContent { get; set; }

        /// <summary>
        ///     原因
        /// </summary>
        public string CaseContent { get; set; }

        /// <summary>
        ///     解决措施
        /// </summary>
        public string Trentent { get; set; }

        /// <summary>
        ///     机型唯一标识
        /// </summary>
        public string ModelId { get; set; }
    }
}