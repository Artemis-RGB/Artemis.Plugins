using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Asus.AsusDeviceProvider;

namespace Artemis.Plugins.Devices.Asus
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "ASUS Device Provider")]
    public class AsusDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public AsusDeviceProvider(ILogger logger, IRgbService rgbService)
        {
            _logger = logger;
            _rgbService = rgbService;

            // The ASUS SDK does not handle extra LEDs very well at all (stack corruption and all that!)
            CreateMissingLedsSupported = false;
        }

        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;
        
        public override void Enable()
        {
            RgbDeviceProvider.Exception += Provider_OnException;
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Exception += Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Asus Exception: {message}", args.Exception.Message);
    }
}