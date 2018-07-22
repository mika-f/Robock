using System;
using System.Diagnostics;
using System.Windows.Forms;

using Robock.Shared.Communication;
using Robock.Shared.Win32;

namespace Robock.Models
{
    /// <summary>
    ///     Desktop is equals to Monitor, Monitor has a one Desktop.
    /// </summary>
    public class Desktop : IDisposable
    {
        private readonly RobockClient _client;
        private readonly Process _process;
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
            var uuid = $"Background.Desktop{index}";
            _client = new RobockClient(uuid);
            _process = Process.Start("Robock.Background.exe", $"{uuid}");
        }

        public void Dispose()
        {
            try
            {
                _client.Close();
                _process?.CloseMainWindow();
                _process?.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void Handshake()
        {
            _client.Handshake((int) X, (int) Y, (int) Height, (int) Width);
        }

        public void ApplyWallpaper(IntPtr hWnd, RECT? rect)
        {
            _client.ApplyWallpaper(hWnd, rect);
        }

        public void DiscardWallpaper()
        {
            _client.DiscardWallpaper();
        }
    }
}