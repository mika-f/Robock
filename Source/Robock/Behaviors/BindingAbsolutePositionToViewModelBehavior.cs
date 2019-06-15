using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    internal class BindingAbsolutePositionToViewModelBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var root = Window.GetWindow(AssociatedObject);
            if (root != null)
                root.SizeChanged += ControlOnSizeChanged;
            AssociatedObject.SizeChanged += ControlOnSizeChanged;

            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            while (parent != null)
            {
                if (parent is ScrollViewer scrollViewer)
                    scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
        }

        protected override void OnDetaching()
        {
            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            while (parent != null)
            {
                if (parent is ScrollViewer scrollViewer)
                    scrollViewer.ScrollChanged -= ScrollViewerOnScrollChanged;
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }

            var root = Window.GetWindow(AssociatedObject);
            if (root != null)
                root.SizeChanged -= ControlOnSizeChanged;
            AssociatedObject.SizeChanged -= ControlOnSizeChanged;

            base.OnDetaching();
        }

        private void ControlOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateAbsolutePosition();
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CalculateAbsolutePosition();
        }

        private void CalculateAbsolutePosition()
        {
            var position = AssociatedObject.TransformToAncestor(Window.GetWindow(AssociatedObject) ?? throw new NotSupportedException()).Transform(new Point());
            Position = new Rect(position, new Size(AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));
        }

        #region Position

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(Rect), typeof(BindingAbsolutePositionToViewModelBehavior));

        public Rect Position
        {
            get => (Rect) GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        #endregion
    }
}