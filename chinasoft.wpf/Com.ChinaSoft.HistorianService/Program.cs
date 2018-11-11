using Com.ChinaSoft.DataAcquisition;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.Threading;

namespace Com.ChinaSoft.HistorianService
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                var threadDataAcquisition = new Thread(DataAcquisition) { IsBackground = true };
                threadDataAcquisition.Start();

                string listeningPort = ConfigurationManager.AppSettings["ServiceListeningPort"];
                string baseAddress = $"http://localhost:{listeningPort}/";

                // Start OWIN host
                using (WebApp.Start<Startup>(baseAddress))
                {
                    Console.WriteLine($"服务已启动，监听地址为：{baseAddress}");
                    Console.WriteLine($"按下 X 键退出程序.{Environment.NewLine}");

                    while (Console.ReadKey(false).Key != ConsoleKey.X) { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 启动数据采集
        /// </summary>
        private static void DataAcquisition()
        {
            DataAcquisitionMain dataAcquisition = DataAcquisitionMain.Instance;

            bool flag = dataAcquisition.ConnectHistorian();

            flag = dataAcquisition.AddTags();
            if (flag)
                dataAcquisition.StoreValuesToHistorian();
            else
                Console.WriteLine(dataAcquisition.LastError);
        }
    }
}