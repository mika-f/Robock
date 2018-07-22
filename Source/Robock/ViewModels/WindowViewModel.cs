using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Shared.Extensions;
using Robock.Shared.Mvvm;

namespace Robock.ViewModels
{
    public class WindowViewModel : ViewModel
    {
        private readonly Window _window;
        public ReactiveProperty<string> Title { get; }
        public IntPtr Handle => _window.Handle;

        public WindowViewModel(Window window)
        {
            _window = window;
            Title = window.ObserveProperty(w => w.Title).ToReactiveProperty().AddTo(this);
        }
    }
}