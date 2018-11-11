using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading;

using Com.ChinaSoft.Utility.Win32;

namespace Com.ChinaSoft.UserControls
{
    /// <summary>
    /// Displays a WebBrowser control over a given placement target element in a WPF Window.
    /// The owner window can be transparent, but not this one, due mixing DirectX and GDI drawing. 
    /// </summary>
    public partial class WebBrowserOverlay : Window
    {
        FrameworkElement _placementTarget;

        public WebBrowser WebBrowser { get { return _wb; } }
        int cnt;
        public bool InitComplete { get; set; } = false;

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(WebBrowserOverlay), new PropertyMetadata(null, (d, e) => (d as WebBrowserOverlay).WebBrowser.Source = e.NewValue == null ? null : new Uri(e.NewValue.ToString())));

        double height;
        public WebBrowserOverlay(FrameworkElement placementTarget, double height)
        {
            InitializeComponent();
            this.Visibility = Visibility.Collapsed;
            _placementTarget = placementTarget;
            Window owner = Window.GetWindow(placementTarget);
            //Debug.Assert(owner != null);

            cnt = 0;
            owner.SizeChanged += delegate { OnSizeLocationChanged(); };
            owner.LocationChanged += delegate { OnSizeLocationChanged(); };
            _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };

            if (owner.IsVisible)
            {
                Owner = owner;
                //Show();
            }
            else
                owner.IsVisibleChanged += delegate
                {
                    if (owner.IsVisible)
                    {
                        Owner = owner;
                        //Show();
                    }
                };

            this.height = height;
            //owner.LayoutUpdated += new EventHandler(OnOwnerLayoutUpdated);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                // Delayed call to avoid crash due to Window bug.
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Owner.Close();
                });
        }

        bool hasOwner = false;
        void TryGetOwner() => this.Dispatcher.Invoke(() => hasOwner = this.Owner != null);
        void OnSizeLocationChanged()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                while (!hasOwner)
                {
                    TryGetOwner();
                    Thread.Sleep(100);
                }

                this.Dispatcher.Invoke(SizeLocationChanged);
            });
            //SizeLocationChanged();
        }

        void SizeLocationChanged()
        {
            Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
            CompositionTarget ct = hwndSource.CompositionTarget;
            offset = ct.TransformToDevice.Transform(offset);
            size = ct.TransformToDevice.Transform(size);

            POINT screenLocation = new POINT(offset);
            Win32API.ClientToScreen(hwndSource.Handle, ref screenLocation);
            POINT screenSize = new POINT(size);

            Win32API.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);

            if (++cnt >= 3)
            {
                this.InitComplete = true;
            }
        }
    };
}
