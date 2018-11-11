using System;
using System.Configuration;

using Com.ChinaSoft.Devinfo.Model.Common;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备结构3D模型文件
    /// </summary>
    public class Append3dFile:AppendFile
    {
        private string path;
        /// <summary>
        /// 路径
        /// </summary>
        public override string Path
        {
            get { return path; }
            set { path = System.IO.Path.Combine(ConfigurationManager.AppSettings["3DUrl"], value).ToUrl(); }
        }
    }
}
