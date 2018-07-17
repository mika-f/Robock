using System;
using System.Reactive.Disposables;

using Prism.Mvvm;

namespace Robock.Mvvm
{
    public class ViewModel : BindableBase, IDisposable
    {
        public CompositeDisposable CompositeDisposable { get; }

        public ViewModel()
        {
            CompositeDisposable = new CompositeDisposable();
        }

        public void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}