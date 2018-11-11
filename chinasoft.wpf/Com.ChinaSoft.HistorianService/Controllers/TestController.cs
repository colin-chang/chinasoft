using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Com.ChinaSoft.HistorianService.Controllers
{
    /// <summary>
    /// 测试服务是否启动
    /// </summary>
    public class TestController : ApiController
    {
        public string[] Get()
        {
            return new string[] { "Value1", "Value2" };
        }
    }
}
