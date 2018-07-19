using System.Collections.ObjectModel;
using System.Windows;

using Robock.Mvvm;
using Robock.ViewModels.Tabs;

namespace Robock.ViewModels
{
    public class VirtualScreenViewModel : ViewModel
    {
        private const double Scale = 15;

        public ObservableCollection<DesktopViewModel> Desktops { get; }

        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth / Scale;
        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight / Scale;

        public VirtualScreenViewModel()
        {
            Desktops = new ObservableCollection<DesktopViewModel>();
        }
    }
}