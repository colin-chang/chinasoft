using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Com.ChinaSoft.Test
{
    /// <summary>
    /// ManipulationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ManipulationWindow : Window
    {
        public ManipulationWindow()
        {
            InitializeComponent();
        }

        private void image_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = touchPad;//设置手势坐标容器控件
            e.Mode = ManipulationModes.All;//支持那些手势（平移、旋转、缩放）
        }

        private void image_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
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

        private void image_ManipulationInertiaStarting(object sender,
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

        private void image_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.Source;
            element.Opacity = 1;//不透明
        }
    }
}
