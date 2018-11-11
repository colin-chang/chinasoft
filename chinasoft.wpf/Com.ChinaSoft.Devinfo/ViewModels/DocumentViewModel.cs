using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using Prism.Commands;
using Newtonsoft.Json;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.Views;
using Com.ChinaSoft.UserControls;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        public DocumentViewModel()
        {
            this.Url = Path.Combine(this.ApiBaseUrl, "equipment", "techdoc", "list").ToUrl();

            Initialize();
        }

        /// <summary>
        /// 文档加载地址
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; private set; } = 1;

        public int PageSize { get { return 40; } }

        /// <summary>
        /// 文件总数
        /// </summary>
        public int? Total { get; set; }

        private DocumentTypeEnum docType;
        /// <summary>
        /// 文件类型
        /// </summary>
        public DocumentTypeEnum DocumentType
        {
            get { return docType; }
            set { SetProperty<DocumentTypeEnum>(ref docType, value); }
        }

        private string filePath;
        /// <summary>
        /// 选中文件路径
        /// </summary>
        public string FilePath
        {
            get { return Path.Combine(this.FileBaseUrl, filePath).ToUrl(); }
            set { SetProperty<string>(ref filePath, value); }
        }

        /// <summary>
        /// 文件
        /// </summary>
        public ObservableCollection<Document> Documents { get; } = new ObservableCollection<Document>();

        public DelegateCommand<Document> PreviewCommand
        {
            get
            {
                return new DelegateCommand<Document>(doc =>
                {
                    switch (doc.Type)
                    {
                        case "word": this.DocumentType = DocumentTypeEnum.Word; break;
                        case "pdf": this.DocumentType = DocumentTypeEnum.Pdf; break;
                        case "html": this.DocumentType = DocumentTypeEnum.Html; break;
                        case "photo": this.DocumentType = DocumentTypeEnum.Photo; break;
                        case "zip": this.DocumentType = DocumentTypeEnum.Zip; break;
                        default: this.DocumentType = DocumentTypeEnum.FileList; break;
                    }
                    this.FilePath = doc.AppendFile.Path;
                });
            }
        }

        async void Initialize()
        {
            this.DocumentType = DocumentTypeEnum.FileList;
            await LoadDocuments();
            this.HideProgressRing();
        }

        /// <summary>
        /// 加载文档列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        async Task LoadDocuments()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"equipmentId\": \"{this.ModuleId}\"}},\"pager\": {{\"pageIndex\": {this.PageIndex},\"pageSize\": {this.PageSize}}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.Url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<DocumentResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                this.Total = result.Total;
                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    result.Item.ForEach(d => this.Documents.Add(d));
                    this.OnPropertyChanged("Documents");
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        /// <summary>
        /// 滚动加载下一页
        /// </summary>
        /// <returns></returns>
        public async Task LoadMore()
        {
            if (this.PageIndex * PageSize >= Total)
                return;

            PageIndex++;
            await LoadDocuments();
        }

        public new DelegateCommand<ProgressRing> GoBackCommand
        {
            get
            {
                return new DelegateCommand<ProgressRing>(ring =>
                {
                    ring.Hide();

                    if (this.DocumentType == DocumentTypeEnum.FileList || this.DocumentType == DocumentTypeEnum.Zip)
                    {
                        this.Panel.Children.Clear();
                        this.Panel.Children.Add(new MainUC());
                    }
                    else
                        this.DocumentType = DocumentTypeEnum.FileList;
                });
            }
        }
    }
}
