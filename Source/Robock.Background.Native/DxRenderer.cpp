#include "stdafx.h"
#include "DxRenderer.h"

using namespace DirectX;
using namespace PackedVector;

DxRenderer::DxRenderer()
{
    this->_width = 0;
    this->_height = 0;
    this->_driverType = D3D_DRIVER_TYPE_NULL;
    this->_featureLevel = D3D_FEATURE_LEVEL_11_0;
    this->_device = nullptr;
    this->_deviceContext = nullptr;
    this->_renderTargetView = nullptr;
    this->_vertexLayout = nullptr;
    this->_vertexBuffer = nullptr;
    this->_vertexShader = nullptr;
    this->_pixelShader = nullptr;
    this->_samplerState = nullptr;
    this->_shaderResourceView = nullptr;
}

HRESULT DxRenderer::Render(void* phWindowSurface, void* phDwmSurface, const int x, const int y, const int width, const int height, const bool isNewSurface)
{
    auto hr = S_OK;

    if (isNewSurface)
    {
        this->_deviceContext->OMSetRenderTargets(0, nullptr, nullptr);
        hr = this->InitRenderTarget(phWindowSurface);

        if (FAILED(hr))
        {
            MessageBoxW(nullptr, L"Failed to initialize a render target", L"Robock.Native Internal Error", MB_OK | MB_ICONEXCLAMATION);
            return hr;
        }
    }
    if (this->_deviceContext != nullptr)
        this->_deviceContext->Flush();
    return hr;
}

HRESULT DxRenderer::Release()
{
    this->_width = 0;
    this->_height = 0;
    this->_driverType = D3D_DRIVER_TYPE_NULL;
    this->_featureLevel = D3D_FEATURE_LEVEL_11_0;
    SAFE_RELEASE(this->_device);
    SAFE_RELEASE(this->_deviceContext);
    SAFE_RELEASE(this->_renderTargetView);
    SAFE_RELEASE(this->_vertexLayout);
    SAFE_RELEASE(this->_vertexBuffer);
    SAFE_RELEASE(this->_vertexShader);
    SAFE_RELEASE(this->_pixelShader);
    SAFE_RELEASE(this->_samplerState);
    SAFE_RELEASE(this->_shaderResourceView);

    return S_OK;
}

HRESULT DxRenderer::Init()
{
    const auto hr = this->InitDevice();
    if (FAILED(hr))
    {
        MessageBoxW(nullptr, L"Failed to create a new device", L"Robock.Native Internal Error", MB_OK | MB_ICONEXCLAMATION);
        return hr;
    }

    return hr;
}

HRESULT DxRenderer::SetViewport(const int width, const int height)
{
    this->_width = width;
    this->_height = height;

    return S_OK;
}

HRESULT DxRenderer::InitDevice()
{
    auto hr = S_OK;

    const UINT flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

    D3D_DRIVER_TYPE driverTypes[] = {
        D3D_DRIVER_TYPE_HARDWARE,
        D3D_DRIVER_TYPE_WARP,
        D3D_DRIVER_TYPE_REFERENCE,
    };

    D3D_FEATURE_LEVEL featureLevels[] = {
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
    };

    for (UINT i = 0; i < 3; i++)
    {
        hr = D3D11CreateDevice(nullptr, driverTypes[i], nullptr, flags, featureLevels, 3, D3D11_SDK_VERSION, &this->_device, &this->_featureLevel, &this->_deviceContext);
        if (SUCCEEDED(hr))
        {
            this->_driverType = driverTypes[i];
            break;
        }
    }

    if (FAILED(hr))
        return hr;

    hr = this->LoadShader();
    if (FAILED(hr))
    {
        MessageBoxW(nullptr, L"Failed to load a shader", L"Robock.Native Internal Error", MB_OK | MB_ICONEXCLAMATION);
        return hr;
    }

    SimpleVertex vertices[] = {
    };

    D3D11_BUFFER_DESC bufferDesc;
    ZeroMemory(&bufferDesc, sizeof bufferDesc);
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.ByteWidth = sizeof(SimpleVertex) * 4;
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bufferDesc.CPUAccessFlags = 0;
    bufferDesc.MiscFlags = 0;

    D3D11_SUBRESOURCE_DATA initData;
    ZeroMemory(&initData, sizeof initData);
    initData.pSysMem = vertices;

    hr = this->_device->CreateBuffer(&bufferDesc, &initData, &this->_vertexBuffer);
    if (FAILED(hr))
        return hr;

    UINT stride = sizeof(SimpleVertex);
    UINT offset = 0;
    this->_deviceContext->IASetVertexBuffers(0, 1, &this->_vertexBuffer, &stride, &offset);

    this->_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof samplerDesc);
    samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;

    this->_device->CreateSamplerState(&samplerDesc, &this->_samplerState);

    return hr;
}

HRESULT DxRenderer::InitRenderTarget(void* pResource)
{
    auto pUnk = static_cast<IUnknown*>(pResource);
    IDXGIResource* pDXGIResource;
    auto hr = pUnk->QueryInterface(__uuidof(IDXGIResource), reinterpret_cast<void**>(&pDXGIResource));
    if (FAILED(hr))
        return hr;

    HANDLE sharedHandle;
    hr = pDXGIResource->GetSharedHandle(&sharedHandle);
    if (FAILED(hr))
        return hr;

    IUnknown* resource;
    hr = this->_device->OpenSharedResource(sharedHandle, __uuidof(ID3D11Resource), reinterpret_cast<void**>(&resource));
    if (FAILED(hr))
        return hr;

    ID3D11Texture2D* output;
    hr = resource->QueryInterface(__uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&output));
    if (FAILED(hr))
        return hr;

    resource->Release();

    D3D11_RENDER_TARGET_VIEW_DESC renderTargetDesc;
    renderTargetDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    renderTargetDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
    renderTargetDesc.Texture2D.MipSlice = 0;

    hr = this->_device->CreateRenderTargetView(output, &renderTargetDesc, &this->_renderTargetView);
    if (FAILED(hr))
        return hr;

    D3D11_TEXTURE2D_DESC resourceDesc;
    output->GetDesc(&resourceDesc);
    if (resourceDesc.Width != this->_width || resourceDesc.Height != this->_height)
    {
        this->SetViewport(resourceDesc.Width, resourceDesc.Height);

        D3D11_VIEWPORT viewport;
        viewport.Width = this->_width;
        viewport.Height = this->_height;
        viewport.MinDepth = 0.0f;
        viewport.MaxDepth = 1.0f;
        viewport.TopLeftX = 0;
        viewport.TopLeftY = 0;
        this->_deviceContext->RSSetViewports(1, &viewport);
    }

    this->_deviceContext->OMSetRenderTargets(1, &this->_renderTargetView, nullptr);
    if (output != nullptr)
        output->Release();

    return hr;
}

HRESULT DxRenderer::LoadShader()
{
    ID3DBlob* vsBlob = nullptr;
    auto hr = this->CompileShaderFromFile(L"shader.hlsl", "VS", "vs_5_0", &vsBlob);
    if (FAILED(hr))
        return hr;

    hr = this->_device->CreateVertexShader(vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), nullptr, &this->_vertexShader);
    if (FAILED(hr))
    {
        vsBlob->Release();
        return hr;
    }

    ID3DBlob* psBlob = nullptr;
    hr = this->CompileShaderFromFile(L"shader.hlsl", "PS", "ps_5_0", &psBlob);
    if (FAILED(hr))
        return hr;

    hr = this->_device->CreatePixelShader(psBlob->GetBufferPointer(), psBlob->GetBufferSize(), nullptr, &this->_pixelShader);
    psBlob->Release();
    if (FAILED(hr))
        return hr;

    D3D11_INPUT_ELEMENT_DESC layout[] =
    {
        {"POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
        {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0},
    };

    hr = this->_device->CreateInputLayout(layout, 2, vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), &this->_vertexLayout);
    vsBlob->Release();
    if (FAILED(hr))
        return hr;

    this->_deviceContext->IASetInputLayout(this->_vertexLayout);
    return hr;
}

HRESULT DxRenderer::CompileShaderFromFile(const LPCWSTR pFileName, const LPCSTR pEntrypoint, const LPCSTR pTarget, ID3D10Blob** ppCode)
{
    ID3D10Blob* pErrorBlob;
    const auto hr = D3DCompileFromFile(pFileName, nullptr, nullptr, pEntrypoint, pTarget, 0, 0, ppCode, &pErrorBlob);
    SAFE_RELEASE(pErrorBlob);

    return hr;
}