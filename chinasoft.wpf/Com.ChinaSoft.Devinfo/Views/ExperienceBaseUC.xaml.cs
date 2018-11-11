using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.ViewModels;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// ExperienceBaseUC.xaml 的交互逻辑
    /// </summary>
    public partial class ExperienceBaseUC : KnowledgeUC
    {
        #region 基本参量
        //搜索
        private const string SEARCH_PART = "部位/图号";
        private const string SEARCH_FAILURE = "故障码/现象";

        readonly ExperienceBaseViewModel vm = new ExperienceBaseViewModel();
        public override KnowledgeViewModel Vm { get { return vm; } }

        public override IInputElement Container { get { return this.container; } }

        public override Canvas Canvas { get { return this.canvas; } }
        #endregion

        public ExperienceBaseUC()
        {
            InitializeComponent();
        }

        private void AllLoaded(object sender, RoutedEventArgs e)
        {
            this.txtPart.Text = SEARCH_PART;
            this.txtFailure.Text = SEARCH_FAILURE;
        }

        protected override void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            base.Canvas_Loaded(sender, e);

            DrawRoot();
        }

        #region 绘图
        protected override async void ExpandBom(Button target)
        {
            IEnumerable<ExperienceBaseNode> nodes = await vm.GetBomAsync(target.GetContext());
            this.DrawBom(target, nodes);
        }

        protected override async void ExpandExp(Button target)
        {
            var context = target.GetContext();
            IEnumerable<ExperienceBaseNode> nodes = context.Level == 0 ? await vm.GetBomAsync(context) : await vm.GetExpsAsync(context);
            this.DrawExp(target, nodes);
        }
        #endregion

        #region 搜索
        private void BeginSearch(object sender, RoutedEventArgs e)
        {
            string part = string.Equals(txtPart.Text, SEARCH_PART) ? string.Empty : txtPart.Text;
            string failure = string.Equals(txtFailure.Text, SEARCH_FAILURE) ? string.Empty : txtFailure.Text;
            if ((string.IsNullOrWhiteSpace(part) && string.IsNullOrWhiteSpace(failure)) || (string.Equals(part, SEARCH_PART) && string.Equals(failure, SEARCH_FAILURE)))
                return;
            this.Navigate2(new ExperienceBaseSearchedUC(part, failure));
        }

        private void PartFocus(object sender, RoutedEventArgs e)
        {
            if (string.Equals(txtPart.Text.Trim(), SEARCH_PART))
                txtPart.Text = string.Empty;
        }

        private void PartLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPart.Text))
                txtPart.Text = SEARCH_PART;
        }

        private void FailureFocus(object sender, RoutedEventArgs e)
        {
            if (string.Equals(txtFailure.Text.Trim(), SEARCH_FAILURE))
                txtFailure.Text = string.Empty;
        }

        private void FailureLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFailure.Text))
                txtFailure.Text = SEARCH_FAILURE;
        }
        #endregion
    }
}