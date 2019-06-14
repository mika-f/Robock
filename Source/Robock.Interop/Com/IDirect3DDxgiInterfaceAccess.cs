using System;
using System.Runtime.InteropServices;

namespace Robock.Interop.Com
{
    [ComImport]
    [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDirect3DDxgiInterfaceAccess : IDisposable
    {
        IntPtr GetInterface([In] ref Guid iid);
    }
}