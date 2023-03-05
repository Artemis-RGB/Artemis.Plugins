using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.Devices.PicoPi
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "PicoPi Device Provider")]
    public class PicoPiDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public PicoPiDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGB.NET.Devices.PicoPi.PicoPiDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            RGB.NET.Devices.PicoPi.PicoPiDeviceProvider.Instance.Exception += Provider_OnException;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGB.NET.Devices.PicoPi.PicoPiDeviceProvider.Instance.Exception -= Provider_OnException;
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "PicoPi Exception: {message}", args.Exception.Message);
    }
}