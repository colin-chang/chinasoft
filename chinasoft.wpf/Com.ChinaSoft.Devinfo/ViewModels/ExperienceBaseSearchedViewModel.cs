using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Prism.Commands;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.Views;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    /// <summary>
    /// 专家经验库搜索结果页ViewModel
    /// </summary>
    public class ExperienceBaseSearchedViewModel : KnowledgeViewModel
    {
        /// <summary>
        /// 部位/图号
        /// </summary>
        public string Part { get; set; }

        /// <summary>
        /// 故障码/现象
        /// </summary>
        public string Failure { get; set; }

        /// <summary>
        /// 数据加载是否完成
        /// </summary>
        public bool LoadCompleted { get; set; }

        /// <summary>
        /// 缓存Bom结构搜索结果
        /// </summary>
        public ObservableCollection<ExperienceBaseNode> SearchedBoms { get; } = new ObservableCollection<ExperienceBaseNode>();

        public ExperienceBaseSearchedViewModel(string part, string failure)
        {
            SearchedBoms.Add(Root);

            this.Part = part;
            this.Failure = failure;
            QuerySearchedBomAsync();
        }

        /// <summary>
        /// 搜索Bom结构
        /// </summary>
        public async void QuerySearchedBomAsync()
        {
            string url = Path.Combine(this.ApiBaseUrl, "model", "bom", "treekl").ToUrl();
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["modelId"] = this.Root.Id,
                ["partNm"] = this.Part,
                ["failureNm"] = this.Failure
            };
            try
            {
                string json = await this.HttpService.GetAsync(url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                ExperienceBaseNodeResult result = JsonConvert.DeserializeObject<ExperienceBaseNodeResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }
                if (result.Item == null)
                    return;
                StoreSearchBoms(result.Item);
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
            finally
            {
                this.LoadCompleted = true;
            }
        }

        /// <summary>
        /// 缓存Bom结构搜索结果
        /// </summary>
        /// <param name="nodes"></param>
        void StoreSearchBoms(List<ExperienceBaseNode> nodes)
        {
            nodes.ForEach(n =>
            {
                ExperienceBaseNode parent;
                if (n.ParentId == null)
                {
                    n.ParentId = Root.Id;
                    parent = SearchedBoms.Where(b => b.Level == 0).FirstOrDefault();
                }
                else
                    parent = SearchedBoms.FirstOrDefault(b => b.Id == n.ParentId && b.Level > 0);

                if (parent != null)
                    n.Level = parent.Level == 0 ? 1 : parent.Level + (float)(parent.Level * 0.1);
                n.ModelId = this.Root.Id;
                n.Parent = parent;
                //if (!SearchedBoms.Select(b=>b.Id).Contains(n.Id))
                if (SearchedBoms.FirstOrDefault(b => b.Id == n.Id && b.Level == n.Level) == null)
                    SearchedBoms.Add(n);
                if (n.Items != null)
                    StoreSearchBoms(n.Items);
            });
        }

        /// <summary>
        /// 获取子Bom结构
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        public IEnumerable<ExperienceBaseNode> GetBom(string parentId, bool isRoot = false)
        {
            var result = SearchedBoms.Where(b => b.ParentId == parentId);

            return isRoot ? result.Where(b => b.Level == 1) : result.Where(b => b.Level > 1);
        }

        /// <summary>
        /// 获取下级专家经验节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Task<IEnumerable<ExperienceBaseNode>> GetExpsAsync(ExperienceBaseNode node) => this.GetExpsAsync(node, this.Failure);

        /// <summary>
        /// 返回
        /// </summary>
        public override DelegateCommand GoBackCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.Panel.Children.Clear();
                    this.Panel.Children.Add(new ExperienceBaseUC());
                });
            }
        }
    }
}
