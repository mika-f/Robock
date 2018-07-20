using System;
using System.Windows;
using System.Windows.Interop;

using Robock.Win32.Native;

using Size = System.Drawing.Size;

namespace Robock.Models
{
    /// <summary>
    ///     Desktop Window Manager Wrapper Class
    /// </summary>
    public class DesktopWindowManager : IDisposable
    {
        private IntPtr _hWnd;
        private IntPtr _thumb;

        public bool IsRendering => _thumb != IntPtr.Zero;

        public DesktopWindowManager()
        {
            _thumb = IntPtr.Zero;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Initialize()
        {
            _hWnd = new WindowInteropHelper(Application.Current.MainWindow ?? throw new InvalidOperationException()).Handle;
        }

        public void Stop()
        {
            if (_thumb != IntPtr.Zero)
                NativeMethods.DwmUnregisterThumbnail(_thumb);
            _thumb = IntPtr.Zero;
        }

        public void Start(IntPtr src, int left, int top, int height, int width)
        {
            Stop();

            var registered = NativeMethods.DwmRegisterThumbnail(_hWnd, src, out _thumb);
            if (registered == 0)
                StartRender(left, top, height, width);
        }

        public void Redender(int left, int top, int height, int width)
        {
            StartRender(left, top, height, width);
        }

        private void StartRender(int left, int top, int height, int width)
        {
            if (_thumb == IntPtr.Zero)
                return;

            Size size;
            NativeMethods.DwmQueryThumbnailSourceSize(_thumb, out size);

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = 0x8 | 0x4 | 0x1,
                opacity = 255,
                rcDestination = new RECT {left = left, top = top, right = left + width, bottom = top + height}
            };

            NativeMethods.DwmUpdateThumbnailProperties(_thumb, ref props);
        }
    }
}