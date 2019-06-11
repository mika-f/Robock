using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    public class CanvasBindingBehavior : Behavior<FrameworkElement>
    {
        #region Left

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(CanvasBindingBehavior), new PropertyMetadata(OnLeftChanged));

        public double Left
        {
            get => (double) GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is double value && d is CanvasBindingBehavior behavior)
                Canvas.SetLeft(behavior.AssociatedObject, value);
        }

        #endregion

        #region Top

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(CanvasBindingBehavior), new PropertyMetadata(OnTopChanged));

        public double Top
        {
            get => (double) GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is double value && d is CanvasBindingBehavior behavior)
                Canvas.SetTop(behavior.AssociatedObject, value);
        }

        #endregion
    }
}