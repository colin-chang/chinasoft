namespace Com.ChinaSoft.AlertCenter.Models
{
    /// <summary>
    ///     预警类别
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        ///     全部
        /// </summary>
        [StrategyType("全部")] All = -1,

        /// <summary>
        ///     故障预警
        /// </summary>
        [StrategyType("故障预警", "/Images/ico/ico_alert_trouble.png")] TroubleAlert = 0,

        /// <summary>
        ///     寿命预警
        /// </summary>
        [StrategyType("寿命预警", "/Images/ico/ico_alert_lifetime.png")] LifetimeAlert = 1,

        /// <summary>
        ///     错料预警
        /// </summary>
        [StrategyType("错料预警", "/Images/ico/ico_alert_wrongmaterial.png")] WrongMaterialAlert = 2
    }
}