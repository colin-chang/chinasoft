using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Sercenter.Views;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.Sercenter.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region Ctor

        public MainViewModel()
        {
            _urlRepairbillList = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "list").ToUrl();

            Initiliaze();
        }

        #endregion Ctor

        #region Constant and Fields

        private const string Placeholder = "请输入搜索关键字";
        private string _searchKeyword = Placeholder; // 默认搜索关键字
        private RepairbillStatus _selectedStatus = RepairbillStatus.All;

        private readonly string _urlRepairbillList;

        #endregion Constant and Fields

        #region Method

        private async void Initiliaze()
        {
            await BindRepairBillList();
        }

        /// <summary>
        ///     加载报修列表数据
        /// </summary>
        /// <returns></returns>
        internal async Task BindRepairBillList(bool isClear = true)
        {
            try
            {
                var dictParams = new Dictionary<string, string>();
                var strSelectedStatus = SelectedStatus == RepairbillStatus.All
                    ? ""
                    : ((int) SelectedStatus).ToString();

                var queryKey = SearchKeyword.Equals(Placeholder) ? "" : SearchKeyword;
                string paramsJson =
                    $"{{'query':{{'status':'{strSelectedStatus}','queryKey':'{queryKey}'}},'pager':{{'pageIndex':'{Pager}','pageSize':'{PageSize}'}},'sort':{{}}}}";
                dictParams.Add("params", paramsJson);

                var json = await HttpService.GetAsync(_urlRepairbillList, dictParams, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ItemsJsonResult<Repairbill>>(json);
                TotalPages = result.TotalPages;

                if (!result.Success)
                {
                    ConfirmBox.Show(MSG_API_FAILED);
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if (isClear) RepairbillCollection.Clear();
                    result.Item?.ForEach(item => RepairbillCollection.Add(item));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
            finally
            {
                HideProgressRing();
            }
        }

        #endregion Method

        #region Command

        /// <summary>
        ///     报修状态选中时的处理
        /// </summary>
        public DelegateCommand<RepairbillStatus?> SelectReparibillStatusCommand =>
            new DelegateCommand<RepairbillStatus?>(async stat =>
            {
                if (!stat.HasValue) return;

                SelectedStatus = stat.Value;

                ShowProgressRing();
                await BindRepairBillList();
                HideProgressRing();
            });

        /// <summary>
        ///     根据不同状态，显示相应窗口
        /// </summary>
        public DelegateCommand<Repairbill> ShowDetailCommand =>
            new DelegateCommand<Repairbill>(async item =>
            {
                Window window = new QuestionDetailWindow(item);
                window.Owner = Application.Current.MainWindow;
                var b = window.ShowDialog();
                if (b.HasValue && b.Value)
                {
                    ShowProgressRing();
                    await BindRepairBillList();
                }
            });

        /// <summary>
        ///     搜索框获取焦点时
        /// </summary>
        public DelegateCommand GotFocusCommand =>
            new DelegateCommand(() =>
            {
                if (_searchKeyword.Equals(Placeholder))
                    SearchKeyword = string.Empty;
            });

        /// <summary>
        ///     搜索框失去焦点时
        /// </summary>
        public DelegateCommand LostFocusCommand =>
            new DelegateCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(_searchKeyword))
                    SearchKeyword = Placeholder;
            });

        /// <summary>
        ///     执行搜索
        /// </summary>
        public DelegateCommand SearchCommand =>
            new DelegateCommand(async () =>
            {
                ShowProgressRing();

                Pager = 1;
                await BindRepairBillList();

                HideProgressRing();
            });

        #endregion Command

        #region Properties

        /// <summary>
        ///     获取起始主页面
        /// </summary>
        public override UserControl MainUC => new MainUC();

        /// <summary>
        ///     报修单集合
        /// </summary>
        public ObservableCollection<Repairbill> RepairbillCollection { get; } = new ObservableCollection<Repairbill>();

        /// <summary>
        ///     当前页码
        /// </summary>
        public int Pager { get; set; } = 1;

        /// <summary>
        ///     分页大小
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        ///     分页总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        ///     搜索关键字
        /// </summary>
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { SetProperty(ref _searchKeyword, value); }
        }

        /// <summary>
        ///     用作筛选的报修状态
        /// </summary>
        public RepairbillStatus SelectedStatus
        {
            get { return _selectedStatus; }
            set { SetProperty(ref _selectedStatus, value); }
        }

        #endregion Properties
    }
}