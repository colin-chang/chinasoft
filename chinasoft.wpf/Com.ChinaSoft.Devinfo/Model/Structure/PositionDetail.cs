using Com.ChinaSoft.Devinfo.Model.Common;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class PositionDetail
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ExplodeFileId { get; set; }

        public ExplodeFile ExplodeFile { get; set; }

        public string Appendfile3dId { get; set; }

        public Append3dFile Appendfile3d { get; set; }

        public string Appendfile2dId { get; set; }

        public Append2dFile Appendfile2d { get; set; }

        public string FigureCode { get; set; }
    }
}
