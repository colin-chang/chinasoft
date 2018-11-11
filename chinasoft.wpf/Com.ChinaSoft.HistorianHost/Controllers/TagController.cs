using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ArchestrA;

namespace Com.ChinaSoft.HistorianService.Controllers
{
    /// <summary>
    /// Tag Controller class handles all the Requests for Tag resources.
    /// </summary>
    public class TagController : ApiController
    {
        public IEnumerable<HistorianTag> Get()
        {
            return null;
        }

        public HistorianTag Get(string tagName)
        {
            //return SpiritBuilder.GetTagInfo(tagName);
            return null;
        }

        public bool Post(HistorianTag tag)
        {
            return true;
        }
    }
}
