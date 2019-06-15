using System.ComponentModel;

namespace Robock.Services.Interfaces
{
    public interface IStatusService : INotifyPropertyChanged
    {
        string Status { get; set; }
    }
}