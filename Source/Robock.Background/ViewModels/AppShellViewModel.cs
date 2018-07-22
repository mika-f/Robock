using System;
using System.Reactive.Linq;

using Robock.Shared.Communication;
using Robock.Shared.Extensions;
using Robock.Shared.Mvvm;

namespace Robock.Background.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        private RobockServer _server;

        public AppShellViewModel()
        {
            Observable.Return(0).Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                _server = new RobockServer().AddTo(this);
                _server.Start();
            });
        }
    }
}