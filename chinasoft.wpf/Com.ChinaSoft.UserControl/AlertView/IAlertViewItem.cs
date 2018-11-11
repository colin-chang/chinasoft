using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Com.ChinaSoft.UserControls
{
    public interface IAlertViewItem
    {
        /// <summary>
        /// Item标识
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Item显示内容
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// 附加内容
        /// </summary>
        object Tag { get; set; }
    }
}
