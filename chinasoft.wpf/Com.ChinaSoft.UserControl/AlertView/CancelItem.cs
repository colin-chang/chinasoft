using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaSoft.UserControls
{
    public class CancelItem : IAlertViewItem
    {
        public string Id { get; set; } = "CANCEL";

        public string Text { get; set; } = "取消";

        public object Tag { get; set; }
    }
}
