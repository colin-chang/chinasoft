using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.WorkOrder.Model.WorkOrder;
using Com.ChinaSoft.WorkOrder.Views;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.WorkOrder.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Constructor

        internal MainViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += RetrieveInProcessData;

            Initialize();
        }

        #endregion Constructor

        #region Constant and Private Fields

        private string url_WorkOrderList; // 待执行工单列表 api url
        private string url_WorkOrderCurrent; // 正在执行的工单 api url
        private string url_WorkOrderInProcess; // 工单生产实时数据 api url
        private string url_WorkOrderStart; // 开始工单 api url
        private string url_WorkOrderEnd; // 结束工单 api url
        private string url_WorkOrderReplay; // 交班/换班 api url

        private readonly DispatcherTimer _timer; // 计时器
        private double _pointerAngle; // 实时工单产量在仪表盘上的指针角度
        private bool _enableStartButton = true; // 是否启用开始按钮，默认true
        private bool _enableEndButton; // 是否启用结束按钮，默认false
        private bool _enableHandOverButton = true; // 是否启用"交班"按钮，默认true
        private bool _enableTakeOverButton; // 是否启用"接班"按钮，默认false

        private WorkOrderItem _curWorkOrder;
        private WorkOrderInProcess _curWorkOrderProcess;

        #endregion Constant and Private Fields

        #region Methods

        /// <summary>
        ///     初始化
        /// </summary>
        private async void Initialize()
        {
            url_WorkOrderList = Path.Combine(ApiBaseUrl, "workorder", "list").ToUrl();
            url_WorkOrderCurrent = Path.Combine(ApiBaseUrl, "workorder", "currorder", "detail", ModuleId).ToUrl();
            url_WorkOrderReplay = Path.Combine(ApiBaseUrl, "workorder", "currorder", "relay", "add").ToUrl();

            await LoadRunningWorkOrdersAsync();
            //taskRunningWK.ContinueWith((t) => LoadRealtimeDataAsync());
            //await LoadRealtimeDataAsync();
            await LoadWaitingWorkOrdersAsync();

            ProgressRing.Hide();
        }

        /// <summary>
        ///     获取工单实时生产数据
        /// </summary>
        private async void RetrieveInProcessData(object sender, EventArgs e)
        {
            //await LoadRunningWorkOrdersAsync();
            await LoadRealtimeDataAsync();
        }

        /// <summary>
        ///     加载待执行工单列表数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadWaitingWorkOrdersAsync()
        {
            var prms = new Dictionary<string, string>();
            prms.Add("params",
                string.Format(
                    "{{'query':{{'equipmentId':'{0}'}},'pager':{{'pageIndex':'{1}','pageSize':'{2}'}},'sort':{{}}}}",
                    ModuleId, 1, 10));

            try
            {
                var json = await HttpService.GetAsync(url_WorkOrderList, prms, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    NotStartedWorkOrderList.Clear();
                    result.Item.ForEach(c => NotStartedWorkOrderList.Add(c));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     加载当前正在执行工单数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadRunningWorkOrdersAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(url_WorkOrderCurrent, null, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderCurrentResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                CurWorkOrder = result.Obj;
                if (CurWorkOrder == null)
                    return;

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if ((CurWorkOrder.Status == WorkOrderStatus.InProcess) ||
                        (CurWorkOrder.Status == WorkOrderStatus.Done))
                    {
                        EnableStartButton = false;
                        EnableEndButton = true;
                    }

                    CurWorkOrderRecipe.Clear();
                    result.Obj.Recipes.ForEach(item => CurWorkOrderRecipe.Add(item));

                    CurWorkOrderTechStandards.Clear();
                    result.Obj.TechStandards.ForEach(item => CurWorkOrderTechStandards.Add(item));

                    QuarterDashboardNumber(CurWorkOrder.PlanOutput);
                }));

                if (CurWorkOrder.Status == WorkOrderStatus.InProcess)
                    await LoadRealtimeDataAsync();
                else
                    CurWorkOrderProcess = new WorkOrderInProcess();
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
            finally
            {
                ProgressRing.Hide();
            }
        }

        /// <summary>
        ///     将计划产品四等分后，显示在仪表盘周围
        /// </summary>
        /// <param name="planOutput">当前正在进行工单的计划产量</param>
        private void QuarterDashboardNumber(double? planOutput)
        {
            const int PART = 4;

            if (planOutput.HasValue)
            {
                var total = planOutput.Value;

                var avg = (int) Math.Floor(total/PART);

                DashboardNumber1 = avg;
                DashboardNumber2 = DashboardNumber1 + avg;
                DashboardNumber3 = DashboardNumber2 + avg;
                DashboardNumber4 = (int) Math.Ceiling(total);
            }
        }

        /// <summary>
        ///     加载实时生产数据
        /// </summary>
        private async Task LoadRealtimeDataAsync()
        {
            try
            {
                url_WorkOrderInProcess =
                    Path.Combine(ApiBaseUrl, "workorder", "currorder", "rtd", CurWorkOrder.Id).ToUrl();
                var json = await HttpService.GetAsync(url_WorkOrderInProcess, null, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderInProcessResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() => { CurWorkOrderProcess = result.Obj; }));

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if (CurWorkOrder != null)
                    {
                        //CurWorkOrder.FactOutput = CurWorkOrderProcess.Output;   // 更新UI上的实际产量

                        // 计算当前实际产量对应的指针角度
                        var planOutput = CurWorkOrder.PlanOutput.Value;
                        var output = CurWorkOrderProcess.Output.Value;
                        if (planOutput == 0)
                        {
                            PointerAngle = 0;
                        }
                        else
                        {
                            var angle = output/planOutput*planOutput;
                            PointerAngle = output <= planOutput ? angle : 180;
                        }

                        if (_enableEndButton)
                            _timer.Start();
                    }
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     开始工单
        /// </summary>
        private async Task<bool> StartWorkOrderAsync()
        {
            try
            {
                url_WorkOrderStart =
                    Path.Combine(ApiBaseUrl, "workorder", "currorder", "start", _curWorkOrder.Id).ToUrl();
                var json = await HttpService.PutAsync(url_WorkOrderStart);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return false;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderBaseResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     结束工单
        /// </summary>
        private async Task<bool> StopWorkOrderAsync()
        {
            try
            {
                url_WorkOrderEnd = Path.Combine(ApiBaseUrl, "workorder", "currorder", "end", _curWorkOrder.Id).ToUrl();
                var json = await HttpService.PutAsync(url_WorkOrderEnd);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return false;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderBaseResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return false;
                }

                CurWorkOrderProcess = new WorkOrderInProcess();

                return true;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return false;
            }
        }

        #endregion Methods

        #region Command

        /// <summary>
        ///     开始工单
        /// </summary>
        public DelegateCommand StartCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    if (CurWorkOrder == null)
                    {
                        ConfirmBox.Show("当前没有正在执行的工单，请稍后重试！", MSG_OPERATION_TITLE);
                        await LoadRunningWorkOrdersAsync();
                        return;
                    }

                    ProgressRing.Show();
                    var result = await StartWorkOrderAsync();
                    if (result)
                    {
                        EnableStartButton = false;
                        EnableEndButton = true;

                        if ((_timer != null) || (_timer.IsEnabled == false))
                        {
                            _timer.Start();
                            PointerAngle = 0D;
                        }

                        await LoadRunningWorkOrdersAsync();
                        await LoadWaitingWorkOrdersAsync();
                    }
                    else
                    {
                        ConfirmBox.Show("开始工单失败！", MSG_OPERATION_TITLE);
                    }
                    ProgressRing.Hide();
                });
            }
        }

        /// <summary>
        ///     结束工单
        /// </summary>
        public DelegateCommand EndCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    var mbResult = ConfirmBox.Show("确认要结束当前工单吗?", "确认信息", MessageBoxButton.OKCancel);
                    if (mbResult)
                    {
                        ProgressRing.Show();
                        var result = await StopWorkOrderAsync();
                        if (result)
                        {
                            EnableStartButton = true;
                            EnableEndButton = false;

                            if ((_timer != null) || _timer.IsEnabled)
                            {
                                _timer.Stop();
                                PointerAngle = 0d;
                            }

                            await LoadRunningWorkOrdersAsync(); // 刷新当前工单数据
                            await LoadWaitingWorkOrdersAsync(); // 刷新待执行工单列表
                        }
                        else
                        {
                            ConfirmBox.Show("结束工单失败！", MSG_OPERATION_TITLE);
                        }
                        ProgressRing.Hide();
                    }
                });
            }
        }

        /// <summary>
        ///     交班/换班操作
        /// </summary>
        public DelegateCommand<string> ReplayCommand
        {
            get
            {
                return new DelegateCommand<string>(async cmdParam =>
                {
                    if (_curWorkOrder == null)
                    {
                        ConfirmBox.Show("当前没有正在执行的工单，操作失败!", MSG_OPERATION_TITLE);
                        return;
                    }

                    var type = (ReplayType) Enum.Parse(typeof(ReplayType), cmdParam);

                    string postData =
                        $"{{\"workOrderId\":\"{_curWorkOrder.OrderNo}\",\"equipmentId\":\"{ModuleId.ToString()}\",\"type\":\"{(int) type}\"}}";

                    if (type == ReplayType.HandOver) // 是否交班
                    {
                        var mbResult = ConfirmBox.Show("您确认这一班工作已完成, 开始交班么?", "确认信息", MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (!mbResult)
                            return;
                    }
                    else if (type == ReplayType.TakeOver) // 是否接班
                    {
                        var mbResult = ConfirmBox.Show("您确认上一班工作已完成, 开始接班么?", "确认信息", MessageBoxButton.OKCancel,
                            MessageBoxImage.Question);
                        if (!mbResult)
                            return;
                    }

                    ProgressRing.Show();
                    var result = await ReplayOperation(postData);
                    if (result)
                        if (type == ReplayType.HandOver)
                        {
                            EnableHandOverButton = false;
                            EnableTakeOverButton = true;
                        }
                        else if (type == ReplayType.TakeOver)
                        {
                            EnableHandOverButton = true;
                            EnableTakeOverButton = false;
                        }
                    ProgressRing.Hide();
                });
            }
        }

        /// <summary>
        ///     执行交班/换班操作
        /// </summary>
        /// <param name="postData">提交的数据</param>
        /// <returns></returns>
        private async Task<bool> ReplayOperation(string postData)
        {
            try
            {
                var json = await HttpService.PostAsync(url_WorkOrderReplay, postData, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return false;
                }

                var result = JsonConvert.DeserializeObject<WorkOrderBaseResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return false;
            }
        }

        #endregion Command

        #region Properties

        public override UserControl MainUC
        {
            get { return new MainUC(); }
        }

        /// <summary>
        ///     当前正在执行工单的概览
        /// </summary>
        public WorkOrderItem CurWorkOrder
        {
            get { return _curWorkOrder; }
            set { SetProperty(ref _curWorkOrder, value); }
        }

        /// <summary>
        ///     待执行工单列表
        /// </summary>
        public ObservableCollection<WorkOrderItem> NotStartedWorkOrderList { get; } =
            new ObservableCollection<WorkOrderItem>();

        /// <summary>
        ///     当前正在执行工单的辅料信息
        /// </summary>
        public ObservableCollection<WorkOrderRecipe> CurWorkOrderRecipe { get; } =
            new ObservableCollection<WorkOrderRecipe>();

        /// <summary>
        ///     当前执行工单的工艺标准
        /// </summary>
        public ObservableCollection<WorkOrderTechStandards> CurWorkOrderTechStandards { get; } =
            new ObservableCollection<WorkOrderTechStandards>();

        /// <summary>
        ///     工单生产实时数据
        /// </summary>
        public WorkOrderInProcess CurWorkOrderProcess
        {
            get { return _curWorkOrderProcess; }
            set { SetProperty(ref _curWorkOrderProcess, value); }
        }

        /// <summary>
        ///     指针角度
        /// </summary>
        public double PointerAngle
        {
            get { return _pointerAngle; }
            set { SetProperty(ref _pointerAngle, value); }
        }

        /// <summary>
        ///     是否启用开始按钮
        /// </summary>
        public bool EnableStartButton
        {
            get { return _enableStartButton; }
            set { SetProperty(ref _enableStartButton, value); }
        }

        /// <summary>
        ///     是否启用结束按钮
        /// </summary>
        public bool EnableEndButton
        {
            get { return _enableEndButton; }
            set { SetProperty(ref _enableEndButton, value); }
        }

        /// <summary>
        ///     是否启用“交班”按钮
        /// </summary>
        public bool EnableHandOverButton
        {
            get { return _enableHandOverButton; }
            set { SetProperty(ref _enableHandOverButton, value); }
        }

        /// <summary>
        ///     是否启用“接班”按钮
        /// </summary>
        public bool EnableTakeOverButton
        {
            get { return _enableTakeOverButton; }
            set { SetProperty(ref _enableTakeOverButton, value); }
        }

        private int _dashboardNumber1;

        /// <summary>
        ///     仪表盘数字1
        /// </summary>
        public int DashboardNumber1
        {
            get { return _dashboardNumber1; }
            set { SetProperty(ref _dashboardNumber1, value); }
        }

        private int _dashboardNumber2;

        /// <summary>
        ///     仪表盘数字2
        /// </summary>
        public int DashboardNumber2
        {
            get { return _dashboardNumber2; }
            set { SetProperty(ref _dashboardNumber2, value); }
        }

        private int _dashboardNumber3;

        /// <summary>
        ///     仪表盘数字3
        /// </summary>
        public int DashboardNumber3
        {
            get { return _dashboardNumber3; }
            set { SetProperty(ref _dashboardNumber3, value); }
        }

        private int _dashboardNumber4;

        /// <summary>
        ///     仪表盘数字4
        /// </summary>
        public int DashboardNumber4
        {
            get { return _dashboardNumber4; }
            set { SetProperty(ref _dashboardNumber4, value); }
        }

        #endregion Properties
    }
}