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
    private readonly IRgbService _rgbService;

    public HueDeviceProvider(ILogger logger, IRgbService rgbService, IHueService hueService)
    {
        _logger = logger;
        _rgbService = rgbService;
        _hueService = hueService;
    }

    public override HueRGBDeviceProvider RgbDeviceProvider => HueRGBDeviceProvider.Instance;

    public override void Enable()
    {
        RgbDeviceProvider.Exception += Provider_OnException;
        RgbDeviceProvider.ClientDefinitions.Clear();
        foreach (PhilipsHueBridge bridge in _hueService.Bridges)
            RgbDeviceProvider.ClientDefinitions.Add(new HueClientDefinition(bridge.IpAddress, bridge.AppKey, bridge.StreamingClientKey));

        _rgbService.AddDeviceProvider(RgbDeviceProvider);
    }

    public override void Disable()
    {
        _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
        
        RgbDeviceProvider.Exception -= Provider_OnException;
        RgbDeviceProvider.Dispose();
    }

    private void Provider_OnException(object sender, ExceptionEventArgs args)
    {
        _logger.Debug(args.Exception, "Philips Hue Exception: {message}", args.Exception.Message);
    }
}