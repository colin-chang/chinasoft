namespace Com.ChinaSoft.Devinfo.Model
{
    public class Lubrication : Detail
    {
        public string AbroadOilId { get; set; }

        /// <summary>
        /// 国外油
        /// </summary>
        public string AbroadOilName { get; set; }

        public string InternalOilId { get; set; }

        /// <summary>
        /// 国内油
        /// </summary>
        public string InternalOilName { get; set; }

        /// <summary>
        /// 加注量
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 润滑点数
        /// </summary>
        public float Points { get; set; }
    }
}
