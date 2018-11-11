using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    class StrategyViewModel : BaseViewModel
    {
        private string mapUrl, checkUrl, lubricationUrl, maintainUrl, repairUrl, techImprUrl, changePartUrl, detectPointUrl;

        StrategyMap currentMap;

        private DetailTypeEnum type;
        public DetailTypeEnum DetailType
        {
            get { return type; }
            set { SetProperty<DetailTypeEnum>(ref type, value); }
        }

        private string detailTitle;
        public string DetailTitle
        {
            get { return detailTitle; }
            set { SetProperty<string>(ref detailTitle, value); }
        }

        /// <summary>
        /// 路径结构
        /// </summary>
        public ObservableCollection<StrategyMap> Paths { get; } = new ObservableCollection<StrategyMap>();
        private ObservableCollection<StrategyMap> maps;

        public ObservableCollection<Check> Checks { get; } = new ObservableCollection<Check>();
        public ObservableCollection<Lubrication> Lubrications { get; } = new ObservableCollection<Lubrication>();
        public ObservableCollection<Maintain> Maintains { get; } = new ObservableCollection<Maintain>();
        public ObservableCollection<Repair> Repairs { get; } = new ObservableCollection<Repair>();
        public ObservableCollection<TechImpr> ProImprs { get; } = new ObservableCollection<TechImpr>();
        public ObservableCollection<TechImpr> BigImprs { get; } = new ObservableCollection<TechImpr>();
        public ObservableCollection<ChangePart> ChangeParts { get; } = new ObservableCollection<ChangePart>();
        public ObservableCollection<DetectPoint> DetectPoints { get; } = new ObservableCollection<DetectPoint>();

        public StrategyViewModel(ObservableCollection<StrategyMap> maps)
        {
            this.maps = maps;
            this.mapUrl = Path.Combine(this.ApiBaseUrl, "standard", "strategymap", "list").ToUrl();
            this.checkUrl = Path.Combine(this.ApiBaseUrl, "standard", "check", "list").ToUrl();
            this.lubricationUrl = Path.Combine(this.ApiBaseUrl, "standard", "lubrication", "list").ToUrl();
            this.maintainUrl = Path.Combine(this.ApiBaseUrl, "standard", "maintain", "list").ToUrl();
            this.repairUrl = Path.Combine(this.ApiBaseUrl, "standard", "repair", "list").ToUrl();
            this.techImprUrl = Path.Combine(this.ApiBaseUrl, "standard", "techimpr", "list").ToUrl();
            this.changePartUrl = Path.Combine(this.ApiBaseUrl, "standard", "changepart", "list").ToUrl();
            this.detectPointUrl = Path.Combine(this.ApiBaseUrl, "standard", "detectpoint", "list").ToUrl();

            Initialize();
        }

        private async void Initialize()
        {
            await LoadMaps();
            this.HideProgressRing();
        }

        private async Task LoadMaps()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["mid"] = this.ModuleId,
                ["pid"] = currentMap?.Id
            };

            try
            {
                string json = await this.HttpService.GetAsync(this.mapUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<StrategyMapResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    if (result.Item.Count() <= 0)
                        return;

                    this.maps.Clear();
                    result.Item.ForEach(m => this.maps.Add(m));
                    if (currentMap == null)
                        this.Paths.Clear();
                    else
                        this.Paths.Add(currentMap);
                    this.OnPropertyChanged(nameof(Paths));
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        public DelegateCommand<StrategyMap> SelecteBomCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async s =>
                {
                    this.ShowProgressRing();
                    this.currentMap = s;
                    await LoadMaps();
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> PathCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async s =>
                {
                    this.ShowProgressRing();
                    this.currentMap = s;
                    int index = Paths.IndexOf(s);
                    if (index >= 0)
                    {
                        for (int i = Paths.Count() - 1; i >= index; i--)
                            this.Paths.RemoveAt(i);
                    }
                    await LoadMaps();
                    this.HideProgressRing();
                });
            }
        }

        private async Task<List<TModel>> LoadDetail<TResult, TModel>(string url, string queryCode)
            where TResult : ItemsJsonResult<TModel>
            where TModel : class, new()
        {
            string param;
            if (this.DetailType == DetailTypeEnum.ProImpr)
                param = $"{{\"query\": {{\"modelId\": \"{ModuleId}\",\"queryCode\": \"{queryCode}\",\"extendField1\": \"0\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\":{int.MaxValue}}},\"sort\": {{}}}}";
            else if (this.DetailType == DetailTypeEnum.BigImpr)
                param = $"{{\"query\": {{\"modelId\": \"{ModuleId}\",\"queryCode\": \"{queryCode}\",\"extendField1\": \"1\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\": {int.MaxValue}}},\"sort\": {{}}}}";
            else
                param = $"{{\"query\": {{\"modelId\": \"{ModuleId}\",\"queryCode\": \"{queryCode}\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\": {int.MaxValue}}},\"sort\": {{}}}}";
            Dictionary<string, string> dict = new Dictionary<string, string> { ["params"] = param };

            try
            {
                string json = await this.HttpService.GetAsync(url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return null;
                }
                TResult result = JsonConvert.DeserializeObject<TResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return null;
                }

                return result.Item;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return null;
            }
        }

        public DelegateCommand<StrategyMap> CheckCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.Check;
                    this.DetailTitle = $"{map.BomName}-点检";
                    List<Check> result = await LoadDetail<CheckResult, Check>(checkUrl, map.QueryCode);
                    Checks.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.Checks.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> LubricationCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.Lubrication;
                    this.DetailTitle = $"{map.BomName}-润滑";
                    List<Lubrication> result = await LoadDetail<LubricationResult, Lubrication>(lubricationUrl, map.QueryCode);
                    Lubrications.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.Lubrications.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> MaintainCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.Maintain;
                    this.DetailTitle = $"{map.BomName}-保养";
                    List<Maintain> result = await LoadDetail<MaintainResult, Maintain>(maintainUrl, map.QueryCode);
                    Maintains.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.Maintains.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> RepairCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.Repair;
                    this.DetailTitle = $"{map.BomName}-维修";
                    List<Repair> result = await LoadDetail<RepairResult, Repair>(repairUrl, map.QueryCode);
                    Repairs.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.Repairs.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<dynamic> TechImprCommand
        {
            get
            {
                return new DelegateCommand<dynamic>(async mapType =>
                {
                    this.ShowProgressRing();
                    this.DetailType = (DetailTypeEnum)mapType.ImprType;
                    var map = mapType.Map as StrategyMap;
                    List<TechImpr> result = await LoadDetail<TechImprResult, TechImpr>(techImprUrl, map.QueryCode);
                    if (DetailType == DetailTypeEnum.ProImpr)
                    {
                        this.DetailTitle = $"{map.BomName}-项修";
                        ProImprs.Clear();
                        await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                        {
                            r.No = result.IndexOf(r) + 1;
                            this.ProImprs.Add(r);
                        })));
                    }
                    else
                    {
                        this.DetailTitle = $"{map.BomName}-大修";
                        BigImprs.Clear();
                        await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                        {
                            r.No = result.IndexOf(r) + 1;
                            this.BigImprs.Add(r);
                        })));
                    }
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> ChangePartCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.ChangePart;
                    this.DetailTitle = $"{map.BomName}-定期换件";
                    List<ChangePart> result = await LoadDetail<ChangePartResult, ChangePart>(changePartUrl, map.QueryCode);
                    ChangeParts.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.ChangeParts.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }

        public DelegateCommand<StrategyMap> DetectPointCommand
        {
            get
            {
                return new DelegateCommand<StrategyMap>(async map =>
                {
                    this.ShowProgressRing();
                    this.DetailType = DetailTypeEnum.DetectPoint;
                    this.DetailTitle = $"{map.BomName}-状态监测";
                    List<DetectPoint> result = await LoadDetail<DetectPointResult, DetectPoint>(detectPointUrl, map.QueryCode);
                    DetectPoints.Clear();
                    await this.UIDispatcher.BeginInvoke(new Action(() => result?.ForEach(r =>
                    {
                        r.No = result.IndexOf(r) + 1;
                        this.DetectPoints.Add(r);
                    })));
                    this.HideProgressRing();
                });
            }
        }
    }
}