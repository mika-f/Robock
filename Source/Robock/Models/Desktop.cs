using System.Windows.Forms;

namespace Robock.Models
{
    /// <summary>
    ///     Desktop is equals to Monitor, Monitor has a one Desktop.
    /// </summary>
    public class Desktop
    {
        private readonly Screen _screen;

        public int No { get; }
        public bool IsPrimary => _screen.Primary;
        public string MonitorName => _screen.DeviceName;

        public Desktop(Screen screen, int index)
        {
            _screen = screen;
            No = index;
        }
    }
}