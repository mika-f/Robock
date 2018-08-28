#pragma once

#define DllExport  __declspec( dllexport )
#include "DxRenderer.h"

DxRenderer* renderer;

extern "C" {
DllExport HRESULT Init(int width, int height);
DllExport HRESULT Render(void* phWindowSurface, void* phDwmSurface, int x, int y, int width, int height, bool isNewSurface);
DllExport HRESULT Release();
}
