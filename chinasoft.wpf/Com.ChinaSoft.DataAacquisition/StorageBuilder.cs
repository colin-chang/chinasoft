using System;
using System.Collections.Generic;
using System.Threading;

namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// Storage engine will simulate data and store to the historian
    /// </summary>
    public class StorageBuilder
    {
        #region --Variables--

        private Random randomgenerator = new Random();
        private List<Thread> storagethreads = new List<Thread>();
        private Boolean stopthreads = false;

        private struct StorageParameters
        {
            public ArchestrA.HistorianAccess access;
            public String[] TagName;
            public int NumberofValues;
            public int sleepbetweenvaluesMS;
            public int sleepbetweenbatchesMS;
            public Boolean storeStreamed;
        }

        #endregion --Variables--

        /// <summary>
        /// Constructor
        /// </summary>
        public StorageBuilder()
        {
        }

        /// <summary>
        /// Storevalues will store simulated data
        /// </summary>
        /// <param name="access">Connection object for the Historian</param>
        /// <param name="TagName">Array of tagnames to store</param>
        /// <param name="NumberofValues">Number of values to store for each tag</param>
        /// <param name="sleepbetweenvaluesMS">Sleep time between each value. Allows simulation of bulk data or regular flow.</param>
        /// <param name="storeStreamed">Store values as realtime</param>
        public void StoreValues(ArchestrA.HistorianAccess access, String[] TagName, int NumberofValues, int sleepbetweenvaluesMS, int sleepbetweenbatchesMS, Boolean storeStreamed)
        {
            StorageParameters parameters = new StorageParameters();
            parameters.access = access;
            parameters.TagName = TagName;
            parameters.NumberofValues = NumberofValues;
            parameters.sleepbetweenvaluesMS = sleepbetweenvaluesMS;
            parameters.sleepbetweenbatchesMS = sleepbetweenbatchesMS;
            parameters.storeStreamed = storeStreamed;
            stopthreads = false;

            Thread thread = new Thread(StoreThreadFunc);
            thread.Start(parameters);
            storagethreads.Add(thread);
        }

        /// <summary>
        /// Storage thread function will process data.
        /// </summary>
        /// <param name="storageparameters"></param>
        private void StoreThreadFunc(object storageparameters)
        {
            StorageParameters parameters = (StorageParameters)storageparameters;
            ArchestrA.HistorianDataValue value = new ArchestrA.HistorianDataValue();
            ArchestrA.HistorianDataValueList collection = null;
            ArchestrA.HistorianTag tag = null;
            ArchestrA.HistorianAccessError error = null;

            while (!stopthreads)
            {
                foreach (String tagname in parameters.TagName)
                {
                    if (tagname != null && tagname != "")
                    {
                        // Must get tag key to store data. Call GetTagInfoByName to obtain the tag information including the tagkey.
                        parameters.access.GetTagInfoByName(tagname, false, out tag, out error);

                        if (tag.TagKey > 0)
                        {
                            if (!parameters.storeStreamed)
                            {
                                collection = parameters.access.CreateHistorianDataValueList(ArchestrA.HistorianDataCategory.NonStreamedOriginal);
                                if (collection == null)
                                    return;
                            }

                            DateTime currentTime = DateTime.Now;
                            for (int i = 0; i < parameters.NumberofValues; i++)
                            {
                                if (!stopthreads)
                                {
                                    value.DataValueType = tag.TagDataType;
                                    value.OpcQuality = 192;
                                    value.StartDateTime = currentTime.Subtract(new TimeSpan(0, 0, 0, 0, parameters.NumberofValues - i + 1)); //Offset in milliseconds
                                    if (tag.TagDataType == ArchestrA.HistorianDataType.DoubleByteString || tag.TagDataType == ArchestrA.HistorianDataType.SingleByteString)
                                    {
                                        String valstr = "";
                                        for (int r = 0; r < randomgenerator.Next(1024); r++)
                                        {
                                            valstr = valstr + r.ToString()[0];
                                        }
                                        value.StringValue = valstr;
                                    }
                                    else
                                    {
                                        value.Value = randomgenerator.Next(100);
                                    }
                                    value.TagKey = tag.TagKey;

                                    if (parameters.storeStreamed)
                                    {
                                        //Must set the LastError to null before calling any function
                                        error = null;
                                        if (!parameters.access.AddStreamedValue(value, out error))
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        error = new ArchestrA.HistorianAccessError();
                                        if (!collection.Add(value, out error))
                                        {
                                            return;
                                        }
                                    }

                                    System.Threading.Thread.Sleep(parameters.sleepbetweenvaluesMS);
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (!parameters.storeStreamed)
                            {
                                if (!parameters.access.SendValues(collection, out error))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(parameters.sleepbetweenbatchesMS);
            }
        }

        /// <summary>
        /// Stop threads from storing data.
        /// </summary>
        public void StopStoring()
        {
            stopthreads = true;
        }
    }
}
