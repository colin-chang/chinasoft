using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    /// <summary>
    /// 专家经验库基ViewModel
    /// </summary>
    public abstract class KnowledgeViewModel : BaseViewModel
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public ExperienceBaseNode Root { get; private set; }

        public KnowledgeViewModel()
        {
            this.Root = new ExperienceBaseNode { Id = this.ModuleId, Name = this.EquipmentName, Level = 0 };
        }

        /// <summary>
        /// 获取下级专家经验节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<ExperienceBaseNode>> GetExpsAsync(ExperienceBaseNode node, string failure = null)
        {
            string baseUrl = Path.Combine(this.ApiBaseUrl, "knowledge");
            Dictionary<string, string> dict = new Dictionary<string, string> { ["moduleId"] = this.Root.Id };

            var emptyCollection = new List<ExperienceBaseNode>();
            try
            {
                if (node == null || node.Level < 1)
                    return emptyCollection;
                else if (node.Level < 2)
                {
                    //获取现象
                    string url = Path.Combine(baseUrl, "listfaultfailure").ToUrl();
                    dict.Add("partId", node.BusId);
                    if (!string.IsNullOrWhiteSpace(failure))
                        dict.Add("kw", failure);
                    string json = await this.HttpService.GetAsync(url, dict, true);
                    ExperienceBaseNodeResult result = JsonConvert.DeserializeObject<ExperienceBaseNodeResult>(json);
                    if (!result.Success)
                    {
                        ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                        return emptyCollection;
                    }
                    result.Item.ForEach(n =>
                    {
                        n.Level = 2;
                        n.ModelId = this.Root.Id;
                        n.PartId = node.BusId;
                        n.ParentId = node.Id;
                        n.Parent = node;
                    });
                    return result.Item;
                }
                else if (node.Level < 3)
                {
                    //获取原因
                    string url = Path.Combine(baseUrl, "listfaultcause").ToUrl();
                    dict.Add("partId", node.PartId);
                    dict.Add("failureId", node.Id);
                    string json = await this.HttpService.GetAsync(url, dict, true);
                    ExperienceBaseNodeResult result = JsonConvert.DeserializeObject<ExperienceBaseNodeResult>(json);
                    if (!result.Success)
                    {
                        ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                        return emptyCollection;
                    }
                    result.Item.ForEach(n =>
                    {
                        n.Level = 3;
                        n.ModelId = this.Root.Id;
                        n.PartId = node.PartId;
                        n.FailureId = node.Id;
                        n.ParentId = node.Id;
                        n.Parent = node;
                    });
                    return result.Item;
                }
                else if (node.Level < 4)
                {
                    //获取解决方案
                    string url = Path.Combine(baseUrl, "listfaulttrentment").ToUrl();
                    dict.Add("partId", node.PartId);
                    dict.Add("failureId", node.FailureId);
                    dict.Add("causeId", node.Id);
                    string json = await this.HttpService.GetAsync(url, dict, true);
                    ExperienceBaseNodeResult result = JsonConvert.DeserializeObject<ExperienceBaseNodeResult>(json);
                    if (!result.Success)
                    {
                        ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                        return emptyCollection;
                    }
                    result.Item.ForEach(n =>
                    {
                        n.Level = 4;
                        n.ModelId = this.Root.Id;
                        n.PartId = node.PartId;
                        n.FailureId = node.FailureId;
                        n.CauseId = node.Id;
                        n.ParentId = node.Id;
                    });
                    return result.Item;
                }
                else
                    return emptyCollection;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return emptyCollection;
            }
        }
    }
}
