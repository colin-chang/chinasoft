using ArchestrA;
using Com.ChinaSoft.DataAcquisition.DatabaseRule;
using Com.ChinaSoft.DataAcquisition.Models;
using Com.ChinaSoft.Utility.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

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

        private static readonly ILogBuilder logger = Log4NetLogger.Instance;

        private static volatile DataAcquisitionMain _instance;
        private static object locker = new object();

        private readonly List<TagSymbolMapping> _tagSymbolMappings = new List<TagSymbolMapping>();
        private readonly string _mappingFileName;

        private string _lastError;  // 运行时错误消息
        private string _logMessage;
        private bool isConnected;   // 是否连接到 Historian

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

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }

        #endregion Properties

        #region Constructor

        private DataAcquisitionMain()
        {
            _mappingFileName = @"Data\TagSymbolMapping.txt";
        }

        public static DataAcquisitionMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new DataAcquisitionMain();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion Constructor

        #region 1. 加载映射文件

        /// <summary>
        /// 加载映射文件，包含 TAG-SymbolAddress 的对应关系等信息
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool LoadMappingFile()
        {
            try
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), _mappingFileName)))
                {
                    _lastError = "当前应用程序根目录下，不存在 <Data\\TagSymbolMapping.txt>.";
                    return false;
                }

                var allRows = File.ReadAllLines(_mappingFileName);
                if (allRows.Length <= 0)
                {
                    _lastError = "映射文件是空的.";
                    return false;
                }

                foreach (var row in allRows)
                {
                    var cols = row.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (cols.Length <= 0) continue;

                    _tagSymbolMappings.Add(ConvertToMapping(cols));
                }
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
            mapping.Interval = Convert.ToInt32(col[6]);
            mapping.Description = col[7];

            return mapping;
        }

        #endregion 1. 加载映射文件

        #region 2.连接 Historian，写入 Tags

        /// <summary>
        /// 新增 Tag 并提交到 Historian Server
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool AddTags()
        {
            if (!isConnected || !LoadMappingFile())
            {
                logger.Error("与 Historian 的连接已断开！");
                return false;
            }

            foreach (TagSymbolMapping mapping in TagSymbolMappings)
            {
                if (!_connectionStatus.ErrorOccurred)
                {
                    if (!_tagBuilder.AddTag(_connectionBuilder.HistorianAccess, mapping.TagName, mapping.Description,
                        false, HistorianDataType.Float, HistorianStorageType.Delta))
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

            isConnected = _connectionStatus != null && _connectionStatus.ErrorOccurred == false;

            if (_connectionStatus == null)
                _lastError = _connectionBuilder.LastError.ErrorDescription;
            else if (_connectionStatus.ErrorOccurred)
                _lastError = _connectionStatus.Error.ErrorDescription;

            return isConnected;
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

        /// <summary>
        /// 将数据存储到 Historian Server 中
        /// </summary>
        public void StoreValuesToHistorian()
        {
            if (!isConnected) return; ;

            // 按照间隔时间分组，每组分配一个线程执行采集操作
            Dictionary<int, List<TagSymbolMapping>> groupDictionary =
                TagSymbolMappings.GroupBy(x => x.Interval).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var groupItem in groupDictionary)
            {
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    // 线程名称以间隔时间做后缀，日志中可以以此判断哪一组出错了
                    Thread.CurrentThread.Name = $"HistorianStroreThread-{groupItem.Key}";
                    List<TagSymbolMapping> value = groupItem.Value;

                    while (true)
                    {
                        if (!isConnected) break; ;

                        foreach (TagSymbolMapping mapping in value)
                        {
                            // 调用 HMI 获取设备数据
                            object hmiValue = HMILibOperation.GetSymbolicAdrData(mapping);
                            if (hmiValue == null) continue;

                            if (!StoreOneValueToHistorian(mapping, hmiValue))
                                logger.Error($"执行采集数据的线程出错：线程名称：{Thread.CurrentThread.Name}，错误信息：{_lastError}");
                        }

                        Thread.Sleep(groupItem.Key);
                    }
                });
            }
        }

        // 是否存储为 Streamed 类型的值.
        // Streamed 的值即按照时间顺序写入 Historian ，对时间顺序有要求。 Streamed 和非 Streamed 方式调用的方法不同
        private bool storeStreamed = true;

        /// <summary>
        /// 向 Tag 存储一条数据并提交到 Historian Server 中
        /// </summary>
        /// <param name="mapping"><see cref="TagSymbolMappings"/>对象，Tag和地址符号的映射关系</param>
        /// <param name="hmiValue">从设备中读取的数据</param>
        /// <returns>成功返回true，否则返回false</returns>
        public bool StoreOneValueToHistorian(TagSymbolMapping mapping, object hmiValue)
        {
            if (!isConnected) return false;

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

            if (!storeStreamed)
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
                value.Value = Convert.ToSingle(hmiValue);
            }

            if (storeStreamed) // 按照时间戳顺序存储原始数据（当前项目中设置为默认值）
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

                if (!storeStreamed)
                {
                    if (!_connectionBuilder.HistorianAccess.SendValues(collection, out accessError))
                    {
                        _lastError =
                            $"存储设备的实时数据时出错（SendValues）：Tag名称=<{mapping.TagName}>,错误消息=<{accessError.ErrorDescription}>";
                        logger.Error(_lastError);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion 3.采集设备数据，写入 Historian 中

        #region 4. 读取 Historian 历史和实时数据

        public Tuple<bool, Dictionary<string, IList<CustomQueryResult>>> RetrieveHisotry(QueryParam param)
        {
            if (!isConnected)
            {
                logger.Error("与 Historian 的连接已断开！");
                return new Tuple<bool, Dictionary<string, IList<CustomQueryResult>>>(false, null);
            }

            _lastError = null;

            if (ConnectionBuilder == null)
            {
                if (!ConnectHistorian()) return new Tuple<bool, Dictionary<string, IList<CustomQueryResult>>>(false, null);
            }

            HistoryQuery query = ConnectionBuilder.HistorianAccess.CreateHistoryQuery();

            var tagNameCollection = param.TagNames;
            if (tagNameCollection == null || tagNameCollection.Count <= 0)
            {
                tagNameCollection = new StringCollection();
                tagNameCollection.AddRange(_tagSymbolMappings.Select(x => x.TagName).ToArray());
            }

            var queryArgs = new HistoryQueryArgs();
            queryArgs.TagNames = tagNameCollection;
            queryArgs.StartDateTime = DateTime.Now.AddSeconds(-param.LastSeconds);
            queryArgs.EndDateTime = DateTime.Now;
            queryArgs.RetrievalMode = HistorianRetrievalMode.Delta;
            queryArgs.Resolution = 10000;
            queryArgs.TimeDeadband = 0;
            queryArgs.ValueDeadband = 0;

            HistorianAccessError error;
            if (!query.StartQuery(queryArgs, out error))
            {
                _lastError = $"Failed to start query：<{error.ErrorDescription}>";
                return new Tuple<bool, Dictionary<string, IList<CustomQueryResult>>>(false, null);
            }

            var queryResults = new Dictionary<string, IList<CustomQueryResult>>();

            while (query.MoveNext(out error))
            {
                var tagName = query.QueryResult.TagName;

                var customQueryResult = new CustomQueryResult
                {
                    TagName = query.QueryResult.TagName,
                    TagKey = query.QueryResult.TagKey,
                    DataVersion = query.QueryResult.DataVersion,
                    EndDateTime = query.QueryResult.EndDateTime,
                    StartDateTime = query.QueryResult.StartDateTime,
                    Quality = query.QueryResult.Quality,
                    QualityDetail = query.QueryResult.QualityDetail,
                    Value = query.QueryResult.Value,
                    StringValue = query.QueryResult.StringValue,
                    RetrievalMode = query.QueryResult.RetrievalMode
                };

                if (queryResults.ContainsKey(tagName))
                {
                    queryResults[tagName].Add(customQueryResult);
                    queryResults[tagName] = queryResults[tagName].OrderByDescending(x => x.EndDateTime).ToList();
                }
                else
                    queryResults[tagName] = new List<CustomQueryResult>() { customQueryResult };
            }

            if (!query.EndQuery(out error))
            {
                _lastError = $"Failed to end query: {error.ErrorDescription}";
                return new Tuple<bool, Dictionary<string, IList<CustomQueryResult>>>(false, null);
            }

            return new Tuple<bool, Dictionary<string, IList<CustomQueryResult>>>(true, queryResults);
        }

        /// <summary>
        /// 从INSQL中查询实时数据
        /// </summary>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        public Tuple<bool, IList<CustomQueryResult>> RetrieveLive(StringCollection tagNames)
        {
            _lastError = null;

            try
            {
                string[] arrayTagNames = GetTagNamesForLive(tagNames);
                string tagNamesWithComma = String.Join("','", arrayTagNames);

                string sql = $@"
SELECT DateTime, TagName, wwTagKey, Value, vValue, wwRetrievalMode
FROM Live
WHERE TagName IN('{tagNamesWithComma}')";

                DataSet dataSet = DbHelper.Query(sql);
                IList<CustomQueryResult> queryResults = ReadLiveData(dataSet.Tables[0]);

                return Tuple.Create<bool, IList<CustomQueryResult>>(true, queryResults);
                ;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return Tuple.Create<bool, IList<CustomQueryResult>>(false, null);
            }
        }

        /// <summary>
        /// 处理Tag名称集合，返回可用的Tag名称集合
        /// </summary>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        private string[] GetTagNamesForLive(StringCollection tagNames)
        {
            string[] arrayTagNames;
            if (tagNames == null || tagNames.Count <= 0)
            {
                var query = _tagSymbolMappings.Select(x => x.TagName);
                arrayTagNames = query.ToArray();
            }
            else
            {
                arrayTagNames = new string[tagNames.Count];
                tagNames.CopyTo(arrayTagNames, 0);
            }
            return arrayTagNames;
        }

        /// <summary>
        /// 读取实时数据行
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IList<CustomQueryResult> ReadLiveData(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count <= 0) return null;

            var queryResults = new List<CustomQueryResult>();
            foreach (DataRow row in dataTable.Rows)
            {
                var r = new CustomQueryResult();
                r.Value = Convert.ToDouble(DbHelper.DbNullToObject(row["Value"]) ?? 0);
                r.StringValue = DbHelper.DbNullToObject(row["vValue"])?.ToString();
                r.TagKey = Convert.ToUInt32(row["wwTagKey"]);
                r.TagName = DbHelper.DbNullToObject(row["TagName"])?.ToString();
                r.EndDateTime = Convert.ToDateTime(row["DateTime"]);
                r.RetrievalMode = DbHelper.DbNullToObject(row["wwRetrievalMode"])?.ToString();

                queryResults.Add(r);
            }
            return queryResults;
        }

        #endregion 4. 读取 Historian 历史和实时数据
    }
}