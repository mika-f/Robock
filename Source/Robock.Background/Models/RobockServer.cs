using System;
using System.ServiceModel;
using System.Windows;

using Robock.Shared.Communication;
using Robock.Shared.Win32;

namespace Robock.Background.Models
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RobockServer : IRobockDuplex, IDisposable
    {
        private readonly BackgroundService _backgroundService;
        private readonly string _uuid;
        private int _height;
        private ServiceHost _serviceHost;
        private int _width;
        private int _x;
        private int _y;

        private IRobockDuplexCallback Callback => OperationContext.Current.GetCallbackChannel<IRobockDuplexCallback>();

        public RobockServer()
        {
            var args = Environment.GetCommandLineArgs();
            _uuid = args[1];
            _backgroundService = BackgroundService.Instance;
        }

        public void Dispose()
        {
            _serviceHost?.Close();
            ((IDisposable) _serviceHost)?.Dispose();
        }

        public void Handshake(int x, int y, int height, int width)
        {
            _x = x;
            _y = y;
            _height = height;
            _width = width;
        }

        public void ApplyWallpaper(IntPtr src, RECT? rect)
        {
            _backgroundService.StartRender(src, _x, _y, _width, _height);

            Callback.ApplyWallpaperCallback(true);
        }

        public void DiscardWallpaper()
        {
            _backgroundService.StopRender();

            Callback.DiscardWallpaperCallback();
        }

        public void Close()
        {
            DiscardWallpaper();
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