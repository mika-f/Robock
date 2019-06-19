using System;
using System.ComponentModel;
using System.Windows;

namespace Robock.Models.CaptureSources
{
    public interface ICaptureSource : IDisposable, INotifyPropertyChanged
    {
        string Name { get; }

        bool IsAvailablePreview { get; }

        int Height { get; }

        int Width { get; }

        void RenderThumbnail(IntPtr hDest, Rect position, Rect available);

        object[] RenderParameters();

        // Window 間で同じインスタンス共有できない (Dispose される)
        ICaptureSource Clone();
    }
}