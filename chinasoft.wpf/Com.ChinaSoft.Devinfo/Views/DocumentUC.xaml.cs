using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Xps.Packaging;

using Com.ChinaSoft.Devinfo.Model;
using Com.ChinaSoft.Devinfo.ViewModels;
using Com.ChinaSoft.Utility.Network;

namespace Com.ChinaSoft.Devinfo.Views
{
    /// <summary>
    /// DocumentUC.xaml 的交互逻辑
    /// </summary>
    public partial class DocumentUC : UserControl
    {
        #region 变量常量
        DocumentViewModel vm;
        HttpHelper httpService = new HttpHelper();
        #endregion

        public DocumentUC()
        {
            InitializeComponent();
            vm = new DocumentViewModel();
            this.DataContext = vm;
        }

        /// <summary>
        /// 加载文件列表
        /// </summary>
        /// <param name="panel"></param>
        private void DocumentsUpdated(object sender, DataTransferEventArgs e)
        {
            Panel panel = e.OriginalSource as Panel;
            //if (panel == null) return;
            panel?.Children.Clear();
            foreach (var doc in vm.Documents)
            {
                Button btn = new Button { Style = Resources["button-file"] as Style, DataContext = doc, Command = vm.PreviewCommand, CommandParameter = doc };
                panel.Children.Add(btn);
            }
        }

        /// <summary>
        /// 预览文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviewFile(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            var doc = btn?.DataContext as Document;
            vm.FilePath = doc?.AppendFile.Path;
            switch (doc.Type)
            {
                case "word": LoadWord(); break;
                case "pdf": LoadPdf(); break;
                case "photo": LoadPhoto(); break;
                case "html": LoadHtml(); break;
                case "zip": LoadZip(); break;
                default: vm.DocumentType = DocumentTypeEnum.FileList; break;
            }
        }

        /// <summary>
        /// 加载Word文档
        /// </summary>
        /// <param name="dv"></param>
        void LoadWord()
        {
            ring.Show();
            ThreadPool.QueueUserWorkItem(async c =>
            {
                vm.DocumentType = DocumentTypeEnum.Word;

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
        /// <param name="dv"></param>
        async void LoadPdf()
        {
            ring.Show();
            vm.DocumentType = DocumentTypeEnum.Pdf;

            string fileName = await httpService.GetFileAsync(vm.FilePath);
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                return;

            pdfViewer.Source = fileName;
            ring.Hide();
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="img"></param>
        void LoadPhoto()
        {
            vm.DocumentType = DocumentTypeEnum.Photo;
            //photoViewer.Url = vm.FilePath;
            photoViewer.Source = vm.FilePath;
        }

        /// <summary>
        /// 加载网页
        /// </summary>
        /// <param name="wb"></param>
        void LoadHtml()
        {
            vm.DocumentType = DocumentTypeEnum.Html;
            htmlViewer.Navigate(vm.FilePath);
        }

        /// <summary>
        /// 加载Zip文件
        /// </summary>
        async void LoadZip()
        {
            ring.Show();
            vm.DocumentType = DocumentTypeEnum.Zip;
            string fileName = await httpService.GetFileAsync(vm.FilePath);
            ring.Hide();
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                MessageBox.Show($"{ Path.GetFileName(fileName)}文件不存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show($"{Path.GetFileName(fileName)}下载完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
