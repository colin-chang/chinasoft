using System.Threading.Tasks;

namespace Com.ChinaSoft.Model.ViewModels
{
    /// <summary>
    /// 懒加载数据ViewModel
    /// </summary>
    public interface ILazyLoadingViewModel
    {
        int PageIndex { get; set; }

        int PageSize { get; set; }

        int Total { get; set; }

        Task LoadMore();
    }
}
