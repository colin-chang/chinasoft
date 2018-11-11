namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// Tag-Symbol 映射类
    /// </summary>
    public class TagSymbolMapping
    {
        /// <summary>
        /// Tag 名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 全局数据表名
        /// </summary>
        public string GlobalTable { get; set; }

        /// <summary>
        /// 符号地址名
        /// </summary>
        public string SymbolAddress { get; set; }

        /// <summary>
        /// 符号地址总长度
        /// </summary>
        public byte TotalLength { get; set; }

        /// <summary>
        /// 偏移
        /// </summary>
        public ushort Offset { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// 间隔时间（毫秒）
        /// </summary>
        public byte Interval { get; set; }

        /// <summary>
        /// 是否存储为 Streamed 类型的值
        /// </summary>
        public bool StoreStreamed { get; set; } = true;

        /// <summary>
        /// 描述
        /// </summary>

        public string Description { get; set; }
    }
}