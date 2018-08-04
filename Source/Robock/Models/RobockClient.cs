using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

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
            Debug.WriteLine("Handshake ended");
            InvokeCallback(nameof(Handshake));
        }

        public void ApplyWallpaperCallback(bool isSucceed)
        {
            Debug.WriteLine(isSucceed ? "Rendering Success" : "Rendering Failed");
            InvokeCallback(nameof(ApplyWallpaper));
        }

        public void DiscardWallpaperCallback()
        {
            Debug.WriteLine("Discard finished.");
            InvokeCallback(nameof(DiscardWallpaper));
        }

        public void CloseCallback()
        {
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
            RegisterCallback(nameof(ApplyWallpaper), action);
            _channel.ApplyWallpaper(src, rect);
        }

        public void DiscardWallpaper(Action action = null)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");
            RegisterCallback(nameof(DiscardWallpaper), action);
            _channel.DiscardWallpaper();
        }

        public void Close(Action action = null)
        {
            if (_channel == null)
                throw new InvalidOperationException("Invalid connection");

            try
            {
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