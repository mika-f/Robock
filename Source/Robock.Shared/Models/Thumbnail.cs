using System;
using System.Drawing;

using Prism.Mvvm;

namespace Robock.Shared.Models
{
    public class Thumbnail : BindableBase
    {
        public Thumbnail()
        {
            Handle = IntPtr.Zero;
            IsRendering = false;
            Size = new Size(1, 1);
        }

        #region Handle

        private IntPtr _handle;

        public IntPtr Handle
        {
            get => _handle;
            set => SetProperty(ref _handle, value);
        }

        #endregion

        #region IsRendering

        private bool _isRendering;

        public bool IsRendering
        {
            get => _isRendering;
            set => SetProperty(ref _isRendering, value);
        }

        #endregion

        #region Size

        private Size _size;

        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        #endregion
    }
}