using Com.ChinaSoft.Model.Service;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class SpareModelResult:ObjJsonResult<SpareModel>
    {
    }

    public class SpareModel
    {
        public string Appendfile2dId { get; set; }

        public Append2dFile Appendfile2d { get; set; }

        public string Appendfile3dId { get; set; }

        public Append3dFile Appendfile3d { get; set; }
    }
}
