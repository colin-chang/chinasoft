using System;
using Prism.Mvvm;

namespace Com.ChinaSoft.AlertCenter.Models
{
    public class BomDetail : BindableBase
    {
        #region Property

        /// <summary>
        ///     bom结构唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     零件唯一标识
        /// </summary>
        public string ComId { get; set; }

        /// <summary>
        ///     零件名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        ///     规格型号
        /// </summary>
        public string ElementModel { get; set; }

        /// <summary>
        ///     寿命(单位:月份)
        /// </summary>
        public float UsedLife { get; set; }

        public string UsedLifeStr => string.Concat(UsedLife.ToString(), "个月");

        /// <summary>
        ///     生产厂家
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        ///     寿命比例
        /// </summary>
        public double LifeRate { get; set; }

        /// <summary>
        ///     2D预警图
        /// </summary>
        public Appendfile Appendfile2d { get; set; }

        /// <summary>
        ///     3D预警图
        /// </summary>
        public Appendfile Appendfile3d { get; set; }

        /// <summary>
        ///     上一次修改的信息
        /// </summary>
        public ChangeInfo LastChangeInfo { get; set; }

        /// <summary>
        ///     材质
        /// </summary>
        public string Nature { get; set; }

        #endregion Property

        #region 额外属性

        private int _changeNum = 1;
        private string _changeReason = "寿命到期";
        private string _inputUser = "李勇";
        private DateTime? _changeDataStr = DateTime.Now;

        /// <summary>
        ///     换件数量
        /// </summary>
        public int ChangeNum
        {
            get { return _changeNum; }
            set { SetProperty(ref _changeNum, value); }
        }

        /// <summary>
        ///     换件原因
        /// </summary>
        public string ChangeReason
        {
            get { return _changeReason; }
            set { SetProperty(ref _changeReason, value); }
        }

        /// <summary>
        ///     换件人
        /// </summary>
        public string InputUser
        {
            get { return _inputUser; }
            set { SetProperty(ref _inputUser, value); }
        }

        /// <summary>
        ///     换件日期
        /// </summary>
        public DateTime? ChangeDateStr
        {
            get { return _changeDataStr; }
            set { SetProperty(ref _changeDataStr, value); }
        }

        #endregion 额外属性
    }
}