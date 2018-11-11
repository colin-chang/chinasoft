using System.Threading.Tasks;

namespace Com.ChinaSoft.Model.ViewModels
{
    public abstract class LazyLoadingViewModel : ViewModelBase, ILazyLoadingViewModel
    {
        public int PageIndex { get; set; } = 1;

        public virtual int PageSize { get; set; } = 15;

        public int Total { get; set; }

        /// <summary>
        /// 加载更多数据
        /// </summary>
        /// <returns></returns>
        public async Task LoadMore()
        {
            if (PageIndex * PageSize >= Total)
                return;

            ShowProgressRing();
            PageIndex++;
            await AppendData();
            HideProgressRing();
        }

        /// <summary>
        /// 加载DataGrid列表数据的方法
        /// </summary>
        /// <returns></returns>
        public abstract Task AppendData();
    }
}
