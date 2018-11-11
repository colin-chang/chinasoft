using Com.ChinaSoft.Model.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class QueryCodeResult : OthersJsonResult<QueryCode>
    {
        public string QueryCode { get { return this.Others.FirstOrDefault()?.Value; } }
    }

    public class QueryCode
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
