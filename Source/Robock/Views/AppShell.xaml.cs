using System;

using MetroRadiance.UI.Controls;

using Robock.ViewModels;

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

        protected override void OnActivated(EventArgs e)
        {
            (DataContext as AppShellViewModel)?.Initialize();
            base.OnActivated(e);
        }
    }
}