using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace System.Windows
{
    public static class WindowExp
    {
        /// <summary>
        /// 获取进程启动参数
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public static StartupArgs GetStartupArgs(this Window win) => Application.Current.Properties["StartupArgs"] as StartupArgs;

        /// <summary>
        /// 获取程序启动页面
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public static UserControl GetStartUC(this Window win) => GetStartupArgs(win)?.StartUC;

        /// <summary>
        /// 获取程序启动相关参数
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public static List<string> GetStartParams(this Window win) => GetStartupArgs(win)?.StartParams;
    }

    /// <summary>
    /// 进程启动参数
    /// </summary>
    public class StartupArgs
    {
        /// <summary>
        /// 启动页(TypeFullName)
        /// </summary>
        public string StartupUri { get; set; }

        /// <summary>
        /// 相关参数
        /// </summary>
        public List<string> StartParams { get; set; }

        public UserControl StartUC
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.StartupUri))
                    return null;

                Type type = Assembly.GetEntryAssembly().GetType(this.StartupUri);
                if (type == null)
                    return null;

                return Activator.CreateInstance(type) as UserControl;
            }
        }

        public StartupArgs()
        {
        }

        public StartupArgs(string startupUri)
            : this(startupUri, null)
        {
        }

        public StartupArgs(string startupUri, params string[] startParams)
        {
            this.StartupUri = startupUri;
            if (startParams != null && startParams.Length > 0)
                this.StartParams = new List<string>(startParams);
        }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public static StartupArgs Parse(string jsonArgs)
        {
            if (string.IsNullOrWhiteSpace(jsonArgs))
                return null;

            return JsonConvert.DeserializeObject<StartupArgs>(jsonArgs.ToJsonFormat());
        }
    }
}
