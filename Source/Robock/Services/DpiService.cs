using System;
using System.Windows;
using System.Windows.Interop;

using MetroRadiance.Interop;

using Prism.Mvvm;

namespace Robock.Services
{
    public class DpiService : BindableBase
    {
        private static DpiService _instance;
        private readonly Window _window;

        private Dpi _currentDpi;
        public static DpiService Instance => _instance ??= new DpiService();

        public Dpi CurrentDpi
        {
            get => _currentDpi;
            set => SetProperty(ref _currentDpi, value);
        }

        private DpiService()
        {
            if (!PerMonitorDpi.IsSupported)
                throw new NotSupportedException();

            _window = Application.Current.MainWindow;
            if (_window == null)
                throw new NotSupportedException();

            CurrentDpi = PerMonitorDpi.GetDpi(new WindowInteropHelper(_window).Handle);
            _window.DpiChanged += OnDpiChanged;
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            CurrentDpi = PerMonitorDpi.GetDpi(new WindowInteropHelper(_window).Handle);
        }
    }
}