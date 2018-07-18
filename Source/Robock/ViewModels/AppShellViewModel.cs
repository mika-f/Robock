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

        public AppShellViewModel()
        {
            Title = new ReactiveProperty<string>("Robock");
            Tabs = new ReactiveCollection<TabViewModel>
            {
                new AboutTabViewModel()
            };

            var desktopManager = new DesktopManager();
            desktopManager.Desktops.CollectionChangedAsObservable().Subscribe(w =>
            {
                if (w.Action == NotifyCollectionChangedAction.Add && w.NewItems[0] is Desktop desktop)
                    Tabs.Insert(desktop.No - 1, new DesktopViewModel(desktop));
            });

            desktopManager.Initialize();
        }
    }
}