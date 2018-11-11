using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Prism.Commands;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.ViewModels;
using System.Windows.Input;

namespace Com.ChinaSoft.Devinfo.Views
{
    public abstract class KnowledgeUC : UserControl
    {
        #region 基本参量
        //画树
        protected double originW, originH,//画布尺寸
                 btnWidth = 120.0,//节点尺寸
                 btnMaxWidth = 150,
                 btnHeight = 30.0,
                 hSeper = 50.0, //节点间距
                 vSeper = 28.0;

        public abstract KnowledgeViewModel Vm { get; }

        public virtual IInputElement Container { get; }//画布容器

        public abstract Canvas Canvas { get; }//主画布

        //public virtual TranslateTransform Translate { get; }//平移

        //public virtual ScaleTransform Scale { get; }//缩放

        protected bool captureNode = false;//节点是否被点击

        protected DispatcherTimer Timer { get; private set; } = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        
        #endregion

        public KnowledgeUC()
        {
            Timer.Tick += (a, b) => ResetCanvasSize();
            this.DataContext = Vm;
        }

        protected virtual void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            originW = this.Canvas.ActualWidth;
            originH = this.Canvas.ActualHeight;

            Timer.Start();
        }

        #region 响应树结构变化
        public DelegateCommand<Button> BomCommand { get { return new DelegateCommand<Button>(BomClick); } }

        void BomClick(Button node)
        {
            float level = node.GetLevel();
            if (level < 0 || level >= 2)
                return;

            if (node.GetCollapseStatus() != CollapseStatusEnum.BomExpanded)
                ExpandBom(node);
            else
                CollapseBom(node);

            //ResetCanvasSize();
        }


        public DelegateCommand<Button> ExpCommand { get { return new DelegateCommand<Button>(ExpClick); } }

        void ExpClick(Button node)
        {
            float level = node.GetLevel();
            if (level < 1 || level > 3)
                return;

            if (node.GetCollapseStatus() != CollapseStatusEnum.ExpExpanded)
                ExpandExp(node);
            else
                CollapseExp(node);

            //ResetCanvasSize();
        }

        /// <summary>
        /// 显示省略部分
        /// </summary>
        public DelegateCommand<ExperienceBaseNode> ShowClippedCommand
        {
            get
            {
                return new DelegateCommand<ExperienceBaseNode>(node =>
                {
                    //this.nodeClicked = true;

                    if (node == null || !node.HasClipped)
                        return;

                    KnowledgeWindow win = new KnowledgeWindow(node.OriginName) { Owner = Application.Current.MainWindow };
                    win.ShowDialog();
                });
            }
        }
        #endregion

        #region 绘图
        /// <summary>
        /// 展开Bom节点
        /// </summary>
        /// <param name="target"></param>
        protected abstract void ExpandBom(Button target);

        /// <summary>
        /// 折叠Bom节点
        /// </summary>
        /// <param name="start"></param>
        private void CollapseBom(Button start)
        {
            if (start == null || start.GetContext().HasNoBomOrExp)
                return;

            if (start.GetLevel() == 0)
            {
                start.SetStyle(Resources, NodeStyleEnum.Root);
                start.SetCollapseStatus(CollapseStatusEnum.Collapsed);
                GetNodeVisualTree().Where(e => !Object.ReferenceEquals(e, start)).ToList().ForEach(e => e.FadeOut());
            }
            else
            {
                start.SetStyle(Resources, NodeStyleEnum.Collapsed);
                start.SetCollapseStatus(CollapseStatusEnum.Collapsed);
                this.HideBom(start);

                var tree = start.GetVisualTree();
                if (tree == null)
                    return;
                var delta = 0 - start.ExpandBomsCount() * (vSeper + btnHeight);
                StretchAncestorsBomsVerticalLink(start, delta);
            }

            this.RemoveNodeVisualTree();
            start.ClearVisualBoms();
        }

        /// <summary>
        /// 展开Exp节点
        /// </summary>
        /// <param name="target"></param>
        protected abstract void ExpandExp(Button target);

        /// <summary>
        /// 折叠Exp节点
        /// </summary>
        /// <param name="start"></param>
        private void CollapseExp(Button start)
        {
            start.SetCollapseStatus(CollapseStatusEnum.Collapsed);
            if (start == null || start.GetContext().HasNoBomOrExp)
                return;

            start.SetStyle(Resources, NodeStyleEnum.Collapsed);
            this.HideExp(start);

            this.RemoveNodeVisualTree();
            start.ClearVisualExps();
        }

        /// <summary>
        /// 绘制Bom结构
        /// </summary>
        /// <param name="target"></param>
        /// <param name="nodes"></param>
        protected void DrawBom(Button target, IEnumerable<ExperienceBaseNode> nodes)
        {
            bool isRoot = target.GetLevel() == 0;

            #region 折叠相关节点
            if (!isRoot)
            {
                var siblings = target.GetSiblingBomsIncludeSelf();
                foreach (Button btn in siblings)
                {
                    var status = btn.GetCollapseStatus();
                    if (status == CollapseStatusEnum.ExpExpanded)
                        this.CollapseExp(btn);
                    else if (status == CollapseStatusEnum.BomExpanded)
                        this.CollapseBom(btn);
                }
            }
            #endregion

            #region 切换状态
            target.SetStyle(Resources, isRoot ? NodeStyleEnum.ExpandRoot : NodeStyleEnum.ExpandBom);
            target.SetCollapseStatus(CollapseStatusEnum.BomExpanded);
            #endregion

            #region 重置缓存结构
            var tree = new ExperienceBaseNodeVisualTree();
            #endregion

            #region 查询数据
            int cnt;
            if ((cnt = nodes.Count()) <= 0)
                return;
            #endregion

            #region 初始化绘图参数
            var pleft = target.GetLeft();
            var ptop = target.GetTop();
            var left = isRoot ? pleft + btnWidth + hSeper * 2 : pleft + btnWidth / 2 + hSeper;
            double y1 = 0, y2 = 0;
            #endregion

            for (int i = 0; i < cnt; i++)
            {
                #region 绘制按钮节点
                var node = nodes.ElementAt(i);
                node.CollapseStatus = CollapseStatusEnum.Collapsed;
                CalculateNodeLength(node);
                Button btn = new Button
                {
                    DataContext = node,
                    Tag = this,
                    Opacity = 0
                };
                btn.MouseEnter += (s, e) => this.captureNode = true;
                btn.MouseLeave += (s, e) => this.captureNode = false;
                btn.SetStyle(Resources, node.HasExp != false || !node.Leaf ? NodeStyleEnum.Collapsed : NodeStyleEnum.Leaf);
                btn.SetParentBom(target);

                btn.SetLeft(left);
                var top = isRoot ? (nodes.Count() % 2 == 0 ?
                    (ptop + btnHeight / 2 - vSeper / 2 - btnHeight) + (vSeper + btnHeight) * (i - cnt / 2 + 1) :
                    (ptop + (btnHeight + vSeper) * (0 - cnt / 2 + i))) : ptop + (vSeper + btnHeight) * (i + 1);
                btn.SetTop(top);
                this.Canvas.Children.Add(btn);
                tree.Boms.Add(btn);//缓存绘制的节点 
                #endregion

                #region 连接线
                Line l = new Line { X1 = (isRoot ? left - hSeper : left - hSeper), Y1 = top + btnHeight / 2, X2 = left, Y2 = top + btnHeight / 2 };
                this.Canvas.Children.Add(l);
                btn.SetConnectionLine(l);//缓存节点前面的连接短线
                #endregion

                #region 记录起始点坐标
                if (i == 0)
                    y1 = isRoot ? top + btnHeight / 2 : ptop + btnHeight;
                if (i == cnt - 1)
                    y2 = top + btnHeight / 2;
                #endregion
            }
            #region 绘制连接线
            double x = isRoot ? pleft + btnWidth + hSeper : pleft + btnWidth / 2;
            Line verLink = new Line { X1 = x, Y1 = y1, X2 = x, Y2 = y2 };
            this.Canvas.Children.Add(verLink);
            tree.BomsVerticalLink = verLink;//缓存垂直连接线

            //绘制根节点后的水平连接线
            if (isRoot)
            {
                double x1 = pleft + btnWidth;
                double x2 = x1 + hSeper;
                double y = ptop + btnHeight / 2;
                Line rootL = new Line { X1 = x1, X2 = x2, Y1 = y, Y2 = y };
                this.Canvas.Children.Add(rootL);
                tree.RootConnectionLine = rootL;
            }
            #endregion

            #region 更新缓存
            target.SetVisualTree(tree);
            #endregion

            #region 下移关联节点
            if (!isRoot)
            {
                var delta = (vSeper + btnHeight) * cnt;
                StretchAncestorsBomsVerticalLink(target, delta);
            }
            #endregion

            #region 淡入动画
            tree.FadeIn();
            #endregion
        }

        /// <summary>
        /// 隐藏Bom结构
        /// </summary>
        /// <param name="start"></param>
        void HideBom(Button start)
        {
            var tree = start.GetVisualTree();
            if (tree == null)
                return;

            tree.FadeOut();
            tree.Boms.ToList().ForEach(b =>
            {
                var status = b.GetCollapseStatus();
                if (status == CollapseStatusEnum.ExpExpanded)
                    HideExp(b);
                if (status == CollapseStatusEnum.BomExpanded)
                    HideBom(b);
            });
        }

        /// <summary>
        /// 绘制Exp结构
        /// </summary>
        /// <param name="target"></param>
        /// <param name="nodes"></param>
        protected void DrawExp(Button target, IEnumerable<ExperienceBaseNode> nodes)
        {
            #region 折叠关联节点
            var siblings = target.GetSiblingExps().ToList();
            var level = target.GetLevel();
            if (level > 0 && level < 2)
                siblings.AddRange(target.GetSiblingBoms());
            foreach (Button btn in siblings)
            {
                var status = btn.GetCollapseStatus();
                if (status == CollapseStatusEnum.ExpExpanded)
                    this.CollapseExp(btn);
                else if (status == CollapseStatusEnum.BomExpanded)
                    this.CollapseBom(btn);
            }
            #endregion

            #region 切换状态
            target.SetStyle(Resources, NodeStyleEnum.ExpandExp);
            target.SetCollapseStatus(CollapseStatusEnum.ExpExpanded);
            #endregion

            #region 重置缓存结构
            var tree = new ExperienceBaseNodeVisualTree();
            #endregion

            #region 查询数据
            int cnt;
            if ((cnt = nodes.Count()) <= 0)
                return;
            #endregion

            #region 初始化绘图参数
            var pleft = target.GetLeft();
            var ptop = target.GetTop();
            var left = pleft + btnWidth + hSeper * 2;

            string pathFormate = "M{0},{1} H{2}";
            StringBuilder sbPath = new StringBuilder();
            sbPath.AppendFormat(pathFormate, pleft + btnWidth, ptop + btnHeight / 2, pleft + btnWidth + hSeper);//当前点击节点后面短线
            double x = 0, y = 0, h = 0;
            #endregion

            for (int i = 0; i < nodes.Count(); i++)
            {
                #region 绘制按钮节点
                var node = nodes.ElementAt(i);
                string content = node.Name;
                CalculateNodeLength(node);
                Button btn = new Button
                {
                    DataContext = node,
                    Tag = this,
                    Opacity = 0
                };
                btn.MouseEnter += (s, e) => this.captureNode = true;
                btn.MouseLeave += (s, e) => this.captureNode = false;
                NodeStyleEnum style = NodeStyleEnum.Collapsed;
                if (node.Level > 3)
                {

                    btn.Width = btnMaxWidth;
                    style = NodeStyleEnum.Leaf;
                }
                btn.SetStyle(Resources, style);
                btn.SetParentExp(target);

                btn.SetLeft(left);
                var top = nodes.Count() % 2 == 0 ?
                    (ptop + btnHeight / 2 - vSeper / 2 - btnHeight) + (vSeper + btnHeight) * (i - cnt / 2 + 1) :
                    (ptop + (btnHeight + vSeper) * (0 - cnt / 2 + i));
                btn.SetTop(top);
                this.Canvas.Children.Add(btn);
                tree.Exps.Add(btn);//缓存绘制的节点 
                #endregion

                #region 连接线路径
                sbPath.AppendFormat(pathFormate, left - hSeper, top + btnHeight / 2, left);//当前按钮前面短线
                                                                                           //连接各水平线的竖线
                if (i == 0)
                {
                    x = left - hSeper;
                    y = top + btnHeight / 2;
                }
                if (i == cnt - 1)
                {
                    h = top + btnHeight / 2;
                    sbPath.AppendFormat("M {0},{1} V{2}", x, y, h);
                }
                #endregion
            }

            #region 绘制连接线
            Path path = new Path() { Data = Geometry.Parse(sbPath.ToString()), Stroke = Brushes.Orange, Opacity = 0 };
            this.Canvas.Children.Add(path);
            #endregion

            #region 更新缓存
            tree.ExpLines = path;//缓存绘制的连接线
            target.SetVisualTree(tree);
            #endregion

            #region 淡入动画
            tree.FadeIn();
            #endregion
        }

        /// <summary>
        /// 隐藏结构树
        /// </summary>
        /// <param name="start"></param>
        void HideExp(Button start)
        {
            float level = start.GetLevel();
            if (level < 0 || level > 3)
                return;

            var tree = start.GetVisualTree();
            if (tree == null)
                return;

            tree.FadeOut();
            foreach (Button b in tree.Exps)
                HideExp(b);
        }

        /// <summary>
        /// 伸缩祖级Bom节点垂直连接线
        /// </summary>
        /// <param name="start"></param>
        void StretchAncestorsBomsVerticalLink(Button start, double delta)
        {
            var bs = start.GetBottomSiblingBoms();
            bs.ToList().ForEach(n =>
            {
                //移动同级兄弟节点和水平连接线
                n.SetTop(n.GetTop() + delta);
                var l = n.GetConnectionLine();
                l.SetTop(l.GetTop().ToNonNaN() + delta);
            });
            var parent = start.GetParentBom();
            if (parent == null)
                return;
            if (bs.Count() > 0)
            {
                var bvl = parent.GetBomsVerticalLink();
                bvl.Y2 += delta;
            }
            StretchAncestorsBomsVerticalLink(parent, delta);
        }

        /// <summary>
        /// 移除缓存结构树
        /// </summary>
        void RemoveNodeVisualTree()
        {
            GetNodeVisualTree().Where(e => e.Opacity < 1).ToList().ForEach(ele => this.Canvas.Children.Remove(ele));
        }

        /// <summary>
        /// 绘制根节点
        /// </summary>
        /// <returns>根节点</returns>
        protected Button DrawRoot()
        {
            Button btn = new Button
            {
                DataContext = Vm.Root,
                Tag = this
            };
            btn.MouseEnter += (s, e) => this.captureNode = true;
            btn.MouseLeave += (s, e) => this.captureNode = false;
            btn.SetStyle(Resources, NodeStyleEnum.Root);
            btn.SetLeft(5);
            btn.SetTop((originH - btnHeight) / 2);
            this.Canvas.Children.Add(btn);

            return btn;
        }

        /// <summary>
        /// 重新计算画布尺寸
        /// </summary>
        protected void ResetCanvasSize()
        {
            this.Canvas.Width = originW;
            this.Canvas.Height = originH;
            this.Canvas.Margin = new Thickness(0);

            var nodes = GetAllNodes();
            if (nodes == null || nodes.Count() <= 0)
                return;
            double maxTop = nodes.Max(n => n.GetTop()) + btnHeight;
            double minTop = nodes.Min(n => n.GetTop());
            double maxLeft = nodes.Max(n => n.GetLeft()) + btnWidth;

            if (maxTop > originH)
                this.Canvas.Height = originH + (maxTop - originH);
            if (minTop < 0)
                this.Canvas.Margin = new Thickness(0, Math.Abs(minTop), 0, 0);

            if (maxLeft > originW)
                this.Canvas.Width = originW + (maxLeft - originW);
        }

        private void CalculateNodeLength(ExperienceBaseNode node)
        {
            if (string.IsNullOrWhiteSpace(node.Name))
                return;

            int nodeMaxLength;
            if (node.Level > 3 || node.HasNoBomOrExp)
                //没有Bom和Exp
                nodeMaxLength = 8;
            else if (!node.Leaf && node.HasExp == true)
                //有Bom和Exp
                nodeMaxLength = 4;
            else
                //只有Exp   
                nodeMaxLength = 5;

            if (node.Name.Length > nodeMaxLength)
            {
                node.OriginName = node.Name;
                node.Name = $"{node.Name.Substring(0, nodeMaxLength - 1)}…";
                node.HasClipped = true;
            }
        }
        #endregion

        //#region 缩放
        ///// <summary>
        ///// 放大
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ZoomIn(object sender, RoutedEventArgs e)
        //{
        //    var scaleValue = Scale.ScaleX * 1.2;
        //    Scale.ScaleX = scaleValue;
        //    Scale.ScaleY = scaleValue;
        //}

        ///// <summary>
        ///// 缩小
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ZoomOut(object sender, RoutedEventArgs e)
        //{
        //    var scaleValue = Scale.ScaleX / 1.2;
        //    Scale.ScaleX = scaleValue;
        //    Scale.ScaleY = scaleValue;
        //}

        ///// <summary>
        ///// 原始尺寸
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ResetOrigin(object sender, RoutedEventArgs e)
        //{
        //    //缩放倍率
        //    Scale.ScaleX = 1;
        //    Scale.ScaleY = 1;

        //    //平移位置
        //    Translate.X = 0;
        //    Translate.Y = 0;
        //}

        ///// <summary>
        ///// 鼠标滚轮滚动
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    //鼠标相对图片位置
        //    var mosePos = e.GetPosition(Canvas);

        //    //固定缩放倍率为   1.2或1/1.2
        //    var scaleValue = Scale.ScaleX * (e.Delta > 0 ? 1.2 : 1 / 1.2);
        //    Scale.ScaleX = scaleValue;
        //    Scale.ScaleY = scaleValue;

        //    //平移对应缩放尺寸
        //    var newPos = e.GetPosition(Canvas);
        //    Translate.X += (newPos.X - mosePos.X);
        //    Translate.Y += (newPos.Y - mosePos.Y);
        //}
        //#endregion

        //#region 拖拽
        //Point dragStart;//平移前位置
        ///// <summary>
        ///// 按住
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    dragStart = e.GetPosition(Container);
        //}

        ///// <summary>
        ///// 拖动
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void Canvas_MouseMove(object sender, MouseEventArgs e)
        //{
        //    //是否按住左键
        //    if (e.LeftButton != MouseButtonState.Pressed)
        //        return;
        //    if (this.captureNode)
        //        return;

        //    //当前位置
        //    var current = e.GetPosition(Container);
        //    Translate.X += (current.X - dragStart.X) / Scale.ScaleX;
        //    Translate.Y += (current.Y - dragStart.Y) / Scale.ScaleY;

        //    dragStart = current;
        //}
        //#endregion

        protected void canvas_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = Container;//设置手势坐标容器控件
            e.Mode = ManipulationModes.All;//支持那些手势（平移、旋转、缩放）
        }

        protected void canvas_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.Source;//手势触发元素
            element.Opacity = 0.5;//设置半透明

            Matrix matrix = ((MatrixTransform)element.RenderTransform).Matrix;//变换对象
            var deltaManipulation = e.DeltaManipulation;//手势操作对象
            Point center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
            center = matrix.Transform(center);//设置变换中心点

            matrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);//缩放
            //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);//旋转
            matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);//平移

            ((MatrixTransform)element.RenderTransform).Matrix = matrix;//应用手势

            if (e.IsInertial)
            {
                Rect containingRect = new Rect(((FrameworkElement)e.ManipulationContainer).RenderSize);//容器边界
                Rect shapeBounds = element.RenderTransform.TransformBounds(new Rect(element.RenderSize));
                if (e.IsInertial && !containingRect.Contains(shapeBounds))
                {
                    e.ReportBoundaryFeedback(e.DeltaManipulation);
                    e.Complete();
                }
            }
        }

        protected void canvas_ManipulationInertiaStarting(object sender,
    ManipulationInertiaStartingEventArgs e)
        {
            //平移惯性
            e.TranslationBehavior = new InertiaTranslationBehavior();
            e.TranslationBehavior.InitialVelocity = e.InitialVelocities.LinearVelocity;//移动速率
            e.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);//减速

            //缩放惯性
            e.ExpansionBehavior = new InertiaExpansionBehavior();
            e.ExpansionBehavior.InitialVelocity = e.InitialVelocities.ExpansionVelocity;//缩放速率
            e.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / 1000.0 * 1000.0;

            //旋转惯性
            //e.RotationBehavior = new InertiaRotationBehavior();
            //e.RotationBehavior.InitialVelocity = e.InitialVelocities.AngularVelocity;//角度速率
            //e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);

            e.Handled = true;
        }

        protected void canvas_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.Source;
            element.Opacity = 1;//不透明
        }

        #region 工具方法

        protected IEnumerable<Button> GetAllNodes()
        {
            return this.Canvas.Children.OfType<Button>().Where(b => b.Opacity == 1);
        }

        protected IEnumerable<UIElement> GetNodeVisualTree()
        {
            return this.Canvas.Children.OfType<UIElement>();
        }

        #endregion
    }
}