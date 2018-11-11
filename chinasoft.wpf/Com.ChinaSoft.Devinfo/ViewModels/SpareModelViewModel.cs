using System;
using System.Threading.Tasks;
using Com.ChinaSoft.Devinfo.Views;
using Newtonsoft.Json;
using Com.ChinaSoft.Devinfo.Model;
using System.IO;
using Prism.Commands;
using Com.ChinaSoft.UserControls.ConfirmBox;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public class SpareModelViewModel : BaseViewModel
    {
        private string url;
        private SpareModelWindow win;

        public SpareModelViewModel(SpareModelWindow win, string id)
        {
            this.win = win;

            this.url = Path.Combine(this.ApiBaseUrl, "component", "bom", "detail", id).ToUrl();
            LoadItem();
        }

        private string _2d;
        public string Append2d
        {
            get { return _2d; }
            set { SetProperty<string>(ref _2d, value); }
        }

        private string _3d;
        public string Append3d
        {
            get { return _3d; }
            set { SetProperty<string>(ref _3d, value); }
        }

        private async Task LoadItem()
        {
            try
            {
                string json = await this.HttpService.GetAsync(this.url, null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                var result = JsonConvert.DeserializeObject<SpareModelResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(string.Format(MSG_API_FAILED, result.ErrorCode, result.ErrorMsg));
                    return;
                }
                this.Append2d = result.Obj.Appendfile2d?.Path;
                this.Append3d = result.Obj.Appendfile3d?.Path;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        public DelegateCommand CloseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.win.DialogResult = false;
                });
            }
        }
    }
}
