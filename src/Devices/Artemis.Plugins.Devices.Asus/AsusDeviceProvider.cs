using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.Asus
{
    // ReSharper disable once UnusedMember.Global
    [PluginFeature(Name = "ASUS Device Provider")]
    public class AsusDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;

        public AsusDeviceProvider(IRgbService rgbService) : base(RGB.NET.Devices.Asus.AsusDeviceProvider.Instance)
        {
            _rgbService = rgbService;

            // The ASUS SDK does not handle extra LEDs very well at all (stack corruption and all that!)
            CreateMissingLedsSupported = false;
        }

        public override void Enable()
        {
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }

        public override void Disable()
        {
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
            RGB.NET.Devices.Asus.AsusDeviceProvider.Instance.Dispose();
        }
    }
}