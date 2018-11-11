using Com.ChinaSoft.AlertCenter.Models;
using Com.ChinaSoft.AlertCenter.Views;
using Com.ChinaSoft.Model;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.UserControls;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.UserControls.PageTransitions;
using Com.ChinaSoft.UserControls.RoundCorner;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Com.ChinaSoft.AlertCenter.ViewModels
{
    /// <summary>
    ///     故障预警详情ViewModel
    /// </summary>
    internal class FaultDetailViewModel : BaseViewModel
    {
        #region Ctor

        public FaultDetailViewModel(string alertId, string strategyId)
        {
            _alertId = alertId;
            _strategyId = strategyId;

            _urlFaultDetail = Path.Combine(ApiBaseUrl, "warning", "strategy", "fault", "detail", _strategyId).ToUrl();
            _urlFaultHistory = Path.Combine(ApiBaseUrl, "warning", "fault", "list").ToUrl();
            _urlRepairBillType = Path.Combine(ApiDictBaseUrl, "dict_rb_type").ToUrl();

            _urlRepairBillDetail =
                Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "alert", "detail", _alertId).ToUrl();
            _urlSaveRepairBill = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "add").ToUrl();
            _urlSubmitRepairBill = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "submit").ToUrl();

            Initialize();
        }

        #endregion Ctor

        #region 报修单字段

        private string _id;
        private string _inputTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        private string _title;
        private string _content;
        private int _isSendFac;
        private string _typeId;

        /// <summary>
        ///     唯一标识
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        /// <summary>
        ///     输入时间
        /// </summary>
        public string InputTime
        {
            get { return _inputTime; }
            set { SetProperty(ref _inputTime, value); }
        }

        /// <summary>
        ///     标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        ///     编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     报修单内容
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        /// <summary>
        ///     是否发送给厂家(0.不发送 1.发送)
        /// </summary>
        public int IsSendFac
        {
            get { return _isSendFac; }
            set { SetProperty(ref _isSendFac, value); }
        }

        /// <summary>
        ///     问题类型
        /// </summary>
        public string TypeId
        {
            get { return _typeId; }
            set { SetProperty(ref _typeId, value); }
        }

        /// <summary>
        ///     反馈途径(0.设备反馈 1.企业反馈 2.服务中心反馈)
        /// </summary>
        public int FbWay { get; set; }

        /// <summary>
        ///     状态（0.保存,1.提交）。保存后还可以修改，提交后只能查看不能再修改
        /// </summary>
        public RepairBillStatus Status { get; set; }

        /// <summary>
        ///     设备唯一标识
        /// </summary>
        public string EquipmentId { get; set; }

        #endregion 报修单字段

        #region Constant and Fields

        private const string Placeholder = "请输入错误代码或现象描述";

        private readonly string _urlFaultDetail; // 故障预警详情 api url
        private readonly string _urlFaultHistory; // 故障预警历史 api url
        private readonly string _urlRepairBillType; // 获取报修单问题类型 api url

        private readonly string _urlRepairBillDetail; // 报修单详情 api url
        private readonly string _urlSubmitRepairBill; // 提交报修单 api url
        private readonly string _urlSaveRepairBill; // 保存报修单 api url
        private string _urlUpdateRepairBill; // 更新报修单 api url

        private Visibility _submitButtonVisibity = Visibility.Visible;
        private Visibility _saveButtonVisibity = Visibility.Visible;

        private readonly string _alertId; // 报警信息唯一标识
        private readonly string _strategyId; // 预警策略唯一标识
        private string _imagePath; // 预警图

        private FaultDetail _faultDetail;

        #endregion Constant and Fields

        #region Method

        private async void Initialize()
        {
            var task = BindFaultDetailAsync();
            Task taskFaultHistory = task.ContinueWith(t => BindFaultHistoryAsync());
            await taskFaultHistory.ContinueWith(t => BindRepairBillTypeAsync());

            await BindRepairBillDetailAsync(); // 绑定报修单详情

            ProgressRing.Hide();
        }

        /// <summary>
        ///     故障预警详情 DataBinding
        /// </summary>
        private async Task BindFaultDetailAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(_urlFaultDetail, null, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ObjJsonResult<FaultDetail>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                if (result.Obj != null)
                {
                    FaultDetail = result.Obj;

                    //TODO:目前仅显示2D图，今后要替换成3D模型图
                    var path = FaultDetail.Appendfile2D?.Path;
                    // FaultDetail.Appendfile3D == null ? FaultDetail.Appendfile2D?.Path : FaultDetail.Appendfile3D.Path;
                    if (path != null) ImagePath = Path.Combine(FileBaseUrl, path)?.ToUrl();

                    // 将该故障预警的部件ID存储在全局属性中
                    Application.Current.Properties[PropertyKey.Key_FaultDetailPartID] = FaultDetail.PartId;
                }
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     故障预警历史 DataBinding
        /// </summary>
        private async Task BindFaultHistoryAsync(int pager = 1, int pageSize = 20)
        {
            try
            {
                var param = new Dictionary<string, string>
                {
                    {
                        "params",
                        $"{{'query':{{'strategyId':'{_alertId}'}},'pager':{{'pageIndex':'{pager}','pageSize':'{pageSize}'}},'sort':{{}}}}"
                    }
                };

                var json = await HttpService.GetAsync(_urlFaultHistory, param, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ItemsJsonResult<FaultHistory>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    FaultHistoryList.Clear();
                    result.Item?.ForEach(item => FaultHistoryList.Add(item));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     报修单问题类型 DataBinding
        /// </summary>
        private async void BindRepairBillTypeAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(_urlRepairBillType, null, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<DictType[]>(json);

                if (result == null)
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    AllRepairBillType.Clear();
                    Array.ForEach(result, item => AllRepairBillType.Add(item));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     报修单详情
        /// </summary>
        /// <param name="detail"></param>
        private async Task BindRepairBillDetailAsync()
        {
            if (_faultDetail == null)
            {
                SubmitButtonVisibity = SaveButtonVisibity = Visibility.Hidden;
                InitContent();
                return;
            }

            try
            {
                var json = await HttpService.GetAsync(_urlRepairBillDetail, null, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ObjJsonResult<RepairBillDetail>>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await UIDispatcher.BeginInvoke(new Action(() =>
                {
                    var obj = result.Obj;
                    UpdateUi(obj);
                    InitContent();
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     更新报修单UI元素的值
        /// </summary>
        /// <param name="info"></param>
        private void UpdateUi(RepairBillDetail info)
        {
            if (info == null) return;

            Code = info.Code;
            Content = info.Content;
            EquipmentId = info.EquipmentId;
            FbWay = info.FbWay;
            Id = info.Id;
            InputTime = info.InputTime;
            IsSendFac = info.IsSendFac;
            Status = info.Status;
            Title = info.Title;
            TypeId = info.TypeId;

            if (info.Status == RepairBillStatus.Submit)
                SubmitButtonVisibity = SaveButtonVisibity = Visibility.Hidden;
        }

        /// <summary>
        ///     初始化报修内容
        /// </summary>
        private void InitContent()
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                var newContent = new StringBuilder();
                newContent.AppendFormat("预警部件:{0}{1}", _faultDetail?.PartName, Environment.NewLine);
                newContent.AppendFormat("预警公式:{0}{1}", _faultDetail?.Expression, Environment.NewLine);
                newContent.AppendFormat("预警等级:{0}{1}", _faultDetail?.Level, Environment.NewLine);
                newContent.AppendFormat("影响程度:{0}{1}", _faultDetail?.Extent, Environment.NewLine);
                newContent.AppendFormat("原因:{0}{1}", _faultDetail?.Knowledge?.CaseContent, Environment.NewLine);
                newContent.AppendFormat("措施:{0}{1}", _faultDetail?.Knowledge?.Trentent, Environment.NewLine);
                Content = newContent.ToString();
            }
        }

        /// <summary>
        ///     验证输入
        /// </summary>
        /// <returns></returns>
        private bool ValidateInput()
        {
            var errorMsg = string.Empty;
            if (string.IsNullOrEmpty(_title)) errorMsg = "标题不能为空";
            else if (string.IsNullOrEmpty(_typeId)) errorMsg = "问题类型不能为空";
            else if (string.IsNullOrEmpty(_content)) errorMsg = "内容不能为空";

            if (!string.IsNullOrEmpty(errorMsg))
            {
                ConfirmBox.Show(errorMsg);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     生成提交参数json
        /// </summary>
        /// <returns></returns>
        private string GenPostJson()
        {
            return JsonConvert.SerializeObject(new
            {
                title = _title,
                alertId = _alertId,
                content = _content,
                typeId = _typeId,
                fbWay = 0,
                isSendFac = _isSendFac,
                equipmentId = ModuleId,
                recardUserName = NameOfFeedback
            });
        }

        /// <summary>
        ///     提交保修单
        /// </summary>
        /// <param name="detail"></param>
        private async Task SubmitRepairBillAsync()
        {
            try
            {
                var param = GenPostJson();

                var json = await HttpService.PostAsync(_urlSubmitRepairBill, param, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<JsonResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                SubmitButtonVisibity = SaveButtonVisibity = Visibility.Hidden;

                ConfirmBox.Show("提交成功", MSG_OPERATION_TITLE, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     保存保修单
        /// </summary>
        private async Task SaveRepairBillAsync()
        {
            try
            {
                var param = GenPostJson();
                var json = await HttpService.PostAsync(_urlSaveRepairBill, param, true);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<JsonResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }
                ConfirmBox.Show("保存成功", MSG_OPERATION_TITLE, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        ///     更新报修单
        /// </summary>
        private async Task UpdateRepairBillAsync()
        {
            if (_faultDetail == null)
                throw new ArgumentNullException(nameof(_faultDetail), MSG_FaultDetail_NULL);

            try
            {
                var param = GenPostJson();
                _urlUpdateRepairBill = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "update", Id).ToUrl();
                var json = await HttpService.PutAsync(_urlUpdateRepairBill, param);

                if (string.IsNullOrEmpty(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<JsonResult>(json);

                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }
                ConfirmBox.Show("修改成功", MSG_OPERATION_TITLE, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        #endregion Method

        #region Command

        /// <summary>
        ///     切换到"专家经验库查询"页面
        /// </summary>
        public DelegateCommand SwitchToKnowledgeCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var win =
                        Application.Current.Properties[PropertyKey.Key_FaultDetailWin] as FaultDetailWinow;
                    var mainUc = win?.FindName("mainUC") as UserControl;
                    if (mainUc == null) return;

                    var pt = mainUc.FindName("pageTranstion_FaultInfo") as PageTransition;
                    if (pt == null) throw new ArgumentNullException("PageTransition", "找不到PageTransition");

                    var kbUc = new KnowledgeBaseUC();
                    pt.TransitionType = PageTransitionType.CustomFade;
                    pt.ShowPage(kbUc);
                });
            }
        }

        /// <summary>
        ///     提交报修单
        /// </summary>
        [JsonIgnore]
        public DelegateCommand SubmitRepairBillCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    if (ValidateInput())
                    {
                        ProgressRing.Show();
                        await SubmitRepairBillAsync();
                        ProgressRing.Hide();
                    }
                });
            }
        }

        /// <summary>
        ///     保存报修单
        /// </summary>
        [JsonIgnore]
        public DelegateCommand SaveRepairBillCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    if (ValidateInput())
                    {
                        ProgressRing.Show();

                        if (string.IsNullOrWhiteSpace(Id))
                            await SaveRepairBillAsync();
                        else
                            await UpdateRepairBillAsync();

                        ProgressRing.Hide();
                    }
                });
            }
        }

        [JsonIgnore]
        public DelegateCommand CloseWindowCommand => new DelegateCommand(() => { this.CurrentWindow.Close(); });

        #endregion Command

        #region Properties

        public override UserControl MainUC => new FaultInfoMainUC();

        public string ApiDictBaseUrl => ConfigurationManager.AppSettings["DictUrl"];

        // 隐藏父类的UIDispatcher、Panel、ProgressRing 属性
        private new Dispatcher UIDispatcher => Session[PropertyKey.Key_FaultDetailWinUIDispatcher] as Dispatcher;

        private new Panel Panel => Session?[PropertyKey.Key_FaultDetailWinPanel] as Panel;

        private new ProgressRing ProgressRing
        {
            get { return Panel.FindName("ring") as ProgressRing; }
        }

        /// <summary>
        ///     故障预警详情
        /// </summary>
        public FaultDetail FaultDetail
        {
            get { return _faultDetail; }
            set { SetProperty(ref _faultDetail, value); }
        }

        /// <summary>
        ///     故障预警历史列表
        /// </summary>
        public ObservableCollection<FaultHistory> FaultHistoryList { get; } = new ObservableCollection<FaultHistory>();

        /// <summary>
        ///     专家经验库数据
        /// </summary>
        public ObservableCollection<KnowledgeDetail> KnowledgeList { get; } =
            new ObservableCollection<KnowledgeDetail>();

        /// <summary>
        ///     故障预警图路径
        /// </summary>
        public string ImagePath
        {
            get { return _imagePath; }
            set { SetProperty(ref _imagePath, value); }
        }

        /// <summary>
        ///     报修单问题类型
        /// </summary>
        public ObservableCollection<DictType> AllRepairBillType { get; } = new ObservableCollection<DictType>();

        /// <summary>
        ///     报修单-提交按钮是否可见
        /// </summary>
        public Visibility SubmitButtonVisibity
        {
            get { return _submitButtonVisibity; }
            set { SetProperty(ref _submitButtonVisibity, value); }
        }

        /// <summary>
        ///     报修单-保存按钮是否可见
        /// </summary>
        public Visibility SaveButtonVisibity
        {
            get { return _saveButtonVisibity; }
            set { SetProperty(ref _saveButtonVisibity, value); }
        }

        /// <summary>
        ///     报修单-反馈人
        /// </summary>
        public string NameOfFeedback { get; set; } = "刘晓明";

        /// <summary>
        ///     报修单-反馈时间
        /// </summary>
        public DateTime TimeOfFeedback { get; set; } = DateTime.Now;

        /// <summary>
        ///     报修单-反馈时间字符串格式
        /// </summary>
        public string TimeOfFeedbackStr
        {
            get { return TimeOfFeedback.ToString("yyyy-MM-dd HH:mm"); }
        }

        #endregion Properties
    }
}