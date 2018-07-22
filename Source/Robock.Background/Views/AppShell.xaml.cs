using System;
using System.Windows;

using Robock.Background.Models;

namespace Robock.Background.Views
{
    /// <summary>
    ///     AppShell.xaml の相互作用ロジック
    /// </summary>
    public partial class AppShell : Window
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // 表示されてから
            BackgroundService.Initialize();
            BackgroundService.MoveToOutsideOfDesktop();
        }
    }
}