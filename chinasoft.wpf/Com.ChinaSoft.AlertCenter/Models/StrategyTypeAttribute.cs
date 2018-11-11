using System.ComponentModel;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     报警类别自定义Attribute
    /// </summary>
    public class StrategyTypeAttribute : DescriptionAttribute
    {
        public StrategyTypeAttribute(string description, string icopath = "")
            : base(description)
        {
            Icopath = icopath;
        }

        /// <summary>
        ///     Ico图片路径
        /// </summary>
        public string Icopath { get; set; }
    }
}