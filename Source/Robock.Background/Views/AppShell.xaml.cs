using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

using Robock.Background.Models;

namespace Robock.Background.Views
{
    /// <summary>
    ///     AppShell.xaml の相互作用ロジック
    /// </summary>
    public partial class AppShell : Window
    {
        private bool _isFirstRun = true;

        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // 表示されてから
            if (!_isFirstRun)
                return;

            _isFirstRun = false;

            BackgroundService.Instance.Initialize(new WindowInteropHelper(this).Handle, InteropImage);
            BackgroundService.Instance.MoveToOutsideOfVirtualScreen();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            BackgroundService.Instance.Release();

            base.OnClosing(e);
        }
    }
}