using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using Artemis.Plugins.PhilipsHue.Models;
using Artemis.Plugins.PhilipsHue.RGB.NET;
using Artemis.Plugins.PhilipsHue.RGB.NET.Hue;
using Artemis.Plugins.PhilipsHue.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.PhilipsHue;

public class HueDeviceProvider : DeviceProvider
{
    private readonly IHueService _hueService;
    private readonly ILogger _logger;
    private readonly IDeviceService _deviceService;

    public HueDeviceProvider(ILogger logger, IDeviceService deviceService, IHueService hueService)
    {
        _logger = logger;
        _deviceService = deviceService;
        _hueService = hueService;
    }

    public override HueRGBDeviceProvider RgbDeviceProvider => HueRGBDeviceProvider.Instance;

    public override void Enable()
    {
        RgbDeviceProvider.Exception += Provider_OnException;
        RgbDeviceProvider.ClientDefinitions.Clear();
        foreach (PhilipsHueBridge bridge in _hueService.Bridges)
            RgbDeviceProvider.ClientDefinitions.Add(new HueClientDefinition(bridge.IpAddress, bridge.AppKey, bridge.StreamingClientKey));

        _deviceService.AddDeviceProvider(this);
    }

    public override void Disable()
    {
        _deviceService.RemoveDeviceProvider(this);
        
        RgbDeviceProvider.Exception -= Provider_OnException;
        RgbDeviceProvider.Dispose();
    }

    private void Provider_OnException(object sender, ExceptionEventArgs args)
    {
        _logger.Debug(args.Exception, "Philips Hue Exception: {message}", args.Exception.Message);
    }
}