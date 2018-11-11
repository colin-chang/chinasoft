using Com.ChinaSoft.Devinfo.Helper;
using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    /// <summary>
    /// 设备结构电路图模块
    /// </summary>
    public partial class StructureViewModel
    {
        private const string CATALOGCODE = "catalog";
        private const string POSITIONCODE = "position";
        private const string ELEMENTCODE = "element";
        private const string CIRCUITCODE = "circuit";

        private string baseCircuitUrl, circuitResourceUrl, circuitFunUrl;

        private string code;
        /// <summary>
        /// 电路图元件编号
        /// </summary>
        public string Code
        {
            get { return code; }
            set { SetProperty<string>(ref code, value); }
        }

        private WebBrowser browser;
        /// <summary>
        /// 电路图宿主浏览器
        /// </summary>
        private WebBrowser Browser
        {
            get { return browser; }
            set
            {
                browser = value;
                browser.ObjectForScripting = new ObjectForScripting(ChangeSelectCode, LoadCircuitFunction);
            }
        }

        public bool FromWeb { get; set; } = false;
        public void ChangeSelectCode(string code)
        {
            this.FromWeb = true;
            this.Code = code;
        }

        /// <summary>
        /// 电路图路径
        /// </summary>
        private string Circuit
        {
            set
            {
                if (value == null)
                    Browser.Source = null;
                else
                    Browser.Source = new Uri($"{baseCircuitUrl}{value}");
            }
        }
        private string circuitBusId;

        /// <summary>
        /// 电路图元件
        /// </summary>
        public ObservableCollection<CircuitResource> CircuitResources { get; } = new ObservableCollection<CircuitResource>();

        /// <summary>
        /// 电路图部件选择
        /// </summary>
        public DelegateCommand<string> CircuitSelected
        {
            get
            {
                return new DelegateCommand<string>(code =>
                {
                    if (string.IsNullOrWhiteSpace(code))
                        return;
                    if (this.FromWeb)
                    {
                        this.FromWeb = false;
                        return;
                    }

                    try { Browser.InvokeScript("seleElement", circuitBusId, code); }
                    catch { }
                });
            }
        }

        /// <summary>
        /// 购买
        /// </summary>
        public DelegateCommand<string> PurchaseCommand
        {
            get
            {
                return new DelegateCommand<string>(id =>
                {
                });
            }
        }

        /// <summary>
        /// 电路图部件选择跳转
        /// </summary>
        public DelegateCommand<IAlertViewItem> CircuitFunctionCommand
        {
            get
            {
                return new DelegateCommand<IAlertViewItem>(async item =>
                {
                    string code = item.Tag?.ToString();
                    if (string.IsNullOrWhiteSpace(code))
                        return;

                    this.CircuitResources.Clear();
                    ChangeSelectCode(code);
                    this.FromWeb = false;

                    //加载导航和右侧电路图和元件信息
                    string queryCode = await GetQueryCodeAsyn(item.Id);
                    await SearchResultSelected(new StructureItem { QueryCode = queryCode });

                    //加载左侧Bom结构
                    this.Boms.Clear();
                    this.Boms.Add(this.Paths.LastOrDefault());
                    Reload();
                });
            }
        }

        async Task<string> GetQueryCodeAsyn(string busId)
        {
            try
            {
                string json = await this.HttpService.GetAsync(Path.Combine(this.ApiBaseUrl, $"equipment/bom/bom/qc?eid={this.ModuleId}&bid={busId}"), null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                var result = JsonConvert.DeserializeObject<QueryCodeResult>(json);
                if (!result.Success)
                {
                    MessageBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                return result.QueryCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// 加载电路图资源
        /// </summary>
        /// <param name="circuitId"></param>
        /// <returns></returns>
        async Task LoadCircuitResource(string circuitId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"circuitCode\": \"{circuitId}\"}},\"pager\": {{\"pageIndex\": 1,\"pageSize\": {int.MaxValue}}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.circuitResourceUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = JsonConvert.DeserializeObject<CircuitResourceResult>(json);
                if (!result.Success)
                {
                    MessageBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    this.CircuitResources.Clear();
                    result.Item.ForEach(c => this.CircuitResources.Add(c));
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 加载电路图元件功能
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        async Task LoadCircuitFunction(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return;
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string> { ["rcode"] = code };
                string json = await this.HttpService.GetAsync(circuitFunUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = JsonConvert.DeserializeObject<CircuitFunctionResult>(json);
                if (!result.Success)
                {
                    MessageBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    AlertView alert = new AlertView(this.CircuitFunctionCommand, this.CurrentWindow);
                    alert.ClearItems();

                    string id = result.Item.FirstOrDefault(c => c.IsMain == 1)?.CircuitCode;
                    result.Item.ForEach(c =>
                    {
                        if (c.ResourceCode == code && c.CircuitCode == circuitBusId)
                        {
                            if (c.IsMain == 0)
                            {
                                alert.AppendItem(new CircuitFunction { Id = id, Text = "返回到主元器件电路图", Tag = c.Tag });
                                return;
                            }
                            else
                                result.Item.ForEach(ci => alert.AppendItem(new CircuitFunction { Id = ci.CircuitCode, Text = ci.Text, Tag = ci.Tag }));
                        }
                    });
                    alert.AppendItem(new CancelItem());
                    alert.ShowDialog();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}