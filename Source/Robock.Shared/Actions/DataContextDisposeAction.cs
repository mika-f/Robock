using System;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace Robock.Shared.Actions
{
    public class DataContextDisposeAction : TriggerAction<FrameworkElement>
    {
        #region Overrides of TriggerAction

        protected override void Invoke(object parameter)
        {
            var disposable = AssociatedObject.DataContext as IDisposable;
            disposable?.Dispose();
        }

        #endregion
    }
}