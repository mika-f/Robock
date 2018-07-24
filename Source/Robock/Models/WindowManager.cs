using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Robock.Shared.Win32;

namespace Robock.Models
{
    public class WindowManager : IDisposable
    {
        private IDisposable _timer;
        public ObservableCollection<Window> Windows { get; }

        public WindowManager()
        {
            Windows = new ObservableCollection<Window>();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start()
        {
            _timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1)).Subscribe(_ => FindWindows());
        }

        public void ForceUpdate()
        {
            Observable.Return(1).Delay(TimeSpan.FromSeconds(1)).Subscribe(_ => FindWindows());
        }

        private void FindWindows()
        {
            var windows = new List<Window>();

            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                // Filter by Window is visible
                if (!NativeMethods.IsWindowVisible(hWnd))
                    return true;

                // Filter by Window Title
                var title = new StringBuilder(1024);
                NativeMethods.GetWindowText(hWnd, title, title.Capacity);
                if (string.IsNullOrWhiteSpace(title.ToString()))
                    return true; // Skipped

                if (Ignores.IgnoreWindowTitles.Contains(title.ToString()))
                    return true;
                windows.Add(new Window {Handle = hWnd, Title = title.ToString()});
                return true;
            }, IntPtr.Zero);

            // Unflag
            foreach (var window in Windows)
                window.IsMarked = false;

            // Add windows
            foreach (var window in windows)
                if (Windows.SingleOrDefault(w => w.Handle == window.Handle) == null)
                {
                    window.IsMarked = true;
                    Windows.Add(window);
                }
                else
                {
                    var exist = Windows.Single(w => w.Handle == window.Handle);
                    exist.IsMarked = true;
                    exist.Title = window.Title;
                }

            // Remove killed windows.
            foreach (var window in Windows.ToArray().Where(w => !w.IsMarked))
                Windows.Remove(window);
        }
    }
}