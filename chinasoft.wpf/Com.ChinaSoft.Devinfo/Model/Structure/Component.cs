namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备结构零部件
    /// </summary>
    public class Component : StructureBase
    {
        public string BusId { get; set; }

        public string BusName { get; set; }

        public string BusCode { get; set; }

        public int Num { get; set; }
    }
}
