using System.ServiceModel;

// ReSharper disable OperationContractWithoutServiceContract

namespace Robock.Shared.Communication
{
    public interface IRobockDuplexCallback
    {
        /// <summary>
        ///     Handshake() callback
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        [OperationContract(IsOneWay = true)]
        void HandshakeCallback(string uuid);

        /// <summary>
        ///     ApplyWallpaper() callback
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        /// <param name="isSucceed">If wallpaper applied to background, returns true</param>
        [OperationContract(IsOneWay = true)]
        void ApplyWallpaperCallback(string uuid, bool isSucceed);

        /// <summary>
        ///     DiscardWallpaper() callback
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        [OperationContract(IsOneWay = true)]
        void DiscardWallpaperCallback(string uuid);

        /// <summary>
        ///     Close() callback
        /// </summary>
        /// <param name="uuid">UUID of Robock.Background</param>
        [OperationContract(IsOneWay = true)]
        void CloseCallback(string uuid);
    }
}