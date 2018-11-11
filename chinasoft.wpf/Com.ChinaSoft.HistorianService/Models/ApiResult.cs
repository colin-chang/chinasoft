using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaSoft.HistorianService.Models
{
    /// <summary>
    /// API 响应结果
    /// </summary>
    public class ApiResult
    {
        public ApiResult()
        {
        }

        public ApiResult(object data, string errorMessage = null, ApiResponseType type = ApiResponseType.Successful)
        {
            Timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Type = type;
            ErrorMessage = errorMessage;
            Data = data;
        }

        public long Timestamp { get; set; }
        public ApiResponseType Type { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }

    public enum ApiResponseType
    {
        Failed = 0,
        Successful = 1
    }
}
