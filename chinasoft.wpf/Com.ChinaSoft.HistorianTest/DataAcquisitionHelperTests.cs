using ArchestrA;
using Com.ChinaSoft.DataAcquisition;
using Com.ChinaSoft.DataAcquisition.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace Com.ChinaSoft.HistorianTest
{
    public class DataAcquisitionHelperTests
    {
        private DataAcquisitionMain dataAcquisition;
        private bool flag;

        private HistorianTag tag;
        private HistorianAccess historianAccess;
        private HistorianAccessError error;
        private HistorianConnectionStatus connectionStatus;

        public DataAcquisitionHelperTests()
        {
            dataAcquisition = DataAcquisitionMain.Instance;

            flag = dataAcquisition.ConnectHistorian();
            Assert.True(flag, dataAcquisition.LastError);

            historianAccess = dataAcquisition.ConnectionBuilder.HistorianAccess;
        }

        /// <summary>
        /// 测试-使用 Historian 提供的日志方法记录日志
        /// 日志会在 System Management Console\Log Viewer\Default Group\Local 下显示
        /// </summary>
        [Fact]
        public void LogMessageTest()
        {
            dataAcquisition.ConnectionBuilder.HistorianAccess.LogMessage(HistorianMessageSeverity.Info,
                "==============================SONGZ-PC Info Message", out error);

            dataAcquisition.ConnectionBuilder.HistorianAccess.LogMessage(HistorianMessageSeverity.Error,
                "==============================SONGZ-PC Error Message", out error);

            dataAcquisition.ConnectionBuilder.HistorianAccess.LogMessage(HistorianMessageSeverity.Debug,
                "==============================SONGZ-PC Debug Message", out error);

            dataAcquisition.ConnectionBuilder.HistorianAccess.LogMessage(HistorianMessageSeverity.Warning,
               "==============================SONGZ-PC Warning Message", out error);
        }

        [Fact]
        public void ConnectHistorianTest()
        {
            flag = dataAcquisition.ConnectHistorian();
            Assert.True(flag, dataAcquisition.LastError);
        }

        [Fact]
        public void LoadMappingFileTest()
        {
            flag = dataAcquisition.LoadMappingFile();

            Assert.True(flag, dataAcquisition.LastError);
            Assert.True(dataAcquisition.TagSymbolMappings.Count == 5, "映射文件数据行不足5条!");
        }

        /// <summary>
        /// 测试-写入 TAG
        /// </summary>
        [Fact]
        public void WriteTagTest()
        {
            // 测试加载文件
            flag = dataAcquisition.LoadMappingFile();
            Assert.True(flag, dataAcquisition.LastError);

            // 测试写入TAG
            flag = dataAcquisition.AddTags();
            Assert.True(flag, dataAcquisition.LastError);

            Debug.WriteLine($"LogMessage:{dataAcquisition.LogMessage}");
        }

        /// <summary>
        /// 测试-获取 TAG 信息
        /// </summary>
        [Fact]
        public void TestGetTagInfo()
        {
            flag = dataAcquisition.ConnectHistorian();
            Assert.True(flag);

            dataAcquisition.ConnectionBuilder.HistorianAccess.GetConnectionStatus(ref connectionStatus);

            Assert.True(connectionStatus.ConnectedToServer);
            Assert.True(connectionStatus.ConnectedToServerStorage);
            Assert.True(connectionStatus.ConnectedToStoreForward);

            var tagName = "Tag_G_st_SRMSymAdd_ANPC_0";

            flag = dataAcquisition.ConnectionBuilder.HistorianAccess.GetTagInfoByName(tagName, true, out tag, out error);
            Assert.True(flag, error?.ErrorDescription);
        }

        /// <summary>
        /// 测试-删除 TAG
        /// </summary>
        [Fact]
        public void DeleteTagsTests()
        {
            var tagToDeleted = new HistorianTagStatusList
            {
                new HistorianTagStatus() {TagName = "Tag_G_st_SRMSymAdd_ANPC_0"},
                new HistorianTagStatus() {TagName = "Tag_G_st_SRMSymAdd_ANPC_1"},
                new HistorianTagStatus() {TagName = "Tag_G_st_SRMSymAdd_ANPC_2"},
                new HistorianTagStatus() {TagName = "Tag_G_st_SRMSymAdd_ANPC_3"},
                new HistorianTagStatus() {TagName = "Tag_G_st_SRMSymAdd_ANPC_4"},
            };

            flag = historianAccess.DeleteTags(ref tagToDeleted, out error);
            Assert.True(flag, error?.ErrorDescription);
        }

        /// <summary>
        /// 测试-向Historian存储一条数据
        /// </summary>
        [Fact]
        public void StoreOneValueToHistorianTest()
        {
            // 测试加载文件
            flag = dataAcquisition.LoadMappingFile();
            Assert.True(flag, dataAcquisition.LastError);

            TagSymbolMapping first = dataAcquisition.TagSymbolMappings.First();

            double hmiValue = 111.222D;

            // 执行存储
            flag = dataAcquisition.StoreOneValueToHistorian(first, hmiValue);
            Assert.True(flag, dataAcquisition.LastError);
        }

        /// <summary>
        /// 测试-向Historian 存储批量数据
        /// </summary>
        [Fact]
        public void StoreValuesToHistorianTest()
        {
            dataAcquisition.StoreValuesToHistorian();
            Assert.True(dataAcquisition.LastError == null, dataAcquisition.LastError);
            Thread.Sleep(20000);
        }

        /// <summary>
        /// 读取 Historian 历史数据
        /// </summary>
        [Fact]
        public void RetrievalHisotryTest()
        {
            var tagNames = new StringCollection
            {
                "Tag_G_st_SRMSymAdd_ANPC_0",
                "Tag_G_st_SRMSymAdd_ANPC_1",
                "Tag_G_st_SRMSymAdd_ANPC_2",
                "Tag_G_st_SRMSymAdd_ANPC_3",
                "Tag_G_st_SRMSymAdd_ANPC_4"
            };

            QueryParam param = new QueryParam() { TagNames = tagNames, LastSeconds = 5 };
            var t = dataAcquisition.RetrieveHisotry(param);
            var result = t.Item2;

            Debug.WriteLine(JsonConvert.SerializeObject(result));
            Assert.True(t.Item1, dataAcquisition.LastError);
        }

        /// <summary>
        /// 读取 Historian 实时数据
        /// </summary>
        [Fact]
        public void RetrievalLiveTest()
        {
            var tagNames = new StringCollection
            {
                "Tag_G_st_SRMSymAdd_ANPC_0",
                "Tag_G_st_SRMSymAdd_ANPC_1",
                "Tag_G_st_SRMSymAdd_ANPC_2",
                "Tag_G_st_SRMSymAdd_ANPC_3",
                "Tag_G_st_SRMSymAdd_ANPC_4",
            };

            var t = dataAcquisition.RetrieveLive(tagNames);
            Debug.WriteLine(JsonConvert.SerializeObject(t.Item2));
            Assert.True(t.Item1, dataAcquisition.LastError);
        }
    }
}