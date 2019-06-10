using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    internal class DraggableControlOnCanvasBehavior : Behavior<Thumb>
    {
        private double _deltaX;
        private double _deltaY;
        private double _left;
        private double _top;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DragStarted += AssociatedObjectOnDragStarted;
            AssociatedObject.DragDelta += AssociatedObjectOnDragDelta;
            AssociatedObject.DragCompleted += AssociatedObjectOnDragCompleted;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragCompleted -= AssociatedObjectOnDragCompleted;
            AssociatedObject.DragDelta -= AssociatedObjectOnDragDelta;
            AssociatedObject.DragStarted -= AssociatedObjectOnDragStarted;
            base.OnDetaching();
        }

        private void AssociatedObjectOnDragStarted(object sender, DragStartedEventArgs e)
        {
            _left = Canvas.GetLeft(AssociatedObject);
            _top = Canvas.GetTop(AssociatedObject);
            _deltaX = _deltaY = 0;

            AssociatedObject.Cursor = Cursors.Hand;
        }

        private void AssociatedObjectOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            _deltaX += e.HorizontalChange;
            _deltaY += e.VerticalChange;

            Canvas.SetLeft(AssociatedObject, Canvas.GetLeft(AssociatedObject) + e.HorizontalChange);
            Canvas.SetTop(AssociatedObject, Canvas.GetTop(AssociatedObject) + e.VerticalChange);
        }

        private void AssociatedObjectOnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            AssociatedObject.Cursor = Cursors.Arrow;

            Canvas.SetLeft(AssociatedObject, _left + _deltaX);
            Canvas.SetTop(AssociatedObject, _top + _deltaY);
        }
    }
}