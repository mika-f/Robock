using System;
using System.Windows;

using Windows.Graphics.Capture;

using Prism.Mvvm;

namespace Robock.Models.CaptureSources
{
    public class InteropWindow : BindableBase, ICaptureSource
    {
        private readonly GraphicsCaptureItem _captureItem;

        public InteropWindow(GraphicsCaptureItem captureItem)
        {
            _captureItem = captureItem;
        }

        public void Dispose()
        {
            // nothing to do
        }

        public string Name => _captureItem.DisplayName;
        public bool IsAvailablePreview => false;
        public int Height => 0;
        public int Width => 0;

        public void RenderThumbnail(IntPtr hDest, Rect position, Rect available)
        {
            // do not called in this source
        }

        public object[] RenderParameters()
        {
            return new object[] { _captureItem };
        }

        public ICaptureSource Clone()
        {
            return new InteropWindow(_captureItem);
        }
    }
}