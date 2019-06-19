using System;
using System.ComponentModel;
using System.Windows;

namespace Robock.Models.CaptureSources
{
    public class InteropWindow : ICaptureSource
    {
        public InteropWindow(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            // nothing to do
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }
        public bool IsAvailablePreview => false;
        public int Height => 0;
        public int Width => 0;

        public void RenderThumbnail(IntPtr hDest, Rect position, Rect available)
        {
            // do not called in this source
        }

        public object[] RenderParameters()
        {
            return new object[] { };
        }

        public ICaptureSource Clone()
        {
            return new InteropWindow(Name);
        }
    }
}