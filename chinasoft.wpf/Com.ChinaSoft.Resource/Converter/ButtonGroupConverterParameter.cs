namespace Com.ChinaSoft.Resource.Converter
{
    public class ButtonGroupActiveConverterParameter
    {
        /// <summary>
        /// 按钮对应状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 非激活状态样式名称
        /// </summary>
        public string StyleKey { get; set; }

        public ButtonGroupActiveConverterParameter(string status,string styleKey)
        {
            this.Status = status;
            this.StyleKey = styleKey;
        }
    }
}
