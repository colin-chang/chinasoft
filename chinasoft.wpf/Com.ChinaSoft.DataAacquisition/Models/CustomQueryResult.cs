using System;

namespace Com.ChinaSoft.DataAcquisition.Models
{
    /// <summary>
    /// 由于反序列化 HistoryQueryResult 对象时报错，这里使用DTO类
    /// </summary>
    public class CustomQueryResult
    {
        public string TagName { get; set; }
        public uint TagKey { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double Value { get; set; }
        public string StringValue { get; set; }
        public ushort Quality { get; set; }
        public uint QualityDetail { get; set; }
        public string RetrievalMode { get; set; }
        public string DataVersion { get; set; }
    }
}
