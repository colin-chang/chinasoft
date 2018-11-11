using System.Collections.Generic;

namespace Com.ChinaSoft.DataAcquisition
{
    public static class HMILibOperation
    {
        private static Dictionary<string, double> fakeSymbolicAdrData;   // 模拟设备数据

        static HMILibOperation()
        {
            fakeSymbolicAdrData = new Dictionary<string, double>()
            {
                {"G_st_SRMSymAdd_ANPC_1",111.222},
                {"G_st_SRMSymAdd_ANPC_6",222.333},
                {"G_st_SRMSymAdd_ANPC_11",333.444},
                {"G_st_SRMSymAdd_ANPC_16",444.555},
                {"G_st_SRMSymAdd_ANPC_21",555.666},
            };
        }

        public static object GetSymbolicAdrData(TagSymbolMapping mapping)
        {
            ushort len = mapping.Length;
            ushort status = 0;

            unsafe
            {
                // 生产代码：调用HMI获取设备数据
                //byte value;
                //CommonLibs.Spirit_PDBACC.pdbGetSymbolicAdrData(mapping.TotalLength, 0, mapping.SymbolAddress, 0,
                //    mapping.Offset, &len, &value);

                // 测试代码
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
            value = 0;
            ushort status = 0;

            string key = $"{name}_{offset}";

            double fakeValue;
            if (fakeSymbolicAdrData.TryGetValue(key, out fakeValue))
            {
                value = fakeValue;
            }

            return status;
        }
    }
}