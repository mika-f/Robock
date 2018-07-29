using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;

using Microsoft.Wpf.Interop.DirectX;

using Robock.Background.Native;
using Robock.Shared.Win32;

// ReSharper disable LocalizableElement

namespace Robock.Background.Models
{
    // TODO: Error handling, remove all MessageBox from this model.
    // 様々な理由から Singleton にせざるを得ない (RobockService への注入不可)
    public class BackgroundService
    {
        private D3D11Image _dxImage;
        private int _height;
        private IntPtr _hWnd;
        private TimeSpan _lastRender;
        private DxRenderer _renderer;
        private IntPtr _srcWindowHandle;
        private int _width;
        private int _x;
        private int _y;
        private DGetDxSharedSurface GetDxSharedSurface { get; set; }

        public void Initialize(IntPtr hWnd, D3D11Image dxImage)
        {
            if (hWnd == IntPtr.Zero)
                MessageBox.Show("");
            _hWnd = hWnd;
            _dxImage = dxImage;

            _dxImage.WindowOwner = hWnd;
            _dxImage.OnRender = Render;
        }

        public void MoveToOutsideOfVirtualScreen()
        {
            var (x, y) = GetPreviewWindowPosition();
            NativeMethods.GetWindowRect(_hWnd, out var window);
            if (!NativeMethods.MoveWindow(_hWnd, x, y, window.right - window.left, window.bottom - window.top, true))
                MessageBox.Show("MoveWindow failed");
        }

        public void SetupRenderer(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            // Initialize DirectX devices.
            _renderer = new DxRenderer();

            var funcPtr = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("user32"), "DwmGetDxSharedSurface");
            GetDxSharedSurface = Marshal.GetDelegateForFunctionPointer<DGetDxSharedSurface>(funcPtr);

            _dxImage.Dispatcher.Invoke(() => _dxImage.RequestRender());
        }

        public void StartRender(IntPtr hWnd, int x, int y, int width, int height)
        {
            _srcWindowHandle = hWnd;

            // 1st, change DirectX rendering size and register composition event.
            _dxImage.Dispatcher.Invoke(() =>
            {
                _dxImage.SetPixelSize(_width, _height);
                CompositionTarget.Rendering += CompositionTargetOnRendering;
            });

            // 2st, find WorkerW and send 0x052C (undocumented) message to progman
            var workerW = FindWorkerW();
            if (workerW == IntPtr.Zero)
                Debug.WriteLine("WARNING: Unknown desktop structure"); // SHELLDLL_DefView だとか WorkerW なくても動くが、ログだけ残しとく
            var progman = NativeMethods.FindWindow("Progman", null);

            // 3rd, stick myself to progman
            NativeMethods.SetParent(_hWnd, progman);

            // 4th, move self to rendering position
            NativeMethods.MoveWindow(_hWnd, _x, _y, _width, _height, true);
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            if (!(e is RenderingEventArgs args) || _lastRender == args.RenderingTime)
                return;

            // Request next frame to Render()
            _dxImage.RequestRender();
            _lastRender = args.RenderingTime;
        }

        private void Render(IntPtr hSurface, bool isNewSurface)
        {
            if (_srcWindowHandle == IntPtr.Zero)
                return;

            GetDxSharedSurface(_srcWindowHandle, out var phSurface, out var pAdapterLuid, out var pFmtWindow, out var pPresentFlgs, out var pWin32KUpdateId);
            _renderer.Render(hSurface, phSurface, _width, _height, isNewSurface);
        }

        public void StopRender()
        {
            _srcWindowHandle = IntPtr.Zero;

            // 1st, unregister composition rendering event.
            _dxImage.Dispatcher.Invoke(() => { CompositionTarget.Rendering -= CompositionTargetOnRendering; });

            // 2nd, set parent to desktop (nullptr)
            NativeMethods.SetParent(_hWnd, (IntPtr) null);

            // 3rd, move self to outside of desktop
            MoveToOutsideOfVirtualScreen();
        }

        public void Release()
        {
            _renderer?.Release();
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
            NativeMethods.SendMessageTimeout(progman, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out _);

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

        private delegate bool DGetDxSharedSurface(
            IntPtr hWnd, out IntPtr phSurface, out long pAdapterLuid, out int pFmtWindow, out int pPresentFlgs, out long pWin32KUpdateId);

        #region Singleton

        private static BackgroundService _backgroundService;
        public static BackgroundService Instance => _backgroundService ?? (_backgroundService = new BackgroundService());

        #endregion
    }
}