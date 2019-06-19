using System;
using System.Windows;

using WindowsDesktop;

using MetroRadiance.UI;

using Prism.Ioc;
using Prism.Unity;

using Robock.Models;
using Robock.Services;
using Robock.Services.Interfaces;
using Robock.ViewModels.Dialogs;
using Robock.Views;
using Robock.Views.Dialogs;
using Robock.Views.Windows;

namespace Robock
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDpiService, DpiService>();
            containerRegistry.Register<IStatusService, StatusService>();
            containerRegistry.RegisterSingleton<RenderManager>();
            containerRegistry.RegisterDialogWindow<MetroDialogWindow>();
            containerRegistry.RegisterDialog<WindowPickerDialog, WindowPickerDialogViewModel>(nameof(WindowPickerDialog));
        }

        protected override Window CreateShell()
        {
            if (!VirtualDesktop.IsSupported)
                throw new NotSupportedException();
            return Container.Resolve<AppShell>();
        }
    }
}