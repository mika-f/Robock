﻿using System;
using System.Collections.ObjectModel;
using System.Windows;

using Reactive.Bindings;

using Robock.Mvvm;
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
            SelectedIndex.Subscribe(w =>
            {
                if (w < 0 || w >= Desktops.Count)
                    return;
                foreach (var desktop in Desktops)
                    desktop.IsSelected.Value = false;
                Desktops[w].IsSelected.Value = true;
            });
        }
    }
}