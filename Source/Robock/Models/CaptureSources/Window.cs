using System;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

using Prism.Mvvm;

using Robock.Interop.Win32;

namespace Robock.Models.CaptureSources
{
    public class Window : BindableBase, ICaptureSource
    {
        private readonly IDisposable _disposable;
        private readonly IntPtr _hWnd;
        private IntPtr _hThumb;

        public Window(IntPtr hWnd)
        {
            _hWnd = hWnd;

            NativeMethods.DwmGetWindowAttributeRect(_hWnd, DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out var bounds, Marshal.SizeOf<RECT>());
            Height = bounds.bottom - bounds.top;
            Width = bounds.right - bounds.left;

            if (Width <= 160 && Height <= 31)
            {
                // Window is probably minimized
                var placement = WINDOWPLACEMENT.Default;
                NativeMethods.GetWindowPlacement(_hWnd, ref placement);

                Height = placement.NormalPosition.bottom - placement.NormalPosition.top;
                Width = placement.NormalPosition.right - placement.NormalPosition.left;
            }
            _disposable = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(500)).Subscribe(_ => Update());
        }

        public void Dispose()
        {
            if (_hThumb != IntPtr.Zero)
                NativeMethods.DwmUnregisterThumbnail(_hThumb);
            _disposable.Dispose();
        }

        public bool IsAvailablePreview => true;

        public int Height { get; }
        public int Width { get; }

        public void RenderThumbnail(IntPtr hDest, Rect position, Rect available)
        {
            if (position.IsEmpty || available.IsEmpty || hDest == IntPtr.Zero)
                return;

            if (_hThumb == IntPtr.Zero)
                NativeMethods.DwmRegisterThumbnail(hDest, _hWnd, out _hThumb);

            NativeMethods.DwmQueryThumbnailSourceSize(_hThumb, out var size);

            // 座標変換
            var source = new RECT { top = 0, left = 0, bottom = size.Height, right = size.Width };
            var destination = new RECT { top = (int) position.Top, left = (int) position.Left, bottom = (int) position.Bottom, right = (int) position.Right };
            if (position.Top <= available.Top)
            {
                source.top += (int) Math.Floor((available.Top - position.Top) / position.Height * size.Height);
                destination.top = (int) available.Top;
            }
            if (position.Bottom >= available.Bottom)
            {
                source.bottom -= (int) Math.Floor((position.Bottom - available.Bottom) / position.Height * size.Height);
                destination.bottom = (int) available.Bottom;
            }

            var isVisible = position.Bottom >= available.Top && position.Top <= available.Bottom;
            var thumbnailProps = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = isVisible ? 1 : 0, // 動いてない？
                dwFlags = (int) (DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTSOURCE | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY),
                opacity = (byte) (isVisible ? 255 : 0),
                rcSource = isVisible ? source : new RECT(),
                rcDestination = isVisible ? destination : new RECT(),
                fSourceClientAreaOnly = 1
            };

            NativeMethods.DwmUpdateThumbnailProperties(_hThumb, ref thumbnailProps);
        }

        public ICaptureSource Clone()
        {
            return new Window(_hWnd);
        }

        private void Update()
        {
            var sb = new StringBuilder(1024);
            NativeMethods.GetWindowText(_hWnd, sb, sb.Capacity);

            Name = sb.ToString();
        }

        #region Name

        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        #endregion
    }
}