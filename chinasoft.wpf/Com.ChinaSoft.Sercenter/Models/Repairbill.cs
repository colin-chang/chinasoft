using System;

namespace Com.ChinaSoft.Sercenter.Models
{
    /// <summary>
    ///     报修单
    /// </summary>
    internal class Repairbill
    {
        /// <summary>
        ///     报修单唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        public RepairbillStatus Status { get; set; }

        /// <summary>
        ///     反馈日期
        /// </summary>
        public DateTime? RecardDateStr { get; set; }

        /// <summary>
        ///     问题类型Id
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        ///     问题类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///     反馈人
        /// </summary>
        public string RecardUserName { get; set; }

        /// <summary>
        ///     烟厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        ///     反馈时间
        /// </summary>
        private string _inputTime = null;
        public object InputTime
        {
            get
            {
                return _inputTime;
            }
            set
            {
                if (value != null)
                    _inputTime = new DateTime((long)value).ToString("yyyy-MM-dd");
            }
        }



        /// <summary>
        ///     反馈人
        /// </summary>
        public string InputUser { get; set; }

        /// <summary>
        ///     设备唯一标识
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        ///     客服人员
        /// </summary>
        public string ServiceUser { get; set; }

        /// <summary>
        ///     受理时间
        /// </summary>
        public DateTime? ServiceAcceptTimeStr { get; set; }

        /// <summary>
        ///     转接时间
        /// </summary>
        public DateTime? ServiceDeliverTimeStr { get; set; }

        /// <summary>
        ///     专家人员
        /// </summary>
        public string ExpertUser { get; set; }

        /// <summary>
        ///     专家受理时间
        /// </summary>
        public DateTime? ExpertAcceptTimeStr { get; set; }

        /// <summary>
        ///     专家解决时间
        /// </summary>
        public DateTime? ExpertSolveTimeStr { get; set; }

        /// <summary>
        ///     专家解决能力评分
        /// </summary>
        public float AppraisePoint1 { get; set; }

        /// <summary>
        ///     专家解决问题态度评分
        /// </summary>
        public float AppraisePoint2 { get; set; }

        /// <summary>
        ///     客服支持服务态度评分
        /// </summary>
        public float AppraisePoint3 { get; set; }

        /// <summary>
        ///     评价内容
        /// </summary>
        public string AppraiseContent { get; set; }

        /// <summary>
        ///     评价时间
        /// </summary>
        public string AppraiseTime { get; set; }

        #region 额外属性

        //public string RecardDateStrFormat => RecardDateStr?.ToString("yyyy-MM-dd HH:mm")

        /// <summary>
        ///     年份
        /// </summary>
        public string Year => RecardDateStr?.Year.ToString();

        /// <summary>
        ///     月日
        /// </summary>
        public string MonthDay => RecardDateStr?.ToString("MM-dd");

        #endregion 额外属性
    }
}