using System;
using System.Configuration;
using Com.ChinaSoft.Devinfo.Model.Common;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备结构2D模型文件
    /// </summary>
    public class Append2dFile:AppendFile
    {
        private string path;

        public override string Path
        {
            get { return path; }
            set { path = System.IO.Path.Combine(ConfigurationManager.AppSettings["FileUrl"],value).ToUrl(); }
        }
    }
}
