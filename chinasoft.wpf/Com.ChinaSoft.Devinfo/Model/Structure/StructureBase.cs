namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备结构基模型
    /// </summary>
    public abstract class StructureBase
    {
        public string Id { get; set; }

        public string BomId { get; set; }

        public string BomNm { get; set; }

        public string BomCode { get; set; }

        public string ParentBomId { get; set; }

        public string ParentBomNm { get; set; }

        public int SeqNum { get; set; }

        public int IsEffeted { get; set; }

        public string QueryCode { get; set; }

        public string BomEntryCode { get; set; }

        public string BomEntryId { get; set; }

        public int IsLeaf { get; set; }

        public int ChildrenNum { get; set; }

        public int DeleteFlag { get; set; }
    }
}
