using System;
using System.ServiceModel;
using System.Threading.Tasks;

using Robock.Interop.Win32;

namespace Robock.Shared.Models
{
    [ServiceContract(Namespace = "robock://localhost", SessionMode = SessionMode.Required)]
    public interface IRobockBackgroundConnection
    {
        /// <summary>
        ///     Handshake between Robock and Robock.Background
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="height">Height</param>
        /// <param name="width">Width</param>
        [OperationContract]
        Task Handshake(int x, int y, int height, int width);

        /// <summary>
        ///     Apply wallpaper
        /// </summary>
        /// <param name="src">hWnd of source window</param>
        /// <param name="rect"></param>
        [OperationContract]
        Task ApplyWallpaper(IntPtr src, RECT rect);

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Task Heartbeat();

        /// <summary>
        ///     Discard wallpaper
        /// </summary>
        [OperationContract]
        Task DiscardWallpaper();

        /// <summary>
        ///     Close communication pipe
        /// </summary>
        [OperationContract]
        Task Close();
    }
}