using System.Collections.Generic;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备结构列表对象
    /// </summary>
    public class StructureItem : StructureBase
    {
        public List<StructureItem> Items { get; set; }

        public string BusId { get; set; }

        public string Thumb
        {
            get { return $"/Images/Structure/{this.BomEntryCode}.png"; }
        }
    }
}
