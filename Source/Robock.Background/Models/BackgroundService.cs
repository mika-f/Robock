using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Wpf.Interop.DirectX;

using Robock.Shared.Win32;

// ReSharper disable LocalizableElement

namespace Robock.Background.Models
{
    // TODO: Error handling, remove all MessageBox from this model.
    // 様々な理由から Singleton にせざるを得ない (RobockService への注入不可)
    public class BackgroundService
    {
        private D3D11Image _dxImage;
        private IntPtr _hWnd;
        private IntPtr _srcWindowHandle;
        private DGetDxSharedSurface GetDxSharedSurface { get; set; }

        public void Initialize(IntPtr hWnd, D3D11Image dxImage)
        {
            if (hWnd == IntPtr.Zero)
                MessageBox.Show("");
            _hWnd = hWnd;
            _dxImage = dxImage;
            _dxImage.WindowOwner = hWnd;
            _dxImage.OnRender = Render;
            _dxImage.RequestRender();

            var funcPtr = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("user32"), "DwmGetDxSharedSurface");
            GetDxSharedSurface = Marshal.GetDelegateForFunctionPointer<DGetDxSharedSurface>(funcPtr);
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
            _srcWindowHandle = hWnd;

            // 1st, send 0x052C (undocumented) message to progman
            var workerW = FindWorkerW();
            var progman = NativeMethods.FindWindow("Progman", null);

            // 2nd, move self to rendering position
            var (x1, y1) = GetPreviewWindowPosition();
            NativeMethods.MoveWindow(_hWnd, x1, y1, width, height, true);

            // 3rd, stick myself to progman
            NativeMethods.SetParent(_hWnd, progman);
        }

        private void Render(IntPtr hSurface, bool isNewSurface)
        {
            if (_srcWindowHandle == IntPtr.Zero)
                return;

            GetDxSharedSurface(_srcWindowHandle, out var phSurface, out var pAdapterLuid, out var pFmtWindow, out var pPresentFlgs, out var pWin32KUpdateId);
        }

        public void StopRender()
        {
            _srcWindowHandle = IntPtr.Zero;

            NativeMethods.SetParent(_hWnd, (IntPtr) null);
        }

        public void Release()
        {
            _dxImage.Dispose();
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

        private IntPtr FindWorkerW()
        {
            var progman = NativeMethods.FindWindow("Progman", null);
            NativeMethods.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out var result);

            var workerW = IntPtr.Zero;
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                var shell = NativeMethods.FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero)
                    workerW = NativeMethods.FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            return workerW;
        }

        private delegate bool DGetDxSharedSurface(IntPtr hWnd, out IntPtr phSurface, out long pAdapterLuid, out int pFmtWindow, out int pPresentFlgs, out long pWin32KUpdateId);

        #region Singleton

        private static BackgroundService _backgroundService;
        public static BackgroundService Instance => _backgroundService ?? (_backgroundService = new BackgroundService());

        #endregion
    }
}