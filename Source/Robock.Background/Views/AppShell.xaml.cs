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
        public AppShell()
        {
            InitializeComponent();

            Closing += OnClosing;
            Loaded += OnLoaded;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            BackgroundService.Instance.Release();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            BackgroundService.Instance.Initialize(new WindowInteropHelper(this).Handle, InteropImage);
            BackgroundService.Instance.MoveToOutsideOfVirtualScreen();
        }
    }
}