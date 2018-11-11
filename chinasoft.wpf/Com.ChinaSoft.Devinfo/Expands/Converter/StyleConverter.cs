using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.UserControls;

namespace Com.ChinaSoft.Devinfo.Expands.Converter
{
    public class BomVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as ExperienceBaseNode;
            if (node == null)
                return Visibility.Collapsed;
            if (node.ChildCount > 0 && !node.Leaf)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class ExpVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as ExperienceBaseNode;
            if (node == null)
                return Visibility.Collapsed;
            if (node.HasExp != false)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class BomLeafColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var node = value as ExperienceBaseNode;
            if (node == null)
                return "#189BC0";
            if (node.ChildCount > 0 && !node.Leaf)
                return "#1DB8AB";
            return "#189BC0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class NoResultVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bs = value as ObservableCollection<ExperienceBaseNode>;
            if (bs == null)
                return Visibility.Visible;

            return bs.Count() > 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructureListTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var status = (StructureStatusEnum)value;
            var panel = Application.Current.Properties["Panel"] as Panel;
            if (panel == null)
                return null;
            var currentUC = panel.Children[0] as FrameworkElement;
            if (currentUC == null)
                return null;
            string key = status == StructureStatusEnum.Structure ? "templateStructure" : "templateSearchResult";
            return currentUC.Resources[key] as DataTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructureNextVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int isLeaf = (int)value;
            return isLeaf == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructureContentVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = parameter as string;
            if (string.IsNullOrWhiteSpace(code))
                return Visibility.Collapsed;
            bool isCatalog = string.Equals(code, "catalog");
            var paths = value as ObservableCollection<StructureItem>;
            if (paths == null || paths.Count() <= 1)
                return isCatalog ? Visibility.Visible : Visibility.Collapsed;
            var last = paths.LastOrDefault();
            if (last == null)
                return isCatalog ? Visibility.Visible : Visibility.Collapsed;
            string bec = last.BomEntryCode;
            bec = string.Equals(bec, "position") || string.Equals(bec, "element") ? "position-element" : bec;

            return string.Equals(bec, parameter as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructureToolbarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (value as ObservableCollection<StructureItem>)?.LastOrDefault()?.BomEntryCode;
            string code = parameter as string;
            if (type == null || code == null)
                return Visibility.Collapsed;

            return string.Equals(type, code) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class OpenZoomPopVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Equals((value as ObservableCollection<StructureItem>)?.LastOrDefault()?.BomEntryCode, "circuit");


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class Structure2DHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paths = value as ObservableCollection<StructureItem>;
            if (paths == null || paths.Count() <= 1)
                return 0;
            var bec = paths.LastOrDefault()?.BomEntryCode;
            return string.Equals(bec, "element") ? 355 : 660;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructureModelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            string.Equals(((ModelStatusEnum)value).ToString(), parameter?.ToString()) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StructurePdfViewerOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Equals(((ModelStatusEnum)value).ToString(), parameter?.ToString()) ? 1 : 0;


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class DocumentTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DocumentTypeEnum)value == DocumentTypeEnum.Zip)
                return string.Equals(parameter, "FileList") ? Visibility.Visible : Visibility.Collapsed;

            return string.Equals(value.ToString(), parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class MoonPdfPanelOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (DocumentTypeEnum)value == DocumentTypeEnum.Pdf ? 1 : 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class TimeSpan2DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString().Substring(0, 8);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class BackwardEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (TimeSpan)value != TimeSpan.Zero;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class ForwardEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as MediaPlayerViewModel;
            if (vm == null)
                return false;

            if (vm.Position <= TimeSpan.Zero || vm.Position >= vm.Duration)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class PlayPauseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPalying = (bool)value;
            bool isActive;
            if (!bool.TryParse(parameter as string, out isActive))
                isActive = false;

            if (isActive)
                return isPalying ? "/Images/Video/pause-active.png" : "/Images/Video/play-active.png";
            else
                return isPalying ? "/Images/Video/pause.png" : "/Images/Video/play.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class IsMuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMute = (bool)value;
            bool isActive;
            if (!bool.TryParse(parameter as string, out isActive))
                isActive = false;
            if (isActive)
                return isMute ? "/Images/Video/mute-active.png" : "/Images/Video/volume-active.png";
            else
                return isMute ? "/Images/Video/mute.png" : "/Images/Video/volume.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class PlayerOperationActiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as string;
            if (string.IsNullOrWhiteSpace(source))
                return null;

            return Path.Combine(Path.GetDirectoryName(source), $"{Path.GetFileNameWithoutExtension(source)}-active{Path.GetExtension(source)}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StrategyMapVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            int num;
            if (!int.TryParse(val, out num))
                return Visibility.Collapsed;
            return num <= 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StrategyMapPartVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            int num;
            if (!int.TryParse(val, out num))
                return Visibility.Visible;
            return num <= 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StrategyMapPartAlignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;
            int num;
            if (!int.TryParse(val, out num))
                return HorizontalAlignment.Center;
            return HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class StrategyDetailVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
             string.Equals(((DetailTypeEnum)value).ToString(), parameter?.ToString()) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class SpareIsCustomTimeSpan2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (TimeSpanTypeEnum)value == TimeSpanTypeEnum.Custom ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class SparePercentReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float percent = (float)value;
            float total = float.Parse(parameter?.ToString());
            return percent * total;
            //return (1.0 - percent) * total;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class ChangeInfoDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? date = value as DateTime?;
            return date == null ? parameter : date?.ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}