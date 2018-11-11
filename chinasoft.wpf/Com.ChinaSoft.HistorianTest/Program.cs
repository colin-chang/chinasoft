using ArchestrA;
using Newtonsoft.Json;
using System;

namespace Com.ChinaSoft.HistorianTest
{
    internal class Program
    {
        private static ArchestrA.HistorianAccess access;
        private static HistorianAccessError accessError;

        private static void Main(string[] args)
        {
            access = new ArchestrA.HistorianAccess();

            HistorianConnectionArgs connectionArgs = new HistorianConnectionArgs();
            connectionArgs.ServerName = "192.168.1.194";
            connectionArgs.TcpPort = 32568;
            connectionArgs.UserName = "admin";
            connectionArgs.Password = "admin";
            connectionArgs.MinStoreForwardDuration = 15;
            connectionArgs.StoreForwardPath = @"C:\StoreForward";
            connectionArgs.StoreForwardFreeDiskSpace = 1024;
            connectionArgs.ReadOnly = false;



            if (!access.OpenConnection(connectionArgs, out accessError))
            {
                Console.WriteLine(JsonConvert.SerializeObject(accessError));
            }
            else
            {
                Console.WriteLine("Connected Successfully!");
                //GetTagInfo();

                //DeleteTags();
                AddTag();
            }

            Console.ReadKey();
        }

        private static void GetTagInfo()
        {
            HistorianTag tag;
            if (!access.GetTagInfoByName("SDKTest0_Double", false, out tag, out accessError))
            {
                Console.WriteLine($"{accessError.ErrorCode},{accessError.ErrorDescription},{accessError.ServerName},{accessError.ErrorType}");
            }
            else
            {
                Console.WriteLine(JsonConvert.SerializeObject(tag));
            }
        }

        static void AddTag()
        {
            HistorianTag tag = new HistorianTag("A_Apple", HistorianDataType.Float);

            uint tagKey;
            if (access.AddTag(tag, out tagKey, out accessError))
            {
                Console.WriteLine("添加Tag 成功.");
            }
            else
            {
                Console.WriteLine("添加Tag 失败：" + accessError.ErrorDescription);
            }
        }

        static void DeleteTags()
        {
            HistorianTagStatusList tagToDelete = new HistorianTagStatusList()
            {
                new HistorianTagStatus() {TagName = "SDKTest0_Double"},
                new HistorianTagStatus() {TagName = "SDKTest1_Double"},
                new HistorianTagStatus() {TagName = "SDKTest2_Double"},
                new HistorianTagStatus() {TagName = "SDKTest3_Double"},
                new HistorianTagStatus() {TagName = "SDKTest4_Double"},
                new HistorianTagStatus() {TagName = "SDKTTT0_UInt2"},
                new HistorianTagStatus() {TagName = "SDKTTT1_UInt2"},
                new HistorianTagStatus() {TagName = "SDKTTT2_Int4"},
                new HistorianTagStatus() {TagName = "SDKTTT3_Int4"},
                new HistorianTagStatus() {TagName = "SDKTTT4_Int4"},
            };

            if (access.DeleteTags(ref tagToDelete, out accessError))
            {
                Console.WriteLine("删除成功");
            }
            else
            {
                Console.WriteLine("Delete tags failed: " + accessError.ErrorDescription);
            }


        }
    }
}