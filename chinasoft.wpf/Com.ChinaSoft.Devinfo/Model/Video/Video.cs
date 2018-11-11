using System;
using System.Configuration;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class Video
    {
        public string Id { get; set; }

        public string EquipmentId { get; set; }

        public string EquipmentCode { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string ClassId { get; set; }

        public string ThumbnailId { get; set; }

        public VideoFile Thumbnail { get; set; }

        public string AppendFileId { get; set; }

        public VideoFile AppendFile { get; set; }
    }
   

    public class VideoFile
    {
        public string Id { get; set; }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = System.IO.Path.Combine(ConfigurationManager.AppSettings["FileUrl"], value).ToUrl(); }
        }

        public string FileName { get; set; }

        public string OrigFileName { get; set; }

        public string ExtenFileName { get; set; }

        public string FileType { get; set; }

        public string DirsPath { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public int IsInstaion { get; set; }

        public string Uri { get; set; }
    }
}
