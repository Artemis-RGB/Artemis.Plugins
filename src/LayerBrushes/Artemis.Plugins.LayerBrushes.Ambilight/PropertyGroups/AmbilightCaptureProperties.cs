using ScreenCapture;

namespace Artemis.Plugins.LayerBrushes.Ambilight.PropertyGroups
{
    public struct AmbilightCaptureProperties
    {
        #region Properties & Fields

        public int GraphicsCardVendorId { get; set; }
        public int GraphicsCardDeviceId { get; set; }
        public string DisplayName { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FlipHorizontal { get; set; }
        public bool FlipVertical { get; set; }
        public int DownscaleLevel { get; set; }

        public bool BlackBarDetectionTop { get; set; }
        public bool BlackBarDetectionBottom { get; set; }
        public bool BlackBarDetectionLeft { get; set; }
        public bool BlackBarDetectionRight { get; set; }
        public int BlackBarDetectionThreshold { get; set; }

        #endregion

        #region Constructors

        public AmbilightCaptureProperties(Display? display, int x, int y, int width, int height, bool flipHorizontal, bool flipVertical, int downscaleLevel,
                                          bool blackBarDetectionTop, bool blackBarDetectionBottom, bool blackBarDetectionLeft, bool blackBarDetectionRight, int blackBarDetectionThreshold)
        {
            this.GraphicsCardVendorId = display?.GraphicsCard.VendorId ?? -1;
            this.GraphicsCardDeviceId = display?.GraphicsCard.DeviceId ?? -1;
            this.DisplayName = display?.DeviceName ?? "-";
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.FlipHorizontal = flipHorizontal;
            this.FlipVertical = flipVertical;
            this.DownscaleLevel = downscaleLevel;
            this.BlackBarDetectionTop = blackBarDetectionTop;
            this.BlackBarDetectionBottom = blackBarDetectionBottom;
            this.BlackBarDetectionLeft = blackBarDetectionLeft;
            this.BlackBarDetectionRight = blackBarDetectionRight;
            this.BlackBarDetectionThreshold = blackBarDetectionThreshold;
        }

        #endregion
    }
}