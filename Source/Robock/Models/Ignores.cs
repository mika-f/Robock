using System.Collections.Generic;

namespace Robock.Models
{
    public class Ignores
    {
        public static List<string> IgnoreWindowTitles = new List<string>
        {
            "ChromeWindow", // Robock's window separator
            "Robock", // Myself
            "Robock.Background", // Myself
            "NVIDIA GeForce Overlay", //
            "Program Manager" // Desktop
        };
    }
}