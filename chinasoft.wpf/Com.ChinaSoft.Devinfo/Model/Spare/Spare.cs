using System.Linq;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class Spare
    {
        public string Id { get; set; }

        public int No { get; set; }

        public string ComId { get; set; }

        /// <summary>
        /// 零件名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string ElementModel { get; set; }

        /// <summary>
        /// 材质
        /// </summary>
        public string Nature { get; set; }

        /// <summary>
        /// 寿命
        /// </summary>
        public float UsedLife { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Supplier { get; set; }

        private string cds;
        /// <summary>
        /// 换件日期
        /// </summary>
        public string ChangeDateStr
        {
            get { return cds?.Split(' ').FirstOrDefault(); }
            set { cds = value; }
        }

        /// <summary>
        /// 寿命比例
        /// </summary>
        public float LifeRate { get; set; }
    }
}
