using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

using Robock.Interop.Win32;
using Robock.Shared.Models;

namespace Robock.Models
{
    public class RobockClient : IRobockBackgroundConnection
    {
        private readonly string _uuid;
        private IRobockBackgroundConnection _channel;
        private bool _connecting;

        public RobockClient(string uuid)
        {
            _uuid = uuid;
            _connecting = false;
        }

        #region IRobockDuplex

        public async Task Handshake(int x, int y, int height, int width)
        {
            while (!_connecting)
                try
                {
                    _channel = new ChannelFactory<IRobockBackgroundConnection>(new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}").CreateChannel();
                    await _channel.Handshake(x, y, height, width);
                    _connecting = true;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
        }

        public async Task ApplyWallpaper(IntPtr src, RECT rect)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            await _channel.ApplyWallpaper(src, rect);
        }

        public async Task Heartbeat()
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            await _channel.Heartbeat();
        }

        public async Task DiscardWallpaper()
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            await _channel.DiscardWallpaper();
        }

        public async Task Close()
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");

            try
            {
                await _channel.Close();
                _connecting = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion
    }
}