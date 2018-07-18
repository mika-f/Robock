using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

using Prism.Mvvm;

namespace Robock.Models
{
    public class DesktopManager : BindableBase
    {
        public ObservableCollection<Desktop> Desktops { get; }

        public DesktopManager()
        {
            Desktops = new ObservableCollection<Desktop>();
        }

        public void Initialize()
        {
            foreach (var obj in Screen.AllScreens.Select((w, i) => new {Screen = w, Index = i}))
                Desktops.Add(new Desktop(obj.Screen, obj.Index + 1));
        }
    }
}