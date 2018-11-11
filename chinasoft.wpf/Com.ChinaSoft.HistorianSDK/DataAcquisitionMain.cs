using ArchestrA;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// 数据采集类
    /// 1.加载 TAG-SymbolAddress Mapping 文本文件
    /// 2.连接 Historian，写入 Tags
    /// 3.遍历 Mapping，根据符号地址采集设备数据，写入 Historian 中
    /// 4. 读取 Historian 历史和实时数据
    /// </summary>
    public class DataAcquisitionMain
    {
        #region 声明变量

        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IList<TagSymbolMapping> _tagSymbolMappings = new List<TagSymbolMapping>();
        private readonly string _mappingFileName;

        private string _lastError;
        private string _logMessage;

        private HistorianConnectionStatus _connectionStatus;
        private ConnectionBuilder _connectionBuilder;
        private TagBuilder _tagBuilder;
        private StorageBuilder _storageBuilder;

        #endregion 声明变量

        #region Properties

        public string LastError => _lastError;
        public string LogMessage => _logMessage;
        public IList<TagSymbolMapping> TagSymbolMappings => _tagSymbolMappings;

        public ConnectionBuilder ConnectionBuilder => _connectionBuilder;

        public TagBuilder TagBuilder => _tagBuilder;
        public StorageBuilder StorageBuilder => _storageBuilder;

        #endregion Properties

        #region Constructor

        public DataAcquisitionMain(string mappingFileName)
        {
            _mappingFileName = !string.IsNullOrEmpty(mappingFileName) ? mappingFileName : @"Data\TagSymbolMapping.txt";
        }

        #endregion Constructor

        #region 1. 加载映射文件

        public bool LoadMappingFile()
        {
            try
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), _mappingFileName)))
                {
                    _lastError = "当前应用程序根目录下，不存在 <Data\\TagSymbolMapping.txt>.";
                    return false;
                }

                string[] allRows = File.ReadAllLines(_mappingFileName);
                if (allRows.Length <= 0)
                {
                    _lastError = "映射文件是空的.";
                    return false;
                }

                foreach (string row in allRows)
                {
                    string[] cols = row.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (cols.Length <= 0) continue;

                    _tagSymbolMappings.Add(ConvertToMapping(cols));
                }
            }
            catch (FileNotFoundException ex)
            {
                _lastError = ex.Message;
            }
            catch (FormatException ex)
            {
                _lastError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 将映射文件的每一行转化为一个 <see cref="TagSymbolMapping"/> 对象
        /// </summary>
        /// <param name="col">映射文件一行的所有列</param>
        /// <returns><see cref="TagSymbolMapping"/>对象</returns>
        private TagSymbolMapping ConvertToMapping(string[] col)
        {
            var mapping = new TagSymbolMapping();
            mapping.TagName = col[0];
            mapping.GlobalTable = col[1];
            mapping.SymbolAddress = col[2];
            mapping.TotalLength = Convert.ToByte(col[3]);
            mapping.Offset = Convert.ToUInt16(col[4]);
            mapping.Length = Convert.ToByte(col[5]);
            mapping.Interval = Convert.ToByte(col[6]);
            mapping.Description = col[7];

            return mapping;
        }

        #endregion 1. 加载映射文件

        #region 2.连接 Historian，写入 Tags

        public bool WriteTags()
        {
            ConnectHistorian();

            foreach (TagSymbolMapping mapping in TagSymbolMappings)
            {
                if (!_connectionStatus.ErrorOccurred)
                {
                    if (!_tagBuilder.AddTag(_connectionBuilder.HistorianAccess, mapping.TagName, mapping.Description,
                        false, HistorianDataType.Double, HistorianStorageType.Delta))
                    {
                        _lastError =
                            $"【ERROR】写入TAG时出错，TAG名称为<{mapping.TagName}>，错误消息为：<{_tagBuilder.LastError.ErrorDescription}>";
                        logger.Error(_lastError);
                        return false;
                    }
                }
                else
                {
                    _lastError = $"【ERROR】写入TAG时发生连接错误，已停止写入，错误消息为：<{_connectionStatus.Error.ErrorDescription}>";
                    logger.Error(_lastError);
                    return false;
                }
            }

            _logMessage = $"成功写入全部TAG，总共写入数量： {_tagBuilder.TagsAdded.Count}.";
            return true;
        }

        public bool ConnectHistorian()
        {
            _connectionBuilder = new ConnectionBuilder();
            //_connectionBuilder.UpdateStatus += ConnectionBuilder_UpdateStatus;

            HistorianConnectionArgs connectionArgs = LoadConnectionArgs();

            _connectionStatus = _connectionBuilder.Connect(connectionArgs.MinStoreForwardDuration,
                connectionArgs.Password,
                false, connectionArgs.ServerName, connectionArgs.StoreForwardFreeDiskSpace,
                connectionArgs.StoreForwardPath, connectionArgs.TcpPort, connectionArgs.UserName);

            _tagBuilder = new TagBuilder();
            _storageBuilder = new StorageBuilder();

            if (_connectionStatus?.Error != null) _lastError = _connectionStatus.Error.ErrorDescription;

            return _connectionStatus != null && _connectionStatus.ErrorOccurred == false;
        }

        /// <summary>
        /// 读取 Historian 连接参数
        /// </summary>
        /// <returns>包含参数信息的<see cref="HistorianConnectionArgs"/>对象</returns>
        private HistorianConnectionArgs LoadConnectionArgs()
        {
            var args = new HistorianConnectionArgs();
            string serverName = ConfigurationManager.AppSettings["ServerName"];
            ushort tcpPort = Convert.ToUInt16(ConfigurationManager.AppSettings["TcpPort"]);
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            uint minSfDuration = Convert.ToUInt32(ConfigurationManager.AppSettings["MinSFDuration"]);
            uint freeDiskSpace = Convert.ToUInt32(ConfigurationManager.AppSettings["FreeDiskSpace"]);
            string storeForwardPath = ConfigurationManager.AppSettings["StoreForwardPath"];

            args.ServerName = serverName;
            args.TcpPort = tcpPort;
            args.UserName = userName;
            args.Password = password;
            args.MinStoreForwardDuration = minSfDuration;
            args.StoreForwardFreeDiskSpace = freeDiskSpace;
            args.StoreForwardPath = storeForwardPath;

            return args;
        }

        #endregion 2.连接 Historian，写入 Tags

        #region 3.采集设备数据，写入 Historian 中

        // 3.1 遍历TAG，取出符号地址等参数
        // 3.2 根据符号地址等参数，调用HMI，获取返回值
        // 3.3. 将返回值存储到相应TAG中

        public void StoreValuesToHistorian()
        {
            bool isCompleted = WriteTags();
            if (!isCompleted)
            {
                logger.Error(_lastError);
                return;
            }

            foreach (TagSymbolMapping mapping in TagSymbolMappings)
            {
                // 调用 HMI 获取设备数据
                object hmiValue = HMILibOperation.GetSymbolicAdrData(mapping);
                if (hmiValue == null) continue;

                if (!StoreOneValueToHistorian(mapping, hmiValue))
                {
                }
            }
        }

        public bool StoreOneValueToHistorian(TagSymbolMapping mapping, object hmiValue)
        {
            HistorianTag tag;
            HistorianAccessError accessError;
            HistorianDataValue value = new HistorianDataValue();
            HistorianDataValueList collection = null;

            if (string.IsNullOrWhiteSpace(mapping.TagName)) return false;

            if (!_connectionBuilder.HistorianAccess.GetTagInfoByName(mapping.TagName, true, out tag,
                out accessError))
            {
                _lastError = $"【ERROR】存储数据前，预先获取TAG信息出错：Tag名称=<{mapping.TagName}>,错误消息=<{accessError.ErrorDescription}>";
                logger.Error(_lastError);
                return false;
            }

            if (!(tag?.TagKey > 0)) return false;

            if (!mapping.StoreStreamed)
            {
                collection = _connectionBuilder.HistorianAccess.CreateHistorianDataValueList(
                    HistorianDataCategory.NonStreamedOriginal);
            }

            value.TagKey = tag.TagKey;
            value.DataValueType = tag.TagDataType;
            value.StartDateTime = DateTime.Now;
            value.OpcQuality = 192;

            if (tag.TagDataType == HistorianDataType.DoubleByteString ||
                tag.TagDataType == HistorianDataType.SingleByteString)
            {
                value.StringValue = hmiValue.ToString();
            }
            else
            {
                value.Value = Convert.ToDouble(hmiValue);
            }

            if (mapping.StoreStreamed)  // 按照时间戳顺序存储原始数据（当前项目中设置为默认值）
            {
                if (!_connectionBuilder.HistorianAccess.AddStreamedValue(value, out accessError))
                {
                    _lastError =
                        $"存储数据时出错 AddStreamedValue：Tag名称=<{mapping.TagName}>,错误消息=<{accessError.ErrorDescription}>";
                    logger.Error(_lastError);
                    return false;
                }
            }
            else
            {
                // If the time order of the original data sent to the historian server cannot be maintained due to the nature of the data source, the original data should be sent as original non-streamed data through the HistorianAccess.SendValues() method.
                // 如果由于数据源的自然因素，不能维护发送给 historian server 的原始数据的时间顺序，那么就应该通过 SendValues() 方法将原始数据作为“非流式”的方式发送
                if (!collection.Add(value, out accessError))
                {
                    _lastError =
                        $"向 HistorianDataValueList 中添加值时出错：Tag名称=<{mapping.TagName}>,错误消息=<{accessError.ErrorDescription}>";
                    logger.Error(_lastError);
                    return false;
                }
            }

            if (!mapping.StoreStreamed)
            {
                if (!_connectionBuilder.HistorianAccess.SendValues(collection, out accessError))
                {
                    _lastError =
                        $"存储设备的实时数据时出错（SendValues）：Tag名称=<{mapping.TagName}>,错误消息=<{accessError.ErrorDescription}>";
                    logger.Error(_lastError);
                    return false;
                }
            }

            return true;
        }

        #endregion 3.采集设备数据，写入 Historian 中

        #region 4. 读取 Historian 历史和实时数据

        public bool RetrievalHisotry(StringCollection tagNames, out IList<HistoryQueryResult> queryResults)
        {
            queryResults = new List<HistoryQueryResult>();

            HistoryQuery query = ConnectionBuilder.HistorianAccess.CreateHistoryQuery();

            HistoryQueryArgs queryArgs = new HistoryQueryArgs();
            queryArgs.TagNames = tagNames;
            queryArgs.StartDateTime = DateTime.Now.AddDays(-5);
            queryArgs.EndDateTime = DateTime.Now;
            queryArgs.RetrievalMode = HistorianRetrievalMode.Delta;
            queryArgs.DataVersion = HistorianVersionType.Latest;

            HistorianAccessError error;
            if (!query.StartQuery(queryArgs, out error))
            {
                _lastError = $"Failed to start query：<{error.ErrorDescription}>";
                return false;
            }

            while (query.MoveNext(out error))
            {
                //Debug.WriteLine("DateTime = {0}, vValue = {1}, OPCQuality = {2}, QualityDetail = {3}",
                //query.QueryResult.EndDateTime, query.QueryResult.StringValue,
                //query.QueryResult.OpcQuality, query.QueryResult.QualityDetail);

                queryResults.Add(query.QueryResult);
            }

            if (!query.EndQuery(out error))
            {
                _lastError = $"Failed to end query: {error.ErrorDescription}";
                return false;
            }
            return true;
        }

        public void RetrievalRealtime()
        {
        }

        #endregion 4. 读取 Historian 历史和实时数据
    }
}