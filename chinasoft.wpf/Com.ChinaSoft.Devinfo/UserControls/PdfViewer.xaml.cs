using MoonPdfLib;
using System.Windows;
using System.Windows.Controls;

namespace Com.ChinaSoft.Devinfo.UserControls
{
    /// <summary>
    /// PdfPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PdfViewer : UserControl
    {
        public PdfViewer()
        {
            InitializeComponent();
        }


        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(PdfViewer), new PropertyMetadata(null, (d, e) =>
            {
                var moonPdfPanel = (d as PdfViewer).FindName("moonPdfPanel") as MoonPdfPanel;
                if (moonPdfPanel == null)
                    return;
                moonPdfPanel.OpenFile(e.NewValue?.ToString());
            }));

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomIn();
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomOut();
        }

        private void ResetOrigin(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.Zoom(1.0);
        }

        private void Fit2Height(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomToHeight();
        }
    }
}
