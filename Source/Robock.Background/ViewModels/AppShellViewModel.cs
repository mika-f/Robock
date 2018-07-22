using System;
using System.Reactive.Linq;

using Robock.Background.Models;
using Robock.Shared.Extensions;
using Robock.Shared.Mvvm;

namespace Robock.Background.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        public AppShellViewModel()
        {
            BackgroundService.SaveCurrentWallpaper();

            Observable.Return(0).Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                var server = new RobockServer().AddTo(this);
                server.Start();
            });
        }
    }
}