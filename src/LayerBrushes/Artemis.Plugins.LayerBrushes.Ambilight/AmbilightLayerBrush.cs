using System;
using System.Linq;
using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.Plugins.LayerBrushes.Ambilight.UI;
using Artemis.UI.Shared.LayerBrushes;
using ScreenCapture;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightLayerBrush : LayerBrush<AmbilightPropertyGroup>
    {
        #region Properties & Fields

        private IScreenCaptureService _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;

        private Display? _display;
        private CaptureZone _captureZone;

        #endregion

        #region Methods

        public override void Update(double deltaTime) => _captureZone?.RequestUpdate();

        public override unsafe void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (_captureZone == null) return;

            AmbilightCaptureProperties properties = Properties.Capture.CurrentValue ?? new AmbilightCaptureProperties();
            lock (_captureZone.Buffer)
            {
                ReadOnlySpan<byte> capture = _captureZone.Buffer;
                if (capture.IsEmpty) return;

                fixed (byte* ptr = capture)
                {
                    using SKImage image = SKImage.FromPixels(new SKImageInfo(_captureZone.Width, _captureZone.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), new IntPtr(ptr), _captureZone.BufferWidth * 4);

                    if (properties.BlackBarDetectionTop || properties.BlackBarDetectionBottom || properties.BlackBarDetectionLeft || properties.BlackBarDetectionRight)
                    {
                        canvas.DrawImage(image, new SKRect(properties.BlackBarDetectionLeft ? _captureZone.BlackBars.Left : 0,
                                                           properties.BlackBarDetectionTop ? _captureZone.BlackBars.Top : 0,
                                                           _captureZone.Width - (properties.BlackBarDetectionRight ? _captureZone.BlackBars.Right : 0),
                                                           _captureZone.Height - (properties.BlackBarDetectionBottom ? _captureZone.BlackBars.Bottom : 0)),
                                         bounds, paint);
                    }
                    else
                        canvas.DrawImage(image, bounds, paint);
                }
            }
        }

        public override void EnableLayerBrush()
        {
            ConfigurationDialog = new LayerBrushConfigurationDialog<CapturePropertiesViewModel>(1280, 720);

            Properties.Capture.CurrentValueSet += Capture_OnCurrentValueSet;
            RecreateCaptureZone();
        }

        private void Capture_OnCurrentValueSet(object sender, LayerPropertyEventArgs e) => RecreateCaptureZone();

        private void RecreateCaptureZone()
        {
            RemoveCaptureZone();

            AmbilightCaptureProperties? props = Properties.Capture.CurrentValue;
            if (props == null) return;

            AmbilightCaptureProperties properties = props.Value;
            GraphicsCard? graphicsCard = _screenCaptureService.GetGraphicsCards()
                                                              .Where(gg => (gg.VendorId == properties.GraphicsCardVendorId) && (gg.DeviceId == properties.GraphicsCardDeviceId))
                                                              .Cast<GraphicsCard?>()
                                                              .FirstOrDefault();

            if (graphicsCard != null)
                _display = _screenCaptureService.GetDisplays(graphicsCard.Value)
                                                .Where(d => d.DeviceName.Equals(properties.DisplayName, StringComparison.OrdinalIgnoreCase))
                                                .Cast<Display?>()
                                                .FirstOrDefault();

            if (_display != null)
            {
                _captureZone = _screenCaptureService.GetScreenCapture(_display.Value).RegisterCaptureZone(
                    // Don't go beyond screen resolution
                    Math.Clamp(properties.X, 0, _display.Value.Width),
                    Math.Clamp(properties.Y, 0, _display.Value.Height),
                    Math.Clamp(properties.Width, 0, _display.Value.Width - properties.X),
                    Math.Clamp(properties.Height, 0, _display.Value.Height - properties.Y),
                    properties.DownscaleLevel);
                _captureZone.AutoUpdate = false; //TODO DarthAffe 09.04.2021: config?
                _captureZone.BlackBars.Threshold = properties.BlackBarDetectionThreshold;
            }
        }

        private void RemoveCaptureZone()
        {
            if ((_display != null) && (_captureZone != null))
                _screenCaptureService.GetScreenCapture(_display.Value).UnregisterCaptureZone(_captureZone);
            _captureZone = null;
            _display = null;
        }

        public override void DisableLayerBrush()
        {
            Properties.Capture.CurrentValueSet -= Capture_OnCurrentValueSet;
            RemoveCaptureZone();
        }

        #endregion
    }
}
