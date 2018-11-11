using Com.ChinaSoft.DataAcquisition.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Com.ChinaSoft.DataAcquisition
{
    public static class HMILibOperation
    {
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, double> fakeSymbolicAdrData = new Dictionary<string, double>();   // 模拟设备数据

        private static DataAcquisitionMain dataAcquisition = DataAcquisitionMain.Instance;

        private static readonly Random random = new Random();

        static HMILibOperation()
        {
            InitFakeList();
        }

        private static void InitFakeList()
        {
            if (!dataAcquisition.LoadMappingFile())
            {
                logger.Error(dataAcquisition.LastError);
                return;
            }

            foreach (var mapping in dataAcquisition.TagSymbolMappings)
            {
                fakeSymbolicAdrData[mapping.TagName] = random.NextDouble() * 1000;
            }
        }

        /// <summary>
        /// 读取设备数据
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static object GetSymbolicAdrData(TagSymbolMapping mapping)
        {
            ushort len = mapping.Length;
            ushort status = 0;

            // 要使用unsafe字段，需要在项目属性-Build-General，勾上“允许不安全代码”
            unsafe
            {
                // 生产代码段：调用HMI获取设备数据，实际使用时取消注释
                //byte value;    // value 类型根据实际返回类型确定
                //status = CommonLibs.Spirit_PDBACC.pdbGetSymbolicAdrData(mapping.TotalLength, 0, mapping.SymbolAddress, 0,
                //    mapping.Offset, &len, &value);

                // 测试代码,第三个参数之所以传 TagName，是为了从集合中获取测试数据，生产代码要调用 mapping.SymbolAddress
                double value;
                status = FakeGetSymbolicAdrData(mapping.TotalLength, 0, mapping.SymbolAddress, 0, mapping.Offset,
                    mapping.Length, out value);

                // status=0 表示请求正常返回
                return status == 0 ? value.ToString() : null;
            }
        }

        /// <summary>
        /// 模拟HMI方法，获取实时数据
        /// </summary>
        /// <param name="n"></param>
        /// <param name="f"></param>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ushort FakeGetSymbolicAdrData(byte n, byte f, string name, ushort index, ushort offset,
             ushort len, out double value)
        {
            value = random.NextDouble() * 1000;

            return 0;
        }
    }
}