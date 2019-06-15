using System;
using System.Windows;
using System.Windows.Interop;

using Microsoft.Xaml.Behaviors;

namespace Robock.Behaviors
{
    internal class BindingWindowHandleToViewModelBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Handle = new WindowInteropHelper(Window.GetWindow(AssociatedObject)).Handle;
        }

        #region Handle

        public static readonly DependencyProperty HandleProperty = DependencyProperty.Register(nameof(Handle), typeof(IntPtr), typeof(BindingWindowHandleToViewModelBehavior));

        public IntPtr Handle
        {
            get => (IntPtr) GetValue(HandleProperty);
            set => SetValue(HandleProperty, value);
        }

        #endregion
    }
}