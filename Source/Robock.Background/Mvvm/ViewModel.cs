using System;
using System.Reactive.Disposables;

using Prism.Mvvm;

namespace Robock.Background.Mvvm
{
    public class ViewModel : BindableBase, IDisposable
    {
        public CompositeDisposable CompositeDisposable { get; }

        protected ViewModel()
        {
            CompositeDisposable = new CompositeDisposable();
        }

        public virtual void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}