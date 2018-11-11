using System;

namespace Com.ChinaSoft.Devinfo.Model
{
    /// <summary>
    /// 设备概况
    /// </summary>
    public class DeviceOverview
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string EquipmentNo { get; set; }

        /// <summary>
        /// 机型Id
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 机型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 生产厂ID
        /// </summary>
        public string FactoryId { get; set; }

        /// <summary>
        /// 生产厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 机台号
        /// </summary>
        public string MachineNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 理论寿命
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 设备条码
        /// </summary>
        public string EquipmentBar { get; set; }

        /// <summary>
        /// 3D图附件
        /// </summary>
        public string Appendfile3dId { get; set; }

        /// <summary>
        /// 2D插图标识
        /// </summary>
        public string Appendfiles2dId { get; set; }

        /// <summary>
        /// 2D插图文件
        /// </summary>
        public Append2dFile AppendFiles2dId_File { get; set; }

        /// <summary>
        /// 最大生产速度
        /// </summary>
        public string MaxSpeed { get; set; }

        /// <summary>
        /// 额定生产速度
        /// </summary>
        public string RatedSpeed { get; set; }

        /// <summary>
        /// 最大烟条速度
        /// </summary>
        public string CigarMaxSpeed { get; set; }

        /// <summary>
        /// 额定烟条生产速度
        /// </summary>
        public string CigarRatedSpeed { get; set; }

        /// <summary>
        /// 烟支长度
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 滤嘴长度
        /// </summary>
        public string FilterLength1 { get; set; }

        public string FilterLength2 { get; set; }

        /// <summary>
        /// 烟支直径
        /// </summary>
        public string Diameter { get; set; }

        /// <summary>
        /// 综合噪声
        /// </summary>
        public string SyntheticNoise { get; set; }

        /// <summary>
        /// 单点噪声
        /// </summary>
        public string SingleNoise { get; set; }

        /// <summary>
        /// 有效作业率
        /// </summary>
        public string Equipmenteor { get; set; }

        /// <summary>
        /// 总废品率
        /// </summary>
        public string Equipmentros { get; set; }

        /// <summary>
        /// 含末率
        /// </summary>
        public string Equipmentdcr { get; set; }

        /// <summary>
        /// 出厂日期
        /// </summary>
        public DateTime? OutDate { get; set; }

        /// <summary>
        /// 采购日期
        /// </summary>
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        /// 投产日期
        /// </summary>
        public DateTime? UsedDate { get; set; }

        /// <summary>
        /// 新建日期
        /// </summary>
        public DateTime? InputTime { get; set; }
    }
}
