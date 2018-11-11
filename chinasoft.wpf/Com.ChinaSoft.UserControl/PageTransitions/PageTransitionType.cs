namespace Com.ChinaSoft.UserControls.PageTransitions
{
    public enum PageTransitionType
    {
        None,
        Fade,
        Slide,
        SlideAndFade,
        Grow,
        GrowAndFade,
        Flip,
        FlipAndFade,
        Spin,
        SpinAndFade,

        /// <summary>
        /// 自下向上滑出，无左右偏移
        /// </summary>
        CustomFade,

        /// <summary>
        /// 自上向下滑出，无左右偏移
        /// </summary>
        CustomFade2,

        /// <summary>
        /// 自下向上滑出，速度慢，幅度大
        /// </summary>
        PartSlide
    }
}