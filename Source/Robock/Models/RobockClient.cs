using System;
using System.Diagnostics;
using System.ServiceModel;

using Robock.Shared.Communication;
using Robock.Shared.Win32;

namespace Robock.Models
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
            Debug.WriteLine(isSucceed ? "Rendering Success" : "Rendering Failed");
        }

        public void DiscardWallpaperCallback()
        {
            Debug.WriteLine("Discard finished.");
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

        public void ApplyWallpaper(IntPtr src, RECT? rect)
        {
            _channel.ApplyWallpaper(src, rect);
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