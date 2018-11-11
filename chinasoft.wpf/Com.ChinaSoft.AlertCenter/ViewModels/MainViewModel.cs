using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.Views;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.Utility;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.AlertCenter.ViewModels
{
    /// <summary>
    ///     设备预警ViewModel
    /// </summary>
    internal class MainViewModel : BaseViewModel
    {
        #region Constructor

        public MainViewModel()
        {
            Initialize();
        }

        #endregion Constructor

        #region Constant and Private Fields

        private const string placeholder = "请输入搜索关键字";

        private string url_WarningList; // 设备预警列表 api url
        private string url_warningDispost; // 处理报警信息 api url
        private string _searchKeyword = placeholder; // 默认搜索关键字

        private StrategyType _selectedType = StrategyType.All; // 当前选中的预警类别
        private int _total;

        #endregion Constant and Private Fields

        #region Methods

        private async void Initialize()
        {
            url_WarningList = Path.Combine(ApiBaseUrl, "warning", "list").ToUrl();


            await LoadWarningsAsync();
            ProgressRing.Hide();
        }

        /// <summary>
        ///     加载预警列表数据
        /// </summary>
        /// <param name="isClear">是否要先清除集合中已有元素</param>
        public async Task LoadWarningsAsync(bool isClear = true)
        {
            try
            {
                var param = new Dictionary<string, string>();

                var type = _selectedType == StrategyType.All ? null : ((int)_selectedType).ToString();
                var status = SearchStatus == StrategyStatus.All ? null : ((int)SearchStatus).ToString();
                var kw = _searchKeyword == placeholder ? "" : _searchKeyword;

                string paramJson =
                    $"{{'query':{{'equipmentId':'{ModuleId}','StrategyType':'{type}','status':'{status}','content':'{kw}'}},'pager':{{'pageIndex':'{Pager}','pageSize':'{PageSize}'}},'sort':{{}}}}";

                param.Add("params", paramJson);

                var json = await HttpService.GetAsync(url_WarningList, param, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<WarningListResult>(json);
                _total = result.Total;

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if (isClear) WarningItemList.Clear();

                    result.Item?.ForEach(item => WarningItemList.Add(item));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     处理报警信息
        /// </summary>
        /// <param name="alertId">报警信息唯一标识</param>
        private async Task<bool> WarningDisposeAsync(string alertId)
        {
            url_warningDispost = Path.Combine(ApiBaseUrl, "warning", "dispose", alertId).ToUrl();

            try
            {
                var json = await HttpService.PutAsync(url_warningDispost);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON, MSG_WarningDispose_TITLE);
                    return false;
                }

                var result = JsonConvert.DeserializeObject<JsonResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg),
                        MSG_WarningDispose_TITLE);
                    return false;
                }

                // 通知列表变化，变更当前项的处理状态
                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    var clonedWarningItemList = ObjectCopier.Clone(WarningItemList).AsEnumerable();
                    var clonedItem = clonedWarningItemList.FirstOrDefault(m => m.Id.Equals(alertId));
                    if (clonedItem != null)
                        clonedItem.Status = StrategyStatus.Handled;

                    WarningItemList.Clear();
                    foreach (var item in clonedWarningItemList)
                        WarningItemList.Add(item);
                }));

                return true;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_WarningDispose_TITLE);
                return false;
            }
        }

        /// <summary>
        ///     重置Pager
        /// </summary>
        public void ResetPager()
        {
            Pager = 1;
        }

        #endregion Methods

        #region Command

        /// <summary>
        ///     搜索框获取焦点时
        /// </summary>
        public DelegateCommand GotFocusCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (_searchKeyword.Equals(placeholder))
                        SearchKeyword = string.Empty;
                });
            }
        }

        /// <summary>
        ///     搜索框失去焦点时
        /// </summary>
        public DelegateCommand LostFocusCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (string.IsNullOrWhiteSpace(_searchKeyword))
                        SearchKeyword = placeholder;
                });
            }
        }

        /// <summary>
        ///     执行搜索
        /// </summary>
        public DelegateCommand SearchCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    ProgressRing.Show();

                    ResetPager();
                    await LoadWarningsAsync();

                    ProgressRing.Hide();
                });
            }
        }

        /// <summary>
        ///     显示预警详情窗口
        /// </summary>
        public DelegateCommand<WarningItem> ShowWarningWindowCommand
        {
            get
            {
                return new DelegateCommand<WarningItem>(async item =>
                {
                    Window win = null;

                    switch (item.StrategyType)
                    {
                        case StrategyType.TroubleAlert:
                            win = new FaultDetailWinow(item.Id, item.StrategyId);
                            break;
                        case StrategyType.LifetimeAlert:
                            win = new LifetimeDetailWindow(item.Id, item.StrategyId);
                            break;
                        case StrategyType.WrongMaterialAlert:
                            win = new WrongMaterialWindow();
                            break;
                        case StrategyType.All:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(item.StrategyType));
                    }

                    if (win != null)
                    {
                        win.Owner = Application.Current.MainWindow;
                        if (item.Status == StrategyStatus.Unhandled)
                        {
                            var result = await WarningDisposeAsync(item.Id);
                            if (!result) return;
                        }

                        win.ShowDialog();
                    }
                });
            }
        }

        /// <summary>
        ///     切换预警类别
        /// </summary>
        public DelegateCommand<StrategyType?> SelectStrategyTypeCommand
        {
            get
            {
                return new DelegateCommand<StrategyType?>(async type =>
                {
                    if (type.HasValue)
                    {
                        SelectedType = type.Value;

                        ProgressRing.Show();

                        ResetPager();
                        await LoadWarningsAsync();

                        ProgressRing.Hide();
                    }
                });
            }
        }

        #endregion Command

        #region Properties

        public override UserControl MainUC => new MainUC();

        /// <summary>
        ///     预警列表
        /// </summary>
        public ObservableCollection<WarningItem> WarningItemList { get; } = new ObservableCollection<WarningItem>();

        /// <summary>
        ///     关键字
        /// </summary>
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { SetProperty(ref _searchKeyword, value); }
        }

        /// <summary>
        ///     预警状态
        /// </summary>
        public StrategyStatus SearchStatus { get; set; } = StrategyStatus.All;

        /// <summary>
        ///     分页大小
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        ///     页码
        /// </summary>
        public int Pager { get; set; } = 1;

        /// <summary>
        ///     总页数
        /// </summary>
        public int TotalPagers
        {
            get { return Math.Max((_total - 1) / PageSize + 1, 1); }
        }

        /// <summary>
        ///     所有预警状态
        /// </summary>
        public List<StrategyStatus> AllStrategyStatus { get; set; } = new List<StrategyStatus>
        {
            StrategyStatus.All,
            StrategyStatus.Handled,
            StrategyStatus.Unhandled
        };

        /// <summary>
        ///     当前选中的预警类别
        /// </summary>
        public StrategyType SelectedType
        {
            get { return _selectedType; }
            set { SetProperty(ref _selectedType, value); }
        }

        #endregion Properties
    }
}