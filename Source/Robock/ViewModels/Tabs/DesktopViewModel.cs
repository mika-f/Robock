using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Shared.Extensions;
using Robock.Shared.Models;
using Robock.Shared.Win32;

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

        // Editor
        public ReactiveProperty<string> EditorAspectRatio { get; }
        public ReactiveProperty<int> EditorAreaLeft { get; }
        public ReactiveProperty<int> EditorAreaTop { get; }
        public ReactiveProperty<int> EditorAreaHeight { get; }
        public ReactiveProperty<int> EditorAreaWidth { get; }

        // Selected
        public ReactiveProperty<int> SelectedAreaLeft { get; }
        public ReactiveProperty<int> SelectedAreaTop { get; }
        public ReactiveProperty<int> SelectedAreaHeight { get; }
        public ReactiveProperty<int> SelectedAreaWidth { get; }

        // Preview
        public string AspectRatio { get; }
        public ReactiveProperty<int> PreviewAreaLeft { get; }
        public ReactiveProperty<int> PreviewAreaTop { get; }
        public ReactiveProperty<int> PreviewAreaHeight { get; }
        public ReactiveProperty<int> PreviewAreaWidth { get; }

        // Grid
        public ReactiveProperty<int> GridAreaLeft { get; }
        public ReactiveProperty<int> GridAreaTop { get; }

        public ReactiveCommand ApplyWallpaperCommand { get; }
        public ReactiveCommand DiscardWallpaperCommand { get; }
        public ReactiveCommand ReloadWindowsCommand { get; }

        public string DesktopName => $"Desktop {_desktop.No}";
        public string Resolution => $"{_desktop.Width}x{_desktop.Height}";

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

            // タブの選択状態
            IsSelected = new ReactiveProperty<bool>(false);
            IsSelected.Subscribe(w =>
            {
                if (w && CanRender())
                {
                    Render(0);
                }
                else
                {
                    _desktopWindowManager.Stop(0);
                    _desktopWindowManager.Stop(1);
                }
            }).AddTo(this);

            // エディター
            EditorAspectRatio = new ReactiveProperty<string>("https://placehold.mochizuki.moe/1x1/");
            EditorAreaLeft = new ReactiveProperty<int>();
            EditorAreaTop = new ReactiveProperty<int>();
            EditorAreaHeight = new ReactiveProperty<int>();
            EditorAreaWidth = new ReactiveProperty<int>();
            new[]
            {
                EditorAreaLeft,
                EditorAreaTop,
                EditorAreaHeight,
                EditorAreaWidth
            }.CombineLatest().Throttle(TimeSpan.FromMilliseconds(50))
             .Where(w => CanRender()).Subscribe(w => Render(0)).AddTo(this);
            desktopWindowManager.Thumbnails[0].ObserveProperty(w => w.Size).Subscribe(w =>
            {
                //
                EditorAspectRatio.Value = $"https://placehold.mochizuki.moe/{AspectHelper.Calc(w.Height, w.Width)}/000000%2C000/000000%2C000/";
            }).AddTo(this);

            // 選択範囲
            SelectedAreaLeft = new ReactiveProperty<int>();
            SelectedAreaTop = new ReactiveProperty<int>();
            SelectedAreaHeight = new ReactiveProperty<int>();
            SelectedAreaWidth = new ReactiveProperty<int>();
            new[]
            {
                SelectedAreaLeft,
                SelectedAreaTop,
                SelectedAreaHeight,
                SelectedAreaWidth
            }.CombineLatest().Throttle(TimeSpan.FromMilliseconds(50))
             .Where(w => CanRender()).Subscribe(w => Render(1)).AddTo(this);

            // プレビュー
            AspectRatio = $"https://placehold.mochizuki.moe/{AspectHelper.Calc(_desktop.Height, _desktop.Width)}/000000%2C000/000000%2C000/";
            PreviewAreaLeft = new ReactiveProperty<int>();
            PreviewAreaTop = new ReactiveProperty<int>();
            PreviewAreaHeight = new ReactiveProperty<int>();
            PreviewAreaWidth = new ReactiveProperty<int>();
            new[]
            {
                PreviewAreaLeft,
                PreviewAreaTop,
                PreviewAreaHeight,
                PreviewAreaWidth
            }.CombineLatest().Throttle(TimeSpan.FromMilliseconds(50))
             .Where(w => CanRender()).Subscribe(w =>
             {
                 //
                 Render(1);
             });
            desktopWindowManager.Thumbnails[0]
                .ObserveProperty(w => w.IsRendering)
                .Where(w => w && CanRender())
                .Subscribe(w =>
                {
                    //
                    Render(1);
                }).AddTo(this);

            // 親
            GridAreaLeft = new ReactiveProperty<int>();
            GridAreaTop = new ReactiveProperty<int>();

            // 他
            Windows = windowManager.Windows.ToReadOnlyReactiveCollection(w => new WindowViewModel(w));
            SelectedWindow = new ReactiveProperty<WindowViewModel>();
            SelectedWindow.Where(w => w != null && CanRender()).Subscribe(w =>
            {
                _desktopWindowManager.Stop(0);
                _desktopWindowManager.Stop(1);
                Render(0);
            }).AddTo(this);
            ApplyWallpaperCommand = new[]
            {
                SelectedWindow.Select(w => w != null),
                _desktopWindowManager.Thumbnails[0].ObserveProperty(w => w.IsRendering)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand();
            ApplyWallpaperCommand.Subscribe(_ =>
            {
                // タイミングどこが良いか問題
                _desktop.Handshake(() =>
                {
                    var rect = SelectedAreaHeight.Value != 0
                        ? CalcRenderingRect()
                        : new RECT {top = 0, left = 0, bottom = _desktopWindowManager.Thumbnails[0].Size.Height, right = _desktopWindowManager.Thumbnails[0].Size.Width};
                    _desktop.ApplyWallpaper(SelectedWindow.Value.Handle, SelectedAreaHeight.Value != 0 ? CalcRenderingRect() : rect);
                });
            }).AddTo(this);
            DiscardWallpaperCommand = new[]
            {
                SelectedWindow.Select(w => w != null)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand();
            DiscardWallpaperCommand.Subscribe(_ =>
            {
                //
                _desktop.DiscardWallpaper();
            }).AddTo(this);
            ReloadWindowsCommand = new ReactiveCommand();
            ReloadWindowsCommand.Subscribe(_ => windowManager.ForceUpdate()).AddTo(this);
        }

        private void Render(int index)
        {
            var handle = SelectedWindow.Value.Handle;

            if (index == 0)
            {
                if (_desktopWindowManager.Thumbnails[0].IsRendering)
                    _desktopWindowManager.Rerender(0, EditorAreaLeft.Value, EditorAreaTop.Value, EditorAreaHeight.Value, EditorAreaWidth.Value);
                else
                    _desktopWindowManager.Start(0, handle, EditorAreaLeft.Value, EditorAreaTop.Value, EditorAreaHeight.Value, EditorAreaWidth.Value);
            }
            else
            {
                var rect = RectUtil.AsRect(0, 0, _desktopWindowManager.Thumbnails[0].Size.Height, _desktopWindowManager.Thumbnails[0].Size.Width);
                if (SelectedAreaHeight.Value != 0)
                    rect = CalcRenderingRect();

                if (_desktopWindowManager.Thumbnails[DesktopWindowManager.PreviewIndex].IsRendering)
                    _desktopWindowManager.Rerender(1, PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value, rect);
                else
                    _desktopWindowManager.Start(1, handle, PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value, rect);
            }
        }

        private bool CanRender()
        {
            if (!IsSelected.Value || SelectedWindow.Value == null)
                return false;
            return PreviewAreaHeight.Value != 0 && PreviewAreaWidth.Value != 0;
        }

        private RECT CalcRenderingRect()
        {
            // 描画サイズから、縮小された割合を計算
            var multi = _desktopWindowManager.Thumbnails[0].Size.Height / (double) EditorAreaHeight.Value;

            // Grid と Image のズレが大きいと、描画領域がずれてしまうので、補正する
            var diff = new Size(EditorAreaLeft.Value - GridAreaLeft.Value, EditorAreaTop.Value - GridAreaTop.Value);
            return RectUtil.AsRect(SelectedAreaTop.Value - diff.Height, SelectedAreaLeft.Value - diff.Width, SelectedAreaHeight.Value, SelectedAreaWidth.Value, multi);
        }
    }
}