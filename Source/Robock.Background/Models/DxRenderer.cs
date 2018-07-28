using System;

namespace Robock.Background.Models
{
    public class DxRenderer : IDisposable
    {
        public void Dispose() { }

        // 基本 hSurface, phSurface は変わらないと思われ
        public void Render(IntPtr hSurface, IntPtr phSurface, int width, int height, bool isNewSurface) { }
    }
}