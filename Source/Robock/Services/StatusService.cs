using Prism.Mvvm;

using Robock.Services.Interfaces;

namespace Robock.Services
{
    internal class StatusService : BindableBase, IStatusService
    {
        private string _status;

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }
}