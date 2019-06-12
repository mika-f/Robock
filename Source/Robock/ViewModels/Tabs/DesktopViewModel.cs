using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Models.Renderer;
using Robock.Shared.Extensions;

namespace Robock.ViewModels.Tabs
{
    public class DesktopViewModel : TabViewModel
    {
        private const double Scale = 15;

        private readonly Desktop _desktop;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public ReactiveProperty<bool> IsSelected { get; }
        public ReactiveProperty<WindowViewModel> SelectedWindow { get; }
        public ReactiveProperty<double> PreviewHeight { get; set; }
        public ReactiveProperty<double> PreviewWidth { get; set; }
        public ReactiveProperty<double> RenderTop { get; set; }
        public ReactiveProperty<double> RenderLeft { get; set; }
        public ReactiveProperty<double> RenderHeight { get; set; }
        public ReactiveProperty<double> RenderWidth { get; set; }
        public ReactiveProperty<double> RenderScale { get; set; }
        public ReadOnlyReactiveCollection<WindowViewModel> Windows { get; }

        public ReactiveCommand ApplyWallpaperCommand { get; }
        public ReactiveCommand DiscardWallpaperCommand { get; }
        public ReactiveCommand ReloadWindowsCommand { get; }
        public ReactiveCommand ClearSelectCommand { get; }

        public string DesktopName => $"Desktop {_desktop.No}";
        public double Width => _desktop.Width;
        public double Height => _desktop.Height;
        public string Resolution => $"{_desktop.Width}x{_desktop.Height}";
        public double VirtualScreenX => (_offsetX + _desktop.X) / Scale;
        public double VirtualScreenY => (_offsetY + _desktop.Y) / Scale;
        public double VirtualScreenHeight => _desktop.Height / Scale;
        public double VirtualScreenWidth => _desktop.Width / Scale;
        public ReadOnlyReactiveProperty<IRenderer> Renderer { get; }
        public ReadOnlyReactiveProperty<string> Wallpaper { get; }
        public ReadOnlyReactiveProperty<bool> IsSelectedWindow { get; }

        public DesktopViewModel(Desktop desktop, WindowManager windowManager, DesktopWindowManager _)
            : base($":Desktop: Desktop {desktop.No}")
        {
            _desktop = desktop;

            // 仮想スクリーン周りの計算
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
            Windows = windowManager.Windows.ToReadOnlyReactiveCollection(w => new WindowViewModel(w));
            SelectedWindow = new ReactiveProperty<WindowViewModel>();
            IsSelectedWindow = SelectedWindow.Select(w => w != null).ToReadOnlyReactiveProperty().AddTo(this);
            Renderer = IsSelectedWindow.Do(_ => Renderer?.Value?.Dispose())
                                       .Select(w => w ? (IRenderer) new SharedSurfaceRenderer(SelectedWindow.Value.Handle) : null)
                                       .ToReadOnlyReactiveProperty().AddTo(this);
            ApplyWallpaperCommand = new[]
            {
                SelectedWindow.Select(w => w != null),
                desktop.ObserveProperty(w => w.IsConnecting).Select(w => !w)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand();
            ApplyWallpaperCommand.Subscribe(() =>
            {
                // TODO
            }).AddTo(this);
            DiscardWallpaperCommand = new[]
            {
                SelectedWindow.Select(w => w != null),
                desktop.ObserveProperty(w => w.IsConnecting)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand();
            DiscardWallpaperCommand.Subscribe(() => Task.Run(async () => await _desktop.DiscardWallpaper())).AddTo(this);
            ReloadWindowsCommand = new ReactiveCommand();
            ReloadWindowsCommand.Subscribe(_ => windowManager.ForceUpdate()).AddTo(this);
            ClearSelectCommand = SelectedWindow.Select(w => w != null).ToReactiveCommand();
            ClearSelectCommand.Subscribe(_ => SelectedWindow.Value = null).AddTo(this);
        }
    }
}