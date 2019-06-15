using System.ComponentModel;

using MetroRadiance.Interop;

namespace Robock.Services.Interfaces
{
    public interface IDpiService : INotifyPropertyChanged
    {
        Dpi CurrentDpi { get; }
    }
}