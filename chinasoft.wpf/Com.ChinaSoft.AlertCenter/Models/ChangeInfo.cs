using System;

namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     修改信息
    /// </summary>
    public class ChangeInfo
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     换件人
        /// </summary>
        public string InputUser { get; set; }

        /// <summary>
        ///     换件数量
        /// </summary>
        public string ChangeNum { get; set; }

        /// <summary>
        ///     换件原因
        /// </summary>
        public string ChangeReason { get; set; }

        /// <summary>
        ///     换件日期
        /// </summary>
        public string ChangeDateStr { get; set; }

        /// <summary>
        ///     换件日期“年-月-日”格式
        /// </summary>
        public string ChangeDateStrFormat
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParse(ChangeDateStr, out dt))
                    return dt.ToString("yyyy-MM-dd");
                return ChangeDateStr;
            }
        }
    }
}