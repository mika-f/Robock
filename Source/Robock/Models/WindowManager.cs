using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Text;

using WindowsDesktop;

using Robock.Interop.Win32;
using Robock.Services;

namespace Robock.Models
{
    public class WindowManager : IDisposable
    {
        private IDisposable _timer;
        public ObservableCollection<Window> Windows { get; }

        public WindowManager()
        {
            Windows = new ObservableCollection<Window>();
            InitializeComObjects();
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

        private async void InitializeComObjects()
        {
            try
            {
                await VirtualDesktopProvider.Default.Initialize();
            }
            catch
            {
                // ignored
            }
        }

        private void FindWindows()
        {
            var windows = new List<Window>();
            StatusTextService.Instance.Status = "Synchronizing visible windows...";

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

                // Universal Windows (invisible / background)
                NativeMethods.DwmGetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out var isCloaked, Marshal.SizeOf(typeof(bool)));
                if (isCloaked && !IsVisibleWindowOnOtherDesktop(hWnd))
                    return true;

                if (Ignores.IgnoreWindowTitles.Contains(title.ToString()))
                    return true;

                NativeMethods.GetWindowThreadProcessId(hWnd, out var processId);
                windows.Add(new Window
                {
                    Handle = hWnd,
                    Title = title.ToString(),
                    ProcessName = Process.GetProcessById((int) processId).ProcessName
                });
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

            StatusTextService.Instance.Status = "Synchronized windows";
        }

        private bool IsVisibleWindowOnOtherDesktop(IntPtr hWnd)
        {
            var placement = WINDOWPLACEMENT.Default;
            NativeMethods.GetWindowPlacement(hWnd, ref placement);
            if (placement.ShowCmd != ShowWindowCommands.Normal)
                return false;

            var desktop = VirtualDesktop.FromHwnd(hWnd);
            return desktop != null && VirtualDesktop.Current.Id != desktop.Id;
        }
    }
}