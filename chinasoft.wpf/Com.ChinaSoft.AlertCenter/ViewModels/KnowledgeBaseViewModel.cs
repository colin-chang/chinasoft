using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.Views;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.UserControls.PageTransitions;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.AlertCenter.ViewModels
{
    internal class KnowledgeBaseViewModel : BaseViewModel, ILazyLoadingViewModel
    {
        #region Constructor

        public KnowledgeBaseViewModel()
        {
            partId = Session[PropertyKey.Key_FaultDetailPartID] as string;
            url_knowledgeBase = Path.Combine(ApiBaseUrl, "knowledge", "list").ToUrl();

            Initialize();
        }

        #endregion Constructor

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Total { get; set; }

        public async Task LoadMore()
        {
            if (PageIndex * PageSize >= Total) return;

            PageIndex++;

            await BindKnowledgeBaseAsync();
        }

        #region Contant and Fields

        private const string PLACEHOLDER = "请输入错误代码或现象描述";
        private string _searchKnowledgeKeyword = PLACEHOLDER;

        private readonly string url_knowledgeBase; // 获取专家经验库列表 api url

        private readonly string partId; // 故障预警的部件ID

        #endregion Contant and Fields

        #region Method

        private async void Initialize()
        {
            await BindKnowledgeBaseAsync();
        }

        /// <summary>
        ///     专家经验库 DataBinding
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        private async Task BindKnowledgeBaseAsync()
        {
            if (string.IsNullOrEmpty(partId)) return;

            try
            {
                var keyword = _searchKnowledgeKeyword == PLACEHOLDER ? null : _searchKnowledgeKeyword;

                var paramDict = new Dictionary<string, string>();
                string queryJson =
                    $"{{'query':{{'modelId':'{ModuleId}','partId':'{partId}','queryKey':'{keyword}'}},'pager':{{'pageIndex':'{PageIndex}','pageSize':'{PageSize}'}},'sort':{{}}}}";
                paramDict.Add("params", queryJson);

                var json = await HttpService.GetAsync(url_knowledgeBase, paramDict, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ItemsJsonResult<KnowledgeDetail>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    Total = result.Total;

                    if (PageIndex == 1) KnowledgeList.Clear();
                    result.Item?.ForEach(item => KnowledgeList.Add(item));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        #endregion Method

        #region Command

        /// <summary>
        ///     返回到"故障详情"页面
        /// </summary>
        public DelegateCommand SwitchBackToFaultDetailCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var win =
                        Application.Current.Properties[PropertyKey.Key_FaultDetailWin] as FaultDetailWinow;
                    if (win == null) return;
                    var mainUC = win.FindName("mainUC") as UserControl;
                    if (mainUC == null) return;

                    var pt = mainUC.FindName("pageTranstion_FaultInfo") as PageTransition;
                    if (pt == null) throw new ArgumentNullException("PageTransition", "找不到PageTransition");

                    var uc = new FaultInfoUC();
                    pt.TransitionType = PageTransitionType.CustomFade2;
                    pt.ShowPage(uc);
                });
            }
        }

        /// <summary>
        ///     搜索专家经验库
        /// </summary>
        public DelegateCommand SearchKnowledgeCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    ProgressRing.Show();
                    PageIndex = 1;
                    await BindKnowledgeBaseAsync();
                    ProgressRing.Hide();
                });
            }
        }

        /// <summary>
        ///     专家经验库搜索文本框，获得焦点 Command
        /// </summary>
        public DelegateCommand GotFocusCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (SearchKnowledgeKeyword == PLACEHOLDER) SearchKnowledgeKeyword = null;
                });
            }
        }

        /// <summary>
        ///     专家经验库搜索文本框，失去焦点 Command
        /// </summary>
        public DelegateCommand LostFocusCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(SearchKnowledgeKeyword)) SearchKnowledgeKeyword = PLACEHOLDER;
                });
            }
        }

        #endregion Command

        #region Property

        public override UserControl MainUC => null;

        /// <summary>
        ///     专家经验库数据
        /// </summary>
        public ObservableCollection<KnowledgeDetail> KnowledgeList { get; } =
            new ObservableCollection<KnowledgeDetail>();

        /// <summary>
        ///     专家经验库搜索关键词
        /// </summary>
        public string SearchKnowledgeKeyword
        {
            get { return _searchKnowledgeKeyword; }
            set { SetProperty(ref _searchKnowledgeKeyword, value); }
        }

        #endregion Property
    }
}