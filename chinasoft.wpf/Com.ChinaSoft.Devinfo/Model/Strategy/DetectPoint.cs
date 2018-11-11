namespace Com.ChinaSoft.Devinfo.Model
{
    public class DetectPoint:Detail
    {
        /// <summary>
        /// 检测内容
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上限值
        /// </summary>
        public float UpperLimitValue { get; set; }

        /// <summary>
        /// 下限值
        /// </summary>
        public float DownLimitValue { get; set; }

        /// <summary>
        /// 采集频度
        /// </summary>
        public float Freq { get; set; }

        /// <summary>
        /// 采集值单位
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 采集频度单位
        /// </summary>
        public string FreqUnit { get; set; }

        /// <summary>
        /// 传感器位置
        /// </summary>
        public string Location { get; set; }
    }
}
