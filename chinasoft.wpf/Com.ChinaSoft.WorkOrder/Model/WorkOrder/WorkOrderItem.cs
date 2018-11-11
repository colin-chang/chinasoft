using System;
using System.Collections.Generic;

namespace Com.ChinaSoft.WorkOrder.Model.WorkOrder
{
    [Serializable]
    public class WorkOrderItem
    {
        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     工单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        ///     班组
        /// </summary>
        public string OrderClass { get; set; }

        /// <summary>
        ///     机台号
        /// </summary>
        public string MachinaName { get; set; }

        /// <summary>
        ///     牌号
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        ///     计划开始时间
        /// </summary>
        public string PlanStartTimeStr { get; set; }

        /// <summary>
        ///     计划结束时间
        /// </summary>
        public string PlanEndTimeStr { get; set; }

        /// <summary>
        ///     实际开始时间
        /// </summary>
        public string FactStartTimeStr { get; set; }

        /// <summary>
        ///     实际结束时间
        /// </summary>
        public string FactEndTimeStr { get; set; }

        /// <summary>
        ///     计划产量
        /// </summary>
        public double? PlanOutput { get; set; }

        /// <summary>
        ///     实际产量
        /// </summary>
        public double? FactOutput { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        public WorkOrderStatus Status { get; set; }

        /// <summary>
        ///     状态文本
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        ///     班次（如：甲班）
        /// </summary>
        public string Shift { get; set; }

        /// <summary>
        ///     设备唯一标识
        /// </summary>
        public string EquipmentId { get; set; }

        /// <summary>
        ///     辅料信息
        /// </summary>
        public List<WorkOrderRecipe> Recipes { get; set; }

        /// <summary>
        ///     工艺标准
        /// </summary>
        public List<WorkOrderTechStandards> TechStandards { get; set; }
    }
}