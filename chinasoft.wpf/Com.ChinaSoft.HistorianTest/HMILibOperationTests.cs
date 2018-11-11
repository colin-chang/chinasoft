using Com.ChinaSoft.DataAcquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using Com.ChinaSoft.DataAcquisition.Models;
using Xunit;

namespace Com.ChinaSoft.HistorianTest
{
    public class HMILibOperationTests
    {
        private DataAcquisitionMain helper = DataAcquisitionMain.Instance;

        [Fact]
        public void GetSymbolicAdrDataTest()
        {
            var isLoadedMappingFile = helper.LoadMappingFile();
            Assert.True(isLoadedMappingFile);

            IList<object> symbolAdrDataList = new List<object>();

            foreach (TagSymbolMapping mapping in helper.TagSymbolMappings)
            {
                var value = HMILibOperation.GetSymbolicAdrData(mapping);
                symbolAdrDataList.Add(value);
            }

            Assert.True(symbolAdrDataList.Count == 5);
            Assert.Equal(Convert.ToDouble(symbolAdrDataList[0]), 111.222D);
            Assert.Equal(Convert.ToDouble(symbolAdrDataList.Last()), 555.666D);
        }
    }
}