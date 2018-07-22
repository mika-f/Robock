using System;
using System.ServiceModel;

namespace Robock.Shared.Communication
{
    public class RobockClient : IRobockDuplexCallback
    {
        private readonly IRobockDuplex _channel;

        public RobockClient()
        {
            _channel = new DuplexChannelFactory<IRobockDuplex>(this, new NetNamedPipeBinding(), "net.pipe://localhost/Robock").CreateChannel();
        }

        public void HandshakeCallback(string uuid)
        {
            // throw new NotImplementedException();
        }

        public void ApplyWallpaperCallback(string uuid, bool isSucceed)
        {
            // throw new NotImplementedException();
        }

        public void DiscardWallpaperCallback(string uuid)
        {
            // throw new NotImplementedException();
        }

        public void CloseCallback(string uuid)
        {
            // throw new NotImplementedException();
        }

        #region IRobockDuples

        public void Handshake(int index)
        {
            _channel.Handshake(index);
        }

        public void ApplyWallpaper(string uuid, IntPtr src, int left, int top, int height, int width)
        {
            _channel.ApplyWallpaper(uuid, src, left, top, height, width);
        }

        public void DiscardWallpaper(string uuid)
        {
            _channel.DiscardWallpaper(uuid);
        }

        public void Close(string uuid)
        {
            _channel.Close(uuid);
        }

        #endregion
    }
}