using System.IO;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.Devices.SoundBlasterX;

[PluginFeature(Name = "Sound BlasterX Device Provider")]
public class SoundBlasterXDeviceProvider : DeviceProvider
{
    private readonly ILogger _logger;
    private readonly IDeviceService _deviceService;
    private SoundBlasterXRGBDeviceProvider? _rgbDeviceProvider;

    public SoundBlasterXDeviceProvider(ILogger logger, IDeviceService deviceService)
    {
        _logger = logger;
        _deviceService = deviceService;
        CreateMissingLedsSupported = false;
    }

    public override IRGBDeviceProvider RgbDeviceProvider =>
        _rgbDeviceProvider ??= new SoundBlasterXRGBDeviceProvider(Path.Combine(Plugin.Directory.FullName, "x64"));

    public override void Enable()
    {
        RgbDeviceProvider.Exception += OnException;
        _deviceService.AddDeviceProvider(this);
    }

    // Disposing turns the devices' LEDs off and hands them back to Sound Blaster Connect (see CreativeLedManager.Dispose).
    public override void Disable()
    {
        _deviceService.RemoveDeviceProvider(this);
        if (_rgbDeviceProvider == null)
            return;

        _rgbDeviceProvider.Exception -= OnException;
        _rgbDeviceProvider.Dispose();
        _rgbDeviceProvider = null;
    }

    private void OnException(object? sender, ExceptionEventArgs args) =>
        _logger.Warning(args.Exception, "Sound BlasterX: {Message}", args.Exception.Message);
}
