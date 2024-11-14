using System;
using System.Linq;
using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups;
using Artemis.Plugins.LayerBrushes.Ambilight.Screens;
using Artemis.UI.Shared.LayerBrushes;
using HPPH;
using ScreenCapture.NET;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight
{
    public class AmbilightLayerBrush : LayerBrush<AmbilightPropertyGroup>
    {
        #region Properties & Fields

        private IScreenCaptureService? _screenCaptureService => AmbilightBootstrapper.ScreenCaptureService;
        public bool PropertiesOpen { get; set; }

        private Display? _display;
        private ICaptureZone? _captureZone;
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
            using (_captureZone.Lock())
            {
                // DarthAffe 11.09.2023: Accessing the low-level images is a source of potential errors in the future since we assume the pixel-format. Currently both used providers are BGRA, but if there are ever issues with shifted colors, this is the place to start investigating.
                RefImage<ColorBGRA> image = _captureZone.GetRefImage<ColorBGRA>();
                if (properties.BlackBarDetectionTop || properties.BlackBarDetectionBottom || properties.BlackBarDetectionLeft || properties.BlackBarDetectionRight)
                    image = image.RemoveBlackBars(properties.BlackBarDetectionThreshold,
                                                  properties.BlackBarDetectionTop, properties.BlackBarDetectionBottom,
                                                  properties.BlackBarDetectionLeft, properties.BlackBarDetectionRight);

                fixed (byte* img = image)
                {
                    using SKImage skImage = SKImage.FromPixels(new SKImageInfo(image.Width, image.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), (nint)img, image.RawStride);
                    canvas.DrawImage(skImage, bounds, paint);
                }
            }
        }

        public override void EnableLayerBrush()
        {
            ConfigurationDialog = new LayerBrushConfigurationDialog<CapturePropertiesViewModel>(1300, 650);
            RecreateCaptureZone();
        }

        public void RecreateCaptureZone()
        {
            if (PropertiesOpen || _creatingCaptureZone || _screenCaptureService == null)
                return;

            try
            {
                _creatingCaptureZone = true;
                RemoveCaptureZone();
                AmbilightCaptureProperties props = Properties.Capture;
                bool defaulting = props.GraphicsCardDeviceId == 0 || props.GraphicsCardVendorId == 0 || props.DisplayName.CurrentValue == null;

                GraphicsCard? graphicsCard = _screenCaptureService.GetGraphicsCards()
                    .Where(gg => defaulting || (gg.VendorId == props.GraphicsCardVendorId) && (gg.DeviceId == props.GraphicsCardDeviceId))
                    .Cast<GraphicsCard?>()
                    .FirstOrDefault();
                if (graphicsCard == null)
                    return;

                _display = _screenCaptureService.GetDisplays(graphicsCard.Value)
                    .Where(d => defaulting || d.DeviceName.Equals(props.DisplayName.CurrentValue, StringComparison.OrdinalIgnoreCase))
                    .Cast<Display?>()
                    .FirstOrDefault();
                if (_display == null)
                    return;

                // If we're defaulting or always capturing full screen, apply the display to the properties
                if (defaulting || props.CaptureFullScreen.CurrentValue)
                    props.ApplyDisplay(_display.Value, true);

                // Stick to a valid region within the display
                int width = Math.Min(_display.Value.Width, props.Width);
                int height = Math.Min(_display.Value.Height, props.Height);
                int x = Math.Min(_display.Value.Width - width, props.X);
                int y = Math.Min(_display.Value.Height - height, props.Y);
                _captureZone = _screenCaptureService.GetScreenCapture(_display.Value).RegisterCaptureZone(x, y, width, height, props.DownscaleLevel);
                _captureZone.AutoUpdate = false; //TODO DarthAffe 09.04.2021: config?
            }
            finally
            {
                _creatingCaptureZone = false;
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
            RemoveCaptureZone();
        }

        #endregion
    }
}