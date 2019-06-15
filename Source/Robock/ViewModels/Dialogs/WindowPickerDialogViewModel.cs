using System;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using Robock.Extensions;
using Robock.Mvvm;

namespace Robock.ViewModels.Dialogs
{
    internal class WindowPickerDialogViewModel : ViewModel, IDialogAware
    {
        public ReactiveCommand SelectCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public WindowPickerDialogViewModel()
        {
            SelectCommand = new ReactiveCommand();
            SelectCommand.Subscribe(w => { }).AddTo(this);
            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(_ => RequestClose?.Invoke(null)).AddTo(this);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            CompositeDisposable.Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters) { }

        public string Title => "キャプチャーするウィンドウを選択 - Robock";

        public event Action<IDialogResult> RequestClose;
    }
}