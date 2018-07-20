using System;

namespace Robock.Models
{
    // Win32 Unmaneged
    public class Window
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public bool IsMarked { get; set; }
    }
}