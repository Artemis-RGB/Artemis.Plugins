using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using ScreenCapture.NET;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Ambilight.UI
{
    public sealed class DisplayPreview : PropertyChangedBase, IDisposable
    {
        #region Properties & Fields

        private bool _isDisposed;

        private readonly CaptureZone _captureZone;
        private readonly CaptureZone _processedCaptureZone;

        private readonly bool _blackBarDetectionTop;
        private readonly bool _blackBarDetectionBottom;
        private readonly bool _blackBarDetectionLeft;
        private readonly bool _blackBarDetectionRight;

        public Display Display { get; }

        private readonly byte[] _previewBuffer;
        private readonly byte[] _processedPreviewBuffer;

        private WriteableBitmap _preview;

        public WriteableBitmap Preview
        {
            get => _preview;
            set => SetAndNotify(ref _preview, value);
        }

        private WriteableBitmap _processedPreview;

        public WriteableBitmap ProcessedPreview
        {
            get => _processedPreview;
            set => SetAndNotify(ref _processedPreview, value);
        }

        #endregion

        #region Constructors

        public DisplayPreview(Display display, bool highQuality = false)
        {
            Display = display;

            _captureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height, highQuality ? 0 : 2);

            _previewBuffer = new byte[_captureZone.Buffer.Length];
            Preview = new WriteableBitmap(BitmapSource.Create(_captureZone.Width, _captureZone.Height, 96, 96, PixelFormats.Bgra32, null, _previewBuffer, _captureZone.Stride));
        }

        public DisplayPreview(Display display, AmbilightCaptureProperties properties)
        {
            Display = display;
            _blackBarDetectionBottom = properties.BlackBarDetectionBottom;
            _blackBarDetectionTop = properties.BlackBarDetectionTop;
            _blackBarDetectionLeft = properties.BlackBarDetectionLeft;
            _blackBarDetectionRight = properties.BlackBarDetectionRight;

            _captureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(0, 0, display.Width, display.Height);
            _previewBuffer = new byte[_captureZone.Buffer.Length];
            Preview = new WriteableBitmap(BitmapSource.Create(_captureZone.Width, _captureZone.Height, 96, 96, PixelFormats.Bgra32, null, _previewBuffer, _captureZone.Stride));

            if (properties.X + properties.Width <= display.Width && properties.Y + properties.Height <= display.Height)
            {
                _processedCaptureZone = AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(display).RegisterCaptureZone(properties.X, properties.Y, properties.Width, properties.Height, properties.DownscaleLevel);
                _processedCaptureZone.BlackBars.Threshold = properties.BlackBarDetectionThreshold;
                _processedPreviewBuffer = new byte[_processedCaptureZone.Buffer.Length];
                ProcessedPreview = new WriteableBitmap(
                    BitmapSource.Create(_processedCaptureZone.Width, _processedCaptureZone.Height, 96, 96, PixelFormats.Bgra32, null, _processedPreviewBuffer, _processedCaptureZone.Stride)
                );
            }
        }

        #endregion

        #region Methods

        public void Update()
        {
            if (_isDisposed) return;

            lock (_captureZone.Buffer)
            {
                Preview.WritePixels(new Int32Rect(0, 0, _captureZone.Width, _captureZone.Height), _captureZone.Buffer, _captureZone.Stride, 0, 0);
            }

            if (_processedCaptureZone == null)
                return;

            lock (_processedCaptureZone.Buffer)
            {
                if (_processedCaptureZone.Buffer.Length == 0) return;

                if (_blackBarDetectionTop || _blackBarDetectionBottom || _blackBarDetectionLeft || _blackBarDetectionRight)
                {
                    int x = _blackBarDetectionLeft ? _processedCaptureZone.BlackBars.Left : 0;
                    int y = _blackBarDetectionTop ? _processedCaptureZone.BlackBars.Top : 0;
                    int width = _processedCaptureZone.Width - (_blackBarDetectionRight ? _processedCaptureZone.BlackBars.Right : 0) - x;
                    int height = _processedCaptureZone.Height - (_blackBarDetectionBottom ? _processedCaptureZone.BlackBars.Bottom : 0) - y;

                    if ((ProcessedPreview.PixelWidth != width || ProcessedPreview.PixelHeight != height) && width > 0 && height > 0)
                        ProcessedPreview = new WriteableBitmap(
                            BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, _processedPreviewBuffer, _processedCaptureZone.Stride));

                    ProcessedPreview.WritePixels(new Int32Rect(x, y, width, height), _processedCaptureZone.Buffer, _processedCaptureZone.Stride, 0, 0);
                }
                else
                    ProcessedPreview.WritePixels(new Int32Rect(0, 0, _processedCaptureZone.Width, _processedCaptureZone.Height), _processedCaptureZone.Buffer,
                        _processedCaptureZone.Stride, 0, 0);
            }
        }

        public void Dispose()
        {
            AmbilightBootstrapper.ScreenCaptureService.GetScreenCapture(Display).UnregisterCaptureZone(_captureZone);
            _isDisposed = true;
        }

        #endregion
    }
}