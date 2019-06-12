﻿using System;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.DXGI.Resource;

namespace Robock.Models.Renderer
{
    internal abstract class BaseRenderer : IRenderer
    {
        private Device _device;
        private InputLayout _layout;
        private PixelShader _pixelShader;
        private RenderTargetView _renderTargetView;
        private ShaderResourceView _shaderResourceView;
        private Texture2D _texture2D;
        private Buffer _vertexes;
        private VertexShader _vertexShader;

        protected Device Device => _device;

        public void Dispose()
        {
            Utilities.Dispose(ref _renderTargetView);
            Utilities.Dispose(ref _shaderResourceView);
            Utilities.Dispose(ref _texture2D);
            Utilities.Dispose(ref _vertexes);
            Utilities.Dispose(ref _layout);
            Utilities.Dispose(ref _pixelShader);
            Utilities.Dispose(ref _vertexShader);

            _device.ImmediateContext.ClearState();
            _device.ImmediateContext.Dispose();
            Utilities.Dispose(ref _device);
        }

        public void Initialize()
        {
            var featureLevels = new[] { FeatureLevel.Level_12_1, FeatureLevel.Level_12_0, FeatureLevel.Level_11_1, FeatureLevel.Level_11_0 };
            _device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, featureLevels);

            using var vertexShaderByteCode = ShaderBytecode.CompileFromFile("./Shader.fx", "VS", "vs_5_0");
            _vertexShader = new VertexShader(_device, vertexShaderByteCode);

            using var pixelShaderByteCode = ShaderBytecode.CompileFromFile("./Shader.fx", "PS", "ps_5_0");
            _pixelShader = new PixelShader(_device, pixelShaderByteCode);

            _layout = new InputLayout(_device, vertexShaderByteCode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
            });

            _vertexes = Buffer.Create(_device, BindFlags.VertexBuffer, new[]
            {
                new Vertex { Position = new RawVector3(-1.0f, 1.0f, 0.5f), TexCoord = new RawVector2(0.0f, 0.0f) },
                new Vertex { Position = new RawVector3(1.0f, 1.0f, 0.5f), TexCoord = new RawVector2(1.0f, 0.0f) },
                new Vertex { Position = new RawVector3(-1.0f, -1.0f, 0.5f), TexCoord = new RawVector2(0.0f, 1.0f) },
                new Vertex { Position = new RawVector3(1.0f, -1.0f, 0.5f), TexCoord = new RawVector2(1.0f, 1.0f) }
            });

            var samplerStateDescription = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear
            };

            _device.ImmediateContext.InputAssembler.InputLayout = _layout;
            _device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            _device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexes, Utilities.SizeOf<Vertex>(), 0));
            _device.ImmediateContext.VertexShader.Set(_vertexShader);
            _device.ImmediateContext.PixelShader.SetSampler(0, new SamplerState(_device, samplerStateDescription));
            _device.ImmediateContext.PixelShader.Set(_pixelShader);
        }

        public void Render(IntPtr hSurface, bool isNewSurface)
        {
            if (isNewSurface)
            {
                _device.ImmediateContext.OutputMerger.SetRenderTargets(null, (RenderTargetView) null);
                InitializeRenderTarget(hSurface);
            }

            using var texture2d = TryGetNextFrameAsTexture2D();
            if (texture2d == null)
                return;

            if (_texture2D == null || _texture2D.Description.Height != texture2d.Description.Height || _texture2D.Description.Width != texture2d.Description.Width)
            {
                // update shader resource view
                Utilities.Dispose(ref _texture2D);
                Utilities.Dispose(ref _shaderResourceView);

                _texture2D = new Texture2D(_device, CreateTexture2DDescription(texture2d.Description.Width, texture2d.Description.Height));
                _shaderResourceView = new ShaderResourceView(_device, _texture2D);
                _device.ImmediateContext.PixelShader.SetShaderResource(0, _shaderResourceView);
            }

            _device.ImmediateContext.ClearRenderTargetView(_renderTargetView, new RawColor4(1, 1, 1, 1));
            _device.ImmediateContext.CopyResource(texture2d, _texture2D);

            _device.ImmediateContext.Draw(4, 0);
            _device.ImmediateContext.Flush();
        }

        private void InitializeRenderTarget(IntPtr hSurface)
        {
            using var resource = ComObject.QueryInterfaceOrNull<Resource>(hSurface);
            using var texture2D = _device.OpenSharedResource<Texture2D>(resource.SharedHandle);

            var renderTargetViewDescription = new RenderTargetViewDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                Dimension = RenderTargetViewDimension.Texture2D,
                Texture2D = { MipSlice = 0 }
            };
            _renderTargetView = new RenderTargetView(_device, texture2D, renderTargetViewDescription);

            _device.ImmediateContext.Rasterizer.SetViewport(0, 0, texture2D.Description.Width, texture2D.Description.Height);
            _device.ImmediateContext.OutputMerger.SetRenderTargets(_renderTargetView);
        }

        protected Texture2DDescription CreateTexture2DDescription(int width, int height)
        {
            return new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Width = width,
                Usage = ResourceUsage.Default
            };
        }

        protected abstract Texture2D TryGetNextFrameAsTexture2D();
    }

    internal struct Vertex
    {
        public RawVector3 Position;
        public RawVector2 TexCoord;
    }
}