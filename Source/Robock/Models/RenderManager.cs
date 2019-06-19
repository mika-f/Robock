using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Robock.Models.Renderer;

namespace Robock.Models
{
    public class RenderManager
    {
        public ReadOnlyCollection<IRenderer> Renderers { get; }

        public RenderManager()
        {
            var renderers = new List<IRenderer>
            {
                new BitBltRenderer(),
                new GraphicsCaptureRenderer(),
                new SharedSurfaceRenderer()
            };
            Renderers = renderers.Where(w => w.IsSupported).OrderBy(w => w.Priority).ToList().AsReadOnly();
        }
    }
}