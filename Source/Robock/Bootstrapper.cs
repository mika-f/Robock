using System;
using System.Windows;

using WindowsDesktop;

using Microsoft.Practices.Unity;

using Prism.Unity;

using Robock.Views;

namespace Robock
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            if (!VirtualDesktop.IsSupported)
                throw new NotSupportedException();
            return Container.Resolve<AppShell>();
        }

        protected override void InitializeShell()
        {
            // ReSharper disable once PossibleNullReferenceException
            Application.Current.MainWindow.Show();
        }
    }
}