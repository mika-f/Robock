// dllmain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#include "stdafx.h"
#include "DxRenderer.h"
#include "dllmain.h"

HRESULT Init(const int width, const int height)
{
    renderer = new DxRenderer;
    return renderer->Init(width, height);
}

HRESULT Render(void* phWindowSurface, void* phDwmSurface, const int x, const int y, const int width, const int height, const bool isNewSurface)
{
    if (renderer == nullptr)
    {
        MessageBoxW(nullptr, L"You must initialize Robock.Native", L"Robock.Native Internal Error", MB_OK | MB_ICONERROR);
        return E_FAIL;
    }
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