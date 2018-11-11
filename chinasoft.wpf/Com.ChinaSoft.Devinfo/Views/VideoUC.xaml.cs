using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Com.ChinaSoft.Devinfo.ViewModels;
using Com.ChinaSoft.Resource.Converter;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// VideoUC.xaml 的交互逻辑
    /// </summary>
    public partial class VideoUC : UserControl
    {
        VideoViewModel vm;
        public VideoUC()
        {
            InitializeComponent();
            vm = new VideoViewModel(this.player);
            this.DataContext = vm;
        }

        private void ClassesUpdated(object sender, DataTransferEventArgs e)
        {
            Panel panel = e.OriginalSource as Panel;
            if (panel == null) return;
            panel.Children.Clear();
            for (int i = 0; i < vm.Classes.Count(); i++)
            {
                var c = vm.Classes[i];
                Button btn = new Button { Content = c.Text, Command = vm.SelectClassCommand, CommandParameter = c.Value };
                string style = i == 0 ? "button-group-first" : i == vm.Classes.Count() - 1 ? "button-group-last" : "button-group-item";
                btn.SetBinding(Button.StyleProperty, new Binding() { Source = vm, Path = new PropertyPath("ClassId"), Converter = new ButtonGroupActiveStyleConverter(), ConverterParameter = new ButtonGroupActiveConverterParameter(c.Value, style) });
                panel.Children.Add(btn);
            }
        }

        private void VideosUpdated(object sender, DataTransferEventArgs e)
        {
            Panel panel = e.OriginalSource as Panel;
            if (panel == null) return;
            panel.Children.Clear();
            foreach (var vd in vm.Videos)
            {
                Button btn = new Button { Style = Resources["button-file"] as Style, DataContext = vd, Command = vm.PreviewCommand, CommandParameter = vd };
                panel.Children.Add(btn);
            }
        }

        private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            if (sv == null || sv.ScrollableHeight <= 0)
                return;

            if (sv.VerticalOffset < sv.ScrollableHeight)
                return;

            ring.Show();
            await vm.LoadMore();
            ring.Hide();
        }
    }
}
