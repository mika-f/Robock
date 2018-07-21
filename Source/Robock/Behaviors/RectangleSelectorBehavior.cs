﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;

namespace Robock.Behaviors
{
    public class RectangleSelectorBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(int), typeof(RectangleSelectorBehavior));

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(int), typeof(RectangleSelectorBehavior));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(int), typeof(RectangleSelectorBehavior));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(int), typeof(RectangleSelectorBehavior));

        public static readonly DependencyProperty RectangleProperty =
            DependencyProperty.Register(nameof(Rectangle), typeof(Rectangle), typeof(RectangleSelectorBehavior));

        private double _fromPosX;
        private double _fromPosY;

        private bool _isSelecting;

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

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
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
    }
}