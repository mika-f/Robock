using System.Windows;

using MetroRadiance.UI;

namespace Robock
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

            var bootstrap = new Bootstrapper();
            bootstrap.Run();
        }
    }
}