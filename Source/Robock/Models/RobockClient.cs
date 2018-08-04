using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

using Robock.Shared.Communication;
using Robock.Shared.Win32;

namespace Robock.Models
{
    public class RobockClient : IRobockDuplexCallback
    {
        private readonly string _uuid;
        private IRobockDuplex _channel;
        private bool _connecting;

        public RobockClient(string uuid)
        {
            _uuid = uuid;
            _connecting = false;
        }

        public void HandshakeCallback()
        {
            _connecting = true;
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

        #region IRobockDuplex

        public void Handshake(int x, int y, int height, int width)
        {
            while (!_connecting)
                try
                {
                    _channel = new DuplexChannelFactory<IRobockDuplex>(this, new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}").CreateChannel();
                    _channel.Handshake(x, y, height, width);
                    _connecting = true;
                }
                catch
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                }
        }

        public void ApplyWallpaper(IntPtr src, RECT rect)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            _channel.ApplyWallpaper(src, rect);
        }

        public void DiscardWallpaper()
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            _channel.DiscardWallpaper();
        }

        public void Close()
        {
            try
            {
                _channel.Close();
                _connecting = false;
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}