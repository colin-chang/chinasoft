using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Com.ChinaSoft.Devinfo.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Initiallize();
        }

        private DeviceOverview overView;

        public DeviceOverview OverView
        {
            get { return overView; }
            set { SetProperty<DeviceOverview>(ref overView, value); }
        }

        async void Initiallize()
        {
            string url = Path.Combine(this.ApiBaseUrl, "equipment", "info", "detail", this.ModuleId).ToUrl();
            try
            {
                string json = await this.HttpService.GetAsync(url, null, true);
                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }
                this.OverView = JsonConvert.DeserializeObject<DeviceOverview>(json);
                this.EquipmentName = this.OverView.EquipmentName;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }
    }
}
