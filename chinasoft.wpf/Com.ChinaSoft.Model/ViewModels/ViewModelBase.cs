using Com.ChinaSoft.UserControls;
using Com.ChinaSoft.Utility.Network;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Com.ChinaSoft.Model.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        #region Constant Fields

        // 错误消息（Format）
        protected const string MSG_EMPTY_JSON = "抱歉，请求网络数据异常！\r\n" + "，错误消息：数据为空";

        protected const string MSG_API_FAILED = "抱歉，程序出现错误！\r\n错误码：{0}，错误消息：{1}";
        protected const string MSG_API_EXCEPTION = "抱歉，程序出现错误！\r\n错误消息：{0}";
        protected const string MSG_ERROR_TITLE = "数据错误";
        protected const string MSG_OPERATION_TITLE = "操作提示";
        protected const string MSG_WarningDispose_TITLE = "预警提示";
        protected const string MSG_FaultDetail_NULL = "故障预警数据为空";
        protected const string MSG_ChangeBomConfirm = "一旦提交将不能再次修改，是否确认提交？";
        protected const string MSG_RepairBill_Submit_Empty = "报修单数据是空的，请检查后重试！";

        #endregion Constant Fields

        /// <summary>
        /// 判断是否处于设计器模式
        /// </summary>
        protected bool IsInDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        [DllImport("wininet")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// 网络是否可用
        /// </summary>
        public bool NetworkAvailable
        {
            get
            {
                int i = 0;
                return InternetGetConnectedState(out i, 0);
            }
        }

        /// <summary>
        /// 当前活动窗口
        /// </summary>
        public Window CurrentWindow => Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

        public HttpHelper HttpService => new HttpHelper();

        public string ApiBaseUrl => ConfigurationManager.AppSettings["ApiUrl"];

        public string FileBaseUrl => ConfigurationManager.AppSettings["FileUrl"];

        /// <summary>
        /// 设备Id
        /// </summary>
        public string ModuleId => ConfigurationManager.AppSettings["ModuleId"];

        /// <summary>
        /// 进度条
        /// </summary>
        public virtual ProgressRing ProgressRing => (Panel.Children[0] as FrameworkElement)?.FindName("ring") as ProgressRing;

        /// <summary>
        /// 全局共享内存KeyValue表
        /// </summary>
        protected IDictionary Session => Application.Current.Properties;

        /// <summary>
        /// 主线程UI操作队列
        /// </summary>
        protected virtual Dispatcher UIDispatcher => (Session["UIDispatcher"] as Dispatcher);

        /// <summary>
        /// 主面板
        /// </summary>
        public virtual Panel Panel => (Session["Panel"]) as Panel;

        /// <summary>
        /// 返回命令
        /// </summary>
        public virtual DelegateCommand GoBackCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Panel.Children.Clear();
                    Panel.Children.Add(MainUC);
                });
            }
        }

        /// <summary>
        /// 获取起始主页面
        /// </summary>
        public abstract UserControl MainUC { get; }

        protected void ShowProgressRing()
        {
            if (ProgressRing == null)
                return;

            if (Dispatcher.CurrentDispatcher == UIDispatcher)
                ProgressRing.Show();
            else
                UIDispatcher.BeginInvoke(new Action(() => ProgressRing.Hide()), DispatcherPriority.Send);
        }

        protected void HideProgressRing()
        {
            if (ProgressRing == null)
                return;

            if (Dispatcher.CurrentDispatcher == UIDispatcher)
                ProgressRing.Hide();
            else
                UIDispatcher.BeginInvoke(new Action(() => ProgressRing.Hide()), DispatcherPriority.Send);
        }
    }
}