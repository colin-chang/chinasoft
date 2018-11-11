namespace Com.ChinaSoft.AlertCenter.Models
{
    public class Appendfile
    {
        /// <summary>
        ///     文件唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     文件服务器相对路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     文件原名称
        /// </summary>
        public string OrigFileName { get; set; }

        /// <summary>
        ///     文件后缀
        /// </summary>
        public string ExtenFileName { get; set; }

        /// <summary>
        ///     文件类型(photo 图片 doc 文档 video 视频)
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        ///     文件的文件夹路径
        /// </summary>
        public string DirsPath { get; set; }

        /// <summary>
        ///     图片的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        ///     是否内部存储(0.内部存储 1.外部存储)
        /// </summary>
        public string IsInstaion { get; set; }

        /// <summary>
        ///     文件的外部地址(只有为外部存储才有值)
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        ///     图片的高度
        /// </summary>
        public double Height { get; set; }
    }
}