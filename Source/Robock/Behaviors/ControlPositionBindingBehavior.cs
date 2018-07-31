using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Robock.Behaviors
{
    /// <summary>
    ///     アタッチされたコントロールの、 TopLevelWindow に対しての座標を VM にぶんなげる
    /// </summary>
    public class ControlPositionBindingBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(int), typeof(ControlPositionBindingBehavior));

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(int), typeof(ControlPositionBindingBehavior));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(int), typeof(ControlPositionBindingBehavior));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(int), typeof(ControlPositionBindingBehavior));

        public int Left
        {
            get => (int) GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public int Top
        {
            get => (int) GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public int Height
        {
            get => (int) GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public int Width
        {
            get => (int) GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.SizeChanged += MainWindowOnSizeChanged;
            AssociatedObject.SizeChanged += AssociatedObjectSizeChanged;
            AssociatedObject.DataContextChanged += AssociatedObjectOnDataContextChanged;
        }

        private void AssociatedObjectOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AssociatedObjectSizeChanged(null, null);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DataContextChanged -= AssociatedObjectOnDataContextChanged;
            AssociatedObject.SizeChanged -= AssociatedObjectSizeChanged;
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.SizeChanged -= MainWindowOnSizeChanged;
            base.OnDetaching();
        }

        private void MainWindowOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Application.Current.MainWindow == null)
                return;

            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            while (parent != null && !(parent is Window))
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;

            if (parent == null)
                return;
            var relative = AssociatedObject.TransformToAncestor(Application.Current.MainWindow).Transform(new Point());
            Left = (int) relative.X;
            Top = (int) relative.Y;
        }

        private void AssociatedObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = (int) AssociatedObject.ActualHeight;
            Width = (int) AssociatedObject.ActualWidth;

            // When rendered size is changed, should update relative points.
            MainWindowOnSizeChanged(null, null);
        }
    }
}