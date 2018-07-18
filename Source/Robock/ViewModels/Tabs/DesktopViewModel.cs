using Robock.Models;

namespace Robock.ViewModels.Tabs
{
    public class DesktopViewModel : TabViewModel
    {
        private readonly Desktop _desktop;

        public bool IsPrimary => _desktop.IsPrimary;

        public DesktopViewModel(Desktop desktop) : base($":Desktop: Desktop {desktop.No}")
        {
            _desktop = desktop;
        }
    }
}