using System;
using System.Windows;
using System.Windows.Interop;

using Prism.Mvvm;

using Robock.Win32.Native;

namespace Robock.Models
{
    /// <summary>
    ///     Desktop Window Manager Wrapper Class
    /// </summary>
    public class DesktopWindowManager : BindableBase, IDisposable
    {
        private IntPtr _hWnd;
        private IntPtr _thumb;

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
            IsRendering = false;
        }

        public void Start(IntPtr src, int left, int top, int height, int width)
        {
            StartTo(src, _hWnd, left, top, height, width);
        }

        public void StartTo(IntPtr src, IntPtr dest, int left, int top, int height, int width)
        {
            Stop();

            var registered = NativeMethods.DwmRegisterThumbnail(dest, src, out _thumb);
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
            IsRendering = true;

            // 入力ソースのアスペクト比を保つべきか、破棄すべきか
            NativeMethods.DwmQueryThumbnailSourceSize(_thumb, out _);

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = (int) (DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY),
                opacity = 255 / 2,
                rcDestination = new RECT {left = left, top = top, right = left + width, bottom = top + height},
                fSourceClientAreaOnly = true
            };

            NativeMethods.DwmUpdateThumbnailProperties(_thumb, ref props);
        }

        #region IsRendering

        private bool _isRendering;

        public bool IsRendering
        {
            get => _isRendering;
            set => SetProperty(ref _isRendering, value);
        }

        #endregion
    }
}