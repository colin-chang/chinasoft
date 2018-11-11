using System;
using System.Configuration;
using System.Reflection;
using System.Threading;
using ArchestrA;
using log4net;

[assembly: log4net.Config.Repository("Com.ChinaSoft.HistorianAccess")]
[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]

namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// Spirit Builder class handles data from Spirit lib.
    /// </summary>
    public static class SpiritBuilder
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ConnectionBuilder _connectionBuilder;
        private static TagBuilder _tagBuilder;
        private static StorageBuilder _storageBuilder;

        private static bool _isRunning;    // 是否进行中
        private static long _count;

        #endregion Static Fields

        #region Static Properties

        public static ConnectionBuilder ConnectionBuilder => _connectionBuilder;
        public static TagBuilder TagBuilder => _tagBuilder;
        public static StorageBuilder StorageBuilder => _storageBuilder;

        #endregion Static Properties

        #region Private Static Methods

        /// <summary>
        /// 停止采集
        /// </summary>
        private static void Stop()
        {
            _connectionBuilder?.Disconnect();
            _isRunning = false;
        }

        /// <summary>
        /// 从传感器读取数据
        /// </summary>
        /// <returns></returns>
        private static object ReadFromSpirit()
        {
            Logger.Info($"从传感器采集数据：{_count}");
            return _count.ToString();
        }

        /// <summary>
        /// 将数据写入 Historian
        /// </summary>
        private static bool WriteToHistorian(object data)
        {
            //_tagBuilder.AddTag(ConnectionBuilder.)

            Logger.Info($"将数据写入到Historian：{data}");
            return true;
        }

        private static void Connect()
        {
            string ServerName = ConfigurationManager.AppSettings["ServerName"];
            ushort TcpPort = Convert.ToUInt16(ConfigurationManager.AppSettings["TcpPort"]);
            string UserName = ConfigurationManager.AppSettings["UserName"];
            string Password = ConfigurationManager.AppSettings["Password"];
            uint MinSFDuration = Convert.ToUInt32(ConfigurationManager.AppSettings["MinSFDuration"]);
            uint FreeDiskSpace = Convert.ToUInt32(ConfigurationManager.AppSettings["FreeDiskSpace"]);
            string StoreForwardPath = ConfigurationManager.AppSettings["StoreForwardPath"];

            _connectionBuilder = new ConnectionBuilder();
            _tagBuilder = new TagBuilder();
            _connectionBuilder.UpdateStatus += ConnectionBuilder_UpdateStatus; ;
            _connectionBuilder.ConnectAsync(MinSFDuration, Password, false, ServerName, FreeDiskSpace, StoreForwardPath, TcpPort, UserName);
        }

        private static void ConnectionBuilder_UpdateStatus(ArchestrA.HistorianConnectionStatus status)
        {
            if (status.ErrorOccurred)
            {
                // 连接状态异常，停止采集
                Stop();
                Logger.Warn("-------------------------------------------------------------------");
                Logger.Warn($"Historian 连接出现错误：{status.Error.ErrorDescription}");
            }
            else
                Start();
        }

        #endregion Private Static Methods

        #region Public Static Methods

        /// <summary>
        /// 开始采集
        /// </summary>
        public static void Start()
        {
            if (_isRunning) return;

            if (_connectionBuilder == null) Connect();

            _isRunning = true;

            ThreadPool.QueueUserWorkItem(state =>
            {
                while (true)
                {
                    _count++;

                    if (!_isRunning) break;

                    object data = ReadFromSpirit();
                    bool ret = WriteToHistorian(data);
                }
            });
        }

        public static HistorianTag GetTagInfo(string tagName)
        {
            HistorianTag tag;
            HistorianAccessError error;
            ConnectionBuilder.HistorianAccess.GetTagInfoByName(tagName, false, out tag, out error);

            return tag;

        }

        #endregion Public Static Methods
    }
}