using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;

using Robock.Shared.Mvvm;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class VirtualScreenViewModel : ViewModel
    {
        private const double Scale = 15;

        public ObservableCollection<DesktopViewModel> Desktops { get; }
        public ReactiveProperty<int> SelectedIndex { get; }

        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth / Scale;
        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight / Scale;

        public VirtualScreenViewModel()
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
            });
        }
    }
}