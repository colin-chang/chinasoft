using System;
using System.Collections.Specialized;

namespace Com.ChinaSoft.DataAcquisition.Models
{
    [Serializable]
    public class QueryParam
    {
        /// <summary>
        /// 作为查询条件的Tag名称集合
        /// </summary>
        public StringCollection TagNames { get; set; }

        /// <summary>
        /// 最近多少秒。查询最近多少秒的数据
        /// </summary>
        public int LastSeconds { get; set; }
    }
}