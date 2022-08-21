namespace Artemis.Plugins.LayerBrushes.Ambilight.Nodes.Storage;

public class CaptureScreenNodeStorage
{
    public int GraphicsCardVendorId { get; set; }
    public int GraphicsCardDeviceId { get; set; }
    public string DisplayName { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool CaptureFullScreen { get; set; }

    public bool FlipHorizontal { get; set; }
    public bool FlipVertical { get; set; }
    public int DownscaleLevel { get; set; }

    public bool BlackBarDetectionTop { get; set; }
    public bool BlackBarDetectionBottom { get; set; }
    public bool BlackBarDetectionLeft { get; set; }
    public bool BlackBarDetectionRight { get; set; }
    public int BlackBarDetectionThreshold { get; set; }
}