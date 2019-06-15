using System.Diagnostics;
using System.Windows.Input;

using Prism.Commands;

using Robock.Models;
using Robock.Mvvm;

namespace Robock.ViewModels
{
    internal class LicenseViewModel : ViewModel
    {
        private readonly License _license;

        public string Url => _license.Url.Replace("https://", "");
        public string Name => _license.Name;
        public string Authors => string.Join(", ", _license.Authors);
        public bool IsShowAuthors => !string.IsNullOrWhiteSpace(Authors);
        public string LicenseBody => _license.Body.Substring(2); // First character must be NewLine

        public LicenseViewModel(License license)
        {
            _license = license;
        }

        #region OpenHyperlinkCommand

        private ICommand _openHyperlinkCommand;

        public ICommand OpenHyperlinkCommand => _openHyperlinkCommand ??= new DelegateCommand(OpenHyperlink);

        private void OpenHyperlink()
        {
            Process.Start(_license.Url);
        }

        #endregion
    }
}