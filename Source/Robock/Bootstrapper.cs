using System.Windows;

using Microsoft.Practices.Unity;

using Prism.Unity;

using Robock.Views;

namespace Robock
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<AppShell>();
        }

        protected override void InitializeShell()
        {
            // ReSharper disable once PossibleNullReferenceException
            Application.Current.MainWindow.Show();
        }
    }
}