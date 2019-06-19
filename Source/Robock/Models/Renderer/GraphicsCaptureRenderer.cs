using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Windows.Foundation.Metadata;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

using Robock.Interop.Com;
using Robock.Interop.Win32;
using Robock.Models.CaptureSources;

using SharpDX;
using SharpDX.Direct3D11;

namespace Robock.Models.Renderer
{
    internal class GraphicsCaptureRenderer : RendererBase
    {
        private readonly Lazy<bool> _isSupported;
        private Direct3D11CaptureFramePool _captureFramePool;
        private GraphicsCaptureItem _captureItem;
        private GraphicsCaptureSession _captureSession;
        public override string Name => "UWP Interop";
        public override uint Priority => 1;
        public override bool IsSupported => _isSupported.Value;
        public override bool HasOwnWindowPicker => true;

        public GraphicsCaptureRenderer()
        {
            _isSupported = new Lazy<bool>(() => ApiInformation.IsTypePresent("Windows.Graphics.Capture.GraphicsCapturePicker"));
        }

        public override void ConfigureCaptureSource(params object[] parameters)
        {
            var hr = NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(Device.NativePointer, out var pUnknown);
            if (hr != 0)
                throw new InvalidOperationException();

            var winrtDevice = (IDirect3DDevice) Marshal.GetObjectForIUnknown(pUnknown);
            Marshal.Release(pUnknown);

            _captureFramePool = Direct3D11CaptureFramePool.Create(winrtDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, _captureItem.Size);
            _captureSession = _captureFramePool.CreateCaptureSession(_captureItem);
            _captureSession.StartCapture();
        }

        public override ICaptureSource ShowWindowPicker()
        {
            var owner = new WindowInteropHelper(Application.Current.MainWindow ?? throw new InvalidOperationException()).Handle;
            var capturePicker = new GraphicsCapturePicker();

            // ReSharper disable once PossibleInvalidCastException
            // ReSharper disable once SuspiciousTypeConversion.Global
            var initializer = (IInitializeWithWindow) (object) capturePicker;

            initializer.Initialize(owner);
            _captureItem = capturePicker.PickSingleItemAsync().AsTask().Result;
            return _captureItem == null ? null : new InteropWindow(_captureItem.DisplayName);
        }

        protected override Texture2D TryGetNextFrameAsTexture2D()
        {
            using var frame = _captureFramePool?.TryGetNextFrame();
            if (frame == null)
                return null;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var surfaceDxgiInterfaceAccess = (IDirect3DDxgiInterfaceAccess) frame.Surface;
            var pResource = surfaceDxgiInterfaceAccess.GetInterface(new Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d"));

            using var surfaceTexture = new Texture2D(pResource); // shared resource
            var texture2d = new Texture2D(Device, CreateTexture2DDescription(surfaceTexture.Description.Width, surfaceTexture.Description.Height));
            Device.ImmediateContext.CopyResource(surfaceTexture, texture2d);

            return texture2d;
        }

        protected override void ReleaseInternal()
        {
            Utilities.Dispose(ref _captureSession);
            Utilities.Dispose(ref _captureFramePool);
            _captureItem = null;
        }
    }
}