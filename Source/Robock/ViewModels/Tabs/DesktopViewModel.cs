using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Extensions;
using Robock.Models;
using Robock.Models.CaptureSources;
using Robock.Models.Renderer;
using Robock.Views.Dialogs;

namespace Robock.ViewModels.Tabs
{
    public class DesktopViewModel : TabViewModel
    {
        private const double Scale = 15;

        private readonly Desktop _desktop;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public string DesktopName => $"Desktop {_desktop.No}";
        public double Width => _desktop.Width;
        public double Height => _desktop.Height;
        public string Resolution => $"{_desktop.Width}x{_desktop.Height}";
        public double VirtualScreenX => (_offsetX + _desktop.X) / Scale;
        public double VirtualScreenY => (_offsetY + _desktop.Y) / Scale;
        public double VirtualScreenHeight => _desktop.Height / Scale;
        public double VirtualScreenWidth => _desktop.Width / Scale;

        public ReactiveProperty<bool> IsSelected { get; }
        public ReactiveProperty<ICaptureSource> CaptureSource { get; }
        public ReadOnlyReactiveProperty<bool> IsCaptureSourceSelected { get; }
        public ReactiveProperty<double> PreviewHeight { get; set; }
        public ReactiveProperty<double> PreviewWidth { get; set; }
        public ReactiveProperty<double> RenderTop { get; set; }
        public ReactiveProperty<double> RenderLeft { get; set; }
        public ReactiveProperty<double> RenderHeight { get; }
        public ReactiveProperty<double> RenderWidth { get; }
        public ReactiveProperty<double> RenderScale { get; }
        public ObservableCollection<IRenderer> Renderers { get; }
        public ReactiveProperty<IRenderer> SelectedRenderer { get; }
        public ReadOnlyReactiveProperty<string> Wallpaper { get; }

        public ReactiveCommand ApplyWallpaperCommand { get; }
        public ReactiveCommand DiscardWallpaperCommand { get; }
        public ReactiveCommand SelectCaptureSourceCommand { get; }
        public ReactiveCommand ClearSelectCommand { get; }

        public DesktopViewModel(Desktop desktop, IDialogService dialogService, RenderManager renderManager) : base($":Desktop: Desktop {desktop.No}")
        {
            _desktop = desktop;
            _offsetX = (SystemParameters.VirtualScreenLeft < 0 ? -1 : 1) * SystemParameters.VirtualScreenLeft;
            _offsetY = (SystemParameters.VirtualScreenTop < 0 ? -1 : 1) * SystemParameters.VirtualScreenTop;

            PreviewHeight = new ReactiveProperty<double>();
            PreviewWidth = new ReactiveProperty<double>();
            RenderTop = new ReactiveProperty<double>(-2.5);
            RenderLeft = new ReactiveProperty<double>(-2.5);
            RenderHeight = new ReactiveProperty<double>();
            RenderWidth = new ReactiveProperty<double>();
            RenderScale = new[]
            {
                PreviewHeight,
                PreviewWidth
            }.CombineLatest()
             .Where(w => Math.Abs(_desktop.Height / w[0] - _desktop.Width / w[1]) <= 0)
             .Select(w => _desktop.Height / w[0])
             .ToReactiveProperty().AddTo(this);
            Wallpaper = _desktop.ObserveProperty(w => w.Wallpaper).ToReadOnlyReactiveProperty().AddTo(this);
            IsSelected = new ReactiveProperty<bool>(desktop.IsPrimary);
            CaptureSource = new ReactiveProperty<ICaptureSource>();
            CaptureSource.Zip(CaptureSource.Skip(1), (x, _) => x).Subscribe(_ => SelectedRenderer.Value?.ReleaseCaptureSource()).AddTo(this);
            CaptureSource.Where(w => w != null).Subscribe(w => SelectedRenderer.Value.ConfigureCaptureSource(w.RenderParameters())).AddTo(this);
            IsCaptureSourceSelected = CaptureSource.Select(w => w != null).ToReadOnlyReactiveProperty().AddTo(this);
            Renderers = new ObservableCollection<IRenderer>(renderManager.Renderers);
            SelectedRenderer = new ReactiveProperty<IRenderer>(renderManager.Renderers.First());
            SelectedRenderer.Zip(SelectedRenderer.Skip(1), (x, _) => x).Subscribe(w => w?.Release()).AddTo(this);
            SelectedRenderer.Subscribe(w => w?.InitializeRenderer()).AddTo(this);
            ApplyWallpaperCommand = new[]
            {
                CaptureSource.Select(w => w != null),
                desktop.ObserveProperty(w => w.IsConnecting).Select(w => !w)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand().AddTo(this);
            ApplyWallpaperCommand.Subscribe(() =>
            {
                // TODO
            }).AddTo(this);
            DiscardWallpaperCommand = new[]
            {
                CaptureSource.Select(w => w != null),
                desktop.ObserveProperty(w => w.IsConnecting)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand().AddTo(this);
            DiscardWallpaperCommand.Subscribe(() => Task.Run(async () => await _desktop.DiscardWallpaper())).AddTo(this);
            SelectCaptureSourceCommand = SelectedRenderer.Select(w => w != null).ToReactiveCommand();
            SelectCaptureSourceCommand.Subscribe(_ =>
            {
                // Logic では？
                if (SelectedRenderer.Value.HasOwnWindowPicker)
                    CaptureSource.Value = SelectedRenderer.Value.ShowWindowPicker();
                else
                    dialogService.ShowDialog(nameof(WindowPickerDialog), new DialogParameters(), r =>
                    {
                        if (r.Parameters.ContainsKey("CaptureSource"))
                            CaptureSource.Value = r.Parameters.GetValue<ICaptureSource>("CaptureSource");
                        else
                            CaptureSource.Value = null;
                    });
            }).AddTo(this);
            ClearSelectCommand = CaptureSource.Select(w => w != null).ToReactiveCommand();
            ClearSelectCommand.Subscribe(_ => CaptureSource.Value = null).AddTo(this);
        }
    }
}