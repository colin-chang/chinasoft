using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// ExperienceBaseSearchedUC.xaml 的交互逻辑
    /// </summary>
    public partial class ExperienceBaseSearchedUC : KnowledgeUC
    {
        #region 基本参量
        ExperienceBaseSearchedViewModel vm;
        public override KnowledgeViewModel Vm { get { return vm; } }

        public override IInputElement Container { get { return this.container; } }

        public override Canvas Canvas { get { return this.canvas; } }

        /// <summary>
        /// 搜索是否有现象
        /// </summary>
        public bool HasFailure { get; set; }
        #endregion

        public ExperienceBaseSearchedUC()
        {
            InitializeComponent();
        }

        public ExperienceBaseSearchedUC(string part, string failure) : this()
        {
            vm = new ExperienceBaseSearchedViewModel(part, failure);
            this.DataContext = vm;
            this.HasFailure = !string.IsNullOrWhiteSpace(failure);
        }

        protected override void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            base.Canvas_Loaded(sender, e);

            ThreadPool.QueueUserWorkItem(s =>
            {
                //生产者消费者模式
                while (!vm.LoadCompleted)
                    Thread.Sleep(500);

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (vm.SearchedBoms.Count() <= 1)
                    {
                        txtNoResult.Visibility = Visibility.Visible;
                        //this.Canvas.MouseMove -= Canvas_MouseMove;//移除拖动效果
                        //this.toolbar.Children.OfType<Button>().ToList().ForEach(btn => btn.IsEnabled = false);//禁用缩放
                        return;
                    }

                    ExpandFistPath(DrawRoot());
                }), null);
            });
        }

        #region 绘图
        protected override void ExpandBom(Button target)
        {
            IEnumerable<ExperienceBaseNode> nodes = vm.GetBom(target.GetContext().Id, target.GetLevel() == 0);
            this.DrawBom(target, nodes);
        }

        protected override async void ExpandExp(Button target)
        {
            var context = target.GetContext();
            IEnumerable<ExperienceBaseNode> nodes = context.Level == 0 ? vm.GetBom(context.ParentId, true) : await vm.GetExpsAsync(context);
            this.DrawExp(target, nodes);
        }
        #endregion

        #region 展开搜索结果
        /// <summary>
        /// 展开第一条搜索结构
        /// </summary>
        /// <param name="start">起始节点</param>
        void ExpandFistPath(Button start)
        {
            ExpandBom(start);
            var first = start.GetBoms().FirstOrDefault();
            if (first == null)
            {
                if (HasFailure)
                    if (start.GetContext().HasExp == true)
                        ExpandExp(start);
                return;
            }
            ExpandFistPath(first);
        }
        #endregion
    }
}
