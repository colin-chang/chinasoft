using System.Collections.Generic;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 专家经验库节点（Bom/Exp）数据源对象
    /// </summary>
    public class ExperienceBaseNode
    {
        public string Id { get; set; }

        private string name;

        public string Name
        {
            get { return string.IsNullOrWhiteSpace(name) ? this.Content : name; }
            set { name = value; }
        }

        public string Content { get; set; }
        
        /// <summary>
        /// Name显示是否有省略部分
        /// </summary>
        public bool HasClipped { get; set; }

        /// <summary>
        /// 省略前原始Name
        /// </summary>
        public string OriginName { get; set; }

        public float Level { get; set; }

        /// <summary>
        /// 机型ID
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 部位ID
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// 现象ID
        /// </summary>
        public string FailureId { get; set; }

        /// <summary>
        /// 原因ID
        /// </summary>
        public string CauseId { get; set; }

        public string ParentId { get; set; }

        /// <summary>
        /// 父级Bom节点或Exp
        /// </summary>
        public ExperienceBaseNode Parent { get; set; }

        /// <summary>
        /// 是否有下级部位（仅Bom结构节点可用）
        /// </summary>
        public bool? HasExp { get; set; }

        /// <summary>
        /// 下级专家经验节点数量（仅Bom结构节点可用）
        /// </summary>
        public int ExpCount { get; set; }

        /// <summary>
        /// 子节点数量（仅Bom结构节点可用）
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        /// 是否为叶子节点（仅Bom结构节点可用）
        /// </summary>
        public bool Leaf { get; set; }

        public bool HasNoBomOrExp { get { return HasExp == false && Leaf; } }

        public string BusId { get; set; }


        /// <summary>
        /// 折叠状态
        /// </summary>
        public CollapseStatusEnum CollapseStatus { get; set; }

        public ExperienceBaseNode()
        {
            this.CollapseStatus = CollapseStatusEnum.Collapsed;
            this.Level = -1;
        }

        public string UniqueId
        {
            get
            {
                return $"{this.ModelId}-{this.PartId}-{this.FailureId}-{this.CauseId}{this.Id}";
            }
        }

        /// <summary>
        /// 子Bom结构（仅用于搜索）
        /// </summary>
        public List<ExperienceBaseNode> Items { get; set; }
    }
}
