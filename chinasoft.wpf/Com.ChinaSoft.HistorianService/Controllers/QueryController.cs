using Com.ChinaSoft.DataAcquisition;
using Com.ChinaSoft.DataAcquisition.Models;
using System.Web.Http;

namespace Com.ChinaSoft.HistorianService.Controllers
{
    /// <summary>
    /// Tag Controller class handles all the Requests for Tag resources.
    /// </summary>
    public class QueryController : BaseController
    {
        private readonly DataAcquisitionMain dataAcquisition = DataAcquisitionMain.Instance;

        /// <summary>
        /// 根据 Tag 查询历史数据
        /// </summary>
        /// <param name="param">请求参数<see cref="QueryParam"/></param>
        /// <returns><see cref="ApiResult"/>对象</returns>
        [HttpPost]
        public ApiResult History([FromBody]QueryParam param)
        {
            // 执行查询
            var t = dataAcquisition.RetrieveHisotry(param);
            var data = t.Item2;

            if (!t.Item1)
                return GetErrorResponse(dataAcquisition.LastError);

            return new ApiResult(data);
        }

        /// <summary>
        /// 根据 Tag 查询实时数据
        /// </summary>
        /// <param name="tagNames">Tag 名称集合</param>
        /// <returns><see cref="ApiResult"/>对象</returns>
        [HttpPost]
        public ApiResult Live(QueryParam param)
        {
            var t = dataAcquisition.RetrieveLive(param?.TagNames);
            if (!t.Item1) return GetErrorResponse(dataAcquisition.LastError);

            return new ApiResult(t.Item2);
        }
    }
}