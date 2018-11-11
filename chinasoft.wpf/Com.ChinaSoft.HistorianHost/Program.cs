using Microsoft.Owin.Hosting;
using System;
using Com.ChinaSoft.DataAcquisition;

[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]

namespace Com.ChinaSoft.HistorianService
{
    internal class Program
    {
        private static void Main()
        {
            SpiritBuilder.Start();

            string baseAddress = "http://localhost:9000/";

            // Start OWIN host
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"服务已启动，监听地址为：{baseAddress}");

                while (Console.ReadKey(false).Key != ConsoleKey.X) { }
            }
        }
    }
}