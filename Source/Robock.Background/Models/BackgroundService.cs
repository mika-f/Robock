using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;

using Robock.Shared.Win32;

using Application = System.Windows.Application;

namespace Robock.Background.Models
{
    public static class BackgroundService
    {
        private static Bitmap _bitmap;
        public static IntPtr WindowHandle { get; private set; }

        public static void Initialize()
        {
            WindowHandle = new WindowInteropHelper(Application.Current.MainWindow ?? throw new InvalidOperationException()).Handle;
        }

        // 描画範囲外へ飛ばす
        public static void MoveToOutsideOfDesktop()
        {
            if (WindowHandle == null || WindowHandle == IntPtr.Zero)
                throw new InvalidOperationException();

            var rect = SystemInformation.VirtualScreen;
            var x = 10000;
            var y = 10000;

            if (x < rect.Right)
                x = rect.Right;
            if (y < rect.Bottom)
                y = rect.Bottom;

            NativeMethods.GetWindowRect(WindowHandle, out var window);
            if (!NativeMethods.MoveWindow(WindowHandle, x, y, window.right - window.left, window.bottom - window.top, true))
                MessageBox.Show(NativeMethods.GetLastError().ToString());
        }

        public static IntPtr FindWorkerW()
        {
            var workerW = IntPtr.Zero;
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                var shell = NativeMethods.FindWindowEx(hWnd, IntPtr.Zero, "", null);
                if (shell != IntPtr.Zero)
                    workerW = NativeMethods.FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            return workerW;
        }
    }
}