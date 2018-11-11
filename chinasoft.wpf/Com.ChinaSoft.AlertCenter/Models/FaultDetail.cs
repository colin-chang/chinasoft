using System;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     故障预警详情
    /// </summary>
    [Serializable]
    public class FaultDetail
    {
        /// <summary>
        ///     故障预警唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     状态(0.为启用 1.已启用)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        ///     预警逻辑表达公式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///     3d插图的唯一标识
        /// </summary>
        public string Appendfile3DId { get; set; }

        /// <summary>
        ///     3D插图信息
        /// </summary>
        public Appendfile Appendfile3D { get; set; }

        /// <summary>
        ///     2D插图唯一标识
        /// </summary>
        public string Appendfile2DId { get; set; }

        /// <summary>
        ///     2D插图信息
        /// </summary>
        public Appendfile Appendfile2D { get; set; }

        /// <summary>
        ///     机型唯一标识
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        ///     bom结构中的唯一标识
        /// </summary>
        public string BomId { get; set; }

        /// <summary>
        ///     部位唯一标识
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        ///     部位名称
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        ///     专家经验唯一标识
        /// </summary>
        public string EkId { get; set; }

        /// <summary>
        ///     专家经验
        /// </summary>
        public KnowledgeDetail Knowledge { get; set; }

        /// <summary>
        ///     预警等级
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        ///     预警影响程度
        /// </summary>
        public string Extent { get; set; }
    }
}