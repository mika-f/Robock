using System;

using Robock.Interop.Win32;
using Robock.Models.CaptureSources;

using SharpDX.Direct3D11;

namespace Robock.Models.Renderer
{
    internal class SharedSurfaceRenderer : RendererBase
    {
        private IntPtr _hWnd;
        public override string Name => "DWM Shared Surface";
        public override uint Priority => 3;
        public override bool IsSupported => true;
        public override bool HasOwnWindowPicker => false;

        public override void ConfigureCaptureSource(params object[] parameters)
        {
            if (parameters.Length != 1)
                throw new InvalidOperationException();
            _hWnd = (IntPtr) parameters[0];
        }

        public override ICaptureSource ShowWindowPicker()
        {
            throw new InvalidOperationException();
        }

        public override void ReleaseCaptureSource()
        {
            _hWnd = IntPtr.Zero;
        }

        protected override Texture2D TryGetNextFrameAsTexture2D()
        {
            NativeMethods.DwmGetDxSharedSurface(_hWnd, out var phSurface, out _, out _, out _, out _);
            return Device.OpenSharedResource<Texture2D>(phSurface);
        }
    }
}