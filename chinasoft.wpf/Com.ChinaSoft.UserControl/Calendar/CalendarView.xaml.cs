using Com.ChinaSoft.Utility;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Com.ChinaSoft.UserControls.Calendar
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : UserControl
    {
        private const string DateTimeFormat = "yyyy-MM-dd";
        public bool IsOpened { get; set; }

        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime?), typeof(CalendarView),
                new PropertyMetadata(DateTime.Now, (d, e) =>
                {
                    var calendarView = d as CalendarView;
                    if (calendarView != null)
                        calendarView.ThisRun.Text = ((DateTime?)e.NewValue)?.ToString(DateTimeFormat) ?? "选择日期";
                }));

        public CalendarView()
        {
            InitializeComponent();

            ThisCalendar.Language = XmlLanguage.GetLanguage("zh-cn");
        }

        /// <summary>
        /// 打开日历的事件处理
        /// </summary>
        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            TryCloseExistedDialog();

            ThisCalendar.SelectedDate = Value ?? DateTime.Now;
            IsOpened = true;
        }

        /// <summary>
        /// 关闭日历的事件处理
        /// </summary>
        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            IsOpened = false;

            int parameter = Convert.ToInt32(eventArgs.Parameter);
            if (parameter == 0) return; // 点击“取消”，不做任何操作

            if (ThisCalendar.SelectedDate != null) Value = ThisCalendar.SelectedDate.Value;
            ThisRun.Text = Value?.ToString(DateTimeFormat) ?? DateTime.Now.ToString(DateTimeFormat);

        }

        private void TryCloseExistedDialog()
        {
            var parent = Application.Current.MainWindow as FrameworkElement;

            List<FrameworkElement> childList = ControlHelper.FindChildObjects<FrameworkElement>(parent, typeof(CalendarView));
            if (childList.Count > 0)
            {
                for (int idx = 0; idx < childList.Count; idx++)
                {
                    CalendarView view = childList[idx] as CalendarView;

                    if (!ReferenceEquals(view, this) && view.IsOpened)
                        DialogHost.CloseDialogCommand.Execute(0, view.FindName("ButtonCancel") as UIElement);
                }
            }
        }

        private void Button_OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            Mouse.Synchronize();
        }

        private void ThisCalendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
        }
    }
}