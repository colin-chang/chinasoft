using System;
using System.Collections.Generic;

namespace Com.ChinaSoft.DataAcquisition
{
    /// <summary>
    /// TagBuilder class will create tags.
    /// </summary>
    public class TagBuilder
    {
        #region --Variables--

        private Dictionary<string, uint> mTagsAdded = new Dictionary<string, uint>();
        private ArchestrA.HistorianAccessError mLastError = new ArchestrA.HistorianAccessError();

        /// <summary>
        /// Provides the current dictionary of tags that have been added. Contains each tagname and its associated tagkey.
        /// </summary>
        public Dictionary<string, uint> TagsAdded
        {
            get
            {
                return mTagsAdded;
            }
        }

        /// <summary>
        /// Provides the last error from performing historian operations.
        /// </summary>
        public ArchestrA.HistorianAccessError LastError
        {
            get
            {
                return mLastError;
            }
        }

        #endregion --Variables--

        /// <summary>
        /// Constructor
        /// </summary>
        public TagBuilder()
        {
        }

        /// <summary>
        /// Adds a tag to the historian
        /// </summary>
        /// <param name="access">Connection object for the Historian</param>
        /// <param name="TagName">Name of tag to add</param>
        /// <param name="Description">Description of tag to add</param>
        /// <param name="EnableChannelStatus">Late data option</param>
        /// <param name="DataType">Data type</param>
        /// <param name="TagType">Tag type</param>
        /// <param name="StorageType">Storage rule type</param>
        /// <returns>Returns a boolean if successful. If false, you can check the LastError variable for the error.</returns>
        public Boolean AddTag(ArchestrA.HistorianAccess access, String TagName, String Description, Boolean EnableChannelStatus, ArchestrA.HistorianDataType DataType, ArchestrA.HistorianStorageType StorageType)
        {
            //Create tag. Not all variables are passed in this function, but there are many other variables that can be set including Min and Max Engineering units.
            ArchestrA.HistorianTag tag = new ArchestrA.HistorianTag();
            tag.MaxEU = 100;
            tag.MinEU = 0;
            tag.ServerTimestamp = false;
            tag.TagChannelStatus = Convert.ToUInt16(EnableChannelStatus);
            tag.TagDataType = DataType;
            tag.TagDescription = Description;
            tag.TagKey = 0;
            tag.TagName = TagName;
            tag.TagStorageType = StorageType;

            uint tagkey = 0;
            mLastError = null;
            if (access.AddTag(tag, out tagkey, out mLastError))
            {
                if (TagsAdded.ContainsKey(TagName))
                    TagsAdded.Remove(TagName);

                TagsAdded.Add(TagName, tagkey);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="access">Connection object for the Historian</param>
        /// <param name="TagNamePrefix">Tagname prefix for tags that will be added</param>
        /// <param name="Description">Description for tags that will be added</param>
        /// <param name="NumberofTags">Number of tags to add</param>
        /// <param name="EnableChannelStatus">Late data option</param>
        /// <param name="DataType">Data type</param>
        /// <param name="TagType">Tag type</param>
        /// <param name="StorageType">Storage rule type</param>
        /// <returns>Returns a boolean if successful. If false, you can check the LastError variable for the error.</returns>
        public Boolean AddTags(ArchestrA.HistorianAccess access, String TagNamePrefix, String Description, uint NumberofTags, Boolean EnableChannelStatus, ArchestrA.HistorianDataType DataType, ArchestrA.HistorianStorageType StorageType)
        {
            int currenttagcount = TagsAdded.Count;
            for (int i = currenttagcount; i < NumberofTags + currenttagcount; i++)
            {
                if (!AddTag(access, String.Format("{0}{1}_{2}", TagNamePrefix, i, DataType.ToString()), Description, EnableChannelStatus, DataType, StorageType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}