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
            var args = Environment.GetCommandLineArgs();
            _uuid = args[1];
        }

        public void Dispose()
        {
            _serviceHost.Close();
            ((IDisposable) _serviceHost)?.Dispose();
        }

        public void Handshake(int x, int y, int height, int width)
        {
            Callback.HandshakeCallback();
        }

        public void ApplyWallpaper(IntPtr src, int left, int top, int height, int width)
        {
            Callback.ApplyWallpaperCallback(true);
        }

        public void DiscardWallpaper()
        {
            Callback.DiscardWallpaperCallback();
        }

        public void Close()
        {
            Callback.CloseCallback();
            _serviceHost.Close();
        }

        //
        public void Start()
        {
            if (string.IsNullOrWhiteSpace(_uuid))
                throw new ArgumentException("uuid");

            try
            {
                _serviceHost = new ServiceHost(typeof(RobockServer));
                _serviceHost.AddServiceEndpoint(typeof(IRobockDuplex), new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}");
                _serviceHost.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}