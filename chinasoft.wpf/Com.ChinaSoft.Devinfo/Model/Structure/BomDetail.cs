using Prism.Mvvm;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.Model
{
    public class BomDetail : BindableBase
    {
        public string ComId { get; set; }

        private string elementName;
        /// <summary>
        /// 备件名称
        /// </summary>
        public string ElementName
        {
            get { return elementName; }
            set { SetProperty<string>(ref elementName, value); }
        }

        private string elementModel;
        /// <summary>
        /// 规格型号
        /// </summary>
        public string ElementModel
        {
            get { return elementModel; }
            set { SetProperty<string>(ref elementModel, value); }
        }

        private string nature;
        /// <summary>
        /// 材质
        /// </summary>
        public string Nature
        {
            get { return nature; }
            set { SetProperty<string>(ref nature, value); }
        }


        private string supplier;
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Supplier
        {
            get { return supplier; }
            set { SetProperty<string>(ref supplier, value); }
        }

        private string usedLife;
        /// <summary>
        /// 理论寿命
        /// </summary>
        public string UsedLife
        {
            get
            {
                if (string.IsNullOrWhiteSpace(usedLife))
                    return null;
                string ul = usedLife.Split('.').FirstOrDefault();
                return ul.EndsWith("个月") ? ul : $"{ul}个月";
            }
            set { SetProperty<string>(ref usedLife, value); }
        }

        private LastChangeInfo lst;
        /// <summary>
        /// 上次换件信息
        /// </summary>
        public LastChangeInfo LastChangeInfo
        {
            get { return lst; }
            set
            {
                SetProperty<LastChangeInfo>(ref lst, value);
                OnPropertyChanged(nameof(LastChangeDate));
                OnPropertyChanged(nameof(LastInputUser));
                OnPropertyChanged(nameof(LastChangeReason));
            }
        }

        /// <summary>
        /// 上次换件时间
        /// </summary>
        public string LastChangeDate => LastChangeInfo?.ChangeDateStr;

        /// <summary>
        /// 上次换件人
        /// </summary>
        public string LastInputUser => LastChangeInfo?.InputUser;

        /// <summary>
        /// 上次换件原因
        /// </summary>
        public string LastChangeReason => LastChangeInfo?.ChangeReason;


        private string changeDateStr;
        /// <summary>
        /// 本次到期时间
        /// </summary>
        public string ChangeDateStr
        {
            get { return changeDateStr?.Split(' ').FirstOrDefault(); }
            set { SetProperty<string>(ref changeDateStr, value); }
        }

        public string Appendfile3dId { get; set; }

        public Append3dFile Appendfile3d { get; set; }

        public string Appendfile2dId { get; set; }

        public Append2dFile Appendfile2d { get; set; }
    }

    public class LastChangeInfo : BindableBase
    {
        private string changeDateStr;
        /// <summary>
        /// 上次换件时间
        /// </summary>
        public string ChangeDateStr
        {
            get { return changeDateStr?.Split(' ').FirstOrDefault(); }
            set { SetProperty<string>(ref changeDateStr, value); }
        }

        private string inputUser;
        /// <summary>
        /// 上次换件人
        /// </summary>
        public string InputUser
        {
            get { return inputUser; }
            set { SetProperty<string>(ref inputUser, value); }
        }

        private string changeReason;
        /// <summary>
        /// 上次换件原因
        /// </summary>
        public string ChangeReason
        {
            get { return changeReason; }
            set { SetProperty<string>(ref changeReason, value); }
        }
    }
}
