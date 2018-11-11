using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.AlertCenter.ViewModels
{
    /// <summary>
    ///     寿命预警详情view model
    /// </summary>
    internal class LifetimeDetailViewModel : BaseViewModel
    {
        #region Constructor

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="alertId">预警唯一标识</param>
        /// <param name="strategyId">预警策略的唯一标识</param>
        public LifetimeDetailViewModel(string alertId, string strategyId)
        {
            _strategyId = strategyId;
            _alertId = alertId;

            _urlLifetimeDetail = Path.Combine(ApiBaseUrl, "component", "bom", "detail", _strategyId).ToUrl();
            _urlChangelist = Path.Combine(ApiBaseUrl, "component", "changeinfo", "list").ToUrl();
            _urlAddChange = Path.Combine(ApiBaseUrl, "component", "changeinfo", "add").ToUrl();
            _urlBomChangeInfo = Path.Combine(ApiBaseUrl, "warning", "fault", "changeinfo", alertId).ToUrl();

            Initialize();
        }

        #endregion Constructor

        #region Constant And Fields

        private readonly string _urlLifetimeDetail;     // 寿命预警详情 api url 　　
        private readonly string _urlChangelist;         // 换件历史 api url
        private readonly string _urlAddChange;          // 新增换件 api url
        private readonly string _urlBomChangeInfo;      // 新增换件 api url 
        private readonly string _strategyId;            // 预警策略的唯一标识 
        private readonly string _alertId;               // 预警唯一标识 

        private BomDetail _bomDetail;
        private int _selectedTabIndex;
        private Visibility _submitButtonVisibility = Visibility.Visible;

        #endregion Constant And Fields

        #region Method

        private async void Initialize()
        {
            await BindLifetimeDetailAsync();
            await BindChangeListAsync();

            ProgressRing?.Hide();
        }

        /// <summary>
        ///     Binding寿命预警详情的数据
        /// </summary>
        private async Task BindLifetimeDetailAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(_urlLifetimeDetail, null, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ObjJsonResult<BomDetail>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                if (result.Obj != null)
                {
                    BomDetail = result.Obj;
                    BomDetail.InputUser = BomDetail.InputUser ?? "李勇";

                    //TODO:目前仅显示2D图，今后要替换成3D模型图
                    var path = BomDetail.Appendfile2d?.Path;
                    //BomDetail.Appendfile3d == null ? BomDetail.Appendfile2d?.Path : BomDetail.Appendfile3d?.Path;
                    if (path != null) ImagePath = Path.Combine(FileBaseUrl, path)?.ToUrl();

                    await BindBomChangeinfoAsync();
                }
            }
            catch (Exception ex)
            {
                SubmitButtonVisibility = Visibility.Collapsed;
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
            finally
            {
                if (BomDetail == null) SubmitButtonVisibility = Visibility.Hidden;
            }
        }

        /// <summary>
        ///     Binding换件历史的数据
        /// </summary>
        private async Task BindChangeListAsync(int pager = 1, int pageSize = int.MaxValue)
        {
            try
            {
                string queryJson =
                    $"{{'query':{{'bomId':'{_strategyId}'}},'pager':{{'pageIndex':{pager},'pageSize':{pageSize}}},'sort':{{}}}}";

                var param = new Dictionary<string, string> { { "params", queryJson } };
                var json = await HttpService.GetAsync(_urlChangelist, param, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ItemsJsonResult<BomChangeInfo>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    BomChangeList.Clear();
                    result.Item?.ForEach(m => BomChangeList.Add(m));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     Binding零件换件历史详情
        /// </summary>
        private async Task BindBomChangeinfoAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(_urlBomChangeInfo, null, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ObjJsonResult<BomDetail>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                var detail = result.Obj;
                BomDetail.ChangeNum = detail?.ChangeNum ?? 1;
                BomDetail.InputUser = BomDetail.InputUser ?? "李勇";
                BomDetail.ChangeReason = detail?.ChangeReason;
                BomDetail.ChangeDateStr = detail?.ChangeDateStr;
                if (detail != null)
                    SubmitButtonVisibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     新增换件
        /// </summary>
        private async Task AddBomChangeAsync()
        {
            try
            {
                var queryJson = JsonConvert.SerializeObject(new
                {
                    bomId = _bomDetail.Id,
                    changeNum = _bomDetail.ChangeNum,
                    changeReason = _bomDetail.ChangeReason,
                    equmentId = ModuleId,
                    inputUser = _bomDetail.InputUser,
                    inputTime = _bomDetail.ChangeDateStr?.ToString("yyyy-MM-dd"),
                    alertId = _alertId
                });

                var json = await HttpService.PostAsync(_urlAddChange, queryJson, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ItemsJsonResult<BomChangeInfo>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                SubmitButtonVisibility = Visibility.Hidden; // 操作成功，隐藏“提交”按钮
                await BindChangeListAsync();
                SelectedTabIndex = 1;
                await BindLifetimeDetailAsync();

                ConfirmBox.Show("操作成功", MSG_OPERATION_TITLE);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        #endregion Method

        #region Command

        /// <summary>
        ///     新增换件
        /// </summary>
        public DelegateCommand ChangeBomCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    if (!ValidInput()) return;

                    var mbResult = ConfirmBox.Show(MSG_ChangeBomConfirm, MSG_OPERATION_TITLE, MessageBoxButton.OKCancel,
                        MessageBoxImage.Question);
                    if (mbResult)
                    {
                        ProgressRing?.Show();
                        await AddBomChangeAsync();
                        ProgressRing?.Hide();
                    }
                });
            }
        }

        /// <summary>
        ///     验证输入
        /// </summary>
        /// <param name="bomDetail"></param>
        /// <returns></returns>
        private bool ValidInput()
        {
            if (string.IsNullOrWhiteSpace(BomDetail.ChangeReason))
            {
                ConfirmBox.Show("换件原因不能为空", MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (BomDetail.ChangeDateStr == null)
            {
                ConfirmBox.Show("换件日期不能为空", MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(BomDetail.InputUser))
            {
                ConfirmBox.Show("换件人不能为空", MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion Command

        #region Property

        public override UserControl MainUC => null;

        public string ApiDictBaseUrl
        {
            get { return ConfigurationManager.AppSettings["DictUrl"]; }
        }

        // 隐藏父类的UIDispatcher、Panel、ProgressRing 属性
        private new Dispatcher UIDispatcher => Session[PropertyKey.Key_LifetimeDetailUIDispatcher] as Dispatcher;

        private new Panel Panel => Session[PropertyKey.Key_LifetimeDetailPanel] as Panel;

        private new ProgressRing ProgressRing => Panel.FindName("ring") as ProgressRing;

        /// <summary>
        ///     寿命预警详情
        /// </summary>
        public BomDetail BomDetail
        {
            get { return _bomDetail; }
            set { SetProperty(ref _bomDetail, value); }
        }

        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty(ref _imagePath, value); }
        }

        public ObservableCollection<BomChangeInfo> BomChangeList { get; } = new ObservableCollection<BomChangeInfo>();

        /// <summary>
        ///     备件提交按钮的可见性
        /// </summary>
        public Visibility SubmitButtonVisibility
        {
            get { return _submitButtonVisibility; }
            set { SetProperty(ref _submitButtonVisibility, value); }
        }

        /// <summary>
        ///     TabControl当前选中的项的索引值
        /// </summary>
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { SetProperty(ref _selectedTabIndex, value); }
        }

        #endregion Property
    }
}