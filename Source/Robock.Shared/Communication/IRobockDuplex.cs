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
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="height">Height</param>
        /// <param name="width">Width</param>
        [OperationContract(IsOneWay = true)]
        void Handshake(int x, int y, int height, int width);

        /// <summary>
        ///     Apply wallpaper
        /// </summary>
        /// <param name="src">hWnd of source window</param>
        /// <param name="left">left position of rendering area</param>
        /// <param name="top">top position of rendering area</param>
        /// <param name="height">height of rendering area</param>
        /// <param name="width">width of rendering area</param>
        [OperationContract(IsOneWay = true)]
        void ApplyWallpaper(IntPtr src, int left, int top, int height, int width);

        /// <summary>
        ///     Discard wallpaper
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void DiscardWallpaper();

        /// <summary>
        ///     Close communication pipe
        /// </summary>
        void Close();
    }
}