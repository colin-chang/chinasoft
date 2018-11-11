using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    /// <summary>
    /// 专家经验库主页ViewModel
    /// </summary>
    public class ExperienceBaseViewModel : KnowledgeViewModel
    {
        /// <summary>
        /// 获取子Bom结构
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExperienceBaseNode>> GetBomAsync(ExperienceBaseNode node)
        {
            string url = Path.Combine(this.ApiBaseUrl, "model", "bom", "list").ToUrl();
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["moduleId"] = this.Root.Id,
                ["pid"]= node.Level < 1 ? string.Empty : node.Id
            };
            var emptyCollection = new List<ExperienceBaseNode>();
            try
            {
                string json = await this.HttpService.GetAsync(url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return emptyCollection;
                }
                ExperienceBaseNodeResult result = JsonConvert.DeserializeObject<ExperienceBaseNodeResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return emptyCollection;
                }
                result.Item.ForEach(n =>
                {
                    n.Level = node.Level == 0 ? 1 : node.Level + (float)(node.Level * 0.1);
                    n.ModelId = this.Root.Id;
                    n.ParentId = node.Id;
                    n.Parent = node;
                });

                return result.Item;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                return new List<ExperienceBaseNode>();
            }
        }
    }
}
