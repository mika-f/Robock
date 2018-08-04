#include "stdafx.h"
#include "DxRenderer.h"
#include "comdef.h"
#include <string>

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
    this->_indexBuffer = nullptr;
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
            return this->MsgBox(hr, L"Render#InitRenderTarget");
    }

    if (this->_shaderResourceView == nullptr)
    {
        IUnknown* resource;
        hr = this->_device->OpenSharedResource(phDwmSurface, __uuidof(ID3D11Resource), reinterpret_cast<void**>(&resource));
        if (FAILED(hr))
            return this->MsgBox(hr, L"Render#OpenSharedResource<ID3D11Resource>");

        ID3D11Texture2D* texture2D = nullptr;
        hr = resource->QueryInterface(__uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&texture2D));
        if (FAILED(hr))
            return this->MsgBox(hr, L"Render#QueryInterface<ID3D11Texture2D>");

        hr = this->_device->CreateShaderResourceView(texture2D, nullptr, &this->_shaderResourceView);
        if (FAILED(hr))
            return this->MsgBox(hr, L"Render#CreateShaderResourceView");
    }

    const float clearColor[4] = {0, 0, 0, 1}; // RGBA
    this->_deviceContext->ClearRenderTargetView(this->_renderTargetView, clearColor);

    this->_deviceContext->VSSetShader(this->_vertexShader, nullptr, 0);
    this->_deviceContext->PSSetShader(this->_pixelShader, nullptr, 0);

    this->_deviceContext->PSSetSamplers(0, 1, &this->_samplerState);
    this->_deviceContext->PSSetShaderResources(0, 1, &this->_shaderResourceView);

    this->_deviceContext->Draw(4, 0);

    if (this->_deviceContext != nullptr)
        this->_deviceContext->Flush();
    return S_OK;
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
    SAFE_RELEASE(this->_indexBuffer);
    SAFE_RELEASE(this->_samplerState);
    SAFE_RELEASE(this->_shaderResourceView);

    return S_OK;
}

HRESULT DxRenderer::Init()
{
    return this->InitDevice();
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
    const UINT drivers = sizeof featureLevels / sizeof featureLevels[0];

    for (UINT i = 0; i < 3; i++)
    {
        hr = D3D11CreateDevice(nullptr, driverTypes[i], nullptr, flags, featureLevels, drivers, D3D11_SDK_VERSION, &this->_device, &this->_featureLevel, &this->_deviceContext);
        if (SUCCEEDED(hr))
        {
            this->_driverType = driverTypes[i];
            break;
        }
    }
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitDevice#D3D11CreateDevice");

    hr = this->LoadShader();
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitDevice#LoadShader");

    return S_OK;
}

HRESULT DxRenderer::InitRenderTarget(void* pResource)
{
    auto pUnk = static_cast<IUnknown*>(pResource);
    IDXGIResource* pDXGIResource;
    auto hr = pUnk->QueryInterface(__uuidof(IDXGIResource), reinterpret_cast<void**>(&pDXGIResource));
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitRenderTarget#QueryInterface<IDXGIResource>");

    HANDLE sharedHandle;
    hr = pDXGIResource->GetSharedHandle(&sharedHandle);
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitRenderTarget#GetSharedHandle");

    IUnknown* resource;
    hr = this->_device->OpenSharedResource(sharedHandle, __uuidof(ID3D11Resource), reinterpret_cast<void**>(&resource));
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitRenderTarget#OpenSharedResource<ID3D11Resource>");

    ID3D11Texture2D* output;
    hr = resource->QueryInterface(__uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&output));
    if (FAILED(hr))
        return this->MsgBox(hr, L"InitRenderTarget#QueryInterface<ID3D11Texture2D>");

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
        D3D11_VIEWPORT viewport;
        this->_width = resourceDesc.Width;
        this->_height = resourceDesc.Height;
        viewport.Width = static_cast<float>(this->_width);
        viewport.Height = static_cast<float>(this->_height);
        viewport.MinDepth = 0.0f;
        viewport.MaxDepth = 1.0f;
        viewport.TopLeftX = 0;
        viewport.TopLeftY = 0;
        this->_deviceContext->RSSetViewports(1, &viewport);
    }

    this->_deviceContext->OMSetRenderTargets(1, &this->_renderTargetView, nullptr);
    if (output != nullptr)
        output->Release();

    return S_OK;
}

HRESULT DxRenderer::LoadShader()
{
    ID3DBlob* vsBlob;
    auto hr = this->CompileShaderFromFile(L"shader.hlsl", "VS", "vs_5_0", &vsBlob);
    if (FAILED(hr))
        return this->MsgBox(hr, L"LoadShader#CompileShaderFromFile<VS>");

    hr = this->_device->CreateVertexShader(vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), nullptr, &this->_vertexShader);
    if (FAILED(hr))
    {
        SAFE_RELEASE(vsBlob);
        return this->MsgBox(hr, L"LoadShader#CreateVertexShader");
    }

    D3D11_INPUT_ELEMENT_DESC layout[] =
    {
        {"POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
        {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0},
    };
    const UINT elements = sizeof layout / sizeof layout[0];

    hr = this->_device->CreateInputLayout(layout, elements, vsBlob->GetBufferPointer(), vsBlob->GetBufferSize(), &this->_vertexLayout);
    if (FAILED(hr))
        return this->MsgBox(hr, L"LoadShader#CreateInputLayout");

    this->_deviceContext->IASetInputLayout(this->_vertexLayout);

    ID3DBlob* psBlob;
    hr = this->CompileShaderFromFile(L"shader.hlsl", "PS", "ps_5_0", &psBlob);
    if (FAILED(hr))
        return this->MsgBox(hr, L"LoadShader#CompileShaderFromFile<PS>");

    hr = this->_device->CreatePixelShader(psBlob->GetBufferPointer(), psBlob->GetBufferSize(), nullptr, &this->_pixelShader);
    SAFE_RELEASE(vsBlob);
    if (FAILED(hr))
        return this->MsgBox(hr, L"LoadShader#CreatePixelShader");

    SimpleVertex vertices[] =
    {
        {XMFLOAT3(-1.0f, -1.0f, 1.0f), XMFLOAT2(0, 1)},
        {XMFLOAT3(-1.0f, 1.0f, 1.0f), XMFLOAT2(0, 0)},
        {XMFLOAT3(1.0f, -1.0f, 1.0f), XMFLOAT2(1, 1)},
        {XMFLOAT3(1.0f, 1.0f, 1.0f), XMFLOAT2(1, 0)}
    };

    D3D11_BUFFER_DESC bufferDesc;
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.ByteWidth = sizeof(SimpleVertex) * 4;
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bufferDesc.CPUAccessFlags = 0;
    bufferDesc.MiscFlags = 0;
    D3D11_SUBRESOURCE_DATA InitData;
    InitData.pSysMem = vertices;

    hr = this->_device->CreateBuffer(&bufferDesc, &InitData, &this->_vertexBuffer);
    if (FAILED(hr))
        return this->MsgBox(hr, L"LoadShader#CreateBuffer<Vertex>");

    const UINT stride = sizeof(SimpleVertex);
    const UINT offset = 0;
    this->_deviceContext->IASetVertexBuffers(0, 1, &this->_vertexBuffer, &stride, &offset);

    this->_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(D3D11_SAMPLER_DESC));
    samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    this->_device->CreateSamplerState(&samplerDesc, &this->_samplerState);

    return S_OK;
}

HRESULT DxRenderer::CompileShaderFromFile(const LPCWSTR pFileName, const LPCSTR pEntrypoint, const LPCSTR pTarget, ID3D10Blob** ppCode)
{
    ID3D10Blob* pErrorBlob;
    const auto hr = D3DCompileFromFile(pFileName, nullptr, nullptr, pEntrypoint, pTarget, 0, 0, ppCode, &pErrorBlob);
    SAFE_RELEASE(pErrorBlob);

    return hr;
}

HRESULT DxRenderer::MsgBox(const HRESULT hr, const LPCWSTR lpText)
{
    _com_error err(hr);
    const std::wstring base(lpText);
    const auto message = base + L"\r\nMessage: " + err.ErrorMessage();
    MessageBoxW(nullptr, message.c_str(), L"Robock.Native Internal Error", MB_OK | MB_ICONEXCLAMATION);
    return hr;
}