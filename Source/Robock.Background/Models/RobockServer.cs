using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

using Robock.Shared.Models;
using Robock.Shared.Win32;

namespace Robock.Background.Models
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RobockServer : IRobockBackgroundConnection, IDisposable
    {
        private readonly BackgroundService _backgroundService;
        private readonly string _uuid;
        private ServiceHost _serviceHost;

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

        public Task Handshake(int x, int y, int height, int width)
        {
            _backgroundService.SetupRenderer(x, y, width, height);

            return Task.CompletedTask;
        }

        public Task ApplyWallpaper(IntPtr src, RECT rect)
        {
            _backgroundService.StartRender(src, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);

            return Task.CompletedTask;
        }

        // 通信できればそれでよい
        public Task Heartbeat()
        {
            return Task.CompletedTask;
        }

        public Task DiscardWallpaper()
        {
            _backgroundService.StopRender();

            return Task.CompletedTask;
        }

        public Task Close()
        {
            DiscardWallpaper();
            _backgroundService.Kill();

            return Task.CompletedTask;
        }

        //
        public void Start()
        {
            if (string.IsNullOrWhiteSpace(_uuid))
                throw new ArgumentException("uuid");

            try
            {
                _serviceHost = new ServiceHost(typeof(RobockServer));
                _serviceHost.AddServiceEndpoint(typeof(IRobockBackgroundConnection), new NetNamedPipeBinding(), $"net.pipe://localhost/Robock.{_uuid}");
                _serviceHost.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}