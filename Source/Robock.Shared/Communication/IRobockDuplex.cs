using System;
using System.ServiceModel;

namespace Robock.Shared.Communication
{
    [ServiceContract(Namespace = "robock://localhost", SessionMode = SessionMode.Required, CallbackContract = typeof(IRobockDuplexCallback))]
    public interface IRobockDuplex
    {
        /// <summary>
        ///     Handshake between Robock and Robock.Background
        /// </summary>
        /// <param name="index">Wallpaper index (e.g. 0 = Primary)</param>
        [OperationContract(IsOneWay = true)]
        void Handshake(int index);

        /// <summary>
        ///     Apply wallpaper
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        /// <param name="src">hWnd of source window</param>
        /// <param name="left">left position of rendering area</param>
        /// <param name="top">top position of rendering area</param>
        /// <param name="height">height of rendering area</param>
        /// <param name="width">width of rendering area</param>
        [OperationContract(IsOneWay = true)]
        void ApplyWallpaper(string uuid, IntPtr src, int left, int top, int height, int width);

        /// <summary>
        ///     Discard wallpaper
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        [OperationContract(IsOneWay = true)]
        void DiscardWallpaper(string uuid);

        /// <summary>
        ///     Close communication pipe
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        void Close(string uuid);
    }
}