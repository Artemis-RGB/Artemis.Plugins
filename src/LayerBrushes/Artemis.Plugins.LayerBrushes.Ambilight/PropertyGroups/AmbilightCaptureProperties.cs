using Artemis.Core;
using ScreenCapture;

namespace Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups
{
    public class AmbilightCaptureProperties : LayerPropertyGroup
    {
        public IntLayerProperty GraphicsCardVendorId { get; set; }
        public IntLayerProperty GraphicsCardDeviceId { get; set; }
        public LayerProperty<string> DisplayName { get; set; }

        public IntLayerProperty X { get; set; }
        public IntLayerProperty Y { get; set; }
        public IntLayerProperty Width { get; set; }
        public IntLayerProperty Height { get; set; }
        public BoolLayerProperty FlipHorizontal { get; set; }
        public BoolLayerProperty FlipVertical { get; set; }
        public IntLayerProperty DownscaleLevel { get; set; }

        public BoolLayerProperty BlackBarDetectionTop { get; set; }
        public BoolLayerProperty BlackBarDetectionBottom { get; set; }
        public BoolLayerProperty BlackBarDetectionLeft { get; set; }
        public BoolLayerProperty BlackBarDetectionRight { get; set; }
        public IntLayerProperty BlackBarDetectionThreshold { get; set; }


        protected override void PopulateDefaults()
        {
        }

        protected override void EnableProperties()
        {
        }

        protected override void DisableProperties()
        {
        }

        public void ApplyDisplay(Display display, bool includeRegion)
        {
            GraphicsCardVendorId.BaseValue = display.GraphicsCard.VendorId;
            GraphicsCardDeviceId.BaseValue = display.GraphicsCard.DeviceId;
            DisplayName.BaseValue = display.DeviceName;

            if (!includeRegion) return;
            X.BaseValue = 0;
            Y.BaseValue = 0;
            Width.BaseValue = display.Width;
            Height.BaseValue = display.Height;
        }
    }
}