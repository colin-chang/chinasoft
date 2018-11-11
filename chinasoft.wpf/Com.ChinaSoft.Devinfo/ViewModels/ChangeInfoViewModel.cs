using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using Prism.Commands;

using Com.ChinaSoft.Devinfo.Views;
using System.Collections.ObjectModel;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class ChangeInfoViewModel : LazyLoadingBaseViewModel
    {
        private ChangeInfoWindow win;
        string bomId, spareDetailUrl, changeInfoAddUrl, changeInfoListUrl;

        private bool showForm = true;
        public bool ShowForm
        {
            get { return showForm; }
            set
            {
                showForm = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get { return InputDate != null && ShowForm; }
        }

        public ChangeInfoViewModel() { }
        public ChangeInfoViewModel(ChangeInfoWindow win, string id)
        {
            this.win = win;
            this.bomId = id;

            this.spareDetailUrl = Path.Combine(this.ApiBaseUrl, "component", "bom", "detail", this.bomId);
            this.changeInfoAddUrl = Path.Combine(this.ApiBaseUrl, "component", "changeinfo", "add").ToUrl();
            this.changeInfoListUrl = Path.Combine(this.ApiBaseUrl, "component", "changeinfo", "list").ToUrl();

            Initialize();
        }

        public ObservableCollection<ChangeInfoViewModel> History { get; } = new ObservableCollection<ChangeInfoViewModel>();

        async void Initialize()
        {
            await LoadItem();
            await LoadHistory(false);
        }

        private async Task LoadItem()
        {
            try
            {
                string json = await this.HttpService.GetAsync(this.spareDetailUrl, null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<SpareDetailResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                this.GetType().GetProperties().Where(p => p.CanWrite).ToList().ForEach(p => p.SetValue(this, p.GetValue(result.Obj)));
                this.InputUser = "张三";//临时展示代码
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        private async Task<List<ChangeInfoViewModel>> LoadHistory()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"bomId\": \"{this.bomId}\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\": 10}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.changeInfoListUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return null;
                }
                var result = JsonConvert.DeserializeObject<ChangeInfoResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return null;
                }
                this.Total = result.Total;
                return result.Item;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return null;
            }
        }

        private async Task LoadHistory(bool isAppend)
        {
            this.ShowProgressRing();
            var data = await LoadHistory();
            if (data == null)
                return;
            await this.UIDispatcher.BeginInvoke(new Action(() =>
            {
                if (!isAppend)
                    this.History.Clear();
                data.ForEach(c => this.History.Add(c));
            }));
            this.HideProgressRing();
        }

        public override async Task AppendData()
        {
            await LoadHistory(true);
        }


        public DelegateCommand SubmitCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    string formData = $"{{\"bomId\":\"{this.bomId}\",\"componentId\":\"{this.ComId}\",\"changeNum\":\"{this.ChangeNum.ToString()}\",\"changeReason\":\"{this.ChangeReason}\",\"equmentId\":\"{this.ModuleId}\",\"inputUser\":\"{this.InputUser}\",\"inputTime\":\"{(this.InputDate?.ToString("yyyy-MM-dd")) ?? string.Empty}\"}}";
                    try
                    {
                        string json = await this.HttpService.PostAsync(changeInfoAddUrl, formData, true);
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            ConfirmBox.Show(MSG_EMPTY_JSON);
                            return;
                        }
                        var result = JsonConvert.DeserializeObject<AddChangeInfoResult>(json);
                        if (!result.Success)
                        {
                            ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                            return;
                        }
                        ConfirmBox.Show("数据保存成功", "提示");
                        this.win.DialogResult = true;

                    }
                    catch (Exception ex)
                    {
                        ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                    }
                });
            }
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    win.DialogResult = false;
                });
            }
        }

        #region 表单属性

        public string ComId { get; set; }

        private string elementName;
        /// <summary>
        /// 备件名称
        /// </summary>
        public string ElementName
        {
            get { return elementName; }
            set { SetProperty<string>(ref elementName, value); }
        }

        private string elementModel;
        /// <summary>
        /// 规格型号
        /// </summary>
        public string ElementModel
        {
            get { return elementModel; }
            set { SetProperty<string>(ref elementModel, value); }
        }

        private string nature;
        /// <summary>
        /// 材质
        /// </summary>
        public string Nature
        {
            get { return nature; }
            set { SetProperty<string>(ref nature, value); }
        }


        private string supplier;
        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Supplier
        {
            get { return supplier; }
            set { SetProperty<string>(ref supplier, value); }
        }

        private string usedLife;
        /// <summary>
        /// 理论寿命
        /// </summary>
        public string UsedLife
        {
            get
            {
                if (string.IsNullOrWhiteSpace(usedLife))
                    return null;
                string ul = usedLife.Split('.').FirstOrDefault();
                return ul.EndsWith("个月") ? ul : $"{ul}个月";
            }
            set { SetProperty<string>(ref usedLife, value); }
        }

        private int changeNum;
        /// <summary>
        /// 换件数量
        /// </summary>
        public int ChangeNum
        {
            get { return changeNum < 1 ? 1 : changeNum; }
            set { SetProperty<int>(ref changeNum, value); }
        }

        private string changeReason;
        /// <summary>
        /// 换件原因
        /// </summary>
        public string ChangeReason
        {
            get { return changeReason?? "寿命到期"; }
            set { SetProperty<string>(ref changeReason, value); }
        }

        private string changeDateStr;
        /// <summary>
        /// 换件时间
        /// </summary>
        public string ChangeDateStr
        {
            get { return changeDateStr?.Split(' ').FirstOrDefault(); }
            set { SetProperty<string>(ref changeDateStr, value); }
        }

        private DateTime? inputDate;
        public DateTime? InputDate
        {
            get { return inputDate; }
            set
            {
                SetProperty<DateTime?>(ref inputDate, value);
                OnPropertyChanged(nameof(IsValid));
            }
        }



        private string inputUser;
        /// <summary>
        /// 换件人
        /// </summary>
        public string InputUser
        {
            get { return inputUser; }
            set { SetProperty<string>(ref inputUser, value); }
        }

        /// <summary>
        /// 预警时间
        /// </summary>
        public string AlertTimeStr { get; set; }

        #endregion
    }
}
