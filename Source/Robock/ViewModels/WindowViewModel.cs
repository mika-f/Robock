using Robock.Models;
using Robock.Mvvm;

namespace Robock.ViewModels
{
    public class WindowViewModel : ViewModel
    {
        private readonly Window _window;
        public string Name => $"{_window.Title}";

        public WindowViewModel(Window process)
        {
            _window = process;
        }
    }
}