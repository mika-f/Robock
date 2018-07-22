using System;
using System.Reactive.Disposables;

using Prism.Mvvm;

namespace Robock.Shared.Mvvm
{
    public class ViewModel : BindableBase, IDisposable
    {
        public CompositeDisposable CompositeDisposable { get; }

        public ViewModel()
        {
            CompositeDisposable = new CompositeDisposable();
        }

        public virtual void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}