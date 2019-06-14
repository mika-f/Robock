using System;
using System.Windows;

using WindowsDesktop;

using MetroRadiance.UI;

using Prism.Ioc;
using Prism.Unity;

using Robock.Views;

namespace Robock
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //
        }

        protected override Window CreateShell()
        {
            if (!VirtualDesktop.IsSupported)
                throw new NotSupportedException();
            return Container.Resolve<AppShell>();
        }
    }
}