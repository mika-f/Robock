using System;

using Robock.Models;
using Robock.Shared.Mvvm;

namespace Robock.ViewModels
{
    public class WindowViewModel : ViewModel
    {
        private readonly Window _window;
        public string Name => $"{_window.Title}";
        public IntPtr Handle => _window.Handle;

        public WindowViewModel(Window process)
        {
            _window = process;
        }
    }
}