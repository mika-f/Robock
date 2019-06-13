using System;
using System.Drawing;
using System.Drawing.Imaging;

using Robock.Interop.Win32;

using SharpDX;
using SharpDX.Direct3D11;

using Rectangle = System.Drawing.Rectangle;

namespace Robock.Models.Renderer
{
    internal class BitBltRenderer : BaseRenderer
    {
        private readonly IntPtr _hWnd;

        public BitBltRenderer(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        protected override Texture2D TryGetNextFrameAsTexture2D()
        {
            var hdcSrc = NativeMethods.GetDCEx(_hWnd, IntPtr.Zero, DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.LockWindowUpdate);
            var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);

            NativeMethods.GetWindowRect(_hWnd, out var rect);
            var(width, height) = (rect.right - rect.left, rect.bottom - rect.top);

            var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, width, height);
            var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);
            NativeMethods.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, TernaryRasterOperations.SRCCOPY);
            NativeMethods.SelectObject(hdcDest, hOld);
            NativeMethods.DeleteDC(hdcDest);
            NativeMethods.ReleaseDC(_hWnd, hdcSrc);
            using var img = Image.FromHbitmap(hBitmap);
            NativeMethods.DeleteObject(hBitmap);

            using var bitmap = img.Clone(Rectangle.FromLTRB(0, 0, width, height), PixelFormat.Format32bppArgb);
            var bits = bitmap.LockBits(Rectangle.FromLTRB(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var data = new DataBox { DataPointer = bits.Scan0, RowPitch = bits.Width * 4, SlicePitch = bits.Height };
            var texture2d = new Texture2D(Device, CreateTexture2DDescription(width, height), new[] { data });

            bitmap.UnlockBits(bits);

            return texture2d;
        }
    }
}