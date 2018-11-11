using System;
using System.Configuration;

namespace Com.ChinaSoft.Devinfo.Model.Common
{
    public class ExplodeFile:AppendFile
    {
        private string path;
        /// <summary>
        /// 路径
        /// </summary>
        public override string Path
        {
            get { return path; }
            set { path = System.IO.Path.Combine(ConfigurationManager.AppSettings["ExplodeUrl"], value).ToUrl(); }
        }
    }
}
