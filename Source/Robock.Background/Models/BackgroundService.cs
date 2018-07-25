using System;
using System.Windows.Forms;

using Robock.Shared.Win32;

// ReSharper disable LocalizableElement

namespace Robock.Background.Models
{
    // TODO: Error handling, remove all MessageBox from this model.
    // 様々な理由から Singleton にせざるを得ない (RobockService への注入不可)
    public class BackgroundService
    {
        private IntPtr _hWnd;

        public void Initialize(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                MessageBox.Show("");
            _hWnd = hWnd;
        }

        public void MoveToOutsideOfVirtualScreen()
        {
            var (x, y) = GetPreviewWindowPosition();
            NativeMethods.GetWindowRect(_hWnd, out var window);
            if (!NativeMethods.MoveWindow(_hWnd, x, y, window.right - window.left, window.bottom - window.top, true))
                MessageBox.Show("MoveWindow failed");
        }

        public void StartRender(IntPtr hWnd, int x, int y, int width, int height)
        {
            // NativeMethods.SetParent(_hWnd, NativeMethods.FindWindow("Progman", null));
        }

        public void StopRender()
        {
            // NativeMethods.SetParent(_hWnd, (IntPtr) null);
        }

        private (int, int) GetPreviewWindowPosition()
        {
            var rect = SystemInformation.VirtualScreen;
            var x = 10000;
            var y = 10000;

            if (x < rect.Right)
                x = rect.Right;
            if (y < rect.Bottom)
                y = rect.Bottom;

            return (x, y);
        }

        #region Singleton

        private static BackgroundService _backgroundService;
        public static BackgroundService Instance => _backgroundService ?? (_backgroundService = new BackgroundService());

        #endregion
    }
}