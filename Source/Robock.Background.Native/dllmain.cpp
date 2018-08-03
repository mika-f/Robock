#include "stdafx.h"
#include "DxRenderer.h"
#include "dllmain.h"

HRESULT Init()
{
    renderer = new DxRenderer;
    renderer->Init();
    return S_OK;
}

HRESULT Render(void* phWindowSurface, void* phDwmSurface, const int x, const int y, const int width, const int height, const bool isNewSurface)
{
    if (renderer == nullptr)
        return E_FAIL;
    return renderer->Render(phWindowSurface, phDwmSurface, x, y, width, height, isNewSurface);
}

HRESULT Release()
{
    if (renderer != nullptr)
    {
        renderer->Release();
        renderer = nullptr;
    }

    return S_OK;
}