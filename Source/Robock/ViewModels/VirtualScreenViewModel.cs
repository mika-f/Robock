using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Services;
using Robock.Shared.Extensions;
using Robock.Shared.Mvvm;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class VirtualScreenViewModel : ViewModel
    {
        private const double Scale = 15;

        public ObservableCollection<DesktopViewModel> Desktops { get; }
        public ReactiveProperty<int> SelectedIndex { get; }

        public ReactiveProperty<double> VirtualScreenWidth { get; }
        public ReactiveProperty<double> VirtualScreenHeight { get; }

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
            }).AddTo(this);
            VirtualScreenWidth = new ReactiveProperty<double>(SystemParameters.VirtualScreenWidth / Scale * DpiService.Instance.CurrentDpi.ScaleY);
            VirtualScreenHeight = new ReactiveProperty<double>(SystemParameters.VirtualScreenHeight / Scale * DpiService.Instance.CurrentDpi.ScaleX);
            DpiService.Instance.ObserveProperty(w => w.CurrentDpi).Subscribe(w =>
            {
                VirtualScreenWidth.Value = SystemParameters.VirtualScreenWidth / Scale * DpiService.Instance.CurrentDpi.ScaleY;
                VirtualScreenHeight.Value = SystemParameters.VirtualScreenHeight / Scale * DpiService.Instance.CurrentDpi.ScaleX;
            }).AddTo(this);
        }
    }
}