using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Extensions;
using Robock.Models;
using Robock.Mvvm;
using Robock.Services.Interfaces;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        public ReadOnlyReactiveProperty<string> Title { get; }
        public ReadOnlyReactiveProperty<string> Status { get; }
        public ReactiveCollection<TabViewModel> Tabs { get; }
        public VirtualScreenViewModel VirtualScreen { get; }

        public AppShellViewModel(IDialogService dialogService, IDpiService dpiService, IStatusService statusService)
        {
            var desktopManager = new DesktopManager().AddTo(this);

            Title = new ReactiveProperty<string>("Robock").ToReadOnlyReactiveProperty();
            Status = statusService.ObserveProperty(w => w.Status).ToReadOnlyReactiveProperty();
            Status.Throttle(TimeSpan.FromSeconds(30)).Subscribe(_ => statusService.Status = "準備完了").AddTo(this);
            Tabs = new ReactiveCollection<TabViewModel>
            {
                new AboutTabViewModel()
            };
            VirtualScreen = new VirtualScreenViewModel(dpiService);

            // Subscribe
            desktopManager.Desktops.CollectionChangedAsObservable().Subscribe(w =>
            {
                if (w.Action != NotifyCollectionChangedAction.Add || !(w.NewItems[0] is Desktop desktop))
                    return;

                var viewModel = new DesktopViewModel(desktop, dialogService).AddTo(this);
                Tabs.Insert(desktop.No - 1, viewModel);
                VirtualScreen.Desktops.Insert(desktop.No - 1, viewModel);
                if (desktop.IsPrimary)
                    VirtualScreen.SelectedIndex.Value = desktop.No - 1;
            }).AddTo(this);

            desktopManager.Initialize();
        }
    }
}