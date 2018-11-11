using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Com.ChinaSoft.WorkOrder.Model.WorkOrder;

namespace Com.ChinaSoft.WorkOrder.Converters
{
    public class WorkStatusToIcoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (WorkOrderStatus) value;

            var icoName = string.Empty;
            switch (status)
            {
                case WorkOrderStatus.NotStarted:
                    icoName = "work_stop.png";
                    break;
                case WorkOrderStatus.InProcess:
                    icoName = "work_doing.png";
                    break;
                case WorkOrderStatus.Done:
                    icoName = "work_done.png";
                    break;
                default:
                    break;
            }
            return Path.Combine("/Images/StatusIco", icoName).ToUrl();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}