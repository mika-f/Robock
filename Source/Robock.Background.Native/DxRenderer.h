#pragma once
class DxRenderer
{
public:
    DxRenderer();
    HRESULT Render(void* phWindowSurface, void* phDwmSurface, int x, int y, int width, int height, bool isNewSurface);
    HRESULT Release();

    // internal
    HRESULT Init();

private:
    HRESULT InitDevice();
    HRESULT InitRenderTarget(void* pResource);
    HRESULT LoadShader();

    static HRESULT MsgBox(HRESULT hr, LPCWSTR lpText);

    void* _currentDwnSurface;
    int _screenWidth;
    int _screenHeight;
    int _textureWidth;
    int _textureHeight;

    D3D_DRIVER_TYPE _driverType{};
    D3D_FEATURE_LEVEL _featureLevel{};

    ID3D11Device* _device{};
    ID3D11DeviceContext* _deviceContext{};
    ID3D11RenderTargetView* _renderTargetView{};
    ID3D11InputLayout* _vertexLayout{};
    ID3D11Buffer* _vertexBuffer{};
    ID3D11VertexShader* _vertexShader{};
    ID3D11PixelShader* _pixelShader{};
    ID3D11SamplerState* _samplerState{};
    ID3D11ShaderResourceView* _shaderResourceView{};
    ID3D11Buffer* _constantBuffer{};
};

struct SimpleVertex
{
    DirectX::XMFLOAT3 Pos;
    DirectX::XMFLOAT2 Uv;
};

struct ConstantBuffer
{
    float Top;
    float Left;
    float Width;
    float Height;
};
