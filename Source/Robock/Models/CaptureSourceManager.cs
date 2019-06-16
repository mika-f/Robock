using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using WindowsDesktop;

using Prism.Mvvm;

using Robock.Interop.Win32;
using Robock.Models.CaptureSources;

namespace Robock.Models
{
    public class CaptureSourceManager : BindableBase, IDisposable
    {
        private readonly List<string> _blacklist;
        private readonly ObservableCollection<ICaptureSource> _captureSources;
        public ReadOnlyObservableCollection<ICaptureSource> CaptureSources { get; }

        public CaptureSourceManager()
        {
            _blacklist = new List<string>
            {
                "ChromeWindow", // Robock window separator
                "Robock", // Myself
                "Robock.Background" // Robock background worker
            };

            _captureSources = new ObservableCollection<ICaptureSource>();
            CaptureSources = new ReadOnlyObservableCollection<ICaptureSource>(_captureSources);

            InitializeComObjects();
        }

        public void Dispose()
        {
            foreach (var source in CaptureSources)
                source.Dispose();
        }

        public void FetchAll()
        {
            _captureSources.Clear();
            _captureSources.AddRange(FetchWindows());
        }

        private void InitializeComObjects()
        {
            try
            {
                VirtualDesktopProvider.Default.Initialize().Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private List<ICaptureSource> FetchWindows()
        {
            var sources = new List<ICaptureSource>();

            NativeMethods.EnumWindows((hWnd, _) =>
            {
                // remove invisible window
                if (!NativeMethods.IsWindowVisible(hWnd))
                    return true;

                // remove UWP (invisible or background) window
                NativeMethods.DwmGetWindowAttributeBool(hWnd, DWMWINDOWATTRIBUTE.Cloaked, out var isCloaked, Marshal.SizeOf<bool>());
                if (isCloaked && IsInvisibleOnOtherVirtualDesktop(hWnd))
                    return true;

                // remove popup window
                var ws = NativeMethods.GetWindowLongPtr(hWnd, (int) GWL.GWL_STYLE);
                if ((ws.ToInt64() & (long) WindowStyles.WS_POPUP) != 0)
                    return true;

                // remove untitled window
                var sb = new StringBuilder(1024);
                NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
                if (string.IsNullOrEmpty(sb.ToString()))
                    return true;

                if (_blacklist.Contains(sb.ToString()))
                    return true;

                sources.Add(new CaptureSources.Window(hWnd));

                return true;
            }, IntPtr.Zero);

            return sources;
        }

        private bool IsInvisibleOnOtherVirtualDesktop(IntPtr hWnd)
        {
            var placement = WINDOWPLACEMENT.Default;
            NativeMethods.GetWindowPlacement(hWnd, ref placement);
            if (placement.ShowCmd == ShowWindowCommands.Normal)
                return true;

            var desktop = VirtualDesktop.FromHwnd(hWnd);
            return VirtualDesktop.Current.Id == desktop.Id;
        }
    }
}