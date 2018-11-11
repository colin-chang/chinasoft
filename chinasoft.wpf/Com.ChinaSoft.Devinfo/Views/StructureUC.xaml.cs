using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.ViewModels;
using Com.ChinaSoft.UserControls.ConfirmBox;
using Com.ChinaSoft.Utility.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// StructureUC.xaml 的交互逻辑
    /// </summary>
    public partial class StructureUC : UserControl
    {
        private StructureViewModel vm;
        private HttpHelper httpService = new HttpHelper();

        public StructureUC()
        {
            InitializeComponent();
            vm = new StructureViewModel(wb_circuit);
            //vm = new StructureViewModel(new WebBrowser());
            this.DataContext = vm;
        }

        private void PathsUpdated(object sender, DataTransferEventArgs e)
        {
            e.Handled = true;

            var txt = e.OriginalSource as TextBlock;
            var paths = txt?.DataContext as IEnumerable<StructureItem>;
            if (paths == null)
                return;

            txt.Inlines.Clear();
            var last = paths.LastOrDefault();
            if (vm.IsElementOrCircuit(last))
                paths = paths.Take(paths.Count() - 1);
            for (int i = 0; i < paths.Count(); i++)
            {
                var path = paths.ElementAt(i);
                bool isLast = i == paths.Count() - 1;
                Hyperlink lk = new Hyperlink(new Run(isLast ? path.BomNm : path.BomNm + " > ")) { Command = vm.PathCommand, CommandParameter = path, IsEnabled = !isLast };

                txt.Inlines.Add(lk);
            }
        }

        private void ModelClick(object sender, RoutedEventArgs e)
        {
            vm.ModelStatus = (ModelStatusEnum)Convert.ToInt32((e.OriginalSource as Button).Tag);
            if (vm.ModelStatus == ModelStatusEnum.ThreeD)
                wb3D.Refresh();

            AnimatePart(vm.ModelStatus);
            e.Handled = true;
        }
        
        /// <summary>
        ///  加载随机文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentsUpdated(object sender, DataTransferEventArgs e)
        {
            Panel panel = e.OriginalSource as Panel;
            panel?.Children.Clear();
            foreach (var doc in vm.Documents)
            {
                Button btn = new Button { Style = Resources["button-file"] as Style, DataContext = doc, Command = vm.PreviewCommand, CommandParameter = doc };
                panel.Children.Add(btn);
            }
        }

        /// <summary>
        /// 预览随机文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewFile(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            var doc = btn?.DataContext as Document;
            if (string.Equals(doc.Type, "zip"))
            {
                LoadZip(Path.Combine(vm.FileBaseUrl, doc.AppendFile.Path).ToUrl());
                return;
            }

            vm.FilePath = doc?.AppendFile.Path;
            switch (doc.Type)
            {
                case "word": LoadWord(); break;
                case "pdf": LoadPdf(); break;
                case "html": LoadHtml(); break;
                case "photo": LoadPhoto(); break;
                case "video": LoadVideo(); break;
                default: break;
            }

            AnimatePart(vm.ModelStatus);
        }

        /// <summary>
        /// 加载Word文档
        /// </summary>
        private void LoadWord()
        {
            ring.Show();
            ThreadPool.QueueUserWorkItem(async c =>
            {
                string word = await httpService.GetFileAsync(vm.FilePath);
                if (string.IsNullOrWhiteSpace(word) || !File.Exists(word))
                    return;
                string xps = word.ToXpsExtension();
                if (!File.Exists(xps))
                {
                    var doc = new Aspose.Words.Document(word);
                    doc.Save(xps, Aspose.Words.SaveFormat.Xps);
                }

                await this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    wordViewer.Document = new XpsDocument(xps, FileAccess.Read).GetFixedDocumentSequence();
                    ring.Hide();
                }));
            });
        }

        /// <summary>
        /// 加载Pdf文档
        /// </summary>
        private async void LoadPdf()
        {
            ring.Show();
            string fileName = await httpService.GetFileAsync(vm.FilePath);
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return;

            pdfViewer.Source = fileName;
            ring.Hide();
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private void LoadPhoto()
        {
            photoViewer.Source = vm.FilePath;
        }

        /// <summary>
        /// 加载网页
        /// </summary>
        private void LoadHtml()
        {
            htmlViewer.Source = vm.FilePath;
        }

        /// <summary>
        /// 加载视频文件
        /// </summary>
        private void LoadVideo()
        {
            videoViewer.Source = vm.FilePath;
        }

        /// <summary>
        /// 加载Zip文件
        /// </summary>
        private async void LoadZip(string path)
        {
            ring.Show();
            string fileName = await httpService.GetFileAsync(path);
            ring.Hide();
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                ConfirmBox.Show($"{ Path.GetFileName(fileName)}文件不存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                ConfirmBox.Show($"{Path.GetFileName(fileName)}下载完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region 设备结构部位零件的切换动画效果

        private FrameworkElement _currentElement;

        private void AnimatePart(ModelStatusEnum status)
        {
            if (_currentElement == null) _currentElement = GetCurrentVisibleElement();   // 获取当前可见的零部件

            FrameworkElement nextElement = null;
            switch (status)
            {
                case ModelStatusEnum.Explode:
                    nextElement = panelExplode;
                    break;

                case ModelStatusEnum.ThreeD:
                    nextElement = wb3D;
                    break;

                case ModelStatusEnum.TwoD:
                    nextElement = panel2D;
                    break;

                case ModelStatusEnum.Documents:
                    nextElement = panelDocs;
                    break;

                case ModelStatusEnum.Document_Html:
                    nextElement = htmlViewer;
                    break;

                case ModelStatusEnum.Document_PDF:
                    nextElement = pdfViewer;
                    break;

                case ModelStatusEnum.Document_Photo:
                    nextElement = photoViewer;
                    break;

                case ModelStatusEnum.Document_Word:
                    nextElement = wordViewer;
                    break;

                case ModelStatusEnum.Document_Video:
                    nextElement = videoGrid;
                    break;

                case ModelStatusEnum.Document_Zip:
                    break;
                case ModelStatusEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            if (nextElement != null)
            {
                if (ReferenceEquals(nextElement, _currentElement)) return;

                CtrlIn(nextElement);
                _currentElement = nextElement;
            }
        }

        /// <summary>
        /// 滑入控件
        /// </summary>
        /// <param name="element">要被动画显示的控件</param>
        private void CtrlIn(FrameworkElement element)
        {
            Storyboard storyboard = Resources["CtrlIn"] as Storyboard;
            storyboard.Begin(element);
        }

        /// <summary>
        /// 获取当前显示的控件
        /// </summary>
        /// <returns>当前显示的控件</returns>
        private FrameworkElement GetCurrentVisibleElement()
        {
            foreach (var child in gridPartContainer.Children)
            {
                var c = child as FrameworkElement;

                if (ReferenceEquals(c, pdfViewer) && c.Opacity == 1) return c;   // 单独处理PDF

                if (c != null && c.Visibility == Visibility.Visible) return c;
            }
            return null;
        }

        #endregion 设备结构部位零件的切换动画效果

        private void CircuitChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.CircuitSelected.Execute((sender as DataGrid)?.SelectedValue?.ToString());
        }

        #region 电路图缩放
        private void Zoom_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as StackPanel).Children.OfType<Image>().ToList().ForEach(img => img.Loaded += Zoom_TouchUp);
        }

        private void Zoom_TouchDown(object sender, RoutedEventArgs e)
        {
            var img = (e.OriginalSource as Image);
            string tag = img.Tag.ToString();
            img.Source = new BitmapImage(new Uri($"/Images/Structure/{tag}-active.png", UriKind.Relative));

            try
            {
                switch (tag)
                {
                    case "zoomin": wb_circuit.InvokeScript("amplify"); break;
                    case "origin": wb_circuit.InvokeScript("restore"); break;
                    case "zoomout": wb_circuit.InvokeScript("reduce"); break;
                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        private void Zoom_TouchUp(object sender, RoutedEventArgs e)
        {
            var img = (e.OriginalSource as Image);
            img.Source = new BitmapImage(new Uri($"/Images/Structure/{img.Tag}.png", UriKind.Relative));
        }
        #endregion
    }
}