using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Robock.Models;
using Robock.Shared.Extensions;
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

        public ReactiveCommand ApplyWallpaperCommand { get; }
        public ReactiveCommand DiscardWallpaperCommand { get; }
        public ReactiveCommand ReloadWindowsCommand { get; }

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

            // エディター
            EditorAspectRatio = new ReactiveProperty<string>("https://placehold.mochizuki.moe/1x1/");
            EditorAreaLeft = new ReactiveProperty<int>();
            EditorAreaTop = new ReactiveProperty<int>();
            EditorAreaHeight = new ReactiveProperty<int>();
            EditorAreaWidth = new ReactiveProperty<int>();
            var editorArea = new[]
            {
                EditorAreaLeft,
                EditorAreaTop,
                EditorAreaHeight,
                EditorAreaWidth
            }.CombineLatest();
            editorArea.Subscribe(w => Render(DesktopWindowManager.EditorIndex)).AddTo(this);
            desktopWindowManager.Thumbnails[DesktopWindowManager.EditorIndex].ObserveProperty(w => w.Size).Subscribe(w =>
            {
                //
                EditorAspectRatio.Value = $"https://placehold.mochizuki.moe/{AspectHelper.Calc(w.Height, w.Width)}/000000%2C000/000000%2C000/";
            }).AddTo(this);

            // 選択範囲
            SelectedAreaLeft = new ReactiveProperty<int>();
            SelectedAreaTop = new ReactiveProperty<int>();
            SelectedAreaHeight = new ReactiveProperty<int>();
            SelectedAreaWidth = new ReactiveProperty<int>();
            var selectedArea = new[]
            {
                SelectedAreaLeft,
                SelectedAreaTop,
                SelectedAreaHeight,
                SelectedAreaWidth
            }.CombineLatest();
            selectedArea.Subscribe(w => Render(DesktopWindowManager.PreviewIndex)).AddTo(this);

            // プレビュー
            AspectRatio = $"https://placehold.mochizuki.moe/{AspectHelper.Calc(_desktop.Height, _desktop.Width)}/000000%2C000/000000%2C000/";
            PreviewAreaLeft = new ReactiveProperty<int>();
            PreviewAreaTop = new ReactiveProperty<int>();
            PreviewAreaHeight = new ReactiveProperty<int>();
            PreviewAreaWidth = new ReactiveProperty<int>();
            var previewArea = new[]
            {
                EditorAreaLeft,
                EditorAreaTop,
                EditorAreaHeight,
                EditorAreaWidth
            }.CombineLatest();
            previewArea.Subscribe(w => Render(DesktopWindowManager.PreviewIndex)).AddTo(this);
            desktopWindowManager.Thumbnails[DesktopWindowManager.EditorIndex].ObserveProperty(w => w.IsRendering).Subscribe(w =>
            {
                //
                Render(DesktopWindowManager.PreviewIndex);
            }).AddTo(this);

            // 他
            Windows = windowManager.Windows.ToReadOnlyReactiveCollection(w => new WindowViewModel(w));
            IsSelected = new ReactiveProperty<bool>(false);
            IsSelected.Subscribe(w =>
            {
                _desktopWindowManager.Stop(0);
                _desktopWindowManager.Stop(1);
                Render(DesktopWindowManager.EditorIndex);
            }).AddTo(this);
            SelectedWindow = new ReactiveProperty<WindowViewModel>();
            SelectedWindow.Where(w => w != null).Subscribe(w =>
            {
                _desktopWindowManager.Stop(0);
                _desktopWindowManager.Stop(1);
                Render(DesktopWindowManager.EditorIndex);
            }).AddTo(this);
            ApplyWallpaperCommand = new[]
            {
                SelectedWindow.Select(w => w != null),
                _desktopWindowManager.Thumbnails[DesktopWindowManager.EditorIndex].ObserveProperty(w => w.IsRendering)
            }.CombineLatest().Select(w => w.All(v => v)).ToReactiveCommand();
            ApplyWallpaperCommand.Subscribe(_ => { }).AddTo(this);
            ReloadWindowsCommand = new ReactiveCommand();
            ReloadWindowsCommand.Subscribe(_ => windowManager.ForceUpdate()).AddTo(this);
        }

        private void Render(int index)
        {
            if (SelectedWindow?.Value == null)
                return;
            var handle = SelectedWindow.Value.Handle;

            if (index == DesktopWindowManager.EditorIndex)
            {
                if (_desktopWindowManager.Thumbnails[DesktopWindowManager.EditorIndex].IsRendering)
                    _desktopWindowManager.Rerender(EditorAreaLeft.Value, EditorAreaTop.Value, EditorAreaHeight.Value, EditorAreaWidth.Value);
                else
                    _desktopWindowManager.Start(handle, EditorAreaLeft.Value, EditorAreaTop.Value, EditorAreaHeight.Value, EditorAreaWidth.Value);
            }
            else
            {
                var editor = DesktopWindowManager.EditorIndex;

                // 描画サイズから、縮小された割合を計算
                var multi = _desktopWindowManager.Thumbnails[editor].Size.Height / (double) EditorAreaHeight.Value;

                // 選択された領域を取得
                RECT rect;
                if (SelectedAreaHeight.Value != 0)
                    rect = new RECT
                    {
                        top = (int) (SelectedAreaTop.Value * multi),
                        left = (int) (SelectedAreaLeft.Value * multi),
                        bottom = (int) ((SelectedAreaTop.Value + SelectedAreaHeight.Value) * multi),
                        right = (int) ((SelectedAreaLeft.Value + SelectedAreaWidth.Value) * multi)
                    };
                else
                    rect = new RECT
                    {
                        top = 0,
                        left = 0,
                        bottom = _desktopWindowManager.Thumbnails[editor].Size.Height,
                        right = _desktopWindowManager.Thumbnails[editor].Size.Width
                    };

                Debug.WriteLine($"{SelectedAreaTop.Value}, {SelectedAreaLeft.Value}, {SelectedAreaHeight.Value}, {SelectedAreaWidth.Value}");
                if (_desktopWindowManager.Thumbnails[DesktopWindowManager.PreviewIndex].IsRendering)
                    _desktopWindowManager.Rerender(PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value, index, rect);
                else
                    _desktopWindowManager.Start(handle, PreviewAreaLeft.Value, PreviewAreaTop.Value, PreviewAreaHeight.Value, PreviewAreaWidth.Value, index, rect);
            }
        }
    }
}