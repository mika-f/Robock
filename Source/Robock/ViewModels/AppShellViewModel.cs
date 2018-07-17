using Reactive.Bindings;

using Robock.Mvvm;

namespace Robock.ViewModels
{
    public class AppShellViewModel : ViewModel
    {
        public ReactiveProperty<string> Title { get; }

        public AppShellViewModel()
        {
            Title = new ReactiveProperty<string>("Robock");
        }
    }
}