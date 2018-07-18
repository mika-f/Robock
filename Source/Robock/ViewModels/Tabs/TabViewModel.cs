using Robock.Mvvm;

namespace Robock.ViewModels.Tabs
{
    public class TabViewModel : ViewModel
    {
        public string Title { get; }

        public TabViewModel(string title)
        {
            Title = title;
        }
    }
}