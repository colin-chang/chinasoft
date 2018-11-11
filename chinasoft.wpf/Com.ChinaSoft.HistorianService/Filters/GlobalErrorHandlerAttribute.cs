using Com.ChinaSoft.Utility.Log;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Com.ChinaSoft.HistorianService.Filters
{
    /// <summary>
    /// 异常处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GlobalErrorHandlerAttribute : ExceptionFilterAttribute
    {
        private static ILogBuilder logger = Log4NetLogger.Instance;

        public override void OnException(HttpActionExecutedContext context)
        {
            HttpRequestMessage request = context.Request;
            HttpResponseMessage response = context.Response;

            if (context.Request.RequestUri != null)
            {
                logger.Error($"[URL:{context.ActionContext.Request.RequestUri}]==============" +
                             $"{context.Exception.Message}==============" +
                             $"{context.Exception.StackTrace}");
            }

            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
            else
            {
                context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, context.Exception);
            }
        }
    }
}