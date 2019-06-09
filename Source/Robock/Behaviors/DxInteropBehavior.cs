﻿using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Microsoft.Wpf.Interop.DirectX;
using Microsoft.Xaml.Behaviors;

using Robock.Models.Renderer;

namespace Robock.Behaviors
{
    /// <summary>
    ///     Microsoft.Wpf.Interop.DirectX との相互運用をごにょごにょするやつ
    /// </summary>
    public class DxInteropBehavior : Behavior<D3D11Image>
    {
        private TimeSpan _lastRenderingTime;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (Application.Current.MainWindow == null)
                throw new InvalidOperationException();
            AssociatedObject.WindowOwner = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            AssociatedObject.OnRender = OnRender;
        }

        private void ParentOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = e.NewSize;
            AssociatedObject.SetPixelSize((int) size.Width, (int) size.Height);
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            if (!(e is RenderingEventArgs args) || _lastRenderingTime == args.RenderingTime)
                return;

            AssociatedObject.RequestRender();
            _lastRenderingTime = args.RenderingTime;
        }

        private void OnRender(IntPtr hSurface, bool isNewSurface)
        {
            Renderer?.Render(hSurface, isNewSurface);
        }

        protected override void OnDetaching()
        {
            if (Parent != null)
                Parent.SizeChanged -= ParentOnSizeChanged;
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
            AssociatedObject.Dispose();

            base.OnDetaching();
        }

        #region Parent

        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register(nameof(Parent), typeof(FrameworkElement), typeof(DxInteropBehavior), new PropertyMetadata(OnParentChanged));

        public FrameworkElement Parent
        {
            get => (FrameworkElement) GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        private static void OnParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DxInteropBehavior behavior))
                return;

            var parent = (FrameworkElement) e.NewValue;
            parent.SizeChanged += behavior.ParentOnSizeChanged;
        }

        #endregion

        #region State

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(Visibility), typeof(DxInteropBehavior), new PropertyMetadata(OnVisibilityChanged));

        public Visibility State
        {
            get => (Visibility) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DxInteropBehavior behavior))
                return;

            var visibility = (Visibility) e.NewValue;
            if (visibility != Visibility.Visible)
            {
                CompositionTarget.Rendering -= behavior.CompositionTargetOnRendering;
            }
            else
            {
                behavior.Renderer?.Initialize();
                CompositionTarget.Rendering += behavior.CompositionTargetOnRendering;
            }
        }

        #endregion

        #region Renderer

        public static readonly DependencyProperty RendererProperty = DependencyProperty.Register(nameof(Renderer), typeof(IRenderer), typeof(DxInteropBehavior));

        public IRenderer Renderer
        {
            get => (IRenderer) GetValue(RendererProperty);
            set => SetValue(RendererProperty, value);
        }

        #endregion
    }
}