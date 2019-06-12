using System;

using Robock.Interop.Win32;

using SharpDX.Direct3D11;

namespace Robock.Models.Renderer
{
    internal class SharedSurfaceRenderer : BaseRenderer
    {
        private readonly IntPtr _hWnd;

        public SharedSurfaceRenderer(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        protected override Texture2D TryGetNextFrameAsTexture2D()
        {
            NativeMethods.DwmGetDxSharedSurface(_hWnd, out var phSurface, out _, out _, out _, out _);
            return Device.OpenSharedResource<Texture2D>(phSurface);
        }
    }
}