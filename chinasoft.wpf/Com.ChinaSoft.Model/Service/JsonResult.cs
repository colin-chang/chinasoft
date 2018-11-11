using System.Collections.Generic;

namespace Com.ChinaSoft.Model.Service
{
    /// <summary>
    /// API返回Json对象基模型
    /// </summary>
    public class JsonResult
    {
        /// <summary>
        /// 是否成功(0.成功 1.失败)
        /// </summary>
        public int Result { get; set; }

        public bool Success => this.Result == 0;

        /// <summary>
        /// 状态值
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }

        public dynamic Others { get; set; }

        /// <summary>
        /// Item总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 第一页页码
        /// </summary>
        public int TopPageNo { get; set; }
        /// <summary>
        /// 最后一页页码
        /// </summary>
        public int BottomPageNo { get; set; }
        /// <summary>
        /// 下一页页码
        /// </summary>
        public int NextPageNo { get; set; }
        /// <summary>
        /// 上一页页码
        /// </summary>
        public int PreviousPageNo { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
    }

    public class ItemsJsonResult<T> : JsonResult where T : class, new()
    {
        public List<T> Item { get; set; }
    }

    public class ObjJsonResult<T> : JsonResult where T : class, new()
    {
        public T Obj { get; set; }
    }
    public class OthersJsonResult<T> : JsonResult where T : class, new()
    {
        public new List<T> Others { get; set; }
    }
}