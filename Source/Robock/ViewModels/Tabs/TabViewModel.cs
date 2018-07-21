using Robock.Shared.Mvvm;

namespace Robock.ViewModels.Tabs
{
    public class TabViewModel : ViewModel
    {
        public string Title { get; }

        protected TabViewModel(string title)
        {
            Title = title;
        }
    }
}