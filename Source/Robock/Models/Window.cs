using System;

using Prism.Mvvm;

namespace Robock.Models
{
    // Win32 Unmaneged
    public class Window : BindableBase
    {
        public IntPtr Handle { get; set; }
        public bool IsMarked { get; set; }
        public string ProcessName { get; set; }

        #region Title

        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion
    }
}