using Prism.Mvvm;

namespace Robock.Services
{
    public class StatusTextService : BindableBase
    {
        private static StatusTextService _instance;

        private string _status;
        public static StatusTextService Instance => _instance ?? (_instance = new StatusTextService());

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private StatusTextService()
        {
            //
        }
    }
}