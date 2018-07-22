using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Windows;

using Robock.Shared.Communication;
using Robock.Shared.Models;
using Robock.Shared.Win32;

namespace Robock.Background.Models
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RobockServer : IRobockDuplex, IDisposable
    {
        private readonly DesktopWindowManager _desktopWindowManager;
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
            _desktopWindowManager = new DesktopWindowManager();

            Observable.Return(0).ObserveOn(Application.Current.Dispatcher).Subscribe(_ =>
            {
                try
                {
                    _desktopWindowManager.Initialize();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
        }

        public void Dispose()
        {
            _serviceHost.Close();
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
            _desktopWindowManager.Start(src, 0, 0, _height, _width, 0, rect);

            Callback.ApplyWallpaperCallback(_desktopWindowManager.Thumbnails[0].IsRendering);
        }

        public void DiscardWallpaper()
        {
            _desktopWindowManager.Stop();

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