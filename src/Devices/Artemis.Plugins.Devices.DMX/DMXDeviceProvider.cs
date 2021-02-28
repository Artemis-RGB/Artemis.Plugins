using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.DMX
{
    // ReSharper disable once UnusedMember.Global
    public class DMXDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public DMXDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.DMX.DMXDeviceProvider.Instance)
        {
            _rgbService = rgbService;
        }

        public override void Enable()
        {
            // TODO: Load from configuration
            // RGB.NET.Devices.DMX.DMXDeviceProvider.Instance.AddDeviceDefinition();
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RGB.NET.Devices.DMX.DMXDeviceProvider.Instance.Dispose();
        }
    }
}