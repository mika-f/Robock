using System;
using System.ServiceModel;
using System.Windows;

namespace Robock.Shared.Communication
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class RobockServer : IRobockDuplex, IDisposable
    {
        private readonly string _uuid;
        private ServiceHost _serviceHost;

        private IRobockDuplexCallback Callback => OperationContext.Current.GetCallbackChannel<IRobockDuplexCallback>();

        public RobockServer()
        {
            _uuid = Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            _serviceHost.Close();
            ((IDisposable) _serviceHost)?.Dispose();
        }

        public void Handshake(int index)
        {
            Callback.HandshakeCallback(_uuid);
        }

        public void ApplyWallpaper(string uuid, IntPtr src, int left, int top, int height, int width)
        {
            MessageBox.Show("aaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            Callback.ApplyWallpaperCallback(uuid, true);
        }

        public void DiscardWallpaper(string uuid)
        {
            Callback.DiscardWallpaperCallback(uuid);
        }

        public void Close(string uuid)
        {
            Callback.CloseCallback(uuid);
            _serviceHost.Close();
        }

        //
        public void Start()
        {
            _serviceHost = new ServiceHost(typeof(RobockServer));
            _serviceHost.AddServiceEndpoint(typeof(IRobockDuplex), new NetNamedPipeBinding(), "net.pipe://localhost/Robock");
            _serviceHost.Open();
        }
    }
}