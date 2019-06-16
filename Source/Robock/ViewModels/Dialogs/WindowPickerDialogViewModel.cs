using System;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using Robock.Extensions;
using Robock.Models;
using Robock.Mvvm;

namespace Robock.ViewModels.Dialogs
{
    internal class WindowPickerDialogViewModel : ViewModel, IDialogAware
    {
        private readonly CaptureSourceManager _captureSourceManager;
        public ReadOnlyReactiveCollection<CaptureSourceViewModel> CaptureSources { get; }
        public ReactiveProperty<Rect> RenderPosition { get; }
        public ReactiveProperty<CaptureSourceViewModel> SelectedSource { get; }
        public ReactiveCommand SelectCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        public WindowPickerDialogViewModel()
        {
            _captureSourceManager = new CaptureSourceManager().AddTo(this);
            CaptureSources = _captureSourceManager.CaptureSources.ToReadOnlyReactiveCollection(w => new CaptureSourceViewModel(w).AddTo(this)).AddTo(this);
            CaptureSources.ToCollectionChanged().Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(_ => UpdateChildDisplayPosition()).AddTo(this);
            RenderPosition = new ReactiveProperty<Rect>();
            RenderPosition.Where(w => !w.IsEmpty).Subscribe(_ => UpdateChildDisplayPosition()).AddTo(this);
            SelectedSource = new ReactiveProperty<CaptureSourceViewModel>();
            SelectCommand = new ReactiveCommand();
            SelectCommand.Subscribe(_ => RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "CaptureSource", SelectedSource.Value?.CaptureSource?.Clone() } }))).AddTo(this);
            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(_ => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel))).AddTo(this);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            CompositeDisposable.Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _captureSourceManager.FetchAll();
        }

        public string Title => "キャプチャーするウィンドウを選択 - Robock";

        public event Action<IDialogResult> RequestClose;

        private void UpdateChildDisplayPosition()
        {
            foreach (var vm in CaptureSources)
                vm.DisplayPosition.Value = RenderPosition.Value;
        }
    }
}