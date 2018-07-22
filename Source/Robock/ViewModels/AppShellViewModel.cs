﻿using System;
using System.Collections.Specialized;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Shared.Extensions;
using Robock.Shared.Models;
using Robock.Shared.Mvvm;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        private readonly DesktopWindowManager _desktopWindowManager;
        public ReactiveProperty<string> Title { get; }
        public ReactiveCollection<TabViewModel> Tabs { get; }
        public VirtualScreenViewModel VirtualScreen { get; }

        public AppShellViewModel()
        {
            var desktopManager = new DesktopManager();
            var processManager = new WindowManager().AddTo(this);
            _desktopWindowManager = new DesktopWindowManager().AddTo(this);

            Title = new ReactiveProperty<string>("Robock");
            Tabs = new ReactiveCollection<TabViewModel>
            {
                new AboutTabViewModel()
            };
            VirtualScreen = new VirtualScreenViewModel();

            // Subscribe
            desktopManager.Desktops.CollectionChangedAsObservable().Subscribe(w =>
            {
                if (w.Action != NotifyCollectionChangedAction.Add || !(w.NewItems[0] is Desktop desktop))
                    return;

                var viewModel = new DesktopViewModel(desktop, processManager, _desktopWindowManager).AddTo(this);
                Tabs.Insert(desktop.No - 1, viewModel);
                VirtualScreen.Desktops.Insert(desktop.No - 1, viewModel);
            }).AddTo(this);

            desktopManager.Initialize();
            processManager.Start();
        }

        public void Initialize()
        {
            _desktopWindowManager.Initialize();
        }
    }
}