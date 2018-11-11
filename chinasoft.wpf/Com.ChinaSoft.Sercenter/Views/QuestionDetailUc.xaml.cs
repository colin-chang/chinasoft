using System.Windows;
using System.Windows.Controls;
using Com.ChinaSoft.Sercenter.Models;
using Com.ChinaSoft.UserControls.StarRatingControl;

namespace Com.ChinaSoft.Sercenter.Views
{
    /// <summary>
    ///     问题详情用户控件
    /// </summary>
    internal partial class QuestionDetailUc : UserControl
    {
        public QuestionDetailUc()
        {
            InitializeComponent();

            Loaded += QuestionDetailUc_Loaded;
        }

        public RepairbillStatus RepairbillStatus { get; set; }

        private void QuestionDetailUc_Loaded(object sender, RoutedEventArgs e)
        {
            // 如果是非“未处理”状态，则为Grid动态增加三行，并设置每行的控件
            if (RepairbillStatus != RepairbillStatus.Unhandled)
            {
                GridContent.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.5, GridUnitType.Star)});
                GridContent.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
                GridContent.RowDefinitions.Add(new RowDefinition {Height = new GridLength(0.3, GridUnitType.Star)});

                ServiceRow.Visibility = Visibility.Visible;
                Grid.SetRow(ServiceRow, 1);

                ExpertRow.Visibility = Visibility.Visible;
                Grid.SetRow(ExpertRow, 2);

                Grid.SetRow(SubmitRow, 3);
            }
        }

        /// <summary>
        ///     星星的值改变时的处理程序
        /// </summary>
        private void Rating_OnRatingChanged(object sender, RatingChangedEventArgs e)
        {
            var rating = sender as Rating;
            var stackPanel = rating.FindName("stackPanelStars") as StackPanel;

            var countTo = e.Value;
            for (var idx = 0; idx < countTo; idx++)
            {
                var star = stackPanel.Children[idx] as Star;
                star.State = StarState.On;
            }
        }
    }
}