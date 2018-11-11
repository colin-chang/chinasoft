using ArchestrA;
using log4net;
using System;
using System.Configuration;
using System.Reflection;

namespace Com.ChinaSoft.HistorianService
{
/*
    /// <summary>
    /// Historian Config class Initiliaze Historian Connection 
    /// and Start Spirit Builder.
    /// </summary>
    public class HistorianConfig
    {
        public static void Config()
        {
            SpiritBuilder.Start();
        }

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ConnectionBuilder ConnectionBuilder;
        private static readonly TagBuilder TagBuilder;

        //private static readonly string ServerName;
        //private static readonly ushort TcpPort;
        //private static readonly string UserName;
        //private static readonly string Password;
        //private static readonly uint MinSFDuration;
        //private static readonly uint FreeDiskSpace;
        //private static readonly string StoreForwardPath;

        //static HistorianConfig()
        //{
        //    ConnectionBuilder = new ConnectionBuilder();
        //    TagBuilder = new TagBuilder();

        //    ServerName = ConfigurationManager.AppSettings["ServerName"];
        //    TcpPort = Convert.ToUInt16(ConfigurationManager.AppSettings["TcpPort"]);
        //    UserName = ConfigurationManager.AppSettings["UserName"];
        //    Password = ConfigurationManager.AppSettings["Password"];
        //    MinSFDuration = Convert.ToUInt32(ConfigurationManager.AppSettings["MinSFDuration"]);
        //    FreeDiskSpace = Convert.ToUInt32(ConfigurationManager.AppSettings["FreeDiskSpace"]);
        //    StoreForwardPath = ConfigurationManager.AppSettings["StoreForwardPath"];
        //}

        //public static void Connect()
        //{
        //    ConnectionBuilder.UpdateStatus += ConnectionBuilder_UpdateStatus;
        //    ConnectionBuilder.ConnectAsync(MinSFDuration, Password, false, ServerName, FreeDiskSpace, StoreForwardPath, TcpPort, UserName);
        //}

        //public static void Disconnect()
        //{
        //    ConnectionBuilder.Disconnect();
        //}

        /// <summary>
        /// 连接 Historian 成功后回调该函数
        /// </summary>
        /// <param name="status"></param>
        private static void ConnectionBuilder_UpdateStatus(HistorianConnectionStatus status)
        {
            if (!status.ErrorOccurred)
            {
                Logger.Info("-------------------------------------------------------------------");
                Logger.Info($"Historian ConnectedToServer：{status?.ConnectedToServer}");
                Logger.Info($"Historian ConnectedToStoreForward：{status?.ConnectedToStoreForward}");
                Logger.Info($"Historian ConnectedToServerStorage：{status?.ConnectedToServerStorage}");

                // 连接状态正常，执行采集
                SpiritBuilder.Start();
            }
            else
            {
                // 连接状态异常，停止采集
                SpiritBuilder.Stop();
                Logger.Warn("-------------------------------------------------------------------");
                Logger.Warn($"Historian Connection ocurrs error:{status.Error.ErrorDescription}");
            }
        }
    }
*/
}