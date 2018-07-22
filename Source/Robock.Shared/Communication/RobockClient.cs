using System;
using System.Diagnostics;
using System.ServiceModel;

namespace Robock.Shared.Communication
{
    public class RobockClient : IRobockDuplexCallback
    {
        private readonly IRobockDuplex _channel;

        public RobockClient(string uuid)
        {
            _channel = new DuplexChannelFactory<IRobockDuplex>(this, new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{uuid}").CreateChannel();
        }

        public void HandshakeCallback()
        {
            Debug.WriteLine("Handshake ended");
        }

        public void ApplyWallpaperCallback(bool isSucceed)
        {
            // throw new NotImplementedException();
        }

        public void DiscardWallpaperCallback()
        {
            // throw new NotImplementedException();
        }

        public void CloseCallback()
        {
            // throw new NotImplementedException();
        }

        #region IRobockDuples

        public void Handshake(int x, int y, int height, int width)
        {
            _channel.Handshake(x, y, height, width);
        }

        public void ApplyWallpaper(IntPtr src, int left, int top, int height, int width)
        {
            _channel.ApplyWallpaper(src, left, top, height, width);
        }

        public void DiscardWallpaper()
        {
            _channel.DiscardWallpaper();
        }

        public void Close()
        {
            try
            {
                _channel.Close();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}