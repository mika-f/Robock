using System;
using System.Diagnostics;
using System.Windows.Forms;

using Robock.Shared.Communication;

namespace Robock.Models
{
    /// <summary>
    ///     Desktop is equals to Monitor, Monitor has a one Desktop.
    /// </summary>
    public class Desktop
    {
        private readonly RobockClient _client;
        private readonly Screen _screen;

        public int No { get; }
        public bool IsPrimary => _screen.Primary;
        public string MonitorName => _screen.DeviceName;
        public double X => _screen.Bounds.X;
        public double Y => _screen.Bounds.Y;
        public double Height => _screen.Bounds.Height;
        public double Width => _screen.Bounds.Width;

        public Desktop(Screen screen, int index)
        {
            _screen = screen;
            No = index;
            _client = new RobockClient();
            Process.Start("Robock.Background.exe");
        }

        public void ApplyWallpaper()
        {
            _client.ApplyWallpaper("", IntPtr.Zero, 0, 0, 0, 0);
        }
    }
}