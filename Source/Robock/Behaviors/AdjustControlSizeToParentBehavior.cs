using System;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    /// <summary>
    ///     親のコントロールサイズに合わせて、設定されたアスペクト比を維持したまま最大となるように、コントロールサイズを調整します。
    /// </summary>
    internal class AdjustControlSizeToParentBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AttachTo.SizeChanged += AttachedParentOnSizeChanged;
        }

        protected override void OnDetaching()
        {
            AttachTo.SizeChanged -= AttachedParentOnSizeChanged;
            base.OnDetaching();
        }

        private void AttachedParentOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            if (size.IsEmpty || Math.Abs(BaseWidth) <= 0 || Math.Abs(BaseHeight) <= 0)
                return;

            var (x, y) = GetAspectRatio((int) BaseWidth, (int) BaseHeight);
            var (width, height) = (Math.Floor(size.Width / x), Math.Floor(size.Height / y));
            AssociatedObject.Width = x * Math.Min(width, height);
            AssociatedObject.Height = y * Math.Min(width, height);
        }

        private (int, int) GetAspectRatio(int x, int y)
        {
            int CalcGcd(int a, int b)
            {
                return b == 0 ? a : CalcGcd(b, a % b);
            }

            var gcd = CalcGcd(x, y);
            return (x / gcd, y / gcd);
        }

        #region AttachTo

        public static readonly DependencyProperty AttachToProperty = DependencyProperty.Register(nameof(AttachTo), typeof(FrameworkElement), typeof(AdjustControlSizeToParentBehavior));

        public FrameworkElement AttachTo
        {
            get => (FrameworkElement) GetValue(AttachToProperty);
            set => SetValue(AttachToProperty, value);
        }

        #endregion

        #region BaseHeight

        public static readonly DependencyProperty BaseHeightProperty = DependencyProperty.Register(nameof(BaseHeight), typeof(double), typeof(AdjustControlSizeToParentBehavior));

        public double BaseHeight
        {
            get => (double) GetValue(BaseHeightProperty);
            set => SetValue(BaseHeightProperty, value);
        }

        #endregion

        #region BaseWidth

        public static readonly DependencyProperty BaseWidthProperty = DependencyProperty.Register(nameof(BaseWidth), typeof(double), typeof(AdjustControlSizeToParentBehavior));

        public double BaseWidth
        {
            get => (double) GetValue(BaseWidthProperty);
            set => SetValue(BaseWidthProperty, value);
        }

        #endregion
    }
}