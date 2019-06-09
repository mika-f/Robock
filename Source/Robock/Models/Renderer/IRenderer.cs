using System;

namespace Robock.Models.Renderer
{
    public interface IRenderer : IDisposable
    {
        void Initialize();

        void Render(IntPtr hSurface, bool isNewSurface);
    }
}