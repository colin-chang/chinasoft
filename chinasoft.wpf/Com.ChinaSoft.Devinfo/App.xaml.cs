using System;
using System.Linq;
using System.Configuration;

using Com.ChinaSoft.Model.WpfSys;
using Com.ChinaSoft.Utility;
using System.Reflection;
using System.Windows;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace Com.ChinaSoft.Devinfo
{
    public partial class App : AppBase
    {
        public App()
        {
            //设置WebBrowser IE内核版本
            string msg;
            if (!SetWebBrowserVersion(out msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //注册Aspose
            new Aspose.Words.License().SetLicense(LicenseHelper.License.LStream);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegisterForRestartRecovery();//注册ARR(应用程序恢复和重启)
        }

        protected override void OnExit(ExitEventArgs e)
        {
            UnRegisterRestartRecovery();//注销ARR
            base.OnExit(e);
        }

        #region IE内核版本
        /// <summary>
        /// 获取本机IE主版本号
        /// </summary>
        /// <returns></returns>
        int? GetIEVersion()
        {
            RegistryHelper reg = new RegistryHelper(RegistryRoot.LocalMachine, @"SOFTWARE\Microsoft\Internet Explorer");
            string version = reg["svcVersion"] as string;
            if (string.IsNullOrWhiteSpace(version))
                return null;
            var mainVersion = version.Split('.').FirstOrDefault();
            if (string.IsNullOrWhiteSpace(mainVersion))
                return null;
            int v;
            if (!int.TryParse(mainVersion, out v))
                return null;
            return v;
        }

        /// <summary>
        /// 获取IE版本对应的注册表项值
        /// </summary>
        /// <param name="mainVersion"></param>
        /// <returns></returns>
        int GetIEVersionRegNumber(int mainVersion)
        {
            //11001 (0x2EDF) Internet Explorer 11. Webpages are displayed in IE11 Standards mode, regardless of the !DOCTYPE directive
            //11000 (0x2AF8) ：Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode
            //10000 (0x2710) ：Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.
            //10001 (0x2AF7) ：Internet Explorer 10. Webpages are displayed in IE10 Standards mode, regardless of the !DOCTYPE directive.
            //9999 (0x270F) ：Internet Explorer 9. Webpages are displayed in IE9 Standards mode, regardless of the !DOCTYPE directive.
            //9000 (0x2328) ：Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.
            //8888 (0x22B8) ：Webpages are displayed in IE8 Standards mode, regardless of the !DOCTYPE directive.
            //8000 (0x1F40) ：Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.
            //7000 (0x1B58) ：Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.
            switch (mainVersion)
            {
                //case 11: return 0x2EDF;
                case 11: return 0x2AF9;
                case 10: return 0x2710;
                case 9: return 0x270F;
                default: return 0;
            }
        }

        /// <summary>
        /// 设置WebBrowser IE内核版本
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SetWebBrowserVersion(out string msg)
        {
            int? version = GetIEVersion();
            if (version == null)
            {
                msg = "本机没有安装Internet Explorer,请安装Internet Explorer 9或更高版本!";
                return false;
            }
            if (version < 9)
            {
                msg = "本机安装Internet Explorer版本过低,请安装Internet Explorer 9或更高版本!";
                return false;
            }

            string regPath = ConfigurationManager.AppSettings[Environment.Is64BitOperatingSystem ? "WebBrowserRegistryPath_x64" : "WebBrowserRegistryPath_x86"];
            RegistryHelper reg = new RegistryHelper(RegistryRoot.LocalMachine, regPath);
            reg[Assembly.GetEntryAssembly().ManifestModule.Name] = GetIEVersionRegNumber((int)version);

            msg = null;
            return true;
        }
        #endregion

        #region 应用程序崩溃恢复
        /// <summary>
        /// 注册ARR
        /// </summary>
        private void RegisterForRestartRecovery()
        {
            RestartSettings restartSettings = new RestartSettings("restart",
                RestartRestrictions.NotOnReboot | RestartRestrictions.NotOnPatch);
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(restartSettings);

            RecoveryData data = new RecoveryData(new RecoveryCallback(PerformRecovery), null);
            RecoverySettings recoverySettings = new RecoverySettings(data, 0);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recoverySettings);
        }

        /// <summary>
        /// 注销ARR
        /// </summary>
        private void UnRegisterRestartRecovery()
        {
            ApplicationRestartRecoveryManager.UnregisterApplicationRestart();
            ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
        }

        private int PerformRecovery(object state)
        {
            bool isCanceled = ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
            if (isCanceled)
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(false);

            //recovery your work here ...

            ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
            return 0;
        }
        #endregion
    }
}
