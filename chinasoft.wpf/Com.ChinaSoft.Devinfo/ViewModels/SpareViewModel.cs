using System.Windows;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Prism.Commands;
using Newtonsoft.Json;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.Views;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class SpareViewModel : LazyLoadingBaseViewModel
    {
        string url;
        public SpareViewModel()
        {
            this.url = Path.Combine(this.ApiBaseUrl, "component", "spearparts", "list").ToUrl();

            SetPalaceholder();

            this.ReloadData();
        }

        private TimeSpanTypeEnum ts;
        public TimeSpanTypeEnum TimeSpanType
        {
            get { return ts; }
            set { SetProperty<TimeSpanTypeEnum>(ref ts, value); }
        }

        private DateTime? starDate;
        public DateTime? StartDate
        {
            get { return starDate; }
            set { SetProperty<DateTime?>(ref starDate, value); }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { SetProperty<DateTime?>(ref endDate, value); }
        }

        public ObservableCollection<Spare> Spares { get; } = new ObservableCollection<Spare>();

        private async Task<List<Spare>> LoadData()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"equipmentId\": \"{this.ModuleId}\",\"startDate\":\"{(StartDate?.ToString("yyyy-MM-dd")) ?? string.Empty}\",\"endDate\":\"{(EndDate?.ToString("yyyy-MM-dd")) ?? string.Empty}\",\"queryKey\":\"{this.QueryKey}\"}},\"pager\": {{\"pageIndex\": {PageIndex},\"pageSize\": 15}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return null;
                }
                var result = JsonConvert.DeserializeObject<SpareResult>(json);
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
        private async Task LoadData(bool isAppend)
        {
            this.ShowProgressRing();
            var data = await LoadData();
            if (data == null)
                return;
            await this.UIDispatcher.BeginInvoke(new Action(() =>
            {
                if (!isAppend)
                    this.Spares.Clear();
                int i = this.Spares.LastOrDefault()?.No ?? 0;
                data.ForEach(s =>
                {
                    s.No = ++i;
                    s.LifeRate = s.LifeRate < 0 ? 0 : s.LifeRate;
                    this.Spares.Add(s);
                });
            }));
            this.HideProgressRing();
        }

        public async Task ReloadData()
        {
            this.PageIndex = 1;
            await this.LoadData(false);
        }

        public override async Task AppendData()
        {
            await this.LoadData(true);
        }

        const string palaceholder = "备件名称/规格型号";
        private string kw;
        public string Keyword
        {
            get { return kw; }
            set { SetProperty<string>(ref kw, value); }
        }

        private string QueryKey { get { return string.Equals(this.Keyword.Trim(), palaceholder) ? string.Empty : this.Keyword.Trim(); } }

        /// <summary>
        /// 搜索
        /// </summary>
        public DelegateCommand SearchCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await ReloadData();
                });
            }
        }

        public DelegateCommand SearchFocusCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (string.Equals(this.Keyword, palaceholder))
                        this.Keyword = string.Empty;
                });
            }
        }

        public DelegateCommand SearchBlurCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    SetPalaceholder();
                });
            }
        }

        void SetPalaceholder()
        {
            if (string.IsNullOrWhiteSpace(this.Keyword))
                this.Keyword = palaceholder;
        }

        public DelegateCommand<TimeSpanTypeEnum?> ChangeTimeSpanCommand
        {
            get
            {
                return new DelegateCommand<TimeSpanTypeEnum?>(async type =>
                {
                    var tsType = (TimeSpanTypeEnum)(type);
                    this.TimeSpanType = tsType;

                    if (tsType == TimeSpanTypeEnum.Custom)
                    {
                        //TwoDatePickerWindow win = new TwoDatePickerWindow { Owner = Application.Current.MainWindow };
                        //if (win.ShowDialog() != true)
                        //    return;
                        //this.StartDate = win.StartDate;
                        //this.EndDate = win.EndDate;

                        this.StartDate = null;
                        this.EndDate = null;
                    }
                    else
                    {
                        this.StartDate = DateTime.Today;
                        switch (TimeSpanType)
                        {
                            case TimeSpanTypeEnum.All:
                                this.StartDate = null;
                                this.EndDate = null;
                                break;
                            case TimeSpanTypeEnum.Year:
                                this.EndDate = DateTime.Today.AddYears(1);
                                break;
                            case TimeSpanTypeEnum.Quarter:
                                this.EndDate = DateTime.Today.AddMonths(3);
                                break;
                            case TimeSpanTypeEnum.Month:
                                this.EndDate = DateTime.Today.AddMonths(1);
                                break;
                            case TimeSpanTypeEnum.Week:
                                this.EndDate = DateTime.Today.AddDays(7);
                                break;
                            default:
                                return;
                        }
                    }
                    await ReloadData();
                });
            }
        }

        public DelegateCommand CustomScreenCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await ReloadData();
                });
            }
        }

        public DelegateCommand<string> ChangePartCommand
        {
            get
            {
                return new DelegateCommand<string>(async id =>
                {
                    ChangeInfoWindow win = new ChangeInfoWindow(id) { Owner = Application.Current.MainWindow };
                    if (win.ShowDialog() == true)
                        await ReloadData();
                });
            }
        }

        public DelegateCommand <string> ShowModelCommand
        {
            get
            {
                return new DelegateCommand<string>(id =>
                {
                    SpareModelWindow win = new SpareModelWindow(id) { Owner = Application.Current.MainWindow };
                    win.ShowDialog();
                });
            }
        }
    }
}
