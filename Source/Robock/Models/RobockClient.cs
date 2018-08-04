using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

using Robock.Services;
using Robock.Shared.Communication;
using Robock.Shared.Win32;

namespace Robock.Models
{
    public class RobockClient : IRobockDuplexCallback
    {
        private readonly Dictionary<string, Action> _callbacks;
        private readonly string _uuid;
        private IRobockDuplex _channel;
        private bool _connecting;

        public RobockClient(string uuid)
        {
            _uuid = uuid;
            _connecting = false;
            _callbacks = new Dictionary<string, Action>();
        }

        public void HandshakeCallback()
        {
            _connecting = true;
            StatusTextService.Instance.Status = "Handshake success, Connected to background process";
            InvokeCallback(nameof(Handshake));
        }

        public void ApplyWallpaperCallback(bool isSucceed)
        {
            StatusTextService.Instance.Status = isSucceed ? "Rendering success, Start rendering" : "Rendering failed";
            InvokeCallback(nameof(ApplyWallpaper));
        }

        public void DiscardWallpaperCallback()
        {
            StatusTextService.Instance.Status = "Discard wallpaper finished";
            InvokeCallback(nameof(DiscardWallpaper));
        }

        public void CloseCallback()
        {
            StatusTextService.Instance.Status = "Closed connection to background process";
            InvokeCallback(nameof(Close));
        }

        private void InvokeCallback(string key)
        {
            if (!_callbacks.ContainsKey(key))
                return;
            _callbacks[key]?.Invoke();
            _callbacks.Remove(key);
        }

        private void RegisterCallback(string key, Action action)
        {
            if (action == null)
                return;

            if (_callbacks.ContainsKey(key))
                _callbacks.Remove(key);
            _callbacks.Add(key, action);
        }

        #region IRobockDuplex

        public void Handshake(int x, int y, int height, int width, Action action = null)
        {
            StatusTextService.Instance.Status = "Start handshaking between Robock and background process... It may take a little time";
            while (!_connecting)
                try
                {
                    _channel = new DuplexChannelFactory<IRobockDuplex>(this, new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}").CreateChannel();
                    RegisterCallback(nameof(Handshake), action);
                    _channel.Handshake(x, y, height, width);
                    _connecting = true;
                }
                catch
                {
                    Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
                }
        }

        public void ApplyWallpaper(IntPtr src, RECT rect, Action action = null)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            StatusTextService.Instance.Status = "Applying wallpeper...";
            RegisterCallback(nameof(ApplyWallpaper), action);
            _channel.ApplyWallpaper(src, rect);
        }

        public void DiscardWallpaper(Action action = null)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            StatusTextService.Instance.Status = "Discarding wallpeper...";
            RegisterCallback(nameof(DiscardWallpaper), action);
            _channel.DiscardWallpaper();
        }

        public void Close(Action action = null)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");

            try
            {
                StatusTextService.Instance.Status = "Closing connection to background process... It may take a little time";
                RegisterCallback(nameof(Close), action);
                _channel.Close();
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