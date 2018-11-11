using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Interactivity;

using Com.ChinaSoft.Model.ViewModels;

namespace Com.ChinaSoft.Resource.Behaviors
{
    public class ReachingBottomBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var dpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.VerticalOffsetProperty, AssociatedType);
            dpd.AddValueChanged(AssociatedObject, async (sender, args) =>
             {
                 if (AssociatedObject.VerticalOffset >= AssociatedObject.ScrollableHeight)
                     await (AssociatedObject.DataContext as ILazyLoadingViewModel).LoadMore();
             });
        }
    }
}
