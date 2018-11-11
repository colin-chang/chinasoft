using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.UserControls;
using Com.ChinaSoft.Devinfo.Views;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class VideoViewModel : BaseViewModel
    {
        public VideoViewModel(MediaPlayer player)
        {
            this.player = player;

            this.Url = Path.Combine(this.ApiBaseUrl, "equipment", "trainvideo", "list").ToUrl();
            SetPalaceholder();

            Initialize();
        }

        MediaPlayer player;

        /// <summary>
        /// 文档加载地址
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; private set; } = 1;

        public int PageSize { get { return 20; } }

        /// <summary>
        /// 文件总数
        /// </summary>
        public int? Total { get; set; }

        const string palaceholder = "名称";
        private string kw;

        public string Keyword
        {
            get { return kw; }
            set { SetProperty<string>(ref kw, value); }
        }

        private string classId;

        public string ClassId
        {
            get { return classId; }
            set { SetProperty<string>(ref classId, value); }
        }


        private bool showPlayer;
        /// <summary>
        /// 文件类型
        /// </summary>
        public bool ShowPlayer
        {
            get { return showPlayer; }
            set
            {
                SetProperty<bool>(ref showPlayer, value);
                player.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 视频
        /// </summary>
        public ObservableCollection<Video> Videos { get; } = new ObservableCollection<Video>();

        /// <summary>
        /// 视频类型
        /// </summary>
        public ObservableCollection<VideoClass> Classes { get; } = new ObservableCollection<VideoClass>();

        public DelegateCommand<Video> PreviewCommand
        {
            get
            {
                return new DelegateCommand<Video>(vd =>
                {
                    this.ShowPlayer = true;
                    player.Source = vd.AppendFile.Path;
                });
            }
        }

        public DelegateCommand<string> SelectClassCommand
        {
            get
            {
                return new DelegateCommand<string>(async cid =>
                {
                    this.ShowProgressRing();
                    this.ClassId = cid;
                    this.PageIndex = 1;
                    this.Videos.Clear();
                    await LoadData();
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
                    this.PageIndex = 1;
                    this.Videos.Clear();
                    await LoadData();
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

        void SetPalaceholder()
        {
            if (string.IsNullOrWhiteSpace(this.Keyword))
                this.Keyword = palaceholder;
        }

        async void Initialize()
        {
            await LoadClass();
            await LoadData();
            this.HideProgressRing();
        }

        async Task LoadClass()
        {
            try
            {
                string json = await this.HttpService.GetAsync(ConfigurationManager.AppSettings["VideoClassUrl"], null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<List<VideoClass>>(json);

                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    this.Classes.Add(new VideoClass { Text = "全部" });
                    result.ForEach(d => this.Classes.Add(d));
                    this.OnPropertyChanged("Classes");
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        async Task LoadData()
        {
            string name = string.Equals(this.Keyword, palaceholder) ? string.Empty : Keyword;
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                ["params"] = $"{{\"query\": {{\"equipmentId\": \"{ModuleId}\",\"classId\": \"{ClassId}\",\"name\": \"{name}\"}},\"pager\": {{\"pageIndex\": {PageIndex},\"pageSize\": {PageSize}}},\"sort\": {{}}}}"
            };
            try
            {
                string json = await this.HttpService.GetAsync(this.Url, dict, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<VideoResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }

                this.Total = result.Total;
                await this.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    result.Item.ForEach(d => this.Videos.Add(d));
                    this.OnPropertyChanged("Videos");
                }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        public async Task LoadMore()
        {
            if (this.PageIndex * PageSize >= Total)
                return;

            PageIndex++;
            await LoadData();
        }

        public new DelegateCommand GoBackCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.HideProgressRing();
                    if (this.ShowPlayer)
                    {
                        this.ShowPlayer = false;
                        player.Source = null;
                        return;
                    }
                    this.Panel.Children.Clear();
                    this.Panel.Children.Add(new MainUC());
                });
            }
        }
    }
}
