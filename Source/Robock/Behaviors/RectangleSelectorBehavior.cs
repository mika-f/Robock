using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;

namespace Robock.Behaviors
{
    public class RectangleSelectorBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(int), typeof(RectangleSelectorBehavior), new FrameworkPropertyMetadata(LeftPropertyChanged));

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(int), typeof(RectangleSelectorBehavior), new FrameworkPropertyMetadata(TopPropertyChanged));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(int), typeof(RectangleSelectorBehavior), new FrameworkPropertyMetadata(HeightPropertyChanged));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(int), typeof(RectangleSelectorBehavior), new FrameworkPropertyMetadata(WidthPropertyChanged));

        public static readonly DependencyProperty RectangleProperty =
            DependencyProperty.Register(nameof(Rectangle), typeof(Rectangle), typeof(RectangleSelectorBehavior), new FrameworkPropertyMetadata(RectanglePropertyChanged));

        private IDisposable _disposable;

        private double _fromPosX;
        private double _fromPosY;

        private bool _isSelecting;
        private Subject<int> _subject;

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

        public Rectangle Rectangle
        {
            get => (Rectangle) GetValue(RectangleProperty);
            set => SetValue(RectangleProperty, value);
        }

        private static void LeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RectangleSelectorBehavior behavior))
                return;
            behavior._subject.OnNext((int) e.NewValue);
        }

        private static void TopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RectangleSelectorBehavior behavior))
                return;
            behavior._subject.OnNext((int) e.NewValue);
        }

        private static void HeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RectangleSelectorBehavior behavior))
                return;
            if (behavior.Rectangle == null)
                return;

            behavior.Rectangle.Height = (int) e.NewValue;
            behavior.UpdateRenderStatus();
        }

        private static void WidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RectangleSelectorBehavior behavior))
                return;
            if (behavior.Rectangle == null)
                return;

            behavior.Rectangle.Width = (int) e.NewValue;
            behavior.UpdateRenderStatus();
        }

        private static void RectanglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is RectangleSelectorBehavior behavior))
                return;

            // 描画が一瞬ずれた状態で表示されるため (このあと Left/Top の更新で描画されるので OK)
            behavior.Rectangle.Visibility = Visibility.Hidden;
            behavior.Rectangle.Height = behavior.Height;
            behavior.Rectangle.Width = behavior.Width;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _subject = new Subject<int>();
            _disposable = _subject.Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    Rectangle.Margin = new Thickness(Left, Top, 0, 0);
                    UpdateRenderStatus();
                });
            });
            AssociatedObject.DataContextChanged += AssociatedObjectOnDataContextChanged;
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown -= AssociatedObjectOnMouseLeftButtonDown;
            AssociatedObject.DataContextChanged -= AssociatedObjectOnDataContextChanged;
            _subject.Dispose();
            _disposable.Dispose();
            base.OnDetaching();
        }

        private void AssociatedObjectOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Rectangle == null)
                return;
            UpdateRenderStatus();
        }

        private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSelecting = true;
            var pos = e.GetPosition(AssociatedObject);
            _fromPosX = pos.X;
            _fromPosY = pos.Y;

            Rectangle.Margin = new Thickness(pos.X, pos.Y, 0, 0);
            Rectangle.Height = Rectangle.Width = 0;
            Rectangle.Visibility = Visibility.Visible;
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isSelecting = false;
            Top = (int) Rectangle.Margin.Top;
            Left = (int) Rectangle.Margin.Left;
            Width = (int) Rectangle.Width;
            Height = (int) Rectangle.Height;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSelecting)
                return;

            var pos = e.GetPosition(AssociatedObject);

            if (pos.X >= _fromPosX && pos.Y >= _fromPosY)
            {
                Rectangle.Margin = new Thickness(_fromPosX, _fromPosY, 0, 0);
                Rectangle.Width = pos.X - _fromPosX;
                Rectangle.Height = pos.Y - _fromPosY;
            }
            else if (pos.X >= _fromPosX && pos.Y < _fromPosY)
            {
                Rectangle.Margin = new Thickness(_fromPosX, pos.Y, 0, 0);
                Rectangle.Width = pos.X - _fromPosX;
                Rectangle.Height = _fromPosY - pos.Y;
            }
            else if (pos.X < _fromPosX && pos.Y >= _fromPosY)
            {
                Rectangle.Margin = new Thickness(pos.X, _fromPosY, 0, 0);
                Rectangle.Width = _fromPosX - pos.X;
                Rectangle.Height = pos.Y - _fromPosY;
            }
            else
            {
                Rectangle.Margin = new Thickness(pos.X, pos.Y, 0, 0);
                Rectangle.Width = _fromPosX - pos.X;
                Rectangle.Height = _fromPosY - pos.Y;
            }
        }

        private bool IsSelected()
        {
            return Math.Abs(Rectangle.Width) > 0 && Math.Abs(Rectangle.Height) > 0;
        }

        private void UpdateRenderStatus()
        {
            Rectangle.Visibility = IsSelected() ? Visibility.Visible : Visibility.Hidden;
        }
    }
}