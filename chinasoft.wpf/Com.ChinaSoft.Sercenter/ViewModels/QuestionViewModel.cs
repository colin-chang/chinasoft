using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Com.ChinaSoft.Model.Service;
using Com.ChinaSoft.Model.ViewModels;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.Sercenter.Views;
using Com.ChinaSoft.UserControls;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Newtonsoft.Json;
using Prism.Commands;

namespace Com.ChinaSoft.Sercenter.ViewModels
{
    internal class QuestionViewModel : ViewModelBase
    {
        #region Constructor

        public QuestionViewModel(string repairBillId)
        {
            _urlRepairBillDetail = Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "detail", repairBillId).ToUrl();
            _urlRepairBillSubmitAppraise =
                Path.Combine(ApiBaseUrl, "sercenter", "repairbill", "appraise", repairBillId).ToUrl();

            Initializa();
        }

        #endregion Constructor

        #region Constant and Fileds

        private readonly string _urlRepairBillDetail; // 报修单详情 api url
        private readonly string _urlRepairBillSubmitAppraise; // 提交评价 api url
        private Repairbill _repairbill; // 报修单详情对象
        private bool _appraiseContentIsReadOnly; // 评价内容是否只读
        private Visibility _submitButtonVisibility = Visibility.Collapsed;

        #endregion Constant and Fileds

        #region Method

        private async void Initializa()
        {
            await BindRepairDetailAsync();
        }

        /// <summary>
        ///     报修详情调用服务
        /// </summary>
        /// <returns></returns>
        private async Task BindRepairDetailAsync()
        {
            try
            {
                var json = await HttpService.GetAsync(_urlRepairBillDetail, null, true);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<ObjJsonResult<Repairbill>>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(MSG_API_FAILED);
                    return;
                }

                if (result.Obj != null)
                    await UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        RepairbillInfo = result.Obj;

                        if (RepairbillInfo.Status == RepairbillStatus.Scored) // 如果“已评价”
                            AppraiseContentIsReadOnly = true; // 评价内容设为只读

                        if (RepairbillInfo.Status == RepairbillStatus.Handled) // 如果“已处理”
                            SubmitButtonVisibility = Visibility.Visible; // 评价内容设为只读
                    }));
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
                SubmitButtonVisibility = Visibility.Collapsed;
            }
            finally
            {
                HideProgressRing();
            }
        }

        /// <summary>
        ///     提交评论
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task SubmitAppraiseAsync(Repairbill item)
        {
            if (item == null) return;

            try
            {
                string jsonParam =
                    $"{{\"appraisePoint1\":\"{item.AppraisePoint1}\",\"appraisePoint2\":\"{item.AppraisePoint2}\",\"appraisePoint3\":\"{item.AppraisePoint3}\",\"appraiseContent\":\"{item.AppraiseContent}\"}}";

                var json = await HttpService.PutAsync(_urlRepairBillSubmitAppraise, jsonParam);

                if (string.IsNullOrWhiteSpace(json))
                {
                    ConfirmBox.Show(MSG_EMPTY_JSON);
                    return;
                }

                var result = JsonConvert.DeserializeObject<JsonResult>(json);
                if (!result.Success)
                {
                    ConfirmBox.Show(MSG_API_FAILED);
                    return;
                }

                //SubmitButtonVisibility = Visibility.Collapsed;
                ConfirmBox.Show("提交成功！", MSG_OPERATION_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);

                var win = Session[PropertyKey.KeyQuestionDetailWin] as Window;
                win.DialogResult = true;
            }
            catch (Exception ex)
            {
                ConfirmBox.Show(string.Format(MSG_API_EXCEPTION, ex.Message));
            }
            finally
            {
                HideProgressRing();
            }
        }

        private void ValidInput(Repairbill item)
        {
            if (item == null)
                ConfirmBox.Show(MSG_RepairBill_Submit_Empty, MSG_ERROR_TITLE, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }

        #endregion Method

        #region Command

        /// <summary>
        ///     关闭窗口
        /// </summary>
        public DelegateCommand CloseWindowCommand =>
            new DelegateCommand(() =>
            {
                var win = Session[PropertyKey.KeyQuestionDetailWin] as Window;
                win?.Close();
            });

        /// <summary>
        ///     提交评价
        /// </summary>
        public DelegateCommand<Repairbill> SubmitAppraiseCommand =>
            new DelegateCommand<Repairbill>(async item =>
            {
                ShowProgressRing();
                await SubmitAppraiseAsync(item);
            });

        #endregion Command

        #region Property

        public override UserControl MainUC => new QuestionDetailUc();

        // 隐藏父类的UIDispatcher、Panel、ProgressRing 属性
        protected override Dispatcher UIDispatcher
            => Session[PropertyKey.KeyQuestionDetailWinUiDispatcher] as Dispatcher;

        public override Panel Panel => Session?[PropertyKey.KeyQuestionDetailPanel] as Panel;

        public override ProgressRing ProgressRing => Panel.FindName("ring") as ProgressRing;

        /// <summary>
        ///     报修详情
        /// </summary>
        public Repairbill RepairbillInfo
        {
            get { return _repairbill; }
            set { SetProperty(ref _repairbill, value); }
        }

        /// <summary>
        ///     评价内容是否只读
        /// </summary>
        public bool AppraiseContentIsReadOnly
        {
            get { return _appraiseContentIsReadOnly; }
            set { SetProperty(ref _appraiseContentIsReadOnly, value); }
        }

        /// <summary>
        ///     提交评价按钮是否可见
        /// </summary>
        public Visibility SubmitButtonVisibility
        {
            get { return _submitButtonVisibility; }
            set { SetProperty(ref _submitButtonVisibility, value); }
        }

        #endregion Property
    }
}