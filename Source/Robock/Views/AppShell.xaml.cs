using System;
using System.Windows;

using MetroRadiance.UI.Controls;

namespace Robock.Views
{
    /// <summary>
    ///     AppShell.xaml の相互作用ロジック
    /// </summary>
    public partial class AppShell : MetroWindow
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}