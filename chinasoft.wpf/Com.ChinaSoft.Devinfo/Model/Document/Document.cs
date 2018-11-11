namespace Com.ChinaSoft.Devinfo.Model
{
    public class Document
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string ClassId { get; set; }

        public string Introduce { get; set; }

        public string Keyword { get; set; }

        public string ThumbnailId { get; set; }

        public string Thumbnail { get { return $"/Images/Document/_{this.Type}.png"; } }

        public string AppendFileId { get; set; }

        public DocumentFile AppendFile { get; set; }

        public string EquipmentId { get; set; }

        public int DeleteFlag { get; set; }

    }

    public class DocumentFile
    {
        public string Id { get; set; }

        public string Path { get; set; }

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
