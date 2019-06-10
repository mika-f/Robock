using System;
using System.Windows;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    internal class AdjustRectSizeByParentControlBehavior : Behavior<RectangleGeometry>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            if (AttachTo != null)
                AttachTo.SizeChanged += AttachedParentOnSizeChanged;
        }

        protected override void OnDetaching()
        {
            if (AttachTo != null)
                AttachTo.SizeChanged -= AttachedParentOnSizeChanged;
            base.OnDetaching();
        }

        private void AttachedParentOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            if (size.IsEmpty)
                return;

            AssociatedObject.Rect = new Rect(new Point(OffsetX, OffsetY), new Size(size.Width + Math.Abs(OffsetX) * 2, size.Height + Math.Abs(OffsetY) * 2));
        }

        #region AttachTo

        public static readonly DependencyProperty AttachToProperty = DependencyProperty.Register(nameof(AttachTo), typeof(FrameworkElement), typeof(AdjustRectSizeByParentControlBehavior), new PropertyMetadata(OnAttachedToChanged));

        public FrameworkElement AttachTo
        {
            get => (FrameworkElement) GetValue(AttachToProperty);
            set => SetValue(AttachToProperty, value);
        }

        private static void OnAttachedToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is AdjustRectSizeByParentControlBehavior behavior))
                return;

            if (e.OldValue != null && e.OldValue is FrameworkElement removed)
                removed.SizeChanged -= behavior.AttachedParentOnSizeChanged;
            if (e.NewValue != null && e.NewValue is FrameworkElement assigned)
                assigned.SizeChanged += behavior.AttachedParentOnSizeChanged;
        }

        #endregion

        #region OffsetX

        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(AdjustRectSizeByParentControlBehavior));

        public double OffsetX
        {
            get => (double) GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        #endregion

        #region OffsetY

        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(AdjustRectSizeByParentControlBehavior));

        public double OffsetY
        {
            get => (double) GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        #endregion
    }
}