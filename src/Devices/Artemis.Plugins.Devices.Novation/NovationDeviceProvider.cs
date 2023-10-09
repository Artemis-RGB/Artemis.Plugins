using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.Novation.NovationDeviceProvider;

namespace Artemis.Plugins.Devices.Novation
{
    // ReSharper disable once UnusedMember.Global
    public class NovationDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public NovationDeviceProvider(ILogger logger, IRgbService rgbService)
        {
            _logger = logger;
            _rgbService = rgbService;
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
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Novation Exception: {message}", args.Exception.Message);
    }
}