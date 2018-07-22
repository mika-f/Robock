using System.ServiceModel;

// ReSharper disable OperationContractWithoutServiceContract

namespace Robock.Shared.Communication
{
    public interface IRobockDuplexCallback
    {
        /// <summary>
        ///     Handshake() callback
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void HandshakeCallback();

        /// <summary>
        ///     ApplyWallpaper() callback
        /// </summary>
        /// <param name="isSucceed">If wallpaper applied to background, returns true</param>
        [OperationContract(IsOneWay = true)]
        void ApplyWallpaperCallback(bool isSucceed);

        /// <summary>
        ///     DiscardWallpaper() callback
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void DiscardWallpaperCallback();

        /// <summary>
        ///     Close() callback
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void CloseCallback();
    }
}