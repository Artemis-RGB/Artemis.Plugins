using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;
using RGBDeviceProvider = RGB.NET.Devices.PicoPi.PicoPiDeviceProvider;

namespace Artemis.Plugins.Devices.PicoPi
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "PicoPi Device Provider")]
    public class PicoPiDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;

        public PicoPiDeviceProvider(ILogger logger, IDeviceService deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
        }
        
        public override RGBDeviceProvider RgbDeviceProvider => RGBDeviceProvider.Instance;

        public override void Enable()
        {
            RgbDeviceProvider.Exception += Provider_OnException;
            _deviceService.AddDeviceProvider(this);
        }

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);
            RgbDeviceProvider.Exception -= Provider_OnException;
            RgbDeviceProvider.Dispose();
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "PicoPi Exception: {message}", args.Exception.Message);
    }
}