using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

using Robock.Services;
using Robock.Shared.Models;
using Robock.Shared.Win32;

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
            StatusTextService.Instance.Status = "Start handshaking between Robock and background process... It may take a little time";
            while (!_connecting)
                try
                {
                    _channel = new ChannelFactory<IRobockBackgroundConnection>(new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}").CreateChannel();
                    await _channel.Handshake(x, y, height, width);
                    _connecting = true;
                    StatusTextService.Instance.Status = "Handshake success, Connected to background process";
                }
                catch
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                }
        }

        public async Task ApplyWallpaper(IntPtr src, RECT rect)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            StatusTextService.Instance.Status = "Applying wallpeper...";
            await _channel.ApplyWallpaper(src, rect);
            StatusTextService.Instance.Status = "Rendering success, Start rendering";
        }

        public async Task DiscardWallpaper()
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            StatusTextService.Instance.Status = "Discarding wallpeper...";
            await _channel.DiscardWallpaper();
            StatusTextService.Instance.Status = "Discard wallpaper finished";
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