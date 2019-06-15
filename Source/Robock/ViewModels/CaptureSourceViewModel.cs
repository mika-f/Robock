using System;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Extensions;
using Robock.Models;
using Robock.Models.CaptureSources;
using Robock.Mvvm;

namespace Robock.ViewModels
{
    public class CaptureSourceViewModel : ViewModel
    {
        public ICaptureSource CaptureSource { get; }
        public ReadOnlyReactiveProperty<string> Title { get; }
        public int Height => 125;
        public int Width { get; }
        public ReactiveProperty<IntPtr> WindowHandle { get; set; }
        public ReactiveProperty<Rect> DisplayPosition { get; }
        public ReactiveProperty<Rect> RenderPosition { get; }

        public CaptureSourceViewModel(ICaptureSource captureSource)
        {
            CaptureSource = captureSource;

            var (x, y) = Utils.GetAspectRatio(CaptureSource.Width, CaptureSource.Height);
            Width = (int) Math.Floor(x / (double) y * Height);
            Title = CaptureSource.ObserveProperty(w => w.Name).ToReadOnlyReactiveProperty().AddTo(this);
            WindowHandle = new ReactiveProperty<IntPtr>();
            WindowHandle.Where(w => w != IntPtr.Zero).Subscribe(_ => RenderPreview()).AddTo(this);
            DisplayPosition = new ReactiveProperty<Rect>();
            DisplayPosition.Where(w => !w.IsEmpty).Subscribe(_ => RenderPreview()).AddTo(this);
            RenderPosition = new ReactiveProperty<Rect>();
            RenderPosition.Where(w => !w.IsEmpty).Subscribe(_ => RenderPreview()).AddTo(this);
        }

        private void RenderPreview()
        {
            if (WindowHandle == null || RenderPosition == null || DisplayPosition == null)
                return;
            CaptureSource.RenderPreview(WindowHandle.Value, RenderPosition.Value, DisplayPosition.Value);
        }
    }
}