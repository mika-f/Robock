using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Extensions;
using Robock.Mvvm;
using Robock.Services.Interfaces;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class VirtualScreenViewModel : ViewModel
    {
        private const double Scale = 15;

        public ObservableCollection<DesktopViewModel> Desktops { get; }
        public ReactiveProperty<int> SelectedIndex { get; }

        public ReadOnlyReactiveProperty<double> VirtualScreenWidth { get; }
        public ReadOnlyReactiveProperty<double> VirtualScreenHeight { get; }

        public VirtualScreenViewModel(IDpiService dpiService)
        {
            Desktops = new ObservableCollection<DesktopViewModel>();
            SelectedIndex = new ReactiveProperty<int>(0);
            SelectedIndex.Where(w => 0 <= w).Subscribe(w =>
            {
                foreach (var desktop in Desktops)
                    desktop.IsSelected.Value = false;
                if (w >= Desktops.Count)
                    return;
                Desktops[w].IsSelected.Value = true;
            }).AddTo(this);
            VirtualScreenWidth = dpiService.ObserveProperty(w => w.CurrentDpi).Select(w => SystemParameters.VirtualScreenWidth / Scale * w.ScaleY).ToReadOnlyReactiveProperty().AddTo(this);
            VirtualScreenHeight = dpiService.ObserveProperty(w => w.CurrentDpi).Select(w => SystemParameters.VirtualScreenHeight / Scale * w.ScaleX).ToReadOnlyReactiveProperty().AddTo(this);
        }
    }
}