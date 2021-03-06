﻿using System;
using System.Reactive.Linq;

using Robock.Background.Extensions;
using Robock.Background.Models;
using Robock.Background.Mvvm;

namespace Robock.Background.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        public AppShellViewModel()
        {
            Observable.Return(0).Delay(TimeSpan.FromMilliseconds(500)).Subscribe(_ =>
            {
                var server = new RobockServer().AddTo(this);
                server.Start();
            });
        }
    }
}