namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 专家经验库节点折叠状态
    /// </summary>
    public enum CollapseStatusEnum
    {
        /// <summary>
        /// 全部折叠
        /// </summary>
        Collapsed,

        /// <summary>
        /// 仅Bom结构展开
        /// </summary>
        BomExpanded,

        /// <summary>
        /// 仅专家经验节点展开
        /// </summary>
        ExpExpanded,
    }
}
