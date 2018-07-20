using System;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;

using Robock.Models;

namespace Robock.ViewModels.Tabs
{
    public class DesktopViewModel : TabViewModel
    {
        private const double Scale = 15;

        private readonly Desktop _desktop;
        private readonly DesktopWindowManager _desktopWindowManager;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public ReactiveProperty<bool> IsSelected { get; }
        public ReactiveProperty<WindowViewModel> SelectedWindow { get; }
        public ReadOnlyReactiveCollection<WindowViewModel> Windows { get; }
        public string AspectRatio { get; }

        public ReactiveProperty<int> PreviewAreaLeft { get; }
        public ReactiveProperty<int> PreviewAreaTop { get; }
        public ReactiveProperty<int> PreviewAreaHeight { get; }
        public ReactiveProperty<int> PreviewAreaWidth { get; }

        public string DesktopName => $"Desktop {_desktop.No}";
        public string Resolution => $"{_desktop.Width}x{_desktop.Height}";
        public bool IsPrimary => _desktop.IsPrimary;

        public double VirtualScreenX => (_offsetX + _desktop.X) / Scale;
        public double VirtualScreenY => (_offsetY + _desktop.Y) / Scale;
        public double VirtualScreenHeight => _desktop.Height / Scale;
        public double VirtualScreenWidth => _desktop.Width / Scale;

        public DesktopViewModel(Desktop desktop, WindowManager windowManager, DesktopWindowManager desktopWindowManager) : base($":Desktop: Desktop {desktop.No}")
        {
            _desktop = desktop;
            _desktopWindowManager = desktopWindowManager;

            // 仮想スクリーン周りの計算
            _offsetX = (SystemParameters.VirtualScreenLeft < 0 ? -1 : 1) * SystemParameters.VirtualScreenLeft;
            _offsetY = (SystemParameters.VirtualScreenTop < 0 ? -1 : 1) * SystemParameters.VirtualScreenTop;

            // アスペクト比
            AspectRatio = $"http://placehold.jp/ffffff/ffffff/{AspectHelper.Calc(_desktop.Height, _desktop.Width)}.png?text=%20";

            // プレビュー
            PreviewAreaLeft = new ReactiveProperty<int>();
            PreviewAreaTop = new ReactiveProperty<int>();
            PreviewAreaHeight = new ReactiveProperty<int>();
            PreviewAreaWidth = new ReactiveProperty<int>();
            var observer = new[]
            {
                PreviewAreaLeft,
                PreviewAreaTop,
                PreviewAreaHeight,
                PreviewAreaWidth
            }.CombineLatest();
            observer.Subscribe(w => Render());

            IsSelected = new ReactiveProperty<bool>(false);
            IsSelected.Subscribe(w =>
            {
                _desktopWindowManager.Stop();
                Render();
            });
            SelectedWindow = new ReactiveProperty<WindowViewModel>();
            SelectedWindow.Where(w => w != null).Subscribe(w =>
            {
                _desktopWindowManager.Stop();
                Render();
            });
            Windows = windowManager.Windows.ToReadOnlyReactiveCollection(w => new WindowViewModel(w));
        }

        private void Render()
        {
            if (SelectedWindow?.Value == null)
                return;

            if (_desktopWindowManager.IsRendering)
                _desktopWindowManager.Redender(PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value);
            else
                _desktopWindowManager.Start(SelectedWindow.Value.Handle, PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value);
        }
    }
}