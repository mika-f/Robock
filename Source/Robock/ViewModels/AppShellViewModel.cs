using System;
using System.Collections.Specialized;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Mvvm;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        public ReactiveProperty<string> Title { get; }
        public ReactiveCollection<TabViewModel> Tabs { get; }
        public VirtualScreenViewModel VirtualScreen { get; }

        public AppShellViewModel()
        {
            Title = new ReactiveProperty<string>("Robock");
            Tabs = new ReactiveCollection<TabViewModel>
            {
                new AboutTabViewModel()
            };
            VirtualScreen = new VirtualScreenViewModel();

            var desktopManager = new DesktopManager();
            var processManager = new WindowManager();

            // Subscribe
            desktopManager.Desktops.CollectionChangedAsObservable().Subscribe(w =>
            {
                if (w.Action != NotifyCollectionChangedAction.Add || !(w.NewItems[0] is Desktop desktop))
                    return;

                var viewModel = new DesktopViewModel(desktop, processManager);
                Tabs.Insert(desktop.No - 1, viewModel);
                VirtualScreen.Desktops.Insert(desktop.No - 1, viewModel);
            });

            desktopManager.Initialize();
            processManager.Start();

            CompositeDisposable.Add(processManager);
        }
    }
}