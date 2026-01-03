using Artemis.Core;
using Artemis.Plugins.Nodes.ScreenCapture.ScreenCapture;
using HPPH;
using ScreenCapture.NET;
using Serilog;
using SkiaSharp;

namespace Artemis.Plugins.Nodes.ScreenCapture.Nodes;

public class PixelCaptureNode : Node<PixelCaptureNodeEntity, PixelCaptureNodeCustomViewModel>
{
    private readonly ILogger _logger;
    private readonly NodeScreenCaptureService? _screenCaptureService;
    private ICaptureZone? _captureZone;
    private Display? _display;
    private bool _creatingCaptureZone;

    public PixelCaptureNode(ILogger logger)
    {
        _logger = logger;
        _screenCaptureService = Bootstrapper.ScreenCaptureService;

        StorageModified += OnStorageModified;
        Output = CreateOutputPin<SKColor>();
    }

    public OutputPin<SKColor> Output { get; }

    public override void Initialize(INodeScript script)
    {
        CreateCaptureZone();
    }

    public override void Evaluate()
    {
        if (_captureZone == null)
            return;

        _captureZone.RequestUpdate();
        using (_captureZone.Lock())
        {
            RefImage<ColorBGRA> image = _captureZone.GetRefImage<ColorBGRA>();
            ColorBGRA color = image[0, 0];
            Output.Value = new SKColor(color.R, color.G, color.B);
        }
    }

    private void CreateCaptureZone()
    {
        if (_screenCaptureService == null || _creatingCaptureZone)
            return;

        _creatingCaptureZone = true;

        try
        {
            RemoveCaptureZone();
            Storage ??= new PixelCaptureNodeEntity(0, 0, null, 0, 0);
            bool defaulting = Storage.GraphicsCardDeviceId == 0 || Storage.GraphicsCardVendorId == 0 || Storage.DisplayName == null;
            GraphicsCard? graphicsCard = _screenCaptureService.GetGraphicsCards()
                .Where(gg => defaulting || gg.VendorId == Storage.GraphicsCardDeviceId && gg.DeviceId == Storage.GraphicsCardDeviceId)
                .Cast<GraphicsCard?>()
                .FirstOrDefault();
            if (graphicsCard == null)
                return;

            _display = _screenCaptureService.GetDisplays(graphicsCard.Value)
                .Where(d => defaulting || d.DeviceName.Equals(Storage.DisplayName, StringComparison.OrdinalIgnoreCase))
                .Cast<Display?>()
                .FirstOrDefault();
            if (_display == null)
                return;

            int x = Math.Min(_display.Value.Width - 1, Storage.X);
            int y = Math.Min(_display.Value.Height - 1, Storage.Y);
            _captureZone = _screenCaptureService.GetScreenCapture(_display.Value).RegisterCaptureZone(x, y, 1, 1);
            _captureZone.AutoUpdate = false;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error creating capture zone.");
            _captureZone = null;
            _display = null;
        }
        finally
        {
            _creatingCaptureZone = false;
        }
    }

    private void RemoveCaptureZone()
    {
        if (_screenCaptureService == null || _captureZone == null)
            return;

        if (_display != null && _captureZone != null)
            _screenCaptureService.GetScreenCapture(_display.Value).UnregisterCaptureZone(_captureZone);

        _captureZone = null;
        _display = null;
    }

    private void OnStorageModified(object? sender, EventArgs e)
    {
        CreateCaptureZone();
    }
}

public record PixelCaptureNodeEntity(int GraphicsCardVendorId, int GraphicsCardDeviceId, string? DisplayName, int X, int Y);