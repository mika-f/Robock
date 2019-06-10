using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    public class ResetCanvasPositionOnVisibilityChangedBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.IsVisibleChanged += AssociatedObjectOnIsVisibleChanged;
        }

        private void AssociatedObjectOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetLeft(AssociatedObject, 0);
            Canvas.SetTop(AssociatedObject, 0);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObjectOnIsVisibleChanged;
            base.OnDetaching();
        }
    }
}