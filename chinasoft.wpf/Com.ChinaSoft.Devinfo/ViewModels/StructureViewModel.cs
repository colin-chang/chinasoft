using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Newtonsoft.Json;
using Prism.Commands;

using Com.ChinaSoft.Devinfo.Helper;
using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.UserControls;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public partial class StructureViewModel : BaseViewModel
    {
        private string listUrl, positionDetailUrl, docUrl, bomDetailUrl, componentUrl;

        public StructureViewModel(WebBrowser wb)
        {
            this.Browser = wb;
            SetPalaceholder();
            this.Status = StructureStatusEnum.Structure;

            //初始化API路径
            this.listUrl = Path.Combine(this.ApiBaseUrl, "equipment", "bom", "tree").ToUrl();
            this.positionDetailUrl = Path.Combine(this.ApiBaseUrl, "position", "detail").ToUrl();
            this.docUrl = Path.Combine(this.ApiBaseUrl, "equipment", "techdoc", "bom").ToUrl();
            this.bomDetailUrl = Path.Combine(this.ApiBaseUrl, "component", "bom", "detail").ToUrl();
            this.componentUrl = Path.Combine(this.ApiBaseUrl, "equipment", "bom", "list").ToUrl();
            this.baseCircuitUrl = ConfigurationManager.AppSettings["CircuitUrl"];
            this.circuitResourceUrl = Path.Combine(this.ApiBaseUrl, "equipment", "circuit", "resource", "list").ToUrl();
            this.circuitFunUrl = Path.Combine(this.ApiBaseUrl, "equipment", "circuit", "resource", "functions").ToUrl();

            //初始化导航路径
            this.AppendPath(PathRoot);

            //初始化列表
            Initialize();
        }

        #region 属性字段
        private StructureStatusEnum status;
        /// <summary>
        /// 按钮组选中状态
        /// </summary>
        public StructureStatusEnum Status
        {
            get { return status; }
            set { SetProperty<StructureStatusEnum>(ref status, value); }
        }

        /// <summary>
        /// 路径结构
        /// </summary>
        public ObservableCollection<StructureItem> Paths { get; } = new ObservableCollection<StructureItem>();

        /// <summary>
        /// 根路径
        /// </summary>
        public StructureItem PathRoot { get { return new StructureItem { BomNm = this.EquipmentName }; } }

        /// <summary>
        /// 列表数据源
        /// </summary>
        public ObservableCollection<StructureItem> ListItems { get; } = new ObservableCollection<StructureItem>();

        /// <summary>
        /// Bom结构
        /// </summary>
        public List<StructureItem> Boms { get; set; } = new List<StructureItem>();

        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<StructureItem> SearchResults { get; set; } = new List<StructureItem>();

        /// <summary>
        /// 是否处于搜索状态
        /// </summary>
        public bool SearchStatus { get { return SearchResults != null && SearchResults.Count() > 0; } }

        /// <summary>
        /// 列表数据源
        /// </summary>
        public ObservableCollection<Component> Components { get; } = new ObservableCollection<Component>();

        const string palaceholder = "名称/图号";
        private string keyword;
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
            set { SetProperty<string>(ref keyword, value); }
        }

        /// <summary>
        /// 搜索结果一条Bom链
        /// </summary>
        List<StructureItem> bomsLink = new List<StructureItem>();

        private BomDetail bomDetail;
        public BomDetail BomDetail
        {
            get { return bomDetail; }
            set { SetProperty<BomDetail>(ref bomDetail, value); }
        }
        #endregion

        #region 命令

        /// <summary>
        /// 修改状态
        /// </summary>
        public DelegateCommand<string> StatusCommand
        {
            get
            {
                return new DelegateCommand<string>(status =>
                {
                    int result;
                    if (!int.TryParse(status, out result))
                        return;
                    this.Status = (StructureStatusEnum)result;
                    Reload();
                });
            }
        }

        /// <summary>
        /// 目录结构选中
        /// </summary>
        public DelegateCommand<StructureItem> StructureSelectedCommand
        {
            get
            {
                return new DelegateCommand<StructureItem>(async item =>
                {
                    if (item == null)
                        return;

                    this.ShowProgressRing();
                    if (this.Status == StructureStatusEnum.Structure)
                        await BomSelected(item);
                    else
                        await SearchResultSelected(item);
                    this.HideProgressRing();
                });
            }
        }

        /// <summary>
        /// 点击Path
        /// </summary>
        public DelegateCommand<StructureItem> PathCommand
        {
            get
            {
                return new DelegateCommand<StructureItem>(async path =>
                {
                    this.ShowProgressRing();
                    int index = Paths.IndexOf(path);
                    for (int i = Paths.Count() - 1; i >= index; i--)
                        this.Paths.RemoveAt(i);

                    this.Status = StructureStatusEnum.Structure;
                    await BomSelected(path);
                    this.HideProgressRing();
                });
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public DelegateCommand SearchCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    this.ShowProgressRing();
                    if (string.Equals(this.Keyword, palaceholder))
                    {
                        await PathCommand.Execute(Paths.FirstOrDefault());
                        await Initialize();
                        this.SearchResults.Clear();
                        return;
                    }

                    this.Status = StructureStatusEnum.SearchResult;
                    this.SearchResults = await LoadList(null, this.Keyword);
                    Reload();
                    this.HideProgressRing();
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

        #endregion

        #region 工具方法

        /// <summary>
        /// 页面初始化Bom结构
        /// </summary>
        async Task Initialize()
        {
            this.Boms = await LoadList();
            Reload();
            this.HideProgressRing();
        }

        /// <summary>
        /// 加载列表数据
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="queryKey"></param>
        /// <param name="queryCode"></param>
        /// <returns></returns>
        async Task<List<StructureItem>> LoadList(string parentId = null, string queryKey = null, string queryCode = null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string> { ["equipmentId"] = ModuleId };
            if (!string.IsNullOrWhiteSpace(parentId))
                dict.Add("parentId", parentId);
            if (!string.IsNullOrWhiteSpace(queryKey))
                dict.Add("queryKey", queryKey);
            if (!string.IsNullOrWhiteSpace(queryCode))
                dict.Add("queryCode", queryCode);

            try
            {
                string json = await this.HttpService.GetAsync(this.listUrl, dict, true);
                return JsonConvert.DeserializeObject<List<StructureItem>>(json);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return new List<StructureItem>();
            }
        }

        /// <summary>
        /// 加载零部件
        /// </summary>
        /// <returns></returns>
        async Task LoadComponents(string parentBomId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"parentBomId\": \"{parentBomId}\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\": {int.MaxValue}}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.componentUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = JsonConvert.DeserializeObject<ComponentResult>(json);
                if (!result.Success)
                {
                    MessageBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    this.Components.Clear();
                    result.Item.ForEach(c => this.Components.Add(c));
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载零件详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async Task LoadBomDetail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return;
            string url = Path.Combine(this.bomDetailUrl, id).ToUrl();
            try
            {
                string json = await this.HttpService.GetAsync(url, null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = JsonConvert.DeserializeObject<BomDetailResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var obj = result.Obj;
                this.BomDetail = obj;
                this.HasExplode = false;
                this.Has3D = !string.IsNullOrWhiteSpace(obj.Appendfile3dId);
                this.ThreeD = obj.Appendfile3d?.Path;
                this.Has2D = !string.IsNullOrWhiteSpace(obj.Appendfile2dId);
                this.TwoD = obj.Appendfile2d?.Path;

                if (this.Has3D)
                    this.ModelStatus = ModelStatusEnum.ThreeD;
                else if (this.Has2D)
                    this.ModelStatus = ModelStatusEnum.TwoD;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 刷新数据源
        /// </summary>
        void Reload()
        {
            var source = Status == StructureStatusEnum.Structure ? this.Boms : this.SearchResults;
            if (source == null)
                return;
            this.UIDispatcher.BeginInvoke(new Action(() =>
            {
                this.ListItems.Clear();
                source.ForEach(s => this.ListItems.Add(s));
            }));
        }

        /// <summary>
        /// 选中Bom
        /// </summary>
        /// <param name="item"></param>
        async Task BomSelected(StructureItem item)
        {
            if (item.IsLeaf == 1)
            {
                bool isCircuit = string.Equals(item.BomEntryCode, CIRCUITCODE);
                if (string.Equals(item.BomEntryCode, ELEMENTCODE) || isCircuit)
                {
                    this.AppendPath(item);
                    if (isCircuit)//电路图
                    {
                        Circuit = item.BusId;
                        await LoadCircuitResource(item.BusId);
                        this.circuitBusId = item.BusId;
                        await CircuitSelected.Execute(this.Code);//选中指定元件
                    }
                    else
                    {
                        //零件
                        await this.LoadBomDetail(item.Id);
                        if (Has3D && Has2D)
                            this.ModelStatus = ModelStatusEnum.TwoD;
                        await this.LoadDocuments(item.Id);
                    }
                }
                else if (string.Equals(item.BomEntryCode, POSITIONCODE))
                {
                    await LoadPositionDetail(item.BusId);
                    await LoadDocuments(item.Id);
                }
                return;
            }

            this.Boms = SearchStatus && (this.Paths.Count() < bomsLink.Count()) ? this.bomsLink.Where(b => b.ParentBomId == item.Id).ToList() : await LoadList(item.Id);
            if (this.Boms.Count() <= 0)
                return;
            Reload();

            this.AppendPath(item);

            //虚拟目录
            if (string.Equals(item.BomEntryCode, CATALOGCODE) || string.IsNullOrWhiteSpace(item.BomId))
                return;
            //部位
            await LoadPositionDetail(item.BusId);
            await LoadDocuments(item.Id);
            await LoadComponents(item.Id);
        }

        /// <summary>
        /// 选中搜索结果
        /// </summary>
        /// <param name="item"></param>
        async Task SearchResultSelected(StructureItem item)
        {
            this.Status = StructureStatusEnum.Structure;

            this.Boms = await LoadList(null, null, item.QueryCode);
            bomsLink.Clear();
            LoadBomsLink(this.Boms.FirstOrDefault());
            var current = bomsLink.LastOrDefault();
            bomsLink.ForEach(b => b.IsLeaf = 0);
            current.IsLeaf = await GetIsLeaf(current.Id);

            await this.UIDispatcher.BeginInvoke(new Action(async () =>
            {
                this.Paths.Clear();
                this.Paths.Add(PathRoot);
                this.Paths.AddRange<StructureItem>(bomsLink);
                this.Paths.Remove(current);
                this.OnPropertyChanged(nameof(Paths));

                //await BomSelected(bomsLink.LastOrDefault());
                await BomSelected(current);

                ////有子集
                //if (string.Equals(current.BomEntryCode, CATALOGCODE) || string.Equals(current.BomEntryCode, POSITIONCODE)||string.Equals(current.BomEntryCode,ELEMENTCODE))
                //{
                //    await BomSelected(bomsLink.LastOrDefault());
                //}
                //else
                //{
                //    //没有子集
                //    this.ListItems.Clear();
                //    this.ListItems.Add(current);

                //    if (string.Equals(current.BomEntryCode, CATALOGCODE) || string.IsNullOrWhiteSpace(current.BomId))
                //        return;
                //    await LoadComponents(current.Id);
                //}
            }));
        }

        /// <summary>
        /// 加载搜索结果Bom链
        /// </summary>
        /// <param name="bom"></param>
        void LoadBomsLink(StructureItem bom)
        {
            if (bom == null)
                return;
            bomsLink.Add(bom);
            if (bom.Items == null || bom.Items.Count() <= 0)
                return;
            LoadBomsLink(bom.Items.FirstOrDefault());
        }

        /// <summary>
        /// 附加路径
        /// </summary>
        /// <param name="item"></param>
        void AppendPath(StructureItem item)
        {
            if (IsElementOrCircuit(item) && IsElementOrCircuit(this.Paths.LastOrDefault()))
                this.Paths.Remove(this.Paths.LastOrDefault());
            this.Paths.Add(item);
            this.OnPropertyChanged(nameof(Paths));
        }

        public bool IsElementOrCircuit(StructureItem item)
        {
            string code = item.BomEntryCode;
            return string.Equals(code, CIRCUITCODE) || string.Equals(code, ELEMENTCODE);
        }

        /// <summary>
        /// 获取指定Bom的IsLeaf属性
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        async Task<int> GetIsLeaf(string itemId)
        {
            var children = await this.LoadList(itemId);
            return children.Count() <= 0 ? 1 : 0;
        }

        void SetPalaceholder()
        {
            if (string.IsNullOrWhiteSpace(this.Keyword))
                this.Keyword = palaceholder;
        }

        #endregion
    }
}
