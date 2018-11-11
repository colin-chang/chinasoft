using Microsoft.Win32;

namespace Com.ChinaSoft.Utility
{
    /// <summary>
    /// 注册表帮助类
    /// </summary>
    public class RegistryHelper
    {
        /// <summary>
        /// 根节点
        /// </summary>
        RegistryKey root;

        /// <summary>
        /// 构建RegisterHelper实例
        /// </summary>
        /// <param name="subKey"></param>
        public RegistryHelper(RegistryRoot rootName, string subKey)
        {
            RegistryKey rk;
            switch (rootName)
            {
                case RegistryRoot.ClassesRoot:
                    rk = Registry.ClassesRoot;
                    break;
                case RegistryRoot.CurrentConfig:
                    rk = Registry.CurrentConfig;
                    break;
                case RegistryRoot.CurrentUser:
                    rk = Registry.CurrentUser;
                    break;
                case RegistryRoot.DynData:
                    rk = Registry.DynData;
                    break;
                case RegistryRoot.LocalMachine:
                    rk = Registry.LocalMachine;
                    break;
                case RegistryRoot.PerformanceData:
                    rk = Registry.PerformanceData;
                    break;
                case RegistryRoot.Users:
                    rk = Registry.Users;
                    break;
                default:
                    return;
            }
            root = rk.CreateSubKey(subKey);
        }

        /// <summary>
        /// 注册表访问器
        /// </summary>
        /// <param name="key">注册表项名称</param>
        /// <returns>注册表项值</returns>
        public object this[string key]
        {
            get
            {
                return root.GetValue(key);
            }
            set
            {
                root.SetValue(key, value);
            }
        }
    }

    public enum RegistryRoot
    {
        ClassesRoot,
        CurrentConfig,
        CurrentUser,
        DynData,
        LocalMachine,
        PerformanceData,
        Users
    }
}