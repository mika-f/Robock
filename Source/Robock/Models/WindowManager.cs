using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Robock.Win32.Native;

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
            _timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1)).Subscribe(w => FindWindows());
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

                windows.Add(new Window {Handle = hWnd, Title = title.ToString()});
                return true;
            }, IntPtr.Zero);

            // Add windows
            foreach (var process in windows)
                if (Windows.SingleOrDefault(w => w.Handle == process.Handle) == null)
                {
                    Windows.Add(process);
                    process.IsMarked = true;
                }
                else
                {
                    process.IsMarked = true;
                }

            // Remove killed windows.
            foreach (var window in windows.Where(w => !w.IsMarked))
                Windows.Remove(window);
        }
    }
}