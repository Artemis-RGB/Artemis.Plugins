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
        public bool PropertiesOpen { get; set; }

        private Display? _display;
        private CaptureZone _captureZone;
        private bool _creatingCaptureZone;

        #endregion

        #region Methods

        public override void Update(double deltaTime)
        {
            _captureZone?.RequestUpdate();
        }

        public override unsafe void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
        {
            if (_captureZone == null) return;

            AmbilightCaptureProperties properties = Properties.Capture;
            lock (_captureZone.Buffer)
            {
                ReadOnlySpan<byte> capture = _captureZone.Buffer;
                if (capture.IsEmpty) return;

                fixed (byte* ptr = capture)
                {
                    using SKImage image = SKImage.FromPixels(new SKImageInfo(_captureZone.Width, _captureZone.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), new IntPtr(ptr),
                        _captureZone.BufferWidth * 4);

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

            Properties.Capture.LayerPropertyOnCurrentValueSet += CaptureOnLayerPropertyOnCurrentValueSet;
            RecreateCaptureZone();
        }

        private void CaptureOnLayerPropertyOnCurrentValueSet(object sender, LayerPropertyEventArgs e) => RecreateCaptureZone();

        public void RecreateCaptureZone()
        {
            if (PropertiesOpen || _creatingCaptureZone)
                return;

            _creatingCaptureZone = true;

            RemoveCaptureZone();
            AmbilightCaptureProperties props = Properties.Capture;
            GraphicsCard? graphicsCard = _screenCaptureService.GetGraphicsCards()
                .Where(gg => (gg.VendorId == props.GraphicsCardVendorId) && (gg.DeviceId == props.GraphicsCardDeviceId))
                .Cast<GraphicsCard?>()
                .FirstOrDefault();

            // If the display couldn't be found we default to full display capture 
            bool defaulting = false;
            if (graphicsCard == null)
            {
                graphicsCard = _screenCaptureService.GetGraphicsCards().Cast<GraphicsCard?>().FirstOrDefault();
                defaulting = true;
            }

            if (graphicsCard != null)
            {
                _display = _screenCaptureService.GetDisplays(graphicsCard.Value)
                    .Where(d => d.DeviceName.Equals(props.DisplayName.CurrentValue, StringComparison.OrdinalIgnoreCase))
                    .Cast<Display?>()
                    .FirstOrDefault();
                if (_display == null)
                {
                    _display = _screenCaptureService.GetDisplays(graphicsCard.Value).Cast<Display?>().FirstOrDefault();
                    defaulting = true;
                }
            }

            if (_display != null)
            {
                // The display couldn't be found, apply the one we did find
                if (defaulting)
                    props.ApplyDisplay(_display.Value, true);

                _captureZone = _screenCaptureService.GetScreenCapture(_display.Value).RegisterCaptureZone(props.X, props.Y, props.Width, props.Height, props.DownscaleLevel);
                _captureZone.AutoUpdate = false; //TODO DarthAffe 09.04.2021: config?
                _captureZone.BlackBars.Threshold = props.BlackBarDetectionThreshold;
            }

            _creatingCaptureZone = false;
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
            Properties.Capture.LayerPropertyOnCurrentValueSet -= CaptureOnLayerPropertyOnCurrentValueSet;
            RemoveCaptureZone();
        }

        #endregion
    }
}