using Com.ChinaSoft.Utility.Network;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Com.ChinaSoft.Devinfo.UserControls
{
    /// <summary>
    /// ImageViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImageViewer), new PropertyMetadata(null, (d, e) => (d as ImageViewer)?.LoadFromFile(e.NewValue?.ToString())));

        /// <summary>
        /// 从本地文件加载
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromFile(string fileName)
        {
            ring.Show();
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                img.Source = null;
                return;
            }
            ThreadPool.QueueUserWorkItem(c =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(fileName, UriKind.Relative);
                image.DecodePixelWidth = Math.Min((int)container.ActualWidth * 6, 7000);//超过7000px Image控件无法自动更新UI
                image.EndInit();
                image.Freeze();

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    img.Source = image;
                    ring.Hide();
                }));
            });
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(ImageViewer), new PropertyMetadata(null, (d, e) => (d as ImageViewer)?.LoadFromUrl(e.NewValue?.ToString())));



        public bool ShowProgressRing
        {
            get { return (bool)GetValue(ShowProgressRingProperty); }
            set { SetValue(ShowProgressRingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowProgressRing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowProgressRingProperty =
            DependencyProperty.Register("ShowProgressRing", typeof(bool), typeof(ImageViewer), new PropertyMetadata(true,(d,e)=>(d as ImageViewer).ToggleProgressRing((bool)e.NewValue)));


        public void ToggleProgressRing(bool visible)
        {
            if (!visible)
                this.container.Children.Remove(ring);
        }

        /// <summary>
        /// 网路路径
        /// </summary>
        //public string Url { set { this.LoadFromUrl(value); } }

        /// <summary>
        /// 从网络路径加载
        /// </summary>
        /// <param name="url"></param>
        public void LoadFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                this.img.Source = null;
                return;
            }
            ring.Show();
            ThreadPool.QueueUserWorkItem(async c =>
            {
                string fileName = await new HttpHelper().GetFileAsync(url);
                if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                    return;

                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(fileName, UriKind.Relative);
                    image.DecodePixelWidth = Math.Min((int)container.ActualWidth * 6, 7000);//超过7000px Image控件无法自动更新UI
                    image.EndInit();
                    image.Freeze();

                    await this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        img.Source = image;
                    }));
                }
                catch
                {
                    await this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        img.Source = null;
                    }));
                }
                finally
                {
                    await this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ring.Hide();
                    }));
                }
            });
        }

        public ImageViewer()
        {
            InitializeComponent();
        }

        ///// <summary>
        ///// 鼠标滚轮滚动
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    //鼠标相对图片位置
        //    var mosePos = e.GetPosition(img);

        //    //固定缩放倍率为   1.2或1/1.2
        //    var scaleValue = scale.ScaleX * (e.Delta > 0 ? 1.2 : 1 / 1.2);
        //    scale.ScaleX = scaleValue;
        //    scale.ScaleY = scaleValue;

        //    //平移对应缩放尺寸
        //    var newPos = e.GetPosition(img);
        //    translate.X += (newPos.X - mosePos.X);
        //    translate.Y += (newPos.Y - mosePos.Y);
        //}

        //Point dragStart;//平移前位置
        ///// <summary>
        ///// 按住
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    dragStart = e.GetPosition(container);
        //}

        ///// <summary>
        ///// 拖动
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Image_MouseMove(object sender, MouseEventArgs e)
        //{
        //    //是否按住左键
        //    if (e.LeftButton != MouseButtonState.Pressed)
        //        return;

        //    //当前位置
        //    var current = e.GetPosition(container);
        //    translate.X += (current.X - dragStart.X) / scale.ScaleX;
        //    translate.Y += (current.Y - dragStart.Y) / scale.ScaleY;

        //    dragStart = current;
        //}

        ///// <summary>
        ///// 放大
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ZoomIn(object sender, RoutedEventArgs e)
        //{
        //    var scaleValue = scale.ScaleX * 1.2;
        //    scale.ScaleX = scaleValue;
        //    scale.ScaleY = scaleValue;
        //}

        ///// <summary>
        ///// 缩小
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ZoomOut(object sender, RoutedEventArgs e)
        //{
        //    var scaleValue = scale.ScaleX / 1.2;
        //    scale.ScaleX = scaleValue;
        //    scale.ScaleY = scaleValue;
        //}

        ///// <summary>
        ///// 原始尺寸
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ResetOrigin(object sender, RoutedEventArgs e)
        //{
        //    //缩放倍率
        //    scale.ScaleX = 1;
        //    scale.ScaleY = 1;

        //    //平移位置
        //    translate.X = 0;
        //    translate.Y = 0;
        //}

        private void image_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = container;//设置手势坐标容器控件
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
