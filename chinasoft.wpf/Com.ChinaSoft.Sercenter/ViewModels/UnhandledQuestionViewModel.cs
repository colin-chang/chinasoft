using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Sercenter.Views;
using Prism.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;

namespace Com.ChinaSoft.Sercenter.ViewModels
{
    public class UnhandledQuestionViewModel : ViewModelBase
    {
        #region Constant and Fileds

        private readonly string _urlRepairBillDetail;
        private Repairbill _repairbill;

        #endregion

        #region Constructor

        public UnhandledQuestionViewModel(string repairBillId)
        {
            _urlRepairBillDetail = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "detail", repairBillId).ToUrl();

            Initializa();
        }

        #endregion

        #region Method

        private async void Initializa()
        {
            await BindRepairDetailAsync();
        }

        private async Task BindRepairDetailAsync()
        {
            try
            {
                string json = await HttpService.GetAsync(_urlRepairBillDetail, null, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<RepairbillResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(MSG_API_FAILED);
                    return;
                }

                //RepairbillInfo = result?.Item;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public DelegateCommand CloseWindowCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var win = Session[PropertyKey.KeyUnhandledQuestionWin] as Window;
                    win?.Close();
                });
            }
        }

        #endregion

        #region Property

        public override UserControl MainUC => new UnhandledQuestionUc();

        /// <summary>
        /// 报修详情
        /// </summary>
        internal Repairbill RepairbillInfo
        {
            get { return _repairbill; }
            set { SetProperty(ref _repairbill, value); }
        }

        #endregion
    }
}
