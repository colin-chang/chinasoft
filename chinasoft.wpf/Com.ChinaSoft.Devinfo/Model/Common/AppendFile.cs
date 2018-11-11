namespace Com.ChinaSoft.Devinfo.Model.Common
{
    public abstract class AppendFile
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public abstract string Path { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OrigFileName { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string ExtenFileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 目录路径
        /// </summary>
        public string DirsPath { get; set; }

        /// <summary>
        /// 宽度(图片使用)
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 高度(图片使用)
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// url地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否站内
        /// </summary>
        public int IsInstaion { get; set; }
    }
}
