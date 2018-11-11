using Com.ChinaSoft.DataAcquisition.Models;
using System.Net.Http;
using System.Web.Http;

namespace Com.ChinaSoft.HistorianService.Controllers
{
    public class BaseController : ApiController
    {
        /// <summary>
        /// 创建相应错误消息的 <see cref="HttpResponseMessage"/>对象
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>包含响应内容的<see cref="ApiResult"/>对象</returns>
        protected ApiResult GetErrorResponse(string errorMessage)
        {
            return new ApiResult(null, errorMessage, ApiResponseType.Failed);
        }
    }
}