using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.ChinaSoft.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Dictionary<int, Ellipse> movingEllipses = new Dictionary<int, Ellipse>();
        Random rd = new Random();

        private void touchPad_TouchDown(object sender, TouchEventArgs e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 50;
            ellipse.Height = 50;
            ellipse.Stroke = Brushes.White;
            ellipse.Fill = new SolidColorBrush(
                Color.FromRgb(
                    (byte)rd.Next(0, 255),
                    (byte)rd.Next(0, 255),
                    (byte)rd.Next(0, 255))
                    );

            TouchPoint touchPoint = e.GetTouchPoint(touchPad);
            Canvas.SetTop(ellipse, touchPoint.Bounds.Top-touchPoint.Bounds.Height/2.0);
            Canvas.SetLeft(ellipse, touchPoint.Bounds.Left-touchPoint.Bounds.Width/2.0);

            movingEllipses[e.TouchDevice.Id] = ellipse;
            txt.Text = string.Join("/", movingEllipses.Keys);
            touchPad.Children.Add(ellipse);
        }

        private void touchPad_TouchUp(object sender, TouchEventArgs e)
        {
            try
            {
                //movingEllipses.Remove(e.TouchDevice.Id);
                txt.Text = "";
                txt.Text = string.Join("/", movingEllipses.Keys);
                Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
                touchPad.Children.Remove(ellipse);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void touchPad_TouchMove(object sender, TouchEventArgs e)
        {
            Ellipse ellipse = movingEllipses[e.TouchDevice.Id];
            TouchPoint touchPoint = e.GetTouchPoint(touchPad);
            Canvas.SetTop(ellipse, touchPoint.Bounds.Top);
            Canvas.SetLeft(ellipse, touchPoint.Bounds.Left);
        }
    }
}
