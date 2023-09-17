using System;
using System.Threading;
using System.Threading.Tasks;
using ScreenCapture.NET;

namespace Artemis.Plugins.LayerBrushes.Ambilight.ScreenCapture
{
    public sealed class AmbilightScreenCapture : IScreenCapture
    {
        #region Properties & Fields

        private readonly IScreenCapture _screenCapture;

        private int _zoneCount = 0;

        private Task? _updateTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationToken _cancellationToken = CancellationToken.None;

        public Display Display => _screenCapture.Display;

        #endregion

        #region Events

        public event EventHandler<ScreenCaptureUpdatedEventArgs>? Updated;

        #endregion

        #region Constructors

        public AmbilightScreenCapture(IScreenCapture screenCapture)
        {
            this._screenCapture = screenCapture;

            if (screenCapture is DX11ScreenCapture dx11ScreenCapture)
                dx11ScreenCapture.Timeout = 100;
        }

        #endregion

        #region Methods

        private void UpdateLoop()
        {
            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                bool success = _screenCapture.CaptureScreen();
                Updated?.Invoke(this, new ScreenCaptureUpdatedEventArgs(success));
            }
        }

        ICaptureZone IScreenCapture.RegisterCaptureZone(int x, int y, int width, int height, int downscaleLevel)
        {
            lock (_screenCapture)
            {
                ICaptureZone captureZone = _screenCapture.RegisterCaptureZone(x, y, width, height, downscaleLevel);
                _zoneCount++;

                if (_updateTask == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationToken = _cancellationTokenSource.Token;
                    _updateTask = Task.Factory.StartNew(UpdateLoop, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }

                return captureZone;
            }
        }

        public bool UnregisterCaptureZone(ICaptureZone captureZone)
        {
            lock (_screenCapture)
            {
                bool result = _screenCapture.UnregisterCaptureZone(captureZone);
                if (result)
                    _zoneCount--;

                if ((_zoneCount == 0) && (_updateTask != null))
                {
                    _cancellationTokenSource?.Cancel();
                    _updateTask = null;
                }

                return result;
            }
        }

        public void UpdateCaptureZone(ICaptureZone captureZone, int? x = null, int? y = null, int? width = null, int? height = null, int? downscaleLevel = null)
        {
            lock (_screenCapture)
                _screenCapture.UpdateCaptureZone(captureZone, x, y, width, height, downscaleLevel);
        }

        public bool CaptureScreen() => false;

        public void Restart() => _screenCapture.Restart();

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _updateTask = null;

            _screenCapture.Dispose();
        }

        #endregion
    }
}
