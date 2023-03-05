using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugins.Devices.Novation
{
    // ReSharper disable once UnusedMember.Global
    public class NovationDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IRgbService _rgbService;

        public NovationDeviceProvider(ILogger logger, IRgbService rgbService) : base(RGB.NET.Devices.Novation.NovationDeviceProvider.Instance)
        {
            _logger = logger;
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            RGB.NET.Devices.Novation.NovationDeviceProvider.Instance.Exception += Provider_OnException;

            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RgbDeviceProvider.Dispose();

            RGB.NET.Devices.Novation.NovationDeviceProvider.Instance.Exception -= Provider_OnException;
        }

        private void Provider_OnException(object sender, ExceptionEventArgs args) => _logger.Debug(args.Exception, "Novation Exception: {message}", args.Exception.Message);
    }
}