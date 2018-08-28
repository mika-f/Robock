using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Robock.Behaviors
{
    internal class ScrollToTopBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DataContextChanged += OnDataContextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= OnDataContextChanged;
            base.OnDetaching();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AssociatedObject.ScrollToTop();
        }
    }
}