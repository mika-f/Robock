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

        public static void DrawAfterWorkerW(int x, int y, int width, int height)
        {
            try
            {
                if (WindowHandle == IntPtr.Zero)
                    throw new InvalidOperationException();

                var workerW = FindWorkerW();
                NativeMethods.MoveWindow(WindowHandle, x, y, width, height, true);
                if (workerW == IntPtr.Zero)
                    throw new InvalidOperationException();

                NativeMethods.SetParent(WindowHandle, workerW);
                MessageBox.Show(NativeMethods.GetLastError().ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void DrawAsNormal()
        {
            if (WindowHandle == null || WindowHandle == IntPtr.Zero)
                throw new InvalidOperationException();

            NativeMethods.SetParent(WindowHandle, (IntPtr) null);
            MoveToOutsideOfDesktop();
        }

        // いじっちゃうと自前で戻さないと、前描画した物が残り続けるので、予めビットマップをメモリに保存しておく
        public static void SaveCurrentWallpaper()
        {
            var workerW = FindWorkerW();
            var hdcSrc = NativeMethods.GetDCEx(workerW, IntPtr.Zero, DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.LockWindowUpdate);
            var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
            NativeMethods.GetWindowRect(workerW, out var rect);
            var (width, height) = (rect.right - rect.left, rect.bottom - rect.top);

            var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);
            var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(workerW, hdcSrc);

            _bitmap = Image.FromHbitmap(hBitmap);
            NativeMethods.DeleteObject(hBitmap);
        }

        public static void RestoreWallpaper()
        {
            var workerW = FindWorkerW();

            var hdcSrc = NativeMethods.GetDCEx(workerW, IntPtr.Zero, DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.LockWindowUpdate);
            using (var graphic = Graphics.FromHdc(hdcSrc))
                graphic.DrawImage(_bitmap, new PointF(0, 0));
            NativeMethods.ReleaseDC(workerW, hdcSrc);
        }

        public static IntPtr FindWorkerW()
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
    }
}