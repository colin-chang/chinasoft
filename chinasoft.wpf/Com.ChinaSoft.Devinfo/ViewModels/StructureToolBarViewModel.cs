using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Com.ChinaSoft.Devinfo.Model;
using System.Collections.ObjectModel;
using System.IO;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using System.Windows;
using Prism.Commands;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public partial class StructureViewModel
    {
        private ModelStatusEnum ms;
        /// <summary>
        /// 模型状态
        /// </summary>
        public ModelStatusEnum ModelStatus
        {
            get { return ms; }
            set { SetProperty<ModelStatusEnum>(ref ms, value); }
        }

        private bool hasExplode;
        public bool HasExplode
        {
            get { return hasExplode; }
            set { SetProperty<bool>(ref hasExplode, value); }
        }

        private string explode;
        public string Explode
        {
            get { return explode; }
            set { SetProperty<string>(ref explode, value); }
        }

        private bool has3D;
        public bool Has3D
        {
            get { return has3D; }
            set { SetProperty<bool>(ref has3D, value); }
        }

        private string _3D;
        public string ThreeD
        {
            get { return _3D; }
            set { SetProperty<string>(ref _3D, value); }
        }

        private bool has2D;
        public bool Has2D
        {
            get { return has2D; }
            set { SetProperty<bool>(ref has2D, value); }
        }

        private string _2D;
        public string TwoD
        {
            get { return _2D; }
            set { SetProperty<string>(ref _2D, value); }
        }

        private bool hasDocs;
        public bool HasDocs
        {
            get { return hasDocs; }
            set { SetProperty<bool>(ref hasDocs, value); }
        }


        /// <summary>
        /// 文件
        /// </summary>
        public ObservableCollection<Document> Documents { get; } = new ObservableCollection<Document>();

        ///// <summary>
        ///// 电路图元件功能
        ///// </summary>
        //public ObservableCollection<CircuitFunction> CircuitFunctions { get; } = new ObservableCollection<CircuitFunction>();

        private string filePath;
        /// <summary>
        /// 选中文件路径
        /// </summary>
        public string FilePath
        {
            get { return filePath == null ? null : Path.Combine(this.FileBaseUrl, filePath).ToUrl(); }
            set { SetProperty<string>(ref filePath, value); }
        }

        #region 命令
        public DelegateCommand<Document> PreviewCommand
        {
            get
            {
                return new DelegateCommand<Document>(doc =>
                {
                    switch (doc.Type)
                    {
                        case "word": this.ModelStatus = ModelStatusEnum.Document_Word; break;
                        case "pdf": this.ModelStatus = ModelStatusEnum.Document_PDF; break;
                        case "html": this.ModelStatus = ModelStatusEnum.Document_Html; break;
                        case "photo": this.ModelStatus = ModelStatusEnum.Document_Photo; break;
                        case "video": this.ModelStatus = ModelStatusEnum.Document_Video; break;
                        default: break;
                    }
                });
            }
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 加载部位详情（爆炸图、3D）
        /// </summary>
        /// <param name="busId"></param>
        /// <returns></returns>
        async Task LoadPositionDetail(string busId)
        {
            if (string.IsNullOrWhiteSpace(busId))
                return;
            string url = Path.Combine(this.positionDetailUrl, busId).ToUrl();
            try
            {
                string json = await this.HttpService.GetAsync(url, null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<PositionResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                var obj = result.Obj;
                this.HasExplode = !string.IsNullOrWhiteSpace(obj.ExplodeFileId);
                this.Explode = obj.ExplodeFile?.Path;
                this.Has3D = !string.IsNullOrWhiteSpace(obj.Appendfile3dId);
                this.ThreeD = obj.Appendfile3d?.Path;
                this.Has2D = !string.IsNullOrWhiteSpace(obj.Appendfile2dId);
                this.TwoD = obj.Appendfile2d?.Path;

                if (this.HasExplode)
                    this.ModelStatus = ModelStatusEnum.Explode;
                else if (this.Has3D)
                    this.ModelStatus = ModelStatusEnum.ThreeD;
                else if (this.Has2D)
                    this.ModelStatus = ModelStatusEnum.TwoD;

            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        /// 加载部位或文件文档
        /// </summary>
        /// <param name="bomId"></param>
        /// <returns></returns>
        async Task LoadDocuments(string bomId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string> { ["eid"] = ModuleId, ["bid"] = bomId };
            try
            {
                string json = await this.HttpService.GetAsync(docUrl, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON, MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = JsonConvert.DeserializeObject<DocumentResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    this.Documents.Clear();
                    
                    this.HasDocs = result.Item.Count() > 0;
                    result.Item.ForEach(d => this.Documents.Add(d));
                    this.OnPropertyChanged(nameof(Documents));
                    if (!this.HasExplode && !this.Has3D && !this.Has2D)
                        this.ModelStatus =this.HasDocs? ModelStatusEnum.Documents:ModelStatusEnum.None;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message), MSG_ERROR_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
