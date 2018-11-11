using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Com.ChinaSoft.Devinfo.ViewModels;
using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Resource.Converter;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// SparePartsUC.xaml 的交互逻辑
    /// </summary>
    public partial class SpareUC : UserControl
    {
        SpareViewModel vm;
        public SpareUC()
        {
            InitializeComponent();
            vm = new SpareViewModel();
            this.DataContext = vm;
        }

        private void LoadTimeSpanButtons(object sender, RoutedEventArgs e)
        {
            var panel = sender as StackPanel;
            var btns = panel?.Children.OfType<Button>();
            for (int i = 0; i < btns.Count(); i++)
            {
                var btn = btns.ElementAt(i);
                btn.Tag = i;
                btn.Command = vm.ChangeTimeSpanCommand;
                var curStatus = (TimeSpanTypeEnum)i;
                btn.CommandParameter = curStatus;
                string style = i == 0 ? "button-group-first" : i == btns.Count() - 1 ? "button-group-last" : "button-group-item";
                btn.SetBinding(Button.StyleProperty, new Binding { Path = new PropertyPath("TimeSpanType"), Converter = new ButtonGroupActiveStyleConverter(), ConverterParameter = new ButtonGroupActiveConverterParameter(curStatus.ToString(), style) });
            }
        }

        private void Search_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            vm.CustomScreenCommand.Execute();
        }
    }
}
