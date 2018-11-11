using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Com.ChinaSoft.UserControls.PageTransitions
{
    public partial class PageTransition : UserControl
    {
        Stack<Control> pages = new Stack<Control>();

        public Control CurrentPage { get; set; }

        public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register("TransitionType",
            typeof(PageTransitionType),
            typeof(PageTransition), new PropertyMetadata(PageTransitionType.SlideAndFade));

        public PageTransitionType TransitionType
        {
            get
            {
                return (PageTransitionType)GetValue(TransitionTypeProperty);
            }
            set
            {
                SetValue(TransitionTypeProperty, value);
            }
        }

        public PageTransition()
        {
            InitializeComponent();
        }

        public void ShowPage(Control newPage)
        {
            pages.Push(newPage);
            Task.Factory.StartNew(() => ShowNewPage());
        }

        void ShowNewPage()
        {
            Dispatcher.Invoke((Action)delegate
            {

                if (contentPresenter.Content != null)
                {
                    Control oldPage = contentPresenter.Content as Control;

                    if (oldPage != null)
                    {
                        oldPage.Loaded -= newPage_Loaded;

                        UnloadPage(oldPage);

                    }
                }
                else
                {
                    ShowNextPage();
                }

            });
        }

        void ShowNextPage()
        {
            Control newPage = pages.Pop();

            newPage.Loaded += newPage_Loaded;

            contentPresenter.Content = newPage;
        }

        void UnloadPage(Control page)
        {
            Storyboard hidePage = (Resources[$"{TransitionType}Out"] as Storyboard).Clone();

            hidePage.Completed += hidePage_Completed;

            hidePage.Begin(contentPresenter);
        }

        void newPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources[$"{TransitionType}In"] as Storyboard;

            showNewPage.Begin(contentPresenter);

            CurrentPage = sender as Control;
        }

        void hidePage_Completed(object sender, EventArgs e)
        {
            contentPresenter.Content = null;
            ShowNextPage();
        }
    }
}
