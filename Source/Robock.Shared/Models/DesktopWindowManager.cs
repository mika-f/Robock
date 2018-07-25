using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

using Prism.Mvvm;

using Robock.Shared.Win32;

using Size = System.Drawing.Size;

namespace Robock.Shared.Models
{
    /// <summary>
    ///     Desktop Window Manager Wrapper Class
    /// </summary>
    public class DesktopWindowManager : BindableBase, IDisposable
    {
        private IntPtr _hWnd;

        public List<Thumbnail> Thumbnails { get; }

        public DesktopWindowManager(int capacity = 2)
        {
            Thumbnails = new List<Thumbnail>();
            for (var i = 0; i < capacity; i++)
                Thumbnails.Add(new Thumbnail());
        }

        public void Dispose()
        {
            for (var i = 0; i < Thumbnails.Count; i++)
                Stop(i);
        }

        public void Initialize()
        {
            _hWnd = new WindowInteropHelper(Application.Current.MainWindow ?? throw new InvalidOperationException()).Handle;
        }

        public void Stop(int index)
        {
            if (Thumbnails[index].Handle != IntPtr.Zero)
                NativeMethods.DwmUnregisterThumbnail(Thumbnails[index].Handle);
            Thumbnails[index].Handle = IntPtr.Zero;
            Thumbnails[index].IsRendering = false;
            Thumbnails[index].Size = new Size(0, 0);
        }

        public void Start(int index, IntPtr src, int left, int top, int height, int width, RECT? rect = null)
        {
            Stop(index);

            var registered = NativeMethods.DwmRegisterThumbnail(_hWnd, src, out var thumbnail);
            Thumbnails[index].Handle = thumbnail;

            if (registered == 0)
                StartRender(index, left, top, height, width, rect);
            else
                throw new InvalidOperationException();
        }

        public void Rerender(int index, int left, int top, int height, int width, RECT? rect = null)
        {
            Render(index, left, top, height, width, rect);
        }

        private void StartRender(int index, int left, int top, int height, int width, RECT? rect = null)
        {
            if (Thumbnails[index].Handle == IntPtr.Zero)
                return;
            Thumbnails[index].IsRendering = true;

            NativeMethods.DwmQueryThumbnailSourceSize(Thumbnails[index].Handle, out var size);
            Thumbnails[index].Size = size;

            Render(index, left, top, height, width, rect);
        }

        private void Render(int index, int left, int top, int height, int width, RECT? rect = null)
        {
            if (Thumbnails[index].Handle == IntPtr.Zero)
                return;

            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                fVisible = true,
                dwFlags = (int) (DWM_TNP.DWM_TNP_VISIBLE | DWM_TNP.DWM_TNP_OPACITY | DWM_TNP.DWM_TNP_RECTDESTINATION | DWM_TNP.DWM_TNP_SOURCECLIENTAREAONLY),
                opacity = (byte) (255 / (index == EditorIndex ? 2 : 1)),
                rcDestination = new RECT {left = left, top = top, right = left + width, bottom = top + height},
                fSourceClientAreaOnly = true
            };

            if (rect != null && !rect.Value.IsEmpty())
            {
                props.rcSource = rect.Value;
                props.dwFlags |= (int) DWM_TNP.DWM_TNP_RECTSOURCE;
            }

            NativeMethods.DwmUpdateThumbnailProperties(Thumbnails[index].Handle, ref props);
        }

        #region Constants

        public const int EditorIndex = 0;
        public const int PreviewIndex = 1;

        #endregion
    }
}